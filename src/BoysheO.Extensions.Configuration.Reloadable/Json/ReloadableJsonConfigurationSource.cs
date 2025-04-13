using Microsoft.Extensions.Configuration;

namespace BoysheO.Extensions.Configuration.Reloadable.Json
{
    public class ReloadableJsonConfigurationSource : IConfigurationSource
    {
        private readonly ReloadableJsonConfigurationProvider _provider = new();
    
        public IConfigurationProvider Build(IConfigurationBuilder builder) => _provider;

        public void Reload(string json) => _provider.Reload(json);
        public void Reload(byte[] bytes) => _provider.Reload(bytes);
    }
}