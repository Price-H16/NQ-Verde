using OpenNos.Data;
using OpenNos.GameObject.Networking;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.GameObject
{
    public class Account : AccountDTO
    {
        #region Instantiation

        public Account(AccountDTO input)
        {
            AccountId = input.AccountId;
            Authority = input.Authority;
            Email = input.Email;
            Name = input.Name;
            Password = input.Password;
            ReferrerId = input.ReferrerId;
            RegistrationIP = input.RegistrationIP;
            VerificationToken = input.VerificationToken;
            BankMoney = input.BankMoney;
            LastDelete = input.LastDelete;
        }

        #endregion

        #region Properties

        public List<PenaltyLogDTO> PenaltyLogs
        {
            get
            {
                var logs = new PenaltyLogDTO[ServerManager.Instance.PenaltyLogs.Count + 10];
                ServerManager.Instance.PenaltyLogs.CopyTo(logs);
                return logs.Where(s => s != null && s.AccountId == AccountId).ToList();
            }
        }

        #endregion
    }
}