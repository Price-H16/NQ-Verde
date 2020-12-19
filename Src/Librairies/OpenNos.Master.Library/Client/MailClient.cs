using System.Threading.Tasks;
using OpenNos.Data;
using OpenNos.Master.Library.Interface;

namespace OpenNos.Master.Library.Client
{
    internal class MailClient : IMailClient
    {
        #region Methods

        public void MailSent(MailDTO mail)
        {
            Task.Run(() => MailServiceClient.Instance.OnMailSent(mail));
        }

        #endregion
    }
}