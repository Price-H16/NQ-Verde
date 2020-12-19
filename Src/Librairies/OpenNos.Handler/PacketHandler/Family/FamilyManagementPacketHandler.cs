﻿using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Family
{
    public class FamilyManagementPacketHandler : IPacketHandler
    {
        #region Instantiation

        public FamilyManagementPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void FamilyManagement(FamilyManagementPacket familyManagementPacket)
        {
            if (Session.Character.Family == null)
            {
                return;
            }

            Logger.LogUserEvent("GUILDMGMT", Session.GenerateIdentity(),
                $"[FamilyManagement][{Session.Character.Family.FamilyId}]TargetId: {familyManagementPacket.TargetId} AuthorityType: {familyManagementPacket.FamilyAuthorityType.ToString()}");

            if (Session.Character.FamilyCharacter.Authority == FamilyAuthority.Member
                || Session.Character.FamilyCharacter.Authority == FamilyAuthority.Familykeeper)
            {
                return;
            }

            var targetId = familyManagementPacket.TargetId;
            if (DAOFactory.FamilyCharacterDAO.LoadByCharacterId(targetId)?.FamilyId
                != Session.Character.FamilyCharacter.FamilyId)
            {
                return;
            }

            var famChar = DAOFactory.FamilyCharacterDAO.LoadByCharacterId(targetId);
            if (famChar.Authority == familyManagementPacket.FamilyAuthorityType)
            {
                return;
            }

            switch (familyManagementPacket.FamilyAuthorityType)
            {
                case FamilyAuthority.Head:
                    if (Session.Character.FamilyCharacter.Authority != FamilyAuthority.Head)
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NOT_FAMILY_HEAD")));
                        return;
                    }

                    if (famChar.Authority != FamilyAuthority.Familydeputy)
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(
                                Language.Instance.GetMessageFromKey("ONLY_PROMOTE_ASSISTANT")));
                        return;
                    }

                    famChar.Authority = FamilyAuthority.Head;
                    DAOFactory.FamilyCharacterDAO.InsertOrUpdate(ref famChar);

                    Session.Character.Family.Warehouse.ForEach(s =>
                    {
                        s.CharacterId = famChar.CharacterId;
                        DAOFactory.ItemInstanceDAO.InsertOrUpdate(s);
                    });
                    Session.Character.FamilyCharacter.Authority = FamilyAuthority.Familydeputy;
                    var chara2 = Session.Character.FamilyCharacter;
                    DAOFactory.FamilyCharacterDAO.InsertOrUpdate(ref chara2);
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("DONE")));
                    break;

                case FamilyAuthority.Familydeputy:
                    if (Session.Character.FamilyCharacter.Authority != FamilyAuthority.Head)
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("NOT_FAMILY_HEAD")));
                        return;
                    }

                    if (famChar.Authority == FamilyAuthority.Head)
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("HEAD_UNDEMOTABLE")));
                        return;
                    }

                    if (DAOFactory.FamilyCharacterDAO.LoadByFamilyId(Session.Character.Family.FamilyId)
                            .Count(s => s.Authority == FamilyAuthority.Familydeputy) == 2)
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(
                                Language.Instance.GetMessageFromKey("ALREADY_TWO_ASSISTANT")));
                        return;
                    }

                    famChar.Authority = FamilyAuthority.Familydeputy;
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("DONE")));

                    DAOFactory.FamilyCharacterDAO.InsertOrUpdate(ref famChar);
                    break;

                case FamilyAuthority.Familykeeper:
                    if (famChar.Authority == FamilyAuthority.Head)
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("HEAD_UNDEMOTABLE")));
                        return;
                    }

                    if (famChar.Authority == FamilyAuthority.Familydeputy
                        && Session.Character.FamilyCharacter.Authority != FamilyAuthority.Head)
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(
                                Language.Instance.GetMessageFromKey("ASSISTANT_UNDEMOTABLE")));
                        return;
                    }

                    famChar.Authority = FamilyAuthority.Familykeeper;
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("DONE")));
                    DAOFactory.FamilyCharacterDAO.InsertOrUpdate(ref famChar);
                    break;

                case FamilyAuthority.Member:
                    if (famChar.Authority == FamilyAuthority.Head)
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("HEAD_UNDEMOTABLE")));
                        return;
                    }

                    if (famChar.Authority == FamilyAuthority.Familydeputy
                        && Session.Character.FamilyCharacter.Authority != FamilyAuthority.Head)
                    {
                        Session.SendPacket(
                            UserInterfaceHelper.GenerateInfo(
                                Language.Instance.GetMessageFromKey("ASSISTANT_UNDEMOTABLE")));
                        return;
                    }

                    famChar.Authority = FamilyAuthority.Member;
                    Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("DONE")));

                    DAOFactory.FamilyCharacterDAO.InsertOrUpdate(ref famChar);
                    break;
            }

            var character = DAOFactory.CharacterDAO.LoadById(targetId);
            var targetSession = ServerManager.Instance.GetSessionByCharacterId(targetId);

            Session.Character.Family.InsertFamilyLog(FamilyLogType.AuthorityChanged, Session.Character.Name,
                character.Name, authority: familyManagementPacket.FamilyAuthorityType);
            targetSession?.CurrentMapInstance?.Broadcast(targetSession?.Character.GenerateGidx());
            if (familyManagementPacket.FamilyAuthorityType == FamilyAuthority.Head)
            {
                Session.CurrentMapInstance?.Broadcast(Session.Character.GenerateGidx());
            }
        }

        #endregion
    }
}