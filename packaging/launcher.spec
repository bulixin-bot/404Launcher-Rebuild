import os
from pathlib import Path
from PyInstaller.archive.readers import CArchiveReader

root = Path(SPECPATH).resolve().parent
original = Path(os.environ['REBUILD_INPUT_DIR'])
extracted = Path(os.environ['REBUILD_EXTRACTED_DIR'])
archive = CArchiveReader(str(original / '404Launcher.exe'))
toc = []
for name, item in archive.toc.items():
    kind = item[-1]
    if kind == 'm':
        toc.append((name, str(extracted / (name + '.pyc')), 'PYMODULE'))
    elif kind == 's':
        source = root / 'python/launcher' / (name + '.py')
        if name == 'hardened_entry':
            source = Path(SPECPATH) / 'launcher_entry.py'
        if not source.is_file():
            source = extracted / (name + '.pyc')
        toc.append((name, str(source), 'PYSOURCE'))
    elif kind == 'z':
        toc.append((name, str(extracted / name), 'PYZ'))
    else:
        raise ValueError('Unexpected launcher archive type: ' + kind)

exe = EXE(toc, name='404Launcher', exclude_binaries=True, console=False,
          debug=False, strip=False, upx=False, contents_directory='_internal',
          icon=str(original / '_internal/assets/404-icon.ico'))
