using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROSforWindows.Models.Software
{
    public class InstalledSoftware
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }
        
        public int VersionInteger { get; set; }

        public string Category { get; set; }

        public List<AvailableSoftware> AvailableUpdates { get; set; }
    }
}
