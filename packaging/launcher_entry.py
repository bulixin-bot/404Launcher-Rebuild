"""Recovered launcher entry with a deterministic writable working directory."""
from __future__ import annotations

import os
from pathlib import Path
import sys

# The native SDK writes relative device/cache paths. Shortcuts must not inherit
# a protected parent working directory such as WindowsApps or System32.
if getattr(sys, 'frozen', False):
    os.chdir(Path(sys.executable).resolve().parent)

import flet as ft
import gui


async def _main(page: ft.Page) -> None:
    await gui.main(page)


if __name__ == '__main__':
    if '--check' in sys.argv:
        raise SystemExit(gui.check_environment())
    ft.run(_main)
