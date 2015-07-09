using System.Collections.ObjectModel;

namespace PROSforWindows.Models.Software
{
    public class InstalledSoftware
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }
        
        public int VersionInteger { get; set; }

        public string Category { get; set; }

        public AvailableSoftware AvailableUpdate { get; set; }
    }
}
