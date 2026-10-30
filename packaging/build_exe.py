"""Build the mixed-source launcher without altering login or authorization."""
from __future__ import annotations

import argparse
import hashlib
import importlib.metadata
import importlib.util
import json
import os
from pathlib import Path, PureWindowsPath
import shutil
import stat
import struct
import subprocess
import sys
import tempfile
import zipfile

EXPECTED_ARCHIVE = 'ffb959247ca3f4203e8a7da41852072174d70d37d739f8184bb286b63e916a44'
EXPECTED_UPDATE_MANIFEST = 'e3a1c078703b632a76fb217e6b75aeb31ac69427381d6fd470fc4bda916cca09'
EXPECTED_HARDENING_MANIFEST = '77eeb76ef5bcdc73597bd74b587f066e885a81f217a435f94737818b643ae585'
BACKENDS = {
    '404NEL': '_internal/python_app/.bridge_runtime',
    'Codexus.Protocol772Compat': '_internal/python_app/.bridge_runtime',
    'Codexus.Cipher': '_internal/SDK',
    'Codexus.Development.SDK': '_internal/SDK',
    'Codexus.Game.Launcher': '_internal/SDK',
    'Codexus.Interceptors': '_internal/SDK',
    'Codexus.OpenSDK': '_internal/SDK',
}


def digest(path: Path) -> str:
    value = hashlib.sha256()
    with path.open('rb') as stream:
        for block in iter(lambda: stream.read(1048576), b''):
            value.update(block)
    return value.hexdigest()


def inside(root: Path, relative: str) -> Path:
    win_path = PureWindowsPath(relative)
    if win_path.drive or win_path.root or '..' in win_path.parts:
        raise ValueError('Unsafe archive/resource path')
    result = (root / relative.replace('\\', '/')).resolve()
    if not result.is_relative_to(root.resolve()):
        raise ValueError('Path outside build directory')
    return result


def extract_archive(archive: Path, destination: Path) -> None:
    destination.mkdir(parents=True)
    with zipfile.ZipFile(archive) as source:
        for item in source.infolist():
            target = inside(destination, item.filename)
            if stat.S_ISLNK(item.external_attr >> 16):
                raise ValueError('Symlinks are not accepted in the input ZIP')
            if item.is_dir():
                target.mkdir(parents=True, exist_ok=True)
            else:
                target.parent.mkdir(parents=True, exist_ok=True)
                with source.open(item) as reader, target.open('xb') as writer:
                    shutil.copyfileobj(reader, writer)
    # Legacy ZIP filename encoding differs from the UTF-8 paths in the manifest.
    manifest = json.loads((destination / 'update_manifest.json').read_text('utf-8-sig'))
    file_index = None
    for record in manifest['files']:
        wanted = inside(destination, record['path'])
        if not wanted.is_file():
            if file_index is None:
                file_index = {}
                for path in destination.rglob('*'):
                    if path.is_file():
                        file_index.setdefault(digest(path), []).append(path)
            matches = [p for p in file_index.get(record['sha256'].lower(), []) if p.is_file() and p.stat().st_size == record['size']]
            if len(matches) != 1:
                raise ValueError('Unable to restore manifest resource filename')
            wanted.parent.mkdir(parents=True, exist_ok=True)
            matches[0].rename(wanted)
        if wanted.stat().st_size != record['size'] or digest(wanted).lower() != record['sha256'].lower():
            raise ValueError('Input resource does not match original manifest')


def copy_runtime_directory(source: Path, destination: Path) -> None:
    """Copy only hash-verified distribution files, never account/device/cache data."""
    manifest = source / 'update_manifest.json'
    hardening = source / 'HARDENING_HASHES.json'
    if digest(manifest) != EXPECTED_UPDATE_MANIFEST or digest(hardening) != EXPECTED_HARDENING_MANIFEST:
        raise ValueError('Runtime directory does not have the validated original manifests.')
    destination.mkdir(parents=True)
    jar_index = None
    for record in json.loads(manifest.read_text('utf-8-sig'))['files']:
        path = inside(source, record['path'])
        if not path.is_file():
            if jar_index is None:
                jar_index = {}
                for jar in source.rglob('*.jar'):
                    jar_index.setdefault(digest(jar), []).append(jar)
            matches = [p for p in jar_index.get(record['sha256'].lower(), []) if p.stat().st_size == record['size']]
            if len(matches) != 1:
                raise ValueError('Missing original runtime file: ' + record['path'])
            path = matches[0]
        if path.stat().st_size != record['size'] or digest(path).lower() != record['sha256'].lower():
            raise ValueError('Modified original runtime file: ' + record['path'])
        target = inside(destination, record['path'])
        target.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(path, target)
    shutil.copy2(manifest, destination / manifest.name)
    shutil.copy2(hardening, destination / hardening.name)


def run_logged(command: list[str], log: Path, cwd: Path, env=None) -> None:
    with log.open('w', encoding='utf-8') as stream:
        result = subprocess.run(command, cwd=cwd, env=env, stdout=stream, stderr=subprocess.STDOUT)
    if result.returncode:
        raise RuntimeError(f'Build command failed ({result.returncode}); see {log}')


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    inputs = parser.add_mutually_exclusive_group(required=True)
    inputs.add_argument('--archive', type=Path)
    inputs.add_argument('--runtime-dir', type=Path)
    parser.add_argument('--output-dir', type=Path)
    parser.add_argument('--skip-check', action='store_true')
    args = parser.parse_args()
    if os.name != 'nt' or sys.version_info[:2] != (3, 12) or struct.calcsize('P') != 8:
        raise SystemExit('Use Windows x64 with Python 3.12 x64.')
    if importlib.metadata.version('pyinstaller') != '6.22.3':
        raise SystemExit('Install packaging/requirements.txt first.')
    root = Path(__file__).resolve().parent.parent
    archive = args.archive.resolve() if args.archive else None
    if archive and digest(archive) != EXPECTED_ARCHIVE:
        raise SystemExit('Input ZIP is not the validated 404Launcher 1.3.9 archive.')
    output = (args.output_dir or root / 'artifacts/dist/404Launcher1.3.9-Rebuilt-Windows-x64').resolve()
    zip_path = output.with_name(output.name + '.zip')
    if output.exists() or zip_path.exists():
        raise SystemExit('Output already exists; choose a new --output-dir. Nothing overwritten.')
    scratch_root = root / 'artifacts/work'
    scratch_root.mkdir(parents=True, exist_ok=True)
    scratch = Path(tempfile.mkdtemp(prefix='exe139_', dir=scratch_root))
    original, extracted = scratch / 'original', scratch / 'extracted'
    print('Verifying and extracting original runtime...', flush=True)
    if archive:
        extract_archive(archive, original)
    else:
        copy_runtime_directory(args.runtime_dir.resolve(), original)
    extracted.mkdir()
    from PyInstaller.archive.readers import CArchiveReader
    reader = CArchiveReader(str(original / '404Launcher.exe'))
    for name, item in reader.toc.items():
        kind = item[-1]
        data = reader.extract(name)
        if kind in {'m', 's'}:
            target = inside(extracted, name + '.pyc')
            data = importlib.util.MAGIC_NUMBER + bytes(12) + data
        elif kind == 'z':
            target = inside(extracted, name)
        else:
            raise ValueError('Unsupported launcher archive entry type')
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_bytes(data)
    print('Building .NET Release projects...', flush=True)
    run_logged(['dotnet', 'build', str(root / '404Launcher.Rebuild.sln'), '-c', 'Release', '--nologo', '-v', 'quiet'], scratch / 'dotnet-build.log', root)
    env = dict(os.environ, REBUILD_INPUT_DIR=str(original), REBUILD_EXTRACTED_DIR=str(extracted))
    work = scratch / 'pyinstaller'
    print('Building Windows executable...', flush=True)
    run_logged([sys.executable, '-m', 'PyInstaller', str(root / 'packaging/launcher.spec'), '--workpath', str(work), '--distpath', str(scratch / 'dist'), '--noconfirm', '--log-level', 'INFO'], scratch / 'pyinstaller-build.log', root, env)
    output.parent.mkdir(parents=True, exist_ok=True)
    shutil.copytree(original, output)
    shutil.copy2(work / 'launcher/404Launcher.exe', output / '404Launcher.exe')
    for name, destination in BACKENDS.items():
        source = root / 'src' / name / 'bin/Release/net9.0' / (name + '.dll')
        shutil.copy2(source, output / destination / source.name)
    # Downloader remains the original strong-named third-party assembly.
    for name in ['System.Management.dll', 'System.CodeDom.dll']:
        for destination in ['_internal/python_app/.bridge_runtime', '_internal/SDK']:
            shutil.copy2(root / 'lib' / name, output / destination / name)
    shutil.copytree(root / 'lib/licenses', output / 'rebuild-licenses')
    deps_file = output / '_internal/python_app/.bridge_runtime/404NEL.deps.json'
    deps = json.loads(deps_file.read_text('utf-8-sig'))
    target = deps['targets'][deps['runtimeTarget']['name']]
    target['Codexus.Game.Launcher/1.0.0.0'].setdefault('dependencies', {})['System.Management'] = '9.0.0'
    target['System.Management/9.0.0'] = {'dependencies': {'System.CodeDom': '9.0.0'}, 'runtime': {'System.Management.dll': {'assemblyVersion': '9.0.0.0'}}}
    target['System.CodeDom/9.0.0'] = {'runtime': {'System.CodeDom.dll': {'assemblyVersion': '9.0.0.0'}}}
    for name in ['System.Management', 'System.CodeDom']:
        deps['libraries'][name + '/9.0.0'] = {'type': 'reference', 'serviceable': False, 'sha512': ''}
    deps_file.write_text(json.dumps(deps, indent=2) + '\n', encoding='utf-8')
    shutil.copy2(root / 'packaging/BUILD-EXE.md', output / 'BUILD-EXE.md')
    # Capture the clean distribution before --check can generate local runtime data.
    files = sorted(p for p in output.rglob('*') if p.is_file())
    check = {'executed': False}
    if not args.skip_check:
        print('Running rebuilt executable environment check...', flush=True)
        result = subprocess.run([str(output / '404Launcher.exe'), '--check'], cwd=scratch, capture_output=True, timeout=60)
        check = {'executed': True, 'exit_code': result.returncode,
                 'stdout': result.stdout.decode('utf-8', errors='replace').strip(),
                 'stderr': result.stderr.decode('utf-8', errors='replace').strip()}
        if result.returncode:
            (scratch / 'environment-check.json').write_text(json.dumps(check, indent=2), encoding='utf-8')
            raise RuntimeError(f'Environment check failed; inspect {scratch}')
    manifest = output / 'REBUILD-MANIFEST.json'
    manifest.write_text(json.dumps({'original_archive_sha256_expected': EXPECTED_ARCHIVE,
        'input_validation': 'original_zip_sha256' if archive else 'pinned_manifests_and_file_hashes',
        'build_kind': 'mixed_rebuild_with_original_native_dependencies', 'rebuilt_business_dlls': 7,
        'original_cython_business_modules': 7, 'environment_check': check,
        'files': [{'path': p.relative_to(output).as_posix(), 'size': p.stat().st_size, 'sha256': digest(p)} for p in files]}, ensure_ascii=False, indent=2), encoding='utf-8')
    files.append(manifest)
    print('Creating and checking portable ZIP...', flush=True)
    with zipfile.ZipFile(zip_path, 'w', zipfile.ZIP_DEFLATED, compresslevel=6) as z:
        for path in files:
            z.write(path, Path(output.name) / path.relative_to(output))
    with zipfile.ZipFile(zip_path) as z:
        if z.testzip():
            raise RuntimeError('ZIP integrity check failed')
    print(json.dumps({'exe': str(output / '404Launcher.exe'), 'zip': str(zip_path), 'sha256': digest(zip_path), 'environment_check': check}, ensure_ascii=True, indent=2))


if __name__ == '__main__':
    main()
