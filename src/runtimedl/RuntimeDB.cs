using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

using SemanticVersioning;

namespace runtimedl
{
    public class RuntimeDB
    {
        public static readonly string RELEASES_INDEX_URL = "https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/releases-index.json";
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

        #region JSON model classes

        private class ReleasesIndexRoot {
            [JsonPropertyName("releases-index")]
            public List<ChannelEntry> ReleasesIndex { get; set; }
        }

        private class ChannelEntry {
            [JsonPropertyName("channel-version")]
            public string ChannelVersion { get; set; }

            [JsonPropertyName("releases.json")]
            public string ReleasesJsonUrl { get; set; }
        }

        private class ChannelReleasesRoot {
            [JsonPropertyName("releases")]
            public List<Release> Releases { get; set; }
        }

        private class Release {
            [JsonPropertyName("release-version")]
            public string ReleaseVersion { get; set; }

            [JsonPropertyName("runtime")]
            public Component Runtime { get; set; }

            [JsonPropertyName("sdk")]
            public Component Sdk { get; set; }

            [JsonPropertyName("aspnetcore-runtime")]
            public Component AspnetcoreRuntime { get; set; }

            [JsonPropertyName("windowsdesktop")]
            public Component Windowsdesktop { get; set; }
        }

        private class Component {
            [JsonPropertyName("version")]
            public string Version { get; set; }

            [JsonPropertyName("files")]
            public List<FileEntry> Files { get; set; }
        }

        private class FileEntry {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("rid")]
            public string Rid { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("hash")]
            public string Hash { get; set; }
        }

        #endregion

        private static readonly Dictionary<string, string> PlatformToRidPrefix = new Dictionary<string, string> {
            { "windows", "win" },
            { "macos", "osx" },
            { "linux", "linux" }
        };

        private static readonly Dictionary<string, string> ArchToRidSuffix = new Dictionary<string, string> {
            { "arm32", "arm" },
            { "arm64", "arm64" },
            { "x86", "x86" },
            { "x64", "x64" }
        };

        // type string -> accessor for the component on a Release
        private static readonly Dictionary<string, Func<Release, Component>> TypeToComponent = new Dictionary<string, Func<Release, Component>> {
            { "sdk", r => r.Sdk },
            { "runtime", r => r.Runtime },
            { "runtime-aspnetcore", r => r.AspnetcoreRuntime },
            { "runtime-desktop", r => r.Windowsdesktop }
        };

        // All releases across all channels
        private List<Release> _allReleases = new List<Release>();

        public RuntimeDB() {
            var client = new HttpClient();

            var indexJson = client.GetStringAsync(RELEASES_INDEX_URL).Result;
            var index = JsonSerializer.Deserialize<ReleasesIndexRoot>(indexJson);

            foreach (var channel in index.ReleasesIndex) {
                if (string.IsNullOrEmpty(channel.ReleasesJsonUrl))
                    continue;

                try {
                    var channelJson = client.GetStringAsync(channel.ReleasesJsonUrl).Result;
                    var channelReleases = JsonSerializer.Deserialize<ChannelReleasesRoot>(channelJson);
                    if (channelReleases?.Releases != null)
                        _allReleases.AddRange(channelReleases.Releases);
                } catch {
                    // Skip channels that fail to load
                }
            }
        }

        public Entry GetEntry(string runtimeType,
            string platform,
            string architecture,
            string version,
            bool includePrerelease) {

            var m_type = runtimeType.ToLower().Replace('_', '-');
            var m_version = new SemanticVersioning.Range(version);
            var m_platform = platform.ToLower().Replace('_', '-');
            var m_arch = architecture.ToLower().Replace('_', '-');

            if (!TypeToComponent.ContainsKey(m_type))
                throw new Exception("Runtime type not found: " + m_type);

            var componentAccessor = TypeToComponent[m_type];
            var rid = BuildRid(m_platform, m_arch);

            // Collect all versioned entries for this type
            Entry bestEntry = null;
            string bestVersionStr = null;

            foreach (var release in _allReleases) {
                var component = componentAccessor(release);
                if (component?.Version == null || component.Files == null)
                    continue;

                string componentVersion = component.Version;

                SemanticVersioning.Version semVer;
                try {
                    semVer = new SemanticVersioning.Version(componentVersion);
                } catch {
                    continue;
                }

                if (!m_version.IsSatisfied(semVer, includePrerelease))
                    continue;

                // Find the binary file matching the RID
                var file = FindBinaryFile(component.Files, rid, m_platform);
                if (file == null)
                    continue;

                // Keep the best (highest) matching version
                if (bestVersionStr == null ||
                    new SemanticVersioning.Version(componentVersion) > new SemanticVersioning.Version(bestVersionStr)) {
                    bestVersionStr = componentVersion;
                    bestEntry = new Entry { url = file.Url, checksum = file.Hash };
                }
            }

            if (bestEntry == null)
                throw new Exception($"No matching entry found for type={m_type}, platform={m_platform}, arch={m_arch}, version={version}");

            return bestEntry;
        }

        private static string BuildRid(string platform, string arch) {
            // Handle alpine variants: x64-alpine -> linux-musl-x64, arm64-alpine -> linux-musl-arm64
            if (arch.EndsWith("-alpine")) {
                var baseArch = arch.Replace("-alpine", "");
                if (ArchToRidSuffix.ContainsKey(baseArch))
                    baseArch = ArchToRidSuffix[baseArch];
                return "linux-musl-" + baseArch;
            }

            if (!PlatformToRidPrefix.ContainsKey(platform))
                throw new Exception("Platform not found: " + platform);

            var ridArch = ArchToRidSuffix.ContainsKey(arch) ? ArchToRidSuffix[arch] : arch;
            return PlatformToRidPrefix[platform] + "-" + ridArch;
        }

        private static FileEntry FindBinaryFile(List<FileEntry> files, string rid, string platform) {
            foreach (var file in files) {
                if (file.Rid != rid)
                    continue;

                // Skip installers and composite builds
                if (string.IsNullOrEmpty(file.Name))
                    continue;
                if (file.Name.Contains("-composite-"))
                    continue;

                // Select binary archives only
                if (platform == "windows") {
                    if (file.Name.EndsWith(".zip"))
                        return file;
                } else {
                    if (file.Name.EndsWith(".tar.gz"))
                        return file;
                }
            }
            return null;
        }
    }
}
