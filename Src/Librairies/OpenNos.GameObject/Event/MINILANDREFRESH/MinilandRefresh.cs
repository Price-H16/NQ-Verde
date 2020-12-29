using System;
using System.Linq;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;

namespace OpenNos.GameObject.Event
{
    public static class MinilandRefresh
    {
        #region Methods

        public static void GenerateMinilandEvent()
        {
            ServerManager.Instance.SaveAll(true);
            foreach (var chara in DAOFactory.CharacterDAO.LoadAll())
            {
                var gen = DAOFactory.GeneralLogDAO.LoadByAccount(null).LastOrDefault(s =>
                    s.LogData == nameof(MinilandRefresh) && s.LogType == "World" &&
                    s.Timestamp.Day == DateTime.Now.Day);
                var count = DAOFactory.GeneralLogDAO.LoadByAccount(chara.AccountId).Count(s =>
                    s.LogData == "MINILAND" && s.Timestamp > DateTime.Now.AddDays(-1) &&
                    s.CharacterId == chara.CharacterId);

                var Session = ServerManager.Instance.GetSessionByCharacterId(chara.CharacterId);
                if (Session != null)
                {
                    Session.Character.GetReputation(2 * count);
                    Session.Character.MinilandPoint = 2000;
                }
                else if (CommunicationServiceClient.Instance.IsCharacterConnected(ServerManager.Instance.ServerGroup,
                    chara.CharacterId))
                {
                    if (gen == null) chara.Reputation += 2 * count;
                    chara.MinilandPoint = 2000;
                    var chara2 = chara;
                    DAOFactory.CharacterDAO.InsertOrUpdate(ref chara2);
                }
            }

            DAOFactory.GeneralLogDAO.Insert(new GeneralLogDTO
                {LogData = nameof(MinilandRefresh), LogType = "World", Timestamp = DateTime.Now});
            ServerManager.Instance.StartedEvents.Remove(EventType.MINILANDREFRESHEVENT);
        }

        #endregion
    }
}