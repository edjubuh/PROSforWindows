namespace PROSforWindows.Models.Software
{
    public class AvailableSoftware
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public int VersionInteger { get; set; }

        public string UpdateSource { get; set; }

        public string UpdateUrl { get; set; }
    }
}