"""Run with Python 3.12. Compile and compare; never run recovered code."""
from pathlib import Path
import json
import marshal
import sys
import types

if sys.version_info[:2] != (3, 12):
    raise SystemExit('Python 3.12 is required to compare this bytecode.')

def normalize(value):
    if isinstance(value, types.CodeType):
        return (
            value.co_code, value.co_exceptiontable,
            tuple(normalize(item) for item in value.co_consts),
            value.co_names, value.co_varnames, value.co_freevars,
            value.co_cellvars, value.co_argcount, value.co_posonlyargcount,
            value.co_kwonlyargcount, value.co_flags,
        )
    return value

folder = Path(__file__).resolve().parent / 'launcher'
results = []
for name in ['hardened_entry', 'pyi_rth_404_flet']:
    original = marshal.loads((folder / (name + '.pyc')).read_bytes()[16:])
    recovered = compile((folder / (name + '.py')).read_text('utf-8'), name + '.py', 'exec')
    results.append({'name': name, 'syntax_valid': True, 'bytecode_equal_ignoring_source_locations': normalize(original) == normalize(recovered)})
print(json.dumps(results, indent=2))
raise SystemExit(0 if all(item['bytecode_equal_ignoring_source_locations'] for item in results) else 1)
