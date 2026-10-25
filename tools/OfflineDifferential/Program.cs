using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Mono.Cecil;
using Mono.Cecil.Cil;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
internal static class Program
{
    static readonly string[] Names = ["404NEL", "Codexus.Cipher", "Codexus.Development.SDK", "Codexus.Game.Launcher", "Codexus.Interceptors", "Codexus.OpenSDK", "Codexus.Protocol772Compat", "Downloader"];
    static readonly JsonSerializerOptions Json = new() { WriteIndented = true };
    static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes));
    static string FileHash(string path) => Hash(File.ReadAllBytes(path));
    static void Save(string path, object value) => File.WriteAllText(path, JsonSerializer.Serialize(value, Json));
    static IEnumerable<TypeDefinition> Types(IEnumerable<TypeDefinition> types) => types.SelectMany(t => new[] { t }.Concat(Types(t.NestedTypes)));
    static bool Visible(TypeDefinition t) => t.DeclaringType == null ? t.IsPublic : Visible(t.DeclaringType) && (t.IsNestedPublic || t.IsNestedFamily || t.IsNestedFamilyOrAssembly);
    static bool Visible(MethodDefinition m) => m.IsPublic || m.IsFamily || m.IsFamilyOrAssembly;
    static string Attr(Mono.Cecil.ICustomAttributeProvider p) => string.Join(";", p.CustomAttributes.Select(a => a.AttributeType.FullName + ":" + Hash(a.GetBlob())).Order(StringComparer.Ordinal));
    static string Constant(object? value) => Hash(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value)));
    static string Generic(IEnumerable<GenericParameter> gs) => string.Join(";", gs.Select(g => $"{g.Position}:{g.Name}:{g.Attributes}:{string.Join(",", g.Constraints.Select(c => c.ConstraintType.FullName).Order())}:{Attr(g)}"));
    static SortedDictionary<string, string> Api(AssemblyDefinition a)
    {
        var result = new SortedDictionary<string, string>(StringComparer.Ordinal);
        void Add(string key, string value)
        {
            if (!result.TryAdd(key, value)) result[key] += "|DUPLICATE|" + value;
        }
        Add("assembly", $"{a.Name.FullName}|{Attr(a)}");
        foreach (var t in Types(a.MainModule.Types).Where(Visible))
        {
            Add("T:" + t.FullName, $"{t.Attributes}|{t.BaseType?.FullName}|{string.Join(",", t.Interfaces.Select(i => i.InterfaceType.FullName).Order())}|{Generic(t.GenericParameters)}|{t.PackingSize}:{t.ClassSize}|{Attr(t)}");
            foreach (var m in t.Methods.Where(Visible))
                Add("M:" + m.FullName, $"{m.Attributes}|{m.ImplAttributes}|{m.CallingConvention}|{Generic(m.GenericParameters)}|{Attr(m)}|{Attr(m.MethodReturnType)}|{string.Join(";", m.Parameters.Select(p => $"{p.Name}:{p.Attributes}:{(p.HasConstant ? Constant(p.Constant) : "none")}:{Attr(p)}"))}");
            foreach (var f in t.Fields.Where(f => f.IsPublic || f.IsFamily || f.IsFamilyOrAssembly))
                Add("F:" + f.FullName, $"{f.Attributes}|{f.Offset}|{(f.HasConstant ? Constant(f.Constant) : "none")}|{Attr(f)}");
            foreach (var p in t.Properties.Where(p => (p.GetMethod != null && Visible(p.GetMethod)) || (p.SetMethod != null && Visible(p.SetMethod))))
                Add("P:" + p.FullName, $"{p.Attributes}|{p.GetMethod?.Attributes}|{p.SetMethod?.Attributes}|{Attr(p)}");
            foreach (var e in t.Events.Where(e => (e.AddMethod != null && Visible(e.AddMethod)) || (e.RemoveMethod != null && Visible(e.RemoveMethod))))
                Add("E:" + e.FullName, $"{e.Attributes}|{e.AddMethod?.Attributes}|{e.RemoveMethod?.Attributes}|{Attr(e)}");
        }
        foreach (var t in a.MainModule.ExportedTypes) Add("forward:" + t.FullName, $"{t.Attributes}|{t.Scope}");
        return result;
    }
    sealed record Spec(string Assembly, string Type, string Method, string[] Parameters, List<object?[]> Cases);
    static List<Spec> Specs()
    {
        var specs = new List<Spec>();
        var bytes = new byte[][] { [], [0], [255], [0, 127, 128, 255], Enumerable.Range(0, 256).Select(i => (byte)i).ToArray() };
        foreach (string assembly in new[] { "Codexus.Cipher", "Codexus.OpenSDK" })
        {
            string type = assembly + ".Extensions.ByteArrayExtensions";
            var pairs = bytes.Select(b => new object?[] { b, b.Reverse().ToArray() }).ToList();
            pairs.AddRange([new object?[] { new byte[] { 1 }, Array.Empty<byte>() }, new object?[] { null, new byte[] { 1 } }, new object?[] { new byte[] { 1 }, null }]);
            specs.Add(new(assembly, type, "Xor", ["System.Byte[]", "System.Byte[]"], pairs));
            specs.Add(new(assembly, type, "CombineWith", ["System.Byte[]", "System.Byte[]"], pairs));
        }
        specs.Add(new("Codexus.Cipher", "Codexus.Cipher.Extensions.ByteArrayExtensions", "ComputeMd5AsHex", ["System.Byte[]"], bytes.Select(b => new object?[] { b }).Append([null]).ToList()));
        string mpay = "Codexus.Cipher.Extensions.MPayExtensions";
        foreach (string method in new[] { "EncodeMd5", "EncodeBase64", "DecodeHex" })
            specs.Add(new("Codexus.Cipher", mpay, method, ["System.String"], new string?[] { null, "", "abc", "00", "00ffA1", "GG", "测试", "a\0b", new('a', 4096) }.Select(s => new object?[] { s }).ToList()));
        foreach (string method in new[] { "EncodeMd5", "EncodeHex" })
            specs.Add(new("Codexus.Cipher", mpay, method, ["System.Byte[]"], bytes.Select(b => new object?[] { b }).Append([null]).ToList()));
        string netty = "Codexus.Development.SDK.Extensions.NettyExtensions";
        var integers = new List<int> { int.MinValue, -1, 0, 1, 127, 128, 16383, 16384, 2097151, 2097152, 268435455, 268435456, int.MaxValue };
        var random = new Random(404138);
        integers.AddRange(Enumerable.Range(0, 128).Select(_ => (int)random.NextInt64(int.MinValue, (long)int.MaxValue + 1)));
        specs.Add(new("Codexus.Development.SDK", netty, "GetVarIntSize", ["System.Int32"], integers.Select(i => new object?[] { i }).ToList()));
        var encoded = integers.Select(i => { var list = new List<byte>(); uint v = (uint)i; do { byte b = (byte)(v & 127); v >>= 7; list.Add((byte)(b | (v != 0 ? 128 : 0))); } while (v != 0); return new object?[] { list.ToArray() }; }).ToList();
        encoded.AddRange([new object?[] { null }, new object?[] { Array.Empty<byte>() }, new object?[] { new byte[] { 128 } }, new object?[] { Enumerable.Repeat((byte)128, 6).ToArray() }]);
        specs.Add(new("Codexus.Development.SDK", netty, "ReadVarInt", ["System.Byte[]"], encoded));
        return specs;
    }
    static MethodDefinition? Find(AssemblyDefinition a, Spec s) => Types(a.MainModule.Types).FirstOrDefault(t => t.FullName == s.Type)?.Methods.FirstOrDefault(m => m.IsPublic && m.IsStatic && m.Name == s.Method && m.Parameters.Select(p => p.ParameterType.FullName).SequenceEqual(s.Parameters));
    static readonly HashSet<string> SafeBcl = ["System.ArgumentException", "System.ArgumentNullException", "System.IndexOutOfRangeException", "System.Exception", "System.Convert", "System.String", "System.Text.Encoding", "System.Security.Cryptography.MD5", "System.Linq.Enumerable"];
    static List<string> Audit(MethodDefinition method)
    {
        var reasons = new HashSet<string>();
        var visited = new HashSet<string>();
        void Walk(MethodDefinition m)
        {
            if (!visited.Add(m.FullName)) return;
            if (!m.HasBody || m.IsPInvokeImpl) { reasons.Add("No managed body"); return; }
            if (m.DeclaringType.Methods.Any(x => x.IsConstructor && x.IsStatic)) reasons.Add("Type initializer present");
            if (m.Module.Types.Any(t => t.Name == "<Module>" && t.Methods.Any(x => x.Name == ".cctor"))) reasons.Add("Module initializer present");
            foreach (var i in m.Body.Instructions)
            {
                if (i.OpCode.Code is Code.Calli or Code.Stsfld or Code.Ldsfld or Code.Ldsflda or Code.Jmp or Code.Ldftn or Code.Ldvirtftn) reasons.Add("Disallowed opcode: " + i.OpCode.Code);
                if (i.Operand is FieldReference) reasons.Add("Field access not approved");
                if (i.Operand is MethodReference call)
                {
                    if (call.DeclaringType.Scope == m.Module)
                    {
                        var target = Types(m.Module.Types).SelectMany(t => t.Methods).FirstOrDefault(x => x.FullName == call.FullName);
                        if (target == null) reasons.Add("Unresolved local call"); else Walk(target);
                    }
                    else
                    {
                        string type = call.DeclaringType.FullName;
                        string scope = call.DeclaringType.Scope.Name;
                        if (!SafeBcl.Contains(type) || !(scope == "mscorlib" || scope == "netstandard" || scope.StartsWith("System.", StringComparison.Ordinal))) reasons.Add("Unapproved external call: " + type + "::" + call.Name);
                    }
                }
            }
        }
        Walk(method);
        return reasons.Order().ToList();
    }
    sealed class IsolatedContext(string root, string lib) : AssemblyLoadContext(isCollectible: true)
    {
        protected override Assembly? Load(AssemblyName name)
        {
            if (name.Name!.StartsWith("System.", StringComparison.Ordinal) || name.Name is "System" or "netstandard" or "mscorlib") return null;
            foreach (string dir in new[] { root, lib })
            {
                string path = Path.Combine(dir, name.Name + ".dll");
                if (File.Exists(path)) return LoadFromAssemblyPath(path);
            }
            throw new FileNotFoundException();
        }
    }
    sealed record Outcome(string Status, string? ValueHash, string? ExceptionType, bool InputUnchanged);
    static string Encode(object? o) => o is byte[] b ? "bytes:" + Convert.ToHexString(b) : JsonSerializer.Serialize(o);
    static Outcome Invoke(Assembly assembly, MethodDefinition method, object?[] input)
    {
        var args = input.Select(x => x is byte[] b ? b.ToArray() : x).ToArray();
        string before = string.Join("|", args.Select(Encode));
        try
        {
            var resolved = (MethodInfo)assembly.ManifestModule.ResolveMethod(method.MetadataToken.ToInt32())!;
            object? output;
            try { output = resolved.Invoke(null, args); }
            catch (TargetInvocationException e) { return new("exception", null, e.InnerException?.GetType().FullName, before == string.Join("|", args.Select(Encode))); }
            return new("return", Hash(Encoding.UTF8.GetBytes(Encode(output))), null, before == string.Join("|", args.Select(Encode)));
        }
        catch (Exception e) { return new("harness-error", null, e.GetType().FullName, true); }
    }
    static int Main(string[] args)
    {
        try { return Run(args); }
        catch (Exception e) { Console.Error.WriteLine("Offline tool failed: " + e.GetType().FullName); return 2; }
    }
    static int Run(string[] args)
    {
        string root = Path.GetFullPath(args[0]);
        string original = Path.GetFullPath(args[1]);
        string output = Path.Combine(root, "artifacts", "offline-differential");
        Directory.CreateDirectory(output);
        string snapshots = Path.Combine(output, "snapshots");
        var modules = new Dictionary<string, (AssemblyDefinition Original, AssemblyDefinition Rebuilt)>();
        var manifests = new List<object>();
        var summaries = new List<object>();
        var originalPaths = new Dictionary<string, string>();
        var baseline = new Dictionary<string, string>();
        int apiChanges = 0;
        foreach (string name in Names)
        {
            var found = Directory.EnumerateFiles(original, name + ".dll", SearchOption.AllDirectories).ToArray();
            if (found.Length != 1) throw new InvalidOperationException();
            string source = found[0];
            string rebuilt = Path.Combine(root, "src", name, "bin", "Release", "net9.0", name + ".dll");
            originalPaths[name] = source;
            baseline[name] = FileHash(source);
            var pairPaths = new List<string>();
            foreach (var pair in new[] { ("original", source), ("rebuilt", rebuilt) })
            {
                string dir = Path.Combine(snapshots, pair.Item1);
                Directory.CreateDirectory(dir);
                string dest = Path.Combine(dir, name + ".dll");
                File.Copy(pair.Item2, dest, true);
                pairPaths.Add(dest);
                manifests.Add(new { Assembly = name, Side = pair.Item1, Source = pair.Item2, Sha256 = FileHash(dest), Snapshot = dest });
            }
            var left = AssemblyDefinition.ReadAssembly(pairPaths[0]);
            var right = AssemblyDefinition.ReadAssembly(pairPaths[1]);
            modules[name] = (left, right);
            var a = Api(left); var b = Api(right);
            var missing = a.Keys.Except(b.Keys).ToArray();
            var added = b.Keys.Except(a.Keys).ToArray();
            var changed = a.Keys.Intersect(b.Keys).Where(k => a[k] != b[k]).Select(k => new { Key = k, Original = a[k], Rebuilt = b[k] }).ToArray();
            apiChanges += missing.Length + added.Length + changed.Length;
            Save(Path.Combine(output, name + ".api.json"), new { Original = a, Rebuilt = b, Missing = missing, Added = added, Changed = changed });
            summaries.Add(new { Assembly = name, OriginalEntries = a.Count, RebuiltEntries = b.Count, Missing = missing.Length, Added = added.Length, Changed = changed.Length, PublicMethods = Types(left.MainModule.Types).Where(Visible).SelectMany(t => t.Methods).Count(Visible) });
        }
        Save(Path.Combine(output, "manifest.json"), manifests);
        var review = new List<object>();
        var results = new List<object>();
        int passed = 0, failed = 0, skipped = 0, tested = 0;
        var contexts = new Dictionary<string, IsolatedContext>();
        var loaded = new Dictionary<string, Assembly>();
        var originalOut = Console.Out; var originalErr = Console.Error;
        foreach (var spec in Specs())
        {
            var pair = modules[spec.Assembly];
            var a = Find(pair.Original, spec); var b = Find(pair.Rebuilt, spec);
            var reasons = a == null || b == null ? new List<string> { "Public static signature missing" } : Audit(a).Concat(Audit(b)).Distinct().ToList();
            string id = spec.Type + "::" + spec.Method + "(" + string.Join(",", spec.Parameters) + ")";
            review.Add(new { Method = id, Approved = reasons.Count == 0, Reasons = reasons, Cases = spec.Cases.Count, Policy = "Reviewed source and both IL call graphs; no fields, initializers, native calls, IO, networking, logging or unapproved external calls" });
            if (reasons.Count != 0) { skipped += spec.Cases.Count; continue; }
            tested++;
            Assembly Load(string side)
            {
                string key = side + ":" + spec.Assembly;
                if (loaded.TryGetValue(key, out var assembly)) return assembly;
                if (!contexts.TryGetValue(side, out var context)) contexts[side] = context = new(Path.Combine(snapshots, side), Path.Combine(root, "lib"));
                return loaded[key] = context.LoadFromAssemblyPath(Path.Combine(snapshots, side, spec.Assembly + ".dll"));
            }
            Console.SetOut(TextWriter.Null); Console.SetError(TextWriter.Null);
            try
            {
                var left = Load("original"); var right = Load("rebuilt");
                for (int i = 0; i < spec.Cases.Count; i++)
                {
                    var x = Invoke(left, a!, spec.Cases[i]); var y = Invoke(right, b!, spec.Cases[i]);
                    bool equal = x == y && x.Status != "harness-error" && x.InputUnchanged && y.InputUnchanged;
                    if (equal) passed++; else failed++;
                    results.Add(new { Method = id, Case = i, Passed = equal, Original = x, Rebuilt = y });
                }
            }
            finally { Console.SetOut(originalOut); Console.SetError(originalErr); }
        }
        var unchanged = originalPaths.All(p => FileHash(p.Value) == baseline[p.Key]);
        Save(Path.Combine(output, "pure-function-review.json"), review);
        Save(Path.Combine(output, "differential.json"), results);
        var summary = new { Assemblies = summaries, ApiDifferenceEntries = apiChanges, CandidateMethods = Specs().Count, TestedMethods = tested, PassedCases = passed, FailedCases = failed, SkippedCases = skipped, OriginalAssemblyHashesUnchanged = unchanged, Safety = new { SyntheticInputsOnly = true, ValuesAndExceptionMessagesSuppressed = true, NetworkCallsAllowed = false, OsNetworkSandbox = false, OriginalCodeExecutedFromCopiesOnly = true }, Limitations = new[] { "Dynamic coverage is an explicit pure-function whitelist, not full application equivalence", "No login, launcher, network, file IO, stateful or native routines tested", "Attribute blobs and constant values are compared by SHA256, not printed", "Metadata covers public/protected types and members, signatures, flags, generic constraints, attributes, defaults and forwarders; marshaling/security descriptors and reference identity inside type signatures are not exhaustive", "Original integrity check covers the eight DLLs, not unrelated application files" } };
        Save(Path.Combine(output, "summary.json"), summary);
        Console.WriteLine(JsonSerializer.Serialize(new { Assemblies = Names.Length, ApiDifferenceEntries = apiChanges, TestedMethods = tested, PassedCases = passed, FailedCases = failed, SkippedCases = skipped, OriginalAssemblyHashesUnchanged = unchanged }, Json));
        foreach (var p in modules.Values) { p.Original.Dispose(); p.Rebuilt.Dispose(); }
        foreach (var c in contexts.Values) c.Unload();
        return failed == 0 && unchanged ? 0 : 1;
    }
}

