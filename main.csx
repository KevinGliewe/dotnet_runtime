#! "netcoreapp5.0"

#load "nuget:Dotnet.Build, 0.7.1"

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

using System.Text.Json;

using static FileUtils;
using System.Threading;

var repoDir = GetScriptFolder();

string Get(string url)
{
    Console.WriteLine(" -- get : " + url);
    var client = new HttpClient();
    var result = client.GetStringAsync(url).Result;
    //Console.WriteLine(result);
    //File.WriteAllText("test_" + url.GetHashCode() + ".txt", result);
    return result;
}

var rex = @"href=""(?<href>\/en-us\/download\/dotnet\/thank-you\/(?<type>[a-z-]+)-(?<version>[a-z0-9\.-]+)-(?<platform>[a-z]+)-(?<arch>[a-z]+[0-9]*(-[a-z]+)?)-(?<package>[a-z]+))""[^>]*>[\w\d ]+<\/a>";

var rexUrl = new Regex(@"<a class=""form-control text-left overflow-hidden"" id=""directLink"" href=""(?<uri>\S+)"" aria-label");
var rexChecksum = new Regex(@"<input onClick=""this\.select\(\);"" id=""checksum"" type=""text"" class=""form-control"" readonly value=""(?<checksum>[a-z0-9]+)"" aria-labelledby=""checksum-label"" \/>");


var urlBase = "https://dotnet.microsoft.com";

class Entry {
    public string url { get; set; }
    public string checksum { get; set; }
}

// All -> type -> version -> platform -> arch -> Entry
var d_out = new Dictionary<string, Dictionary<string,Dictionary<string,Dictionary<string,Dictionary<string, Entry>>>>>();

foreach(var major in new [] {"7.0", "6.0", "5.0"}) {

    foreach(Match match in Regex.Matches(Get(urlBase + "/download/dotnet/" + major), rex)) {

        

        var m_href = match.Groups["href"].Value;
        var m_type = match.Groups["type"].Value;
        var m_version = match.Groups["version"].Value;
        var m_platform = match.Groups["platform"].Value;
        var m_arch = match.Groups["arch"].Value;
        var m_package = match.Groups["package"].Value;

        Console.WriteLine(m_href);

        void ProcessEntry() {
            Thread.Sleep(500);

            var content = Get(urlBase + m_href);

            var entry = new Entry() {
                url = rexUrl.Match(content).Groups["uri"].Value,
                checksum = rexChecksum.Match(content).Groups["checksum"].Value,
            };

            if(!d_out.ContainsKey(m_type))
                d_out.Add(m_type, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Entry>>>>());
            var d_type = d_out[m_type];

            if(!d_type.ContainsKey(m_version))
                d_type.Add(m_version, new Dictionary<string, Dictionary<string, Dictionary<string, Entry>>>());
            var d_version = d_type[m_version];

            if(!d_version.ContainsKey(m_platform))
                d_version.Add(m_platform, new Dictionary<string, Dictionary<string, Entry>>());
            var d_platform = d_version[m_platform];

            if(!d_platform.ContainsKey(m_arch))
                d_platform.Add(m_arch, new Dictionary<string, Entry>());
            var d_arch = d_platform[m_arch];

            d_arch.Add(m_package, entry);
        }

        for(int itry = 0; itry < 5; itry++ ) {
            try {
                ProcessEntry();
                break;
            } catch(Exception ex) {
                if(itry == 4)
                    throw ex;
            }
        }
    }
}

int totalEntries = 0;
// All -> type -> version -> platform -> arch -> Entry
foreach(var type in d_out) {
    foreach(var version in type.Value) {
        foreach(var platform in version.Value) {
            if(platform.Value.Count == 0)
                throw new Exception($"Empty List: {type.Key}->{version.Key}->{platform.Key}");
            totalEntries += platform.Value.Count;
        }
    }
}

if(totalEntries == 0)
    throw new Exception("No Entries!");

var jsonData = JsonSerializer.Serialize(d_out, new JsonSerializerOptions() { WriteIndented = true});


var outDir = Path.Combine(repoDir, "out");
var outFile = Path.Combine(outDir, "net.json");

Directory.CreateDirectory(outDir);

File.WriteAllText(outFile, jsonData);
