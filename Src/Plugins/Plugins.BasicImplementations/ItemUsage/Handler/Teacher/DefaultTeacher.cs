using System;
using System.Linq;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._ItemUsage;
using OpenNos.GameObject._ItemUsage.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.GameObject.Extension;


namespace Plugins.BasicImplementations.ItemUsage.Handler.Teacher
{
   public class DefaultTeacher : IUseItemRequestHandlerAsync
    {
        public ItemPluginType Type => ItemPluginType.Teacher;
        
        public long EffectId => default;

        public async Task HandleAsync(ClientSession session, InventoryUseItemEvent e)
        {

            if (session.Character.IsVehicled)
            {
                session.SendPacket(
                    session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_DO_VEHICLED"), 10));
                return;
            }

            if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.TalentArenaMapInstance)
            {
                return;
            }

            if (e.PacketSplit == null)
            {
                return;
            }

            void releasePet(MateType mateType, Guid itemToRemoveId)
            {
                if (int.TryParse(e.PacketSplit[3], out var mateTransportId))
                {
                    var mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId && s.MateType == mateType);
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
                    }
                }
            }

            if (e.Item.Item.BCards.Count > 0 && session.Character.MapInstance == session.Character.Miniland)
            {
                e.Item.Item.BCards.ForEach(c => c.ApplyBCards(session.Character.BattleEntity, session.Character.BattleEntity));
                session.Character.Inventory.RemoveItemFromInventory(e.Item.Id);
                return;
            }

            switch (e.Item.Item.Effect)
            {
                case 10:
                    if (int.TryParse(e.PacketSplit[3], out var mateTransportId))
                    {
                        var mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId);
                        if (mate == null || mate.MateType != MateType.Pet || mate.Loyalty >= 1000)
                        {
                            return;
                        }

                        mate.Loyalty += 100;
                        if (mate.Loyalty > 1000)
                        {
                            mate.Loyalty = 1000;
                        }

                        mate.GenerateXp(e.Item.Item.EffectValue);
                        session.SendPacket(mate.GenerateCond());
                        session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5002));
                        session.Character.Inventory.RemoveItemFromInventory(e.Item.Id);
                    }

                    break;

                case 11:
                    if (int.TryParse(e.PacketSplit[3], out mateTransportId))
                    {
                        var mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId);
                        if (mate == null || mate.MateType != MateType.Pet ||
                            mate.Level >= session.Character.Level - 5 ||
                            mate.Level + 1 > ServerManager.Instance.Configuration.MaxLevel)
                        {
                            return;
                        }

                        mate.Level++;
                        session.CurrentMapInstance?.Broadcast(
                            StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 8), mate.PositionX,
                            mate.PositionY);
                        session.CurrentMapInstance?.Broadcast(
                            StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 198), mate.PositionX,
                            mate.PositionY);
                        session.Character.Inventory.RemoveItemFromInventory(e.Item.Id);
                    }

                    break;

                case 12:
                    if (int.TryParse(e.PacketSplit[3], out mateTransportId))
                    {
                        var mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId);
                        if (mate == null || mate.MateType != MateType.Partner ||
                            mate.Level >= session.Character.Level - 5 ||
                            mate.Level + 1 > ServerManager.Instance.Configuration.MaxLevel)
                        {
                            return;
                        }

                        mate.Level++;
                        session.CurrentMapInstance?.Broadcast(
                            StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 8), mate.PositionX,
                            mate.PositionY);
                        session.CurrentMapInstance?.Broadcast(
                            StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 198), mate.PositionX,
                            mate.PositionY);
                        session.Character.Inventory.RemoveItemFromInventory(e.Item.Id);
                    }

                    break;

                case 13:
                    if (int.TryParse(e.PacketSplit[3], out mateTransportId) &&
                        session.Character.Mates.FirstOrDefault(s => s.MateTransportId == mateTransportId) is Mate pet)
                    {
                        if (pet.MateType == MateType.Pet)
                        {
                            session.SendPacket(UserInterfaceHelper.GenerateGuri(10, 1, mateTransportId, 2));
                        }
                        else
                        {
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                    Language.Instance.GetMessageFromKey("CANT_CHANGE_PARTNER_NAME"), 0));
                        }
                    }

                    break;

                case 14:
                    if (int.TryParse(e.PacketSplit[3], out mateTransportId))
                    {
                        if (session.Character.MapInstance == session.Character.Miniland)
                        {
                            var mate = session.Character.Mates.Find(s =>
                                s.MateTransportId == mateTransportId && s.MateType == MateType.Pet);
                            if (mate?.CanPickUp == false)
                            {
                                session.CurrentMapInstance.Broadcast(
                                    StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5));
                                session.CurrentMapInstance.Broadcast(
                                    StaticPacketHelper.GenerateEff(UserType.Npc, mate.MateTransportId, 5002));
                                mate.CanPickUp = true;
                                session.SendPackets(session.Character.GenerateScP());
                                session.SendPacket(
                                    session.Character.GenerateSay(
                                        Language.Instance.GetMessageFromKey("PET_CAN_PICK_UP"), 10));
                                session.Character.Inventory.RemoveItemFromInventory(e.Item.Id);
                            }
                        }
                        else
                        {
                            session.SendPacket(
                                session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_IN_MINILAND"),
                                    12));
                        }
                    }

                    break;

                case 16:
                    if (int.TryParse(e.PacketSplit[3], out mateTransportId))
                    {
                        var mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId);
                        if (mate == null || mate.MateType != MateType.Pet || mate.Level == 1)
                        {
                            return;
                        }

                        mate.Level--;
                        session.Character.Inventory.RemoveItemFromInventory(e.Item.Id);
                    }

                    break;

                case 17:
                    if (int.TryParse(e.PacketSplit[3], out mateTransportId))
                    {
                        var mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId);
                        if (mate?.IsSummonable == false)
                        {
                            mate.IsSummonable = true;
                            session.SendPackets(session.Character.GenerateScP());
                            session.SendPacket(session.Character.GenerateSay(
                                string.Format(Language.Instance.GetMessageFromKey("PET_SUMMONABLE"), mate.Name), 10));
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(
                                string.Format(Language.Instance.GetMessageFromKey("PET_SUMMONABLE"), mate.Name), 0));
                            session.Character.Inventory.RemoveItemFromInventory(e.Item.Id);
                        }
                    }

                    break;

                case 18:
                    if (int.TryParse(e.PacketSplit[3], out mateTransportId))
                    {
                        var mate = session.Character.Mates.Find(s => s.MateTransportId == mateTransportId);
                        if (mate == null || mate.MateType != MateType.Partner || mate.Level == 1)
                        {
                            return;
                        }

                        mate.Level--;
                        session.Character.Inventory.RemoveItemFromInventory(e.Item.Id);
                    }

                    break;

                case 1000:
                    releasePet(MateType.Pet, e.Item.Id);
                    break;

                case 1001:
                    releasePet(MateType.Partner, e.Item.Id);
                    break;

                default:
                    Logger.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_HANDLER_ITEM"), GetType(), e.Item.Item.VNum,
                        e.Item.Item.Effect, e.Item.Item.EffectValue));
                    break;
            }
        }
    }
}