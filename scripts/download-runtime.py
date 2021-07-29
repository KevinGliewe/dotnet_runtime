#!/usr/bin/env python

import os
import optparse
import json
from sys import platform

parser = optparse.OptionParser()

parser.add_option('--architecture',
    action="store", dest="architecture",
    help="Target Architecture", default="Local")

parser.add_option('--platform',
    action="store", dest="platform",
    help="Target Platform", default="Local")

options, args = parser.parse_args()

backupCwd = os.getcwd()
scriptDirectory = os.path.dirname(os.path.realpath(__file__))
repoDirectory = os.path.join(scriptDirectory, "..")

os.chdir(repoDirectory)

runtimeSettings = json.loads(open(".config/runtime.json").read())
runtimeVersion = runtimeSettings["version"]

print(f"Using repo root : '{repoDirectory}'")
print(f"Version: '{runtimeVersion}'")

print(runtimeVersion)

print("Restore runtimedl")
os.system("dotnet tool restore")

print("Downloading runtime")
os.system(f"dotnet runtimedl --version-pattern \"{runtimeVersion}\" --output \"bin\" --platform {options.platform} --architecture {options.architecture}")

# Restore current directory
os.chdir(backupCwd)