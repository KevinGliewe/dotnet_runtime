#!/bin/bash

ARCHITECTURE="Local"
PLATFORM="Local"
VERSION="6.0.0"

POSITIONAL=()

while [[ $# -gt 0 ]]; do
  key="$1"

  case $key in
    -architecture)
      ARCHITECTURE="$2"
      shift # past argument
      shift # past value
      ;;
    -platform)
      PLATFORM="$2"
      shift # past argument
      shift # past value
      ;;
    *)    # unknown option
      POSITIONAL+=("$1") # save it in an array for later
      shift # past argument
      ;;
  esac
done

# Safe the current working directory
BACKUPCWD=$(pwd)

# Change to the directory of the repo
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"
cd $SCRIPT_DIR/..

# Restore the tools (runtimedl)
dotnet tool restore

dotnet runtimedl --version-pattern "$VERSION" --output "bin" --platform $PLATFORM --architecture $ARCHITECTURE # --download false

# Go back to the working directory
cd $BACKUPCWD