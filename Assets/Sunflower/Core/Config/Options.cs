using CommandLine;
namespace Sunflower.Core
{
    public class Options: Singleton<Options>
    {
        [Option("LogLevel", Required = false, Default = 0)]
        public int LogLevel { get; set; }
    }
}