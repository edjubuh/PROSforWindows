using Newtonsoft.Json;
using System;
using System.Configuration;

namespace PROSforWindows.Models.Software
{
    public class InstalledSoftware
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }
        
        public int VersionInteger { get; set; }

        public string Category { get; set; }

        [JsonIgnore]
        public AvailableSoftware AvailableUpdate { get; set; }

        public static InstalledSoftware CreateDefaults()
        {
            return new InstalledSoftware();
        }
    }
}
