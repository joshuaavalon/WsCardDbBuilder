using CommandLine;
using CommandLine.Text;

namespace WsCardDatabaseBuilder
{
    internal class Option
    {
        [Option('f', "force", HelpText = "Force download ignore cache", Required = false, DefaultValue = false)]
        public bool ForceDownload { get; set; }

        [Option('c', "cache", HelpText = "Disable caching download data", Required = false, DefaultValue = false)]
        public bool DisableCache { get; set; }

        [Option('p', "path", HelpText = "Database output path", Required = false, DefaultValue = "wsdb.db")]
        public string OutputPath { get; set; }

        [Option('v', "version", HelpText = "Version of database", Required = false, DefaultValue = 1)]
        public int Version { get; set; }

        [Option('k', "cachepath", HelpText = "Cache path", Required = false, DefaultValue = "Cache")]
        public string CachePath { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("WS Card Database Builder", "1.0.0"),
                Copyright = new CopyrightInfo("Avalon Group", 2016),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("Usage:");
            help.AddOptions(this);
            return help;
        }
    }
}