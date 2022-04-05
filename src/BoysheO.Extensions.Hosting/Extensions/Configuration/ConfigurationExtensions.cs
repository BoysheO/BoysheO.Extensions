using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace BoysheO.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        public static string PrintAllConfigure(this IConfiguration configuration)
        {
            var sb = new StringBuilder();

            void writeToSb(IEnumerable<IConfigurationSection> cfgs, int deep = 0)
            {
                foreach (var cfg in cfgs)
                {
                    var child = cfg.GetChildren().ToArray();
                    var str = $"[{cfg.Key}]{cfg.Value ?? (child.Length == 0 ? "(null)" : "")}";
                    var line = str.PadLeft(deep * 2 + str.Length);
                    sb.AppendLine(line);
                    writeToSb(child, deep + 1);
                }
            }

            writeToSb(configuration.GetChildren());
            return sb.ToString();
        }
    }
}