# This build script builds the distributions

import subprocess
import os

targets = [
    "win-x64",
    "linux-x64",
    "osx-arm64"
]

starter_wd = os.getcwd()

os.chdir("./RetroPath/RetroPath.Cli")

for t in targets:
    subprocess.run(["dotnet", "publish", "-c", "Release", "-r", t])