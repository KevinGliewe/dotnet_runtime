using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace runtimedl
{
    public class RuntimeDB
    {
        public static readonly string DOWNLOAD_DB = "https://raw.githubusercontent.com/KevinGliewe/dotnet_runtime/downloads-db/net.json";
        public static readonly string PACKAGE_TYPE = "binaries";

        public enum RType {
            Sdk,
            Runtime,
            Runtime_aspnetcore,
            Runtime_Desktop
        }

        public enum Platform {
            Windows,
            Macos,
            Linux,
            Local
        }

        public enum Arch {
            Arm32,
            Arm64,
            Arm64_Alpine,
            X86,
            X64,
            Local
        }

        public class Entry {
            public string url { get; set; }
            public string checksum { get; set; }
        }

        // type -> version -> platform -> arch -> package
        private Dictionary<string, Dictionary<string,Dictionary<string,Dictionary<string,Dictionary<string, Entry>>>>>
            DB = new Dictionary<string, Dictionary<string,Dictionary<string,Dictionary<string,Dictionary<string, Entry>>>>>();

        
        public RuntimeDB() {
            var client = new HttpClient();
            var rawJsonDB = client.GetStringAsync(DOWNLOAD_DB).Result;

            DB = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string,Dictionary<string,Dictionary<string,Dictionary<string, Entry>>>>>>(rawJsonDB);
        }

        public Entry GetEntry(string runtimeType, 
            string platform,
            string architecture,
            string version) {
            
            var m_type = runtimeType.ToString().ToLower().Replace('_', '-');
            var m_version = version;
            var m_platform = platform.ToString().ToLower().Replace('_', '-');
            var m_arch = architecture.ToString().ToLower().Replace('_', '-');
            var m_package = PACKAGE_TYPE;

            
            if(!DB.ContainsKey(m_type))
                throw new Exception("Runtime type not found: " + m_type);
            var d_type = DB[m_type];

            if(!d_type.ContainsKey(m_version))
            foreach(var ver in d_type.Keys)
                if(Regex.IsMatch(ver, version)){
                    m_version = ver;
                    break;
                }

            if(!d_type.ContainsKey(m_version))
                throw new Exception("Version not found: " + m_version);
            var d_version = d_type[m_version];

            if(!d_version.ContainsKey(m_platform))
                throw new Exception("Platform not found: " + m_platform);
            var d_platform = d_version[m_platform];

            if(!d_platform.ContainsKey(m_arch))
                throw new Exception("Architecture not found: " + m_arch);
            var d_arch = d_platform[m_arch];

            return d_arch[m_package];
        }
    }
}