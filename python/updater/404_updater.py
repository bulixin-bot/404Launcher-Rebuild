from __future__ import annotations

import argparse
import hashlib
import json
import os
import shutil
import signal
import stat
import subprocess
import time
import traceback
from datetime import datetime
from pathlib import Path, PurePosixPath


def sha256_file(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as source:
        for block in iter(lambda: source.read(1048576), b""):
            digest.update(block)
    return digest.hexdigest()


def relative(value: str) -> Path:
    path = PurePosixPath(str(value).replace("\\", "/").lstrip("/"))
    if path.is_absolute() or ".." in path.parts:
        raise RuntimeError(f"非法更新路径：{value}")
    return Path(*path.parts)


def process_exists(pid: int) -> bool:
    if pid <= 0:
        return False
    if os.name == "nt":
        import ctypes

        kernel32 = ctypes.WinDLL("kernel32", use_last_error=True)
        kernel32.OpenProcess.argtypes = [ctypes.c_ulong, ctypes.c_int, ctypes.c_ulong]
        kernel32.OpenProcess.restype = ctypes.c_void_p
        kernel32.GetExitCodeProcess.argtypes = [ctypes.c_void_p, ctypes.POINTER(ctypes.c_ulong)]
        kernel32.GetExitCodeProcess.restype = ctypes.c_int
        kernel32.CloseHandle.argtypes = [ctypes.c_void_p]
        handle = kernel32.OpenProcess(4096, False, pid)
        if not handle:
            return ctypes.get_last_error() == 5
        try:
            exit_code = ctypes.c_ulong()
            return bool(kernel32.GetExitCodeProcess(handle, ctypes.byref(exit_code))) and exit_code.value == 259
        finally:
            kernel32.CloseHandle(handle)
    try:
        os.kill(pid, 0)
    except ProcessLookupError:
        return False
    except PermissionError:
        return True
    except OSError:
        return False
    return True


def wait_for_process(pid: int, timeout: float = 120.0) -> None:
    deadline = time.monotonic() + timeout
    while time.monotonic() < deadline:
        if not process_exists(pid):
            return
        time.sleep(0.25)
    raise RuntimeError("等待 404 Launcher 退出超时")


def windows_process_parent_map() -> dict[int, int]:
    if os.name != "nt":
        return {}
    import ctypes
    from ctypes import wintypes

    class PROCESSENTRY32W(ctypes.Structure):
        _fields_ = [
            ("dwSize", wintypes.DWORD),
            ("cntUsage", wintypes.DWORD),
            ("th32ProcessID", wintypes.DWORD),
            ("th32DefaultHeapID", ctypes.c_size_t),
            ("th32ModuleID", wintypes.DWORD),
            ("cntThreads", wintypes.DWORD),
            ("th32ParentProcessID", wintypes.DWORD),
            ("pcPriClassBase", ctypes.c_long),
            ("dwFlags", wintypes.DWORD),
            ("szExeFile", wintypes.WCHAR * 260),
        ]

    kernel32 = ctypes.WinDLL("kernel32", use_last_error=True)
    kernel32.CreateToolhelp32Snapshot.argtypes = [wintypes.DWORD, wintypes.DWORD]
    kernel32.CreateToolhelp32Snapshot.restype = wintypes.HANDLE
    kernel32.Process32FirstW.argtypes = [wintypes.HANDLE, ctypes.POINTER(PROCESSENTRY32W)]
    kernel32.Process32FirstW.restype = wintypes.BOOL
    kernel32.Process32NextW.argtypes = [wintypes.HANDLE, ctypes.POINTER(PROCESSENTRY32W)]
    kernel32.Process32NextW.restype = wintypes.BOOL
    kernel32.CloseHandle.argtypes = [wintypes.HANDLE]

    snapshot = kernel32.CreateToolhelp32Snapshot(2, 0)
    invalid_handle = ctypes.c_void_p(-1).value
    if not snapshot or snapshot == invalid_handle:
        return {}
    parent_by_pid = {}
    try:
        entry = PROCESSENTRY32W()
        entry.dwSize = ctypes.sizeof(PROCESSENTRY32W)
        has_entry = bool(kernel32.Process32FirstW(snapshot, ctypes.byref(entry)))
        while has_entry:
            parent_by_pid[int(entry.th32ProcessID)] = int(entry.th32ParentProcessID)
            has_entry = bool(kernel32.Process32NextW(snapshot, ctypes.byref(entry)))
    finally:
        kernel32.CloseHandle(snapshot)
    return parent_by_pid


def windows_descendant_process_ids(root_pid: int) -> list[int]:
    if os.name != "nt" or root_pid <= 0:
        return []
    parent_by_pid = windows_process_parent_map()

    descendants = []
    frontier = {root_pid}
    while frontier:
        children = {
            child_pid
            for child_pid, parent_pid in parent_by_pid.items()
            if parent_pid in frontier and child_pid != root_pid
        }
        children.difference_update(descendants)
        if not children:
            break
        descendants.extend(sorted(children))
        frontier = children
    return descendants


def windows_ancestor_process_ids(process_pid: int, stop_pid: int = 0) -> set[int]:
    if os.name != "nt" or process_pid <= 0:
        return set()
    parent_by_pid = windows_process_parent_map()
    ancestors = set()
    current = process_pid
    while current in parent_by_pid:
        parent = parent_by_pid[current]
        if parent <= 0 or parent == stop_pid or parent in ancestors:
            break
        ancestors.add(parent)
        current = parent
    return ancestors


def taskkill_process_tree(pid: int) -> None:
    if pid <= 0 or pid == os.getpid() or not process_exists(pid):
        return
    subprocess.run(
        ["taskkill", "/PID", str(pid), "/T", "/F"],
        stdout=subprocess.DEVNULL,
        stderr=subprocess.DEVNULL,
        creationflags=getattr(subprocess, "CREATE_NO_WINDOW", 0),
        check=False,
    )


def taskkill_process(pid: int) -> None:
    if pid <= 0 or pid == os.getpid() or not process_exists(pid):
        return
    subprocess.run(
        ["taskkill", "/PID", str(pid), "/F"],
        stdout=subprocess.DEVNULL,
        stderr=subprocess.DEVNULL,
        creationflags=getattr(subprocess, "CREATE_NO_WINDOW", 0),
        check=False,
    )


def close_launcher_process(pid: int, descendants: list[int] | None = None) -> None:
    if pid <= 0 or pid == os.getpid():
        return
    captured_descendants = descendants if descendants is not None else windows_descendant_process_ids(pid)
    updater_processes = windows_ancestor_process_ids(os.getpid(), pid) | {os.getpid()}
    forced = False
    if process_exists(pid):
        try:
            wait_for_process(pid, timeout=15.0)
        except RuntimeError:
            if os.name == "nt":
                for child_pid in reversed(captured_descendants):
                    if child_pid not in updater_processes:
                        taskkill_process_tree(child_pid)
                taskkill_process(pid)
                forced = True
            else:
                os.kill(pid, signal.SIGTERM)
            wait_for_process(pid, timeout=30.0)
    if os.name == "nt" and not forced:
        for child_pid in reversed(captured_descendants):
            if child_pid not in updater_processes:
                taskkill_process_tree(child_pid)


def replace_with_retry(source: Path, target: Path, timeout: float = 90.0) -> None:
    """Atomically replace a file after transient Windows locks are released."""
    deadline = time.monotonic() + timeout
    delay = 0.15
    while True:
        try:
            if target.exists():
                try:
                    target.chmod(target.stat().st_mode | stat.S_IWRITE)
                except OSError:
                    pass
            os.replace(source, target)
            return
        except OSError as exc:
            if getattr(exc, "winerror", None) not in {5, 32}:
                raise
            if time.monotonic() >= deadline:
                raise PermissionError(
                    f"等待文件解除占用超时：{target}；请关闭安全软件扫描或占用该文件的程序后重试"
                ) from exc
            time.sleep(delay)
            delay = min(delay * 1.35, 1.0)


def restore_file(source: Path, target: Path) -> None:
    temporary = target.with_name(target.name + ".404rollback")
    temporary.unlink(missing_ok=True)
    shutil.copy2(source, temporary)
    replace_with_retry(temporary, target)


def is_protected(path: str, prefixes: list[str]) -> bool:
    lowered = path.replace("\\", "/").strip("/").lower()
    return any(lowered == prefix.lower().strip("/") or lowered.startswith(prefix.lower().strip("/") + "/") for prefix in prefixes)


def write_update_log(app_root: Path, message: str) -> None:
    try:
        log_path = app_root / ".404_update" / "updater.log"
        log_path.parent.mkdir(parents=True, exist_ok=True)
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        with log_path.open("a", encoding="utf-8") as output:
            output.write(f"[{timestamp}] {message}\n")
    except OSError:
        pass


def apply(plan_path: Path, wait_pid: int, ready_file: Path | None = None) -> Path:
    plan = json.loads(plan_path.read_text("utf-8-sig"))
    app_root = Path(plan["app_root"]).resolve()
    staging_root = Path(plan["staging_root"]).resolve()
    descendants = windows_descendant_process_ids(wait_pid)
    write_update_log(
        app_root,
        f"开始更新 version={plan['version']} wait_pid={wait_pid} files={len(plan['files'])}",
    )
    write_update_log(app_root, f"已记录 Launcher 子进程：{descendants}")
    if ready_file is not None:
        ready_file.parent.mkdir(parents=True, exist_ok=True)
        ready_file.write_text("ready\n", encoding="utf-8")
        write_update_log(app_root, f"已写入 ready 信号：{ready_file}")
    close_launcher_process(wait_pid, descendants)
    write_update_log(app_root, "Launcher 及残留子进程已退出")
    time.sleep(3.0)
    rollback = app_root / ".404_update" / "rollback" / str(plan["version"])
    if rollback.exists():
        shutil.rmtree(rollback)
    rollback.mkdir(parents=True)
    replaced = []
    created = []
    removed = []
    try:
        for item in plan["files"]:
            rel = relative(item["path"])
            source = staging_root / rel
            target = app_root / rel
            if not source.is_file() or sha256_file(source) != item["sha256"]:
                raise RuntimeError(f"暂存文件校验失败：{item['path']}")
            if target.exists():
                backup = rollback / rel
                backup.parent.mkdir(parents=True, exist_ok=True)
                shutil.copy2(target, backup)
            else:
                created.append(rel)
            target.parent.mkdir(parents=True, exist_ok=True)
            temporary = target.with_name(target.name + ".404new")
            temporary.unlink(missing_ok=True)
            shutil.copy2(source, temporary)
            replace_with_retry(temporary, target)
            replaced.append(rel)
            write_update_log(app_root, f"已替换：{rel.as_posix()}")

        for item in plan["files"]:
            rel = relative(item["path"])
            target = app_root / rel
            if not target.is_file() or sha256_file(target) != item["sha256"]:
                raise RuntimeError(f"更新后文件校验失败：{item['path']}")

        targets = {str(relative(path)).replace("\\", "/").lower() for path in plan["target_files"]}
        deletion_candidates = {str(relative(path)).replace("\\", "/") for path in plan["obsolete_paths"]}
        for root_name in plan.get("managed_roots", []):
            managed = app_root / relative(root_name)
            if managed.is_dir():
                for path in managed.rglob("*"):
                    if path.is_file():
                        rel_text = path.relative_to(app_root).as_posix()
                        if rel_text.lower() not in targets:
                            deletion_candidates.add(rel_text)
        protected = [str(item) for item in plan.get("protected_prefixes", [])]
        for rel_text in sorted(deletion_candidates, reverse=True):
            if is_protected(rel_text, protected):
                continue
            rel = relative(rel_text)
            target = app_root / rel
            if target.is_file():
                backup = rollback / "deleted" / rel
                backup.parent.mkdir(parents=True, exist_ok=True)
                shutil.move(target, backup)
                removed.append((target, backup))
        write_update_log(app_root, f"更新完成 version={plan['version']}")
        return app_root / str(plan.get("entrypoint", "404Launcher.exe"))
    except Exception as exc:
        write_update_log(app_root, f"更新失败，开始回滚：{type(exc).__name__}: {exc}")
        for rel in reversed(replaced):
            backup = rollback / rel
            target = app_root / rel
            if backup.is_file():
                restore_file(backup, target)
        for rel in reversed(created):
            target = app_root / rel
            if target.is_file():
                target.unlink()
        for target, backup in reversed(removed):
            if backup.is_file():
                target.parent.mkdir(parents=True, exist_ok=True)
                shutil.move(backup, target)
        write_update_log(app_root, "回滚完成")
        raise


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--plan", required=True)
    parser.add_argument("--wait-pid", required=True, type=int)
    parser.add_argument("--ready-file")
    args = parser.parse_args()
    ready_file = Path(args.ready_file).resolve() if args.ready_file else None
    plan_path = Path(args.plan).resolve()
    try:
        entrypoint = apply(plan_path, args.wait_pid, ready_file)
    except Exception:
        try:
            plan = json.loads(plan_path.read_text("utf-8-sig"))
            app_root = Path(plan["app_root"]).resolve()
            entrypoint = app_root / str(plan.get("entrypoint", "404Launcher.exe"))
            write_update_log(app_root, traceback.format_exc().rstrip())
            if entrypoint.is_file():
                write_update_log(app_root, "更新失败，正在重新启动原程序")
                subprocess.Popen([str(entrypoint)], cwd=str(entrypoint.parent), close_fds=True)
        except Exception:
            pass
        return

    else:
        write_update_log(entrypoint.parent, "正在启动更新后的程序")
        subprocess.Popen([str(entrypoint)], cwd=str(entrypoint.parent), close_fds=True)


if __name__ == "__main__":
    main()
