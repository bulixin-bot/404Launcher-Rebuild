# 404Launcher 1.3.9 .NET 后端恢复

> 下文保留本地恢复包的构建记录。在本 GitHub 仓库中，原报告的 `rebuild-src/` 对应根目录 `src/`，原报告的原样 C# `src/` 对应 `reference/decompiled-dotnet/`。重建 DLL 只保留在本地构建交付中，仓库以源码与报告为主。

已用 ILSpy 11.1.0.9782 从原包的 8 个项目程序集导出 387 个 C# 文件和 8 个项目。`src/` 保留原样 C# 反编译结果，仅将项目引用路径标准化为相对 `lib/`。`rebuild-src/` 是明确分开的机械编译修复版本，也有 387 个 C# 文件。

8 个修复后的项目均在 .NET SDK 10.0.300、目标框架 net9.0 下编译通过，共 0 错误、265 警告。`rebuilt-binaries/` 包含此次编译的 8 个后端 DLL。它们是类库，不能单独启动；本目录不包含完整启动器 EXE。各项目使用 `lib/` 中原包的其他程序集作为编译依赖，未验证将所有重建 DLL 同时替换后的运行行为。

| 程序集 | C# 文件 | 最终错误 | 最终警告 |
|---|---:|---:|---:|
| 404NEL | 25 | 0 | 64 |
| Codexus.Protocol772Compat | 10 | 0 | 1 |
| Codexus.Cipher | 160 | 0 | 70 |
| Codexus.Development.SDK | 51 | 0 | 37 |
| Codexus.Game.Launcher | 30 | 0 | 43 |
| Codexus.Interceptors | 16 | 0 | 5 |
| Codexus.OpenSDK | 64 | 0 | 42 |
| Downloader | 31 | 0 | 3 |

机械修复明细：

- 404NEL 的混淆元数据含重复嵌套类型和特殊字段名。在 DLL 副本上只重命名 502 个元数据条目后重新反编译；完整 token/旧名/新名表见 `reports/404NEL-metadata-renames.json`，原始和修复副本 SHA-256 见 `reports/dotnet-recovery.json`。原始 `lib/404NEL.dll` 和 `src/404NEL/` 保留。
- Cipher、OpenSDK、Downloader 去掉各 1 个 `GeneratedRegex` 标记，保留反编译出的完整已生成 Regex 实现，避免源码生成器重复处理。
- OpenSDK 的 `X19GameStart` 初始化改为等价的对象初始化器，保持原有赋值次序，满足 `required` 属性编译规则。
- Protocol772Compat 补充 DotNetty.Common 引用；Interceptors 补充 DotNetty.Codecs 引用。
- Game.Launcher 补充原包缺少的 Microsoft `System.Management` 9.0.0 Windows/net9.0 程序集，将 `ManagementObjectEnumerator` 补全为其真实嵌套类型名 `ManagementObjectCollection.ManagementObjectEnumerator`。

`lib/` 有 27 个逐字节复制的原包 DLL，以及 1 个额外官方依赖 System.Management 9.0.0。该新增依赖来自 https://api.nuget.org/v3-flatcontainer/system.management/9.0.0/system.management.9.0.0.nupkg ，许可证、第三方声明和 nuspec 保存在 `lib/licenses/System.Management-9.0.0/`。它不是从原包恢复的文件。

可运行 `pwsh -File ./build-backend.ps1` 重新构建修复版本，中间文件写到本目录的 `.build/`。每个项目的原始/最终构建日志和输入依赖 SHA-256 均在 `reports/`。本次只做静态反编译和编译，没有运行输入 EXE 或加载这些 DLL 执行业务代码，也未改动登录、授权或时长逻辑。编译通过不代表运行验证通过或完整启动器源码已恢复。
