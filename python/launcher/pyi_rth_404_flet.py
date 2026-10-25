from __future__ import annotations

import os
import sys
from pathlib import Path


if getattr(sys, "frozen", False):
    bundled_flet = (
        Path(sys._MEIPASS)
        / "flet_desktop"
        / "app"
        / "flet"
    )
    if (bundled_flet / "flet.exe").is_file():
        os.environ["FLET_VIEW_PATH"] = str(bundled_flet)
        os.environ["FLET_DESKTOP_FLAVOR"] = "full"
