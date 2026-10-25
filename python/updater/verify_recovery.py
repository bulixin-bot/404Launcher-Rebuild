"""Statically compile and compare recovered source; never execute/import it."""
import dis
import hashlib
import json
import marshal
from pathlib import Path
import types


folder = Path(__file__).resolve().parent
original_bytes = (folder / "404_updater.pyc").read_bytes()
source_bytes = (folder / "404_updater.py").read_bytes()
original = marshal.loads(original_bytes[16:])
recovered = compile(source_bytes, "404_updater.py", "exec")


def code_objects(code):
    yield code
    for constant in code.co_consts:
        if isinstance(constant, types.CodeType):
            yield from code_objects(constant)


def constant_key(value):
    if isinstance(value, types.CodeType):
        return ("code", value.co_name)
    if isinstance(value, (tuple, frozenset)):
        items = [constant_key(item) for item in value]
        return (type(value).__name__, sorted(items, key=repr) if isinstance(value, frozenset) else items)
    return value


fields = ["co_code", "co_exceptiontable", "co_names", "co_varnames", "co_freevars", "co_cellvars", "co_argcount", "co_posonlyargcount", "co_kwonlyargcount", "co_flags", "co_stacksize"]
original_objects = list(code_objects(original))
recovered_objects = list(code_objects(recovered))
records = []
for old, new in zip(original_objects, recovered_objects):
    differing_fields = [field for field in fields if getattr(old, field) != getattr(new, field)]
    if [constant_key(c) for c in old.co_consts] != [constant_key(c) for c in new.co_consts]:
        differing_fields.append("co_consts")
    record = {
        "name": old.co_qualname,
        "recovered_name": new.co_qualname,
        "equal_ignoring_source_locations": not differing_fields,
        "differing_fields": differing_fields,
        "original_instruction_count": len(list(dis.get_instructions(old))),
        "recovered_instruction_count": len(list(dis.get_instructions(new))),
    }
    if differing_fields:
        old_instructions = list(dis.get_instructions(old))
        new_instructions = list(dis.get_instructions(new))
        for index, (left, right) in enumerate(zip(old_instructions, new_instructions)):
            if (left.opname, left.arg) != (right.opname, right.arg):
                record["first_instruction_difference"] = {
                    "index": index,
                    "original": [left.offset, left.opname, left.argrepr],
                    "recovered": [right.offset, right.opname, right.argrepr],
                }
                break
    records.append(record)
report = {
    "method": "Python 3.12 marshal parsing and compile only; recovered program not imported or executed",
    "syntax_valid": True,
    "original_pyc_sha256": hashlib.sha256(original_bytes).hexdigest(),
    "recovered_source_sha256": hashlib.sha256(source_bytes).hexdigest(),
    "original_code_object_count": len(original_objects),
    "recovered_code_object_count": len(recovered_objects),
    "all_equal_ignoring_source_locations": len(original_objects) == len(recovered_objects) and all(record["equal_ignoring_source_locations"] for record in records),
    "code_objects": records,
}
(folder / "verification.json").write_text(json.dumps(report, indent=2, ensure_ascii=False), encoding="utf-8")
print(json.dumps(report, indent=2, ensure_ascii=True))
