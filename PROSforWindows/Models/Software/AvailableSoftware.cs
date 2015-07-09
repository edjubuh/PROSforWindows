using System;
using System.Collections.Generic;

namespace PROSforWindows.Models.Software
{
    public class AvailableSoftware
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string AboutUrl { get; set; }

        public string Version { get; set; }

        public int VersionInteger { get; set; }

        public string UpdateSource { get; set; }

        public string UpdateName { get; set; }

        public string DownloadUrl { get; set; }
    }

    public class AvailableSoftwareComparer : IEqualityComparer<AvailableSoftware>
    {
        public bool Equals(AvailableSoftware x, AvailableSoftware y)
        {
            return  x.Key == y.Key &&
                    x.Name == y.Name &&
                    x.Version == y.Version &&
                    x.VersionInteger == y.VersionInteger &&
                    x.DownloadUrl == y.DownloadUrl;
        }

        public int GetHashCode(AvailableSoftware obj)
        {
            throw new NotImplementedException();
        }
    }
}