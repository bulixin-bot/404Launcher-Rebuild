# 404Launcher 1.3.9 Rebuild

404Launcher 1.3.9 的静态反编译与部分源码恢复工程。此提交将仓库当前源码从 1.3.8 升级为 1.3.9；旧版本仍保留在 Git 历史中。

**当前不能认定为完全脱壳或完整启动器源码恢复。** PyInstaller 解包已验证，但界面、启动逻辑等 7 个业务 Cython `.pyd` 仍为原生机器码，尚未恢复为 Python/Cython 源码。Cython 是本机代码编译形式，其存在本身不能直接判为仍有“壳”。.NET 后端仍保留混淆名称及 4 行 ILSpy 类型推断警告注释；整个程序从源码重建与运行等价性尚未证实。详见 `reports/unpacking-audit.md`。

## 已恢复内容

- `src/`：8 个 .NET 后端项目，387 个 C# 文件，采用单独保存的机械编译修复版本。
- `reference/decompiled-dotnet/`：原样 C# 反编译结果，供对照；不是默认构建目标。
- `python/launcher/`：启动器入口及 Flet 启动配置；忽略源码位置后，与原始 Python 3.12 字节码完全一致。
- `python/updater/`：更新器入口，17 个顶层函数，20/20 个代码对象的指令、常量和异常表与原字节码一致。
- `lib/`：编译引用，包括原包依赖及额外官方 `System.Management` 9.0.0；来源和许可证见 `reports/dotnet-recovery.md` 与 `lib/licenses/`。
- `reports/`：输入哈希、字节码验证、编译结果、元数据重命名与脱壳范围审计。
- `packaging/`、`build-exe.ps1`：可启动 EXE 的混合重建配方，含工作目录修复与环境自检。

7 个业务 `.pyd` 的原始哈希及状态列在 `reports/native-modules.json`。本仓库未将这些未恢复的原生模块或整套第三方 Python PYZ 归档作为源码上传。

## 构建后端

需要支持 C# 14 的 .NET SDK（已使用 .NET SDK 10.0.300 验证），目标框架为 `net9.0`：

```powershell
dotnet build 404Launcher.Rebuild.sln -c Release
```

或执行 `./build.ps1`。产出是 8 个后端 DLL，不是可直接启动的 EXE。各项目仍引用 `lib/` 中的原始其他程序集；单项目编译通过不代表全部重建 DLL 同时替换后的运行行为已验证。

本仓库布局已完成一次 Release 构建：8 个项目、0 错误、16 警告。早期独立恢复包的 Debug 构建报告另有 265 个警告，见报告说明。

## 构建可启动 EXE

可用 `build-exe.ps1` 构建 Windows x64 运行版。需 Python 3.12、.NET SDK 10、`packaging/requirements.txt` 中的 PyInstaller，以及原始 1.3.9 ZIP 或经过文件哈希校验的原版目录：

```powershell
py -3.12 -m venv .venv
.\.venv\Scripts\python.exe -m pip install -r packaging/requirements.txt
.\build-exe.ps1 -OriginalArchive 'C:\path\404Launcher1.3.9.zip' -PythonExe '.\.venv\Scripts\python.exe'
```

默认生成 `artifacts/dist/404Launcher1.3.9-Rebuilt-Windows-x64/404Launcher.exe` 及完整 ZIP。保留整个运行目录，不能只复制 EXE。已解压原包也可用 `-OriginalDirectory`；详细步骤与验证边界见 [EXE 构建说明](packaging/BUILD-EXE.md)。

这是真实可启动的混合重建版：入口及 7 个业务后端 DLL 从源码构建，7 个 Cython 模块和必要运行时复用原版。第三方 Downloader 保留原强名称签名依赖，补入官方 System.Management / System.CodeDom 9.0.0 及许可证。登录、订阅和授权逻辑未作绕过修改。

## 完整性与限制

输入压缩包 SHA-256：`ffb959247ca3f4203e8a7da41852072174d70d37d739f8184bb286b63e916a44`。

此前的源码恢复审计采用静态 `marshal` / `dis` / `compile` 检查，没有运行原始 EXE 或执行业务 `.pyd`。运行 `python python/verify_launcher.py` 和 `python python/updater/verify_recovery.py` 可在 Python 3.12 下复核原始入口恢复结果。后续 EXE 构建验证单独记录在 `reports/exe-build-validation.json`，其中执行了新 EXE 的环境自检，并复用了原生模块。

独立审计确认：启动器/更新器的 CArchive 可解析和解压，PYZ 逐字节匹配，3 个 Python 入口共 23 个代码对象一致。251 处 `Not supported: data(...)` 注释对应的恢复数组与原 DLL 的 RVA 数据逐项匹配；这些注释不能直接认定为数据丢失。

编译修复包含 404NEL 的 502 项元数据改名、正则表达式生成器重复处理修复、等价对象初始化器调整及依赖补齐。没有进行运行时行为等价测试；尚无从完整恢复源码重新构建的启动器 EXE。

## 许可

目前未指定本项目开源许可证。公开代码不自动授予使用、修改或再分发许可；第三方组件遵循各自许可。
