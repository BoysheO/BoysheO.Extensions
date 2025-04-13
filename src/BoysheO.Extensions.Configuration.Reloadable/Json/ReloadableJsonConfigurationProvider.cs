using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace BoysheO.Extensions.Configuration.Reloadable.Json
{
    public class ReloadableJsonConfigurationProvider : ConfigurationProvider
    {
        public void Reload(string json)
        {
            Reload(Encoding.UTF8.GetBytes(json));
        }

        public void Reload(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            Data = JsonConfigurationFileParser.Parse(stream);
            OnReload(); // 触发配置变更
        }
    }
}