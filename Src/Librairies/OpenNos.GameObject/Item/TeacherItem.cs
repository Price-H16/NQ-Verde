using System;
using System.Linq;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject
{
    public class TeacherItem : Item
    {
        #region Instantiation

        public TeacherItem(ItemDTO item) : base(item)
        {
        }

        #endregion

        #region Methods

        public override void Use(ClientSession session, ref ItemInstance inv, byte Option = 0,
            string[] packetsplit = null)
        {

            if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.RainbowBattleInstance)
            {
                return;
            }

            if (session.Character.IsVehicled)
            {
                session.SendPacket(
                    session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_DO_VEHICLED"), 10));
                return;
            }

            if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance) return;

            if (packetsplit == null) return;

            void releasePet(MateType mateType, Guid itemToRemoveId)
            {
                if (int.TryParse(packetsplit[3], out var mateTransportId))
                {
                    var mate = session.Character.Mates.Find(s =>
                        s.MateTransportId == mateTransportId && s.MateType == mateType);
                    if (mate != null)
                    {
                        if (!mate.IsTeamMember)
                        {
                            var mateInventory = mate.GetInventory();
                            if (mateInventory.Count > 0)
                            {
                                session.SendPacket(
                                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("EQ_NOT_EMPTY"),
                                        0));
                            }
                            else
                            {
                                session.Character.Mates.Remove(mate);
                                byte i = 0;
                                session.Character.Mates.Where(s => s.MateType == MateType.Partner).ToList().ForEach(s =>
                                {
                                    s.GetInventory().ForEach(item => item.Type = (InventoryType) (13 + i));
                                    s.PetId = i;
                                    i++;
                                });
                                session.SendPacket(
                                    UserInterfaceHelper.GenerateInfo(
                                        Language.Instance.GetMessageFromKey("PET_RELEASED")));
                                session.SendPacket(UserInterfaceHelper.GeneratePClear());
                                session.SendPackets(session.Character.GenerateScP());
                                session.SendPackets(session.Character.GenerateScN());
                                session.CurrentMapInstance?.Broadcast(mate.GenerateOut());
                                session.Character.Inventory.RemoveItemFromInventory(itemToRemoveId);
                            }
                        }
                        else
                        {
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                Language.Instance.GetMessageFromKey("PET_IN_TEAM_UNRELEASABLE"), 0));
                        }
                        session.Character.Savemates();
                    }
                }
            }

            if (BCards.Count > 0 && session.Character.MapInstance == session.Character.Miniland)
            {
                BCards.ForEach(c => c.ApplyBCards(session.Character.BattleEntity, session.Character.BattleEntity));
                session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                return;
            }

            switch (Effect)
            {
                case 10:
                    if (int.TryParse(packetsplit[3], out int mateTransportId))
                    {
                        Mate mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId);
                        if (mate == null || mate.MateType != MateType.Pet || mate.Loyalty >= 1000)
                        {
                            return;
                        }
                        mate.Loyalty += 100;
                        if (mate.Loyalty > 1000) mate.Loyalty = 1000;
                        mate.GenerateXp(EffectValue);
                        session.SendPacket(mate.GenerateCond());
                        session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5002));
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                case 11:
                    if (int.TryParse(packetsplit[3], out mateTransportId))
                    {
                        Mate mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId);
                        if (mate == null || mate.MateType != MateType.Pet || mate.Level >= session.Character.Level - 5 || mate.Level + 1 > ServerManager.Instance.Configuration.MaxLevel)
                        {
                            return;
                        }
                        mate.Level++;
                        mate.Hp = mate.MaxHp;
                        mate.Mp = mate.MaxMp;
                        session.SendPacket($"say 2 {mate.MateTransportId} 0 " + Language.Instance.GetMessageFromKey("MATE_POWER_INCREASED"));
                        session.SendPackets(session.Character.GenerateScP());
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 8), mate.PositionX, mate.PositionY);
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 198), mate.PositionX, mate.PositionY);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                case 12:
                    if (int.TryParse(packetsplit[3], out mateTransportId))
                    {
                        Mate mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId);
                        if (mate == null || mate.MateType != MateType.Partner || mate.Level >= session.Character.Level - 5 || mate.Level + 1 > ServerManager.Instance.Configuration.MaxLevel)
                        {
                            return;
                        }
                        mate.Level++;
                        mate.Hp = mate.MaxHp;
                        mate.Mp = mate.MaxMp;
                        session.SendPacket($"say 2 {mate.MateTransportId} 0 " + Language.Instance.GetMessageFromKey("MATE_POWER_INCREASED"));
                        session.SendPackets(session.Character.GenerateScN());
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 8), mate.PositionX, mate.PositionY);
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 198), mate.PositionX, mate.PositionY);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                case 13:
                    if (int.TryParse(packetsplit[3], out mateTransportId) && session.Character.Mates.FirstOrDefault(s => s.MateTransportId == mateTransportId) is Mate pet)
                    {
                        if (pet.MateType == MateType.Pet)
                        {
                            session.SendPacket(UserInterfaceHelper.GenerateGuri(10, 1, mateTransportId, 2));
                        }
                        else
                        {
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("CANT_CHANGE_PARTNER_NAME"), 0));
                        }
                    }
                    break;

                case 14:
                    if (int.TryParse(packetsplit[3], out mateTransportId))
                    {
                        if (session.Character.MapInstance == session.Character.Miniland)
                        {
                            Mate mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId && s.MateType == MateType.Pet);
                            if (mate?.CanPickUp == false)
                            {
                                session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5));
                                session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5002));
                                session.SendPacket($"say 2 {mate.MateTransportId} 0 " + Language.Instance.GetMessageFromKey("PET_SMART"));
                                mate.CanPickUp = true;
                                session.SendPackets(session.Character.GenerateScP());
                                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PET_CAN_PICK_UP"), 10));
                                session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                            }
                        }
                        else
                        {
                            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_IN_MINILAND"), 12));
                        }
                    }
                    break;

                case 16:
                    if (int.TryParse(packetsplit[3], out mateTransportId))
                    {
                        Mate mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId);
                        if (mate == null || mate.MateType != MateType.Pet || mate.Level == 1)
                        {
                            return;
                        }
                        mate.Level--;
                        mate.Hp = mate.MaxHp;
                        mate.Mp = mate.MaxMp;
                        session.SendPacket($"say 2 {mate.MateTransportId} 0 " + Language.Instance.GetMessageFromKey("MATE_POWER_DECREASED"));
                        session.SendPackets(session.Character.GenerateScP());
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 198), mate.PositionX, mate.PositionY);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                case 17:
                    if (int.TryParse(packetsplit[3], out mateTransportId))
                    {
                        Mate mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId);
                        if (mate?.IsSummonable == false)
                        {
                            mate.IsSummonable = true;
                            session.SendPackets(session.Character.GenerateScP());
                            session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PET_SUMMONABLE"), mate.Name), 10));
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PET_SUMMONABLE"), mate.Name), 0));
                            session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        }
                    }
                    break;

                case 18:
                    if (int.TryParse(packetsplit[3], out mateTransportId))
                    {
                        Mate mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId);
                        if (mate == null || mate.MateType != MateType.Partner || mate.Level == 1)
                        {
                            return;
                        }
                        mate.Level--;
                        mate.Hp = mate.MaxHp;
                        mate.Mp = mate.MaxMp;
                        session.SendPacket($"say 2 {mate.MateTransportId} 0 " + Language.Instance.GetMessageFromKey("MATE_POWER_DECREASED"));
                        session.SendPackets(session.Character.GenerateScN());
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 198), mate.PositionX, mate.PositionY);
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                    break;

                case 1000:
                    releasePet(MateType.Pet, inv.Id);
                    break;

                case 1001:
                    releasePet(MateType.Partner, inv.Id);
                    break;

                case 1339:
                    int rndevolve = ServerManager.RandomNumber(0, 1000);
                    if (rndevolve > 900)
                    {
                        if (int.TryParse(packetsplit[3], out mateTransportId))
                        {

                            Mate mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId);
                            if (mate == null)
                            {
                                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PICK_PET"), 10));
                                return;
                            }
                            if (mate.Level >= 30 && mate.NpcMonsterVNum == 649 && mate.IsTeamMember == true && mate.MateType == MateType.Pet)
                            {
                                mate.Level = 1;
                                mate.NpcMonsterVNum = 650;
                                session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5));
                                session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5002));
                                session.SendPackets(session.Character.GenerateScP());
                                session.SendPackets(session.Character.GenerateScN());
                                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PET_EVOLED"), 10));
                                if (session?.Character != null)
                                {
                                    if (session.Character.Miniland == session.Character.MapInstance)
                                    {
                                        ServerManager.Instance.JoinMiniland(session, session);
                                    }
                                    else
                                    {
                                        ServerManager.Instance.ChangeMapInstance(session.Character.CharacterId,
                                            session.Character.MapInstanceId, session.Character.PositionX, session.Character.PositionY,
                                            true);
                                        session.SendPacket(StaticPacketHelper.Cancel(2));
                                    }
                                }
                            }
                            else
                            {
                                if (mate.Level >= 30 && mate.NpcMonsterVNum == 648 && mate.IsTeamMember == true && mate.MateType == MateType.Pet)
                                {
                                    mate.Level = 1;
                                    mate.NpcMonsterVNum = 649;
                                    session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5));
                                    session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5002));
                                    session.SendPackets(session.Character.GenerateScP());
                                    session.SendPackets(session.Character.GenerateScN());
                                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PET_EVOLED"), 10));
                                    if (session?.Character != null)
                                    {
                                        if (session.Character.Miniland == session.Character.MapInstance)
                                        {
                                            ServerManager.Instance.JoinMiniland(session, session);
                                        }
                                        else
                                        {
                                            ServerManager.Instance.ChangeMapInstance(session.Character.CharacterId,
                                                session.Character.MapInstanceId, session.Character.PositionX, session.Character.PositionY,
                                                true);
                                            session.SendPacket(StaticPacketHelper.Cancel(2));
                                        }
                                    }
                                }
                                else
                                {
                                    if (mate.Level >= 30 && mate.NpcMonsterVNum == 651 && mate.IsTeamMember == true && mate.MateType == MateType.Pet)
                                    {
                                        mate.Level = 1;
                                        mate.NpcMonsterVNum = 652;
                                        session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5));
                                        session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5002));
                                        session.SendPackets(session.Character.GenerateScP());
                                        session.SendPackets(session.Character.GenerateScN());
                                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PET_EVOLED"), 10));
                                        if (session?.Character != null)
                                        {
                                            if (session.Character.Miniland == session.Character.MapInstance)
                                            {
                                                ServerManager.Instance.JoinMiniland(session, session);
                                            }
                                            else
                                            {
                                                ServerManager.Instance.ChangeMapInstance(session.Character.CharacterId,
                                                    session.Character.MapInstanceId, session.Character.PositionX, session.Character.PositionY,
                                                    true);
                                                session.SendPacket(StaticPacketHelper.Cancel(2));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (mate.Level >= 30 && mate.NpcMonsterVNum == 652 && mate.IsTeamMember == true && mate.MateType == MateType.Pet)
                                        {
                                            mate.Level = 1;
                                            mate.NpcMonsterVNum = 653;
                                            session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5));
                                            session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5002));
                                            session.SendPackets(session.Character.GenerateScP());
                                            session.SendPackets(session.Character.GenerateScN());
                                            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PET_EVOLED"), 10));
                                            if (session?.Character != null)
                                            {
                                                if (session.Character.Miniland == session.Character.MapInstance)
                                                {
                                                    ServerManager.Instance.JoinMiniland(session, session);
                                                }
                                                else
                                                {
                                                    ServerManager.Instance.ChangeMapInstance(session.Character.CharacterId,
                                                        session.Character.MapInstanceId, session.Character.PositionX, session.Character.PositionY,
                                                        true);
                                                    session.SendPacket(StaticPacketHelper.Cancel(2));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (mate.Level >= 30 && mate.NpcMonsterVNum == 660 && mate.IsTeamMember == true && mate.MateType == MateType.Pet)
                                            {
                                                mate.Level = 1;
                                                mate.NpcMonsterVNum = 661;
                                                session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5));
                                                session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5002));
                                                session.SendPackets(session.Character.GenerateScP());
                                                session.SendPackets(session.Character.GenerateScN());
                                                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PET_EVOLED"), 10));
                                                if (session?.Character != null)
                                                {
                                                    if (session.Character.Miniland == session.Character.MapInstance)
                                                    {
                                                        ServerManager.Instance.JoinMiniland(session, session);
                                                    }
                                                    else
                                                    {
                                                        ServerManager.Instance.ChangeMapInstance(session.Character.CharacterId,
                                                            session.Character.MapInstanceId, session.Character.PositionX, session.Character.PositionY,
                                                            true);
                                                        session.SendPacket(StaticPacketHelper.Cancel(2));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (mate.Level >= 30 && mate.NpcMonsterVNum == 661 && mate.IsTeamMember == true && mate.MateType == MateType.Pet)
                                                {
                                                    mate.Level = 1;
                                                    mate.NpcMonsterVNum = 662;
                                                    session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5));
                                                    session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5002));
                                                    session.SendPackets(session.Character.GenerateScP());
                                                    session.SendPackets(session.Character.GenerateScN());
                                                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PET_EVOLED"), 10));
                                                    if (session?.Character != null)
                                                    {
                                                        if (session.Character.Miniland == session.Character.MapInstance)
                                                        {
                                                            ServerManager.Instance.JoinMiniland(session, session);
                                                        }
                                                        else
                                                        {
                                                            ServerManager.Instance.ChangeMapInstance(session.Character.CharacterId,
                                                                session.Character.MapInstanceId, session.Character.PositionX, session.Character.PositionY,
                                                                true);
                                                            session.SendPacket(StaticPacketHelper.Cancel(2));
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (mate.Level >= 30 && mate.NpcMonsterVNum == 657 && mate.IsTeamMember == true && mate.MateType == MateType.Pet)
                                                    {
                                                        mate.Level = 1;
                                                        mate.NpcMonsterVNum = 658;
                                                        session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5));
                                                        session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5002));
                                                        session.SendPackets(session.Character.GenerateScP());
                                                        session.SendPackets(session.Character.GenerateScN());
                                                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PET_EVOLED"), 10));
                                                        if (session?.Character != null)
                                                        {
                                                            if (session.Character.Miniland == session.Character.MapInstance)
                                                            {
                                                                ServerManager.Instance.JoinMiniland(session, session);
                                                            }
                                                            else
                                                            {
                                                                ServerManager.Instance.ChangeMapInstance(session.Character.CharacterId,
                                                                    session.Character.MapInstanceId, session.Character.PositionX, session.Character.PositionY,
                                                                    true);
                                                                session.SendPacket(StaticPacketHelper.Cancel(2));
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (mate.Level >= 30 && mate.NpcMonsterVNum == 658 && mate.IsTeamMember == true && mate.MateType == MateType.Pet)
                                                        {
                                                            mate.Level = 1;
                                                            mate.NpcMonsterVNum = 659;
                                                            session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5));
                                                            session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5002));
                                                            session.SendPackets(session.Character.GenerateScP());
                                                            session.SendPackets(session.Character.GenerateScN());
                                                            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PET_EVOLED"), 10));
                                                            if (session?.Character != null)
                                                            {
                                                                if (session.Character.Miniland == session.Character.MapInstance)
                                                                {
                                                                    ServerManager.Instance.JoinMiniland(session, session);
                                                                }
                                                                else
                                                                {
                                                                    ServerManager.Instance.ChangeMapInstance(session.Character.CharacterId,
                                                                        session.Character.MapInstanceId, session.Character.PositionX, session.Character.PositionY,
                                                                        true);
                                                                    session.SendPacket(StaticPacketHelper.Cancel(2));
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (mate.Level >= 30 && mate.NpcMonsterVNum == 660 && mate.IsTeamMember == true && mate.MateType == MateType.Pet)
                                                            {
                                                                mate.Level = 1;
                                                                mate.NpcMonsterVNum = 661;
                                                                session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5));
                                                                session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5002));
                                                                session.SendPackets(session.Character.GenerateScP());
                                                                session.SendPackets(session.Character.GenerateScN());
                                                                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PET_EVOLED"), 10));
                                                                if (session?.Character != null)
                                                                {
                                                                    if (session.Character.Miniland == session.Character.MapInstance)
                                                                    {
                                                                        ServerManager.Instance.JoinMiniland(session, session);
                                                                    }
                                                                    else
                                                                    {
                                                                        ServerManager.Instance.ChangeMapInstance(session.Character.CharacterId,
                                                                            session.Character.MapInstanceId, session.Character.PositionX, session.Character.PositionY,
                                                                            true);
                                                                        session.SendPacket(StaticPacketHelper.Cancel(2));
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (mate.Level >= 30 && mate.NpcMonsterVNum == 988 && mate.IsTeamMember == true && mate.MateType == MateType.Pet)
                                                                {
                                                                    mate.Level = 1;
                                                                    mate.NpcMonsterVNum = 416;
                                                                    session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5));
                                                                    session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5002));
                                                                    session.SendPackets(session.Character.GenerateScP());
                                                                    session.SendPackets(session.Character.GenerateScN());
                                                                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PET_EVOLED"), 10));
                                                                    if (session?.Character != null)
                                                                    {
                                                                        if (session.Character.Miniland == session.Character.MapInstance)
                                                                        {
                                                                            ServerManager.Instance.JoinMiniland(session, session);
                                                                        }
                                                                        else
                                                                        {
                                                                            ServerManager.Instance.ChangeMapInstance(session.Character.CharacterId,
                                                                                session.Character.MapInstanceId, session.Character.PositionX, session.Character.PositionY,
                                                                                true);
                                                                            session.SendPacket(StaticPacketHelper.Cancel(2));
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (mate.Level >= 30 && mate.NpcMonsterVNum == 416 && mate.IsTeamMember == true && mate.MateType == MateType.Pet)
                                                                    {
                                                                        mate.Level = 1;
                                                                        mate.NpcMonsterVNum = 412;
                                                                        session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5));
                                                                        session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5002));
                                                                        session.SendPackets(session.Character.GenerateScP());
                                                                        session.SendPackets(session.Character.GenerateScN());
                                                                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PET_EVOLED"), 10));
                                                                        if (session?.Character != null)
                                                                        {
                                                                            if (session.Character.Miniland == session.Character.MapInstance)
                                                                            {
                                                                                ServerManager.Instance.JoinMiniland(session, session);
                                                                            }
                                                                            else
                                                                            {
                                                                                ServerManager.Instance.ChangeMapInstance(session.Character.CharacterId,
                                                                                    session.Character.MapInstanceId, session.Character.PositionX, session.Character.PositionY,
                                                                                    true);
                                                                                session.SendPacket(StaticPacketHelper.Cancel(2));
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (mate.Level >= 30 && mate.NpcMonsterVNum == 412 && mate.IsTeamMember == true && mate.MateType == MateType.Pet)
                                                                        {
                                                                            mate.Level = 1;
                                                                            mate.NpcMonsterVNum = 414;
                                                                            session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5));
                                                                            session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5002));
                                                                            session.SendPackets(session.Character.GenerateScP());
                                                                            session.SendPackets(session.Character.GenerateScN());
                                                                            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PET_EVOLED"), 10));
                                                                            if (session?.Character != null)
                                                                            {
                                                                                if (session.Character.Miniland == session.Character.MapInstance)
                                                                                {
                                                                                    ServerManager.Instance.JoinMiniland(session, session);
                                                                                }
                                                                                else
                                                                                {
                                                                                    ServerManager.Instance.ChangeMapInstance(session.Character.CharacterId,
                                                                                        session.Character.MapInstanceId, session.Character.PositionX, session.Character.PositionY,
                                                                                        true);
                                                                                    session.SendPacket(StaticPacketHelper.Cancel(2));
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {

                                                                        }
                                                                    }

                                                                }
                                                            }
                                                            break;
                                                        }
                                                        break;
                                                    }
                                                    break;
                                                }
                                                break;
                                            }
                                            break;
                                        }
                                        break;
                                    }
                                    break;
                                }
                                break;
                            }
                            break;
                        }
                        else
                        {
                            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PET_NOT_EVOLED"), 10));

                        }
                        break;
                    }
                    else
                    {
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PET_NOT_EVOLED"), 10));
                    }
                    break;

                case 2137:
                    int rnd = ServerManager.RandomNumber(0, 1000);
                    {
                        if (int.TryParse(packetsplit[3], out mateTransportId))
                        {

                            Mate mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId);
                            if (mate == null)
                            {
                                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PICK_PET"), 10));
                            }
                            else
                            if (mate.Defence <= 9 || mate.Attack <= 9)
                            {
                                if (rnd >= 600 && rnd <= 800)
                                {
                                    mate.Attack++;
                                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ATTACK_PLUS"), 10));
                                    session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3004), session.Character.MapX, session.Character.MapY);
                                }
                                else if (rnd >= 801 && rnd <= 1000)
                                {
                                    mate.Defence++;
                                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DEF_PLUS"), 10));
                                    session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3004), session.Character.MapX, session.Character.MapY);

                                }
                                else if (rnd <= 800 && rnd >= 400)
                                {
                                    if (mate.Defence == 0)
                                    {
                                        return;
                                    }
                                    else
                                    {
                                        mate.Defence--;
                                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DEF_MINUS"), 10));
                                        session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3005), session.Character.MapX, session.Character.MapY);
                                    }
                                }
                                else if (rnd <= 399 && rnd >= 0)
                                {
                                    if (mate.Attack == 0)
                                    {
                                        return;
                                    }
                                    else
                                    {
                                        mate.Attack--;
                                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ATTACK_MINUS"), 10));
                                        session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3005), session.Character.MapX, session.Character.MapY);
                                    }
                                }
                                else
                                {
                                    return;
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                        session.Character.Inventory.RemoveItemAmount(2079, 1);
                    }
                    break;


                default:
                    Logger.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_HANDLER_ITEM"), GetType(), VNum,
                        Effect, EffectValue));
                    break;
            }
        }

        #endregion
    }
}