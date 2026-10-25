# 404Updater Python entry recovery

`404_updater.py` was statically reconstructed from the bundled Python 3.12
`404_updater.pyc`. It contains all 17 top-level functions, the nested Windows
process structure, lambda, generator, and command-line entry point found in
that bytecode. Comments and source formatting are reconstructed, not original.

Validation with the matching Python 3.12 runtime compiled the recovered source
without executing it. All 20 code objects match the original instruction bytes,
constant values, names, variable names, closure metadata, argument counts, flags,
stack sizes, and exception tables. Filename, line numbers, and source-position
tables are intentionally excluded. See `verification.json` and the standalone
`verify_recovery.py` static verification script.

The recovered behavior preserves staged and post-install SHA-256 validation,
update path processing, protected prefixes, process shutdown handling, Windows
file-lock retries, update logging, rollback, and launcher restart logic. No
authentication or entitlement code was modified. Neither the recovered updater
nor the supplied executables were run as part of this recovery.

This is recovery of the updater entry only; it does not recover native Cython
modules or establish that the entire launcher can be rebuilt from source.
