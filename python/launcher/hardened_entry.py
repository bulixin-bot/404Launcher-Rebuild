# Recovered from Python 3.12 bytecode; formatting/comments are reconstructed.
from __future__ import annotations

import sys

import flet as ft
import gui


async def _main(page: ft.Page) -> None:
    await gui.main(page)


if __name__ == "__main__":
    if "--check" in sys.argv:
        raise SystemExit(gui.check_environment())
    ft.run(_main)
