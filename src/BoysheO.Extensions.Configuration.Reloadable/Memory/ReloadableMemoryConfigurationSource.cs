using Microsoft.Extensions.Configuration;

namespace BoysheO.Extensions.Configuration.Reloadable.Memory
{
    public class ReloadableMemoryConfigurationSource : IConfigurationSource
    {
        public ReloadableMemoryConfigurationProvider Provider { get; } = new();
        
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return Provider;
        }
    }
}