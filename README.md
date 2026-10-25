# 404Launcher 1.3.8 Rebuild

404Launcher 1.3.8 的 .NET 重建项目。项目提供重建后的 C# 源码及相关工具；项目提供者已验证可以构建。

## 构建

安装支持 C# 14 的 .NET SDK（例如 .NET 10 SDK），然后在项目根目录执行：

```powershell
dotnet build 404Launcher.Rebuild.sln -c Release
```

项目目标框架为 `net9.0`。运行生成的程序集时需要相应的 .NET 运行环境。

## 目录

- `src/`：重建源码，包括 404NEL、Codexus 相关组件及 Downloader。
- `lib/`：项目通过本地引用使用的依赖程序集。
- `tools/MetadataRepair/`：程序集元数据修复工具。
- `tools/OfflineDifferential/`：离线差分验证工具；部分验证需要原始输入文件，仓库未包含 `artifacts/` 下的输入与报告。

构建缓存、日志和历史分析产物未包含在本仓库中。

## 许可

本仓库目前未指定开源许可证。第三方组件的权利及许可归各自权利人所有；公开源码本身不表示授予额外使用、修改或再分发许可。
