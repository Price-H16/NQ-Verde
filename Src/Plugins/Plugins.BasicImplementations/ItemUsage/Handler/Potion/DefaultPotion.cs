using System;
using System.Linq;
using System.Threading.Tasks;
using ChickenAPI.Enums.Game.BCard;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._ItemUsage;
using OpenNos.GameObject._ItemUsage.Event;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace Plugins.BasicImplementations.ItemUsage.Handler.Produce
{
   public class DefaultPotion : IUseItemRequestHandlerAsync
    {
        public ItemPluginType Type => ItemPluginType.Potion;
        
        public long EffectId => default;

        public async Task HandleAsync(ClientSession session, InventoryUseItemEvent e)
        {

            if (!session.HasCurrentMapInstance)
            {
                return;
            }

            if (session.Character.IsLaurenaMorph())
            {
                return;
            }

            if ((DateTime.Now - session.Character.LastPotion).TotalMilliseconds < 
                (session.CurrentMapInstance.Map.MapTypes.OrderByDescending(s => s.PotionDelay).FirstOrDefault()?.PotionDelay ?? 750))
            {
                return;
            }

            if (session.CurrentMapInstance.MapInstanceType.Equals(MapInstanceType.TalentArenaMapInstance) 
                && e.Item.Item.VNum != 5935 || session.CurrentMapInstance.MapInstanceType.Equals(MapInstanceType.IceBreakerInstance))
            {
                return;
            }

            if (session.CurrentMapInstance.MapInstanceType != MapInstanceType.TalentArenaMapInstance && e.Item.Item.VNum == 5935)
            {
                return;
            }

            if (ServerManager.Instance.ChannelId == 51
                && session.Character.MapId != 130
                && session.Character.MapId != 131
                && (session.Character.Group?.Raid == null || !session.Character.Group.Raid.InstanceBag.Lock)
                && session.Character.MapInstance.MapInstanceType != MapInstanceType.Act4Berios
                && session.Character.MapInstance.MapInstanceType != MapInstanceType.Act4Calvina
                && session.Character.MapInstance.MapInstanceType != MapInstanceType.Act4Hatus
                && session.Character.MapInstance.MapInstanceType != MapInstanceType.Act4Morcos
                && (e.Item.ItemVNum == 1242 || e.Item.ItemVNum == 1243 || e.Item.ItemVNum == 1244 || e.Item.ItemVNum == 5582 ||
                    e.Item.ItemVNum == 5583 || e.Item.ItemVNum == 5584 || e.Item.ItemVNum == 2345))
            {
                return;
            }

            if (ServerManager.Instance.ChannelId == 51 && e.Item.Item.VNum == 1122 && e.Item.Item.VNum == 1011)
            {
                return;
            }

            session.Character.LastPotion = DateTime.Now;

            switch (e.Item.Item.Effect)
            {
                default:
                {
                    var hasPotionBeenUsed = false;

                    var hpLoad = (int) session.Character.HPLoad();
                    var mpLoad = (int) session.Character.MPLoad();

                    if (session.Character.Hp > 0
                        && (session.Character.Hp < hpLoad || session.Character.Mp < mpLoad))
                    {
                        hasPotionBeenUsed = true;

                        var buffRc = session.Character.GetBuff(BCardType.LeonaPassiveSkill,
                                         (byte) BCardSubTypes.LeonaPassiveSkill.IncreaseRecoveryItems)[0] / 100D;

                        var hpAmount = e.Item.Item.Hp + (int) (e.Item.Item.Hp * buffRc);
                        var mpAmount = e.Item.Item.Mp + (int) (e.Item.Item.Mp * buffRc);

                        if (session.Character.Hp + hpAmount > hpLoad)
                        {
                            hpAmount = hpLoad - session.Character.Hp;
                        }

                        if (session.Character.Mp + mpAmount > mpLoad)
                        {
                            mpAmount = mpLoad - session.Character.Mp;
                        }

                        var convertRecoveryToDamage = ServerManager.RandomNumber() <
                                                      session.Character.GetBuff(BCardType.DarkCloneSummon,
                                                              (byte) BCardSubTypes.DarkCloneSummon
                                                                                    .ConvertRecoveryToDamage)[0];

                        if (convertRecoveryToDamage)
                        {
                            session.CurrentMapInstance.Broadcast(session.Character.GenerateDm(hpAmount));

                            session.Character.Hp -= hpAmount;

                            if (session.Character.Hp < 1)
                            {
                                session.Character.Hp = 1;
                            }
                        }
                        else
                        {
                            session.CurrentMapInstance.Broadcast(session.Character.GenerateRc(hpAmount));

                            session.Character.Hp += hpAmount;
                        }

                        session.Character.Mp += mpAmount;

                        switch (e.Item.ItemVNum)
                        {
                            // Full HP Potion
                            case 1242:
                            case 5582:
                            {
                                if (convertRecoveryToDamage)
                                {
                                    session.CurrentMapInstance.Broadcast(
                                        session.Character.GenerateDm(session.Character.Hp - 1));
                                    session.Character.Hp = 1;
                                }
                                else
                                {
                                    session.CurrentMapInstance.Broadcast(
                                        session.Character.GenerateRc(hpLoad - session.Character.Hp));
                                    session.Character.Hp = hpLoad;
                                }
                            }
                                break;

                            // Full MP Potion
                            case 1243:
                            case 5583:
                            {
                                session.Character.Mp = mpLoad;
                            }
                                break;

                            // Full HP & MP Potion
                            case 1244:
                            case 5584:
                            case 9129:
                            {
                                if (convertRecoveryToDamage)
                                {
                                    session.CurrentMapInstance.Broadcast(
                                        session.Character.GenerateDm(session.Character.Hp - 1));
                                    session.Character.Hp = 1;
                                }
                                else
                                {
                                    session.CurrentMapInstance.Broadcast(
                                        session.Character.GenerateRc(hpLoad - session.Character.Hp));
                                    session.Character.Hp = hpLoad;
                                }

                                session.Character.Mp = mpLoad;
                            }
                                break;
                        }

                        session.SendPacket(session.Character.GenerateStat());
                    }

                    foreach (var mate in session.Character.Mates.Where(s => s.IsTeamMember && s.IsAlive))
                    {
                        hpLoad = (int) mate.MaxHp;
                        mpLoad = (int) mate.MaxMp;

                        if (mate.Hp <= 0 || mate.Hp == hpLoad && mate.Mp == mpLoad)
                        {
                            continue;
                        }

                        hasPotionBeenUsed = true;

                        int hpAmount = e.Item.Item.Hp;
                        int mpAmount = e.Item.Item.Mp;

                        if (mate.Hp + hpAmount > hpLoad)
                        {
                            hpAmount = hpLoad - (int) mate.Hp;
                        }

                        if (mate.Mp + mpAmount > mpLoad)
                        {
                            mpAmount = mpLoad - (int) mate.Mp;
                        }

                        mate.Hp += hpAmount;
                        mate.Mp += mpAmount;

                        session.CurrentMapInstance.Broadcast(mate.GenerateRc(hpAmount));

                        switch (e.Item.ItemVNum)
                        {
                            // Full HP Potion
                            case 1242:
                            case 5582:
                                session.CurrentMapInstance.Broadcast(mate.GenerateRc(hpLoad - (int) mate.Hp));
                                mate.Hp = hpLoad;
                                break;

                            // Full MP Potion
                            case 1243:
                            case 5583:
                                mate.Mp = mpLoad;
                                break;

                            // Full HP & MP Potion
                            case 1244:
                            case 5584:
                            case 9129:
                                session.CurrentMapInstance.Broadcast(mate.GenerateRc(hpLoad - (int) mate.Hp));
                                mate.Hp = hpLoad;
                                mate.Mp = mpLoad;
                                break;
                        }

                        session.SendPacket(mate.GenerateStatInfo());
                    }

                    if (session.Character.Mates.Any(m => m.IsTeamMember && m.IsAlive))
                    {
                        session.SendPackets(session.Character.GeneratePst());
                    }

                    if (hasPotionBeenUsed)
                    {
                        session.Character.Inventory.RemoveItemFromInventory(e.Item.Id);
                    }
                }
                    break;
            }
        }
    }
}