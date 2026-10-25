using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Mono.Cecil;

using var assembly = AssemblyDefinition.ReadAssembly(args[0]);
var changes = new List<object>();
void Repair(IEnumerable<TypeDefinition> types)
{
    foreach (var type in types)
    {
        foreach (var group in type.NestedTypes.GroupBy(t => t.Name).Where(g => g.Count() > 1))
        {
            foreach (var nested in group)
            {
                var old = nested.FullName;
                nested.Name = "RecoveredType_" + nested.MetadataToken.ToInt32().ToString("X8");
                changes.Add(new { Token = nested.MetadataToken.ToInt32(), Original = old, Renamed = nested.FullName });
            }
        }
        foreach (var field in type.Fields.Where(f => f.Name == "<>c"))
        {
            var old = field.FullName;
            field.Name = "RecoveredData_" + field.MetadataToken.ToInt32().ToString("X8");
            changes.Add(new { Token = field.MetadataToken.ToInt32(), Original = old, Renamed = field.FullName });
        }
        Repair(type.NestedTypes);
    }
}
Repair(assembly.MainModule.Types);
assembly.Write(args[1]);
File.WriteAllText(args[2], JsonSerializer.Serialize(changes, new JsonSerializerOptions { WriteIndented = true }));
Console.WriteLine($"Renamed metadata entries: {changes.Count}");
