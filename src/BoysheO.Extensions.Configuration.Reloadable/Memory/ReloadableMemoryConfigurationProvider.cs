using Microsoft.Extensions.Configuration;

namespace BoysheO.Extensions.Configuration.Reloadable.Memory
{
    /// <summary>
    /// 在外部拿到这个实例后，调用get/set方法修改Data成员。最后调用Reload来刷新
    /// </summary>
    public class ReloadableMemoryConfigurationProvider : ConfigurationProvider
    {
        public void Reload()
        {
            OnReload();
        }
    }
}