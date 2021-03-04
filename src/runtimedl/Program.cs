using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using ShellProgressBar;

namespace runtimedl
{
    class Program
    {

        static int Main(
            RuntimeDB.RType runtimeType = RuntimeDB.RType.Runtime, 
            RuntimeDB.Platform platform = RuntimeDB.Platform.Local,
            RuntimeDB.Arch architecture = RuntimeDB.Arch.Local,
            string versionPattern = @"^\d+\.\d+\.\d+$",
            DirectoryInfo output = null,
            bool download = true)
        {
            if( output is null )
                output = new DirectoryInfo("./runtime");

            if( platform == RuntimeDB.Platform.Local )
                if ( RuntimeInformation.IsOSPlatform(OSPlatform.Windows) )
                    platform = RuntimeDB.Platform.Windows;
                else if( RuntimeInformation.IsOSPlatform(OSPlatform.Linux) )
                    platform = RuntimeDB.Platform.Linux;
                else if( RuntimeInformation.IsOSPlatform(OSPlatform.OSX) )
                    platform = RuntimeDB.Platform.Macos;
                else
                    throw new Exception("OS is not defined!");
            
            if( architecture == RuntimeDB.Arch.Local )
                if( Environment.Is64BitOperatingSystem )
                    architecture = RuntimeDB.Arch.X64;
                else
                    architecture = RuntimeDB.Arch.X86;

            var m_type = runtimeType.ToString().ToLower().Replace('_', '-');
            var m_platform = platform.ToString().ToLower().Replace('_', '-');
            var m_arch = architecture.ToString().ToLower().Replace('_', '-');

            Console.WriteLine("Runtime Type     : " + m_type);
            Console.WriteLine("Platform         : " + m_platform);
            Console.WriteLine("Architecture     : " + m_arch);
            Console.WriteLine("Version Pattern  : " + versionPattern);
            Console.WriteLine("Output           : " + Path.GetFullPath(output.FullName));

            try {
                var db = new RuntimeDB();
                var entry = db.GetEntry(m_type, m_platform, m_arch, versionPattern);

                Console.WriteLine("Found Entry      :");
                Console.WriteLine("  URL            : " + entry.url);
                Console.WriteLine("  Checksum       : " + entry.checksum);
                Console.WriteLine();

                if(!download)
                    return 0;

                var options = new ProgressBarOptions
                {
                    ProgressCharacter = '─',
                    ProgressBarOnBottom = false,
                    ForegroundColor = Console.ForegroundColor,
                    BackgroundColor = Console.BackgroundColor
                };

                var fileName = entry.url.Split('/')[^1];

                var tempDir = Path.Combine(Path.GetTempPath(), "runtimedl");
                if(!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);
                var tempFile = Path.Combine(tempDir, fileName);

                //if(!File.Exists(tempFile))
                using (var pbar = new ProgressBar(10000, "Downloading to " + tempFile, options))
                {
                    var progress = pbar.AsProgress<float>();
                    WebClient client = new WebClient();
                    client.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) => {
                        progress.Report(((float)e.ProgressPercentage) / 100.0f);
                    };
                    client.DownloadFileAsync(new Uri(entry.url), tempFile);
                    while(client.IsBusy)
                        Thread.Sleep(100);
                }

                Console.WriteLine("Download complete");

                VerifyHash(entry.checksum, tempFile);
                
                Console.WriteLine("Unpacking");
                var outDir = Path.Combine(output.FullName, m_platform, m_arch);
                Directory.CreateDirectory(outDir);

                Unpack(tempFile, outDir);

                Console.WriteLine("Cleaning up");
                File.Delete(tempFile);

                Console.WriteLine("Done");

            } catch(Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error : " + ex.ToString());
                return 1;
            }

            return 0;
        }

        static void VerifyHash(string hash, string file) {
            hash = hash.ToLower();
            using (FileStream stream = File.OpenRead(file))
            {
                var sha = SHA512.Create();
                var hash1 = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-","").ToLower();

                if(hash != hash1)
                    throw new Exception($"Checksum mismatch: {hash} != {hash1}");
                
                Console.WriteLine("Hash OK : " + hash1);
            }
        }

        static void Unpack(string archive, string dest){
            if(archive.EndsWith(".zip"))
                ZipFile.ExtractToDirectory(archive, dest, true);
            else if(archive.EndsWith(".tar"))
                Tar.ExtractTar(archive, dest);
            else if(archive.EndsWith(".tar.gz"))
                Tar.ExtractTarGz(archive, dest);
            else
                throw new Exception("Unknown file extension : " + archive);
        }
    }
}
