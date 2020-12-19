using System;
using System.Linq;
using System.Reactive.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;

namespace OpenNos.Handler.PacketHandler.Family
{
    public class FamilyDismissPacketHandler : IPacketHandler
    {
        #region Instantiation

        public FamilyDismissPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void FamilyDismiss(FamilyDismissPacket familyDissmissPacket)
        {
            if (Session.Character.Family == null || Session.Character.FamilyCharacter == null
                                                 || Session.Character.FamilyCharacter.Authority != FamilyAuthority.Head)
                return;

            var fam = Session.Character.Family;

            fam.FamilyCharacters.ForEach(s => DAOFactory.FamilyCharacterDAO.Delete(s.Character.CharacterId));
            fam.FamilyLogs.ForEach(s => DAOFactory.FamilyLogDAO.Delete(s.FamilyLogId));
            DAOFactory.FamilyDAO.Delete(fam.FamilyId);
            ServerManager.Instance.FamilyRefresh(fam.FamilyId);

            Logger.LogUserEvent("GUILDDISMISS", Session.GenerateIdentity(), $"[FamilyDismiss][{fam.FamilyId}]");

            var sessions = ServerManager.Instance.Sessions
                .Where(s => s.Character?.Family != null && s.Character.Family.FamilyId == fam.FamilyId).ToList();

            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage
            {
                DestinationCharacterId = fam.FamilyId,
                SourceCharacterId = Session.Character.CharacterId,
                SourceWorldId = ServerManager.Instance.WorldId,
                Message = "fhis_stc",
                Type = MessageType.Family
            });

            Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(observer =>
                sessions.ForEach(s =>
                {
                    if (s?.Character != null) s.CurrentMapInstance?.Broadcast(s.Character.GenerateGidx());
                }));
        }

        #endregion
    }
}