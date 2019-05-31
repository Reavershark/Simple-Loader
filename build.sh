#!/bin/sh
set -e

echo Building with these values:
cat Simple-Loader/src/walrus.cs
msbuild /p:Configuration=Release /verbosity:quiet /nologo
if [ -e Simple-Loader.exe ]; then
    rm Simple-Loader.exe
fi
cp Simple-Loader/bin/Release/Simple-Loader.exe .
