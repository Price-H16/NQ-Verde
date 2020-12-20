using System;
using System.Reactive.Linq;
using NosTale.Packets.Packets.FamilyCommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Family.Command
{
    public class FamilyLeavePacketHandler : IPacketHandler
    {
        #region Instantiation

        public FamilyLeavePacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void FamilyLeave(FamilyLeavePacket packet)
        {
            if (Session.Character.Family == null || Session.Character.FamilyCharacter == null) return;

            if (Session.Character.FamilyCharacter.Authority == FamilyAuthority.Head)
            {
                Session.SendPacket(
                    UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("CANNOT_LEAVE_FAMILY")));
                return;
            }

            var familyId = Session.Character.Family.FamilyId;

            DAOFactory.FamilyCharacterDAO.Delete(Session.Character.CharacterId);

            Logger.LogUserEvent("GUILDCOMMAND", Session.GenerateIdentity(),
                $"[FamilyLeave][{Session.Character.Family.FamilyId}]");
            Logger.LogUserEvent("GUILDLEAVE", Session.GenerateIdentity(),
                $"[FamilyLeave][{Session.Character.Family.FamilyId}]");

            Session.Character.Family.InsertFamilyLog(FamilyLogType.FamilyManaged, Session.Character.Name);
            Session.Character.LastFamilyLeave = DateTime.Now.Ticks;
            Session.Character.Family = null;

            ServerManager.Instance.FamilyRefresh(familyId);

            Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(o => ServerManager.Instance.FamilyRefresh(familyId));
            Observable.Timer(TimeSpan.FromSeconds(10)).Subscribe(o => ServerManager.Instance.FamilyRefresh(familyId));

            // Show in real-time
            Session.CurrentMapInstance.Broadcast(Session.Character.GenerateIn());
            Session.CurrentMapInstance.Broadcast(Session.Character.GenerateGidx());
        }

        #endregion
    }
}