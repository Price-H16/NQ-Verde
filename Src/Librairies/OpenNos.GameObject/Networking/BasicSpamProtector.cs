using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.GameObject.Networking
{
    public class BasicSpamProtector : ISpamProtector
    {
        private readonly Dictionary<string, List<DateTime>> _connectionsByIp = new Dictionary<string, List<DateTime>>();
        private readonly TimeSpan _timeBetweenConnection = TimeSpan.FromMilliseconds(1000);

        public bool CanConnect(string ipAddress)
        {
            if (!_connectionsByIp.TryGetValue(ipAddress, out List<DateTime> dates))
            {
                dates = new List<DateTime>();
                _connectionsByIp[ipAddress] = dates;
            }

            DateTime lastConnection = dates.LastOrDefault();
            dates.Add(DateTime.Now);
            if (lastConnection.Add(_timeBetweenConnection) >= DateTime.Now)
            {
                return false;
            }

            return true;
        }
    }
}
