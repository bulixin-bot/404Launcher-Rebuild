# 构建可启动 EXE

这是混合重建：入口和 7 个后端 DLL 从仓库源码构建，7 个 Cython 模块、Flet/Python 依赖与配套更新器复用原包。没有修改登录、订阅或授权逻辑。

## 环境

- Windows x64；Python 3.12 x64。
- .NET SDK 10（C# 14；目标框架 net9.0）。
- 原始 `404Launcher1.3.9.zip`；必须匹配 SHA-256 `ffb959247ca3f4203e8a7da41852072174d70d37d739f8184bb286b63e916a44`。原包提供尚未恢复源码的必要原生组件，仓库本身不包含这些组件。

也可提供原版已解压目录；脚本会校验固定的原始 manifest 和其中 1,207 个文件的内容哈希，仅复制清单文件，排除运行生成的账号、设备和缓存数据。此模式验证文件内容，不声称重新计算过原 ZIP 的哈希。

在仓库根目录执行：

Windows 下建议使用较短的检出路径；原样反编译对照目录包含长文件名，必要时用 `git -c core.longpaths=true clone ...` 启用 Git 长路径支持。

```powershell
py -3.12 -m venv .venv
.\.venv\Scripts\python.exe -m pip install -r packaging/requirements.txt
.\build-exe.ps1 -OriginalArchive 'C:\path\404Launcher1.3.9.zip' -PythonExe '.\.venv\Scripts\python.exe'
```

使用已解压原版目录时，改用：

```powershell
.\build-exe.ps1 -OriginalDirectory 'C:\path\404Launcher1.3.9' -PythonExe '.\.venv\Scripts\python.exe'
```

默认产出：

```text
artifacts/dist/404Launcher1.3.9-Rebuilt-Windows-x64/404Launcher.exe
artifacts/dist/404Launcher1.3.9-Rebuilt-Windows-x64.zip
```

完整解压后双击 EXE，保留 `_internal` 文件夹。程序目录需要可写。脚本拒绝覆盖已有输出；再次构建请用 `-OutputDirectory` 指定新目录。`-SkipRunCheck` 可只构建而不执行环境自检。

## 构建步骤及边界

1. 校验输入 ZIP，安全解压并按原 manifest 修复两个中文资源文件名。
2. 执行 8 个 .NET 后端项目的 Release 构建。
3. 静态读取原 PyInstaller 归档，保留 bootstrap / runtime hook 顺序和原 PYZ；用恢复入口及工作目录修复构建新 EXE。
4. 复制原始运行时，替换 7 个重建的业务 DLL；保留原版强名称签名 `Downloader.dll`，补充 `System.Management` 和 `System.CodeDom` 9.0.0。
5. 在临时工作目录运行新 EXE 的 `--check`，保存退出码和输出；打包后做 ZIP CRC 校验。构建中不执行原版 EXE。

初始运行版在 2026-10-30 实测环境自检通过、.NET 组件加载成功、登录界面正常。该检查不等于真实账号登录、订阅购买、更新或游戏进服全流程验证。

原始反编译入口在 `python/launcher/` 中保持不变，其字节码一致性测试仍然适用；实际 EXE 使用 `packaging/launcher_entry.py`，额外修复了启动工作目录。7 个 Cython 模块没有源码，成品不能称为完整源码重建或完全脱壳。

原配套 `404Updater.exe`、`HARDENING_HASHES.json` 和 `update_manifest.json` 仍保留，两个上游清单描述原版文件，自定义 EXE/DLL 的哈希与其不同。`REBUILD-MANIFEST.json` 才记录本次重建成品的实际哈希；这不表示更改或禁用了授权及完整性校验。
