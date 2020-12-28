using System;
using System.Linq;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.GameObject
{
    public class PotionItem : Item
    {
        #region Instantiation

        public PotionItem(ItemDTO item) : base(item)
        {
        }

        #endregion

        #region Methods

        public override void Use(ClientSession session, ref ItemInstance inv, byte Option = 0,
            string[] packetsplit = null)
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
                (session.CurrentMapInstance.Map.MapTypes.OrderByDescending(s => s.PotionDelay).FirstOrDefault()
                        ?.PotionDelay ?? 750))
            {
                return;
            }

            if (session.CurrentMapInstance.MapInstanceType == MapInstanceType.RainbowBattleInstance)
            {
                return;
            }

            if (session.CurrentMapInstance.MapInstanceType.Equals(MapInstanceType.TalentArenaMapInstance) &&
                VNum != 5935
             || session.CurrentMapInstance.MapInstanceType.Equals(MapInstanceType.IceBreakerInstance))
            {
                return;
            }

            if (session.CurrentMapInstance.MapInstanceType != MapInstanceType.TalentArenaMapInstance &&
                VNum == 5935)
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
             && (inv.ItemVNum == 1242 || inv.ItemVNum == 1243 || inv.ItemVNum == 1244 || inv.ItemVNum == 5582 ||
                 inv.ItemVNum == 5583 || inv.ItemVNum == 5584 || inv.ItemVNum == 2345 || inv.ItemVNum == 1122 || inv.ItemVNum == 1011))
            {
                return;
            }

            if (session.Character.MapInstance?.MapInstanceType == MapInstanceType.ArenaInstance && 
               (inv.ItemVNum == 1242 || inv.ItemVNum == 1243 || inv.ItemVNum == 1244 || inv.ItemVNum == 2345 || inv.ItemVNum == 1011 || inv.ItemVNum == 1122))
            {

                session.SendPacket(session.Character.GenerateSay("Yeah nice try, but you can't.", 11));
                return;
            }

            session.Character.LastPotion = DateTime.Now;

            switch (Effect)
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

                        var buffRc = session.Character.GetBuff(BCardType.CardType.LeonaPassiveSkill,
                                         (byte) AdditionalTypes.LeonaPassiveSkill.IncreaseRecoveryItems)[0] / 100D;

                        var hpAmount = Hp + (int) (Hp * buffRc);
                        var mpAmount = Mp + (int) (Mp * buffRc);

                        if (session.Character.Hp + hpAmount > hpLoad)
                        {
                            hpAmount = hpLoad - session.Character.Hp;
                        }

                        if (session.Character.Mp + mpAmount > mpLoad)
                        {
                            mpAmount = mpLoad - session.Character.Mp;
                        }

                        var convertRecoveryToDamage = ServerManager.RandomNumber() <
                                                      session.Character.GetBuff(BCardType.CardType.DarkCloneSummon,
                                                              (byte) AdditionalTypes.DarkCloneSummon
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

                        switch (inv.ItemVNum)
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

                        int hpAmount = Hp;
                        int mpAmount = Mp;

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

                        switch (inv.ItemVNum)
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
                        session.Character.Inventory.RemoveItemFromInventory(inv.Id);
                    }
                }
                    break;
            }
        }

        #endregion
    }
}