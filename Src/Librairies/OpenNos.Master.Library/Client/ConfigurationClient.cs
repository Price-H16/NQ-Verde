using System.Threading.Tasks;
using OpenNos.Master.Library.Data;
using OpenNos.Master.Library.Interface;

namespace OpenNos.Master.Library.Client
{
    internal class ConfigurationClient : IConfigurationClient
    {
        #region Methods

        public void ConfigurationUpdated(ConfigurationObject configurationObject)
        {
            Task.Run(() => ConfigurationServiceClient.Instance.OnConfigurationUpdated(configurationObject));
        }

        #endregion
    }
}