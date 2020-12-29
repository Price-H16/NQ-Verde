using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._ItemUsage;
using OpenNos.GameObject._ItemUsage.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.GameObject.Extension;


namespace Plugins.BasicImplementations.ItemUsage.Handler.Snack
{
   public class DefaultSnack : IUseItemRequestHandlerAsync
    {
        public ItemPluginType Type => ItemPluginType.Snack;
        
        public long EffectId => default;
        
        private static IDisposable _regenerateDisposable { get; set; }

        public async Task HandleAsync(ClientSession session, InventoryUseItemEvent e)
        {

            if (session.Character.IsVehicled)
            {
                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_DO_VEHICLED"), 10));
                return;
            }

            if (session.CurrentMapInstance?.MapInstanceType != MapInstanceType.TalentArenaMapInstance && e.Item.Item.VNum == 2802)
            {
                return;
            }

            if (session.CurrentMapInstance?.MapInstanceType == MapInstanceType.TalentArenaMapInstance && e.Item.Item.VNum != 2802)
            {
                return;
            }

            var item = e.Item.Item;
            switch (e.Item.Item.Effect)
            {
                default:
                    if (session.Character.Hp <= 0)
                    {
                        return;
                    }
                    if (item.BCards.Find(s => s.Type == 25) is BCard Buff)
                    {
                        if (ServerManager.RandomNumber() < Buff.FirstData)
                        {
                            session.Character.AddBuff(new Buff((short) Buff.SecondData, session.Character.Level),
                                    session.Character.BattleEntity);
                        }

                        session.Character.Inventory.RemoveItemFromInventory(e.Item.Id);
                    }
                    else
                    {
                        if (session.Character.SnackAmount < 0)
                        {
                            session.Character.SnackAmount = 0;
                        }

                        var amount = session.Character.SnackAmount;
                        if (amount < 5)
                        {
                            var workerThread = new Thread(() => Regenerate(session, item));
                            workerThread.Start();
                            session.Character.Inventory.RemoveItemFromInventory(e.Item.Id);
                        }
                        else
                        {
                            session.SendPacket(session.Character.Gender == GenderType.Female
                                ? session.Character.GenerateSay(
                                    Language.Instance.GetMessageFromKey("NOT_HUNGRY_FEMALE"), 1)
                                : session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_HUNGRY_MALE"),
                                    1));
                        }

                        if (amount == 0)
                        {
                            var workerThread2 = new Thread(() => Sync(session));
                            workerThread2.Start();
                        }
                    }

                    break;
            }
        }
        
        private static void Regenerate(ClientSession session, Item item)
        {
            session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 6000));
            session.Character.SnackAmount++;
            session.Character.MaxSnack = 0;
            session.Character.SnackHp += item.Hp / 5;
            session.Character.SnackMp += item.Mp / 5;
            _regenerateDisposable = Observable.Timer(TimeSpan.FromMilliseconds(1800 * 5)).Subscribe(obs =>
            {
                if (session.Character.SnackHp > 0 || session.Character.SnackMp > 0)
                {
                    session.Character.SnackHp -= item.Hp / 5;
                    session.Character.SnackMp -= item.Mp / 5;
                    session.Character.SnackAmount--;
                }
            });
        }

        private static void Sync(ClientSession session)
        {
            for (session.Character.MaxSnack = 0; session.Character.MaxSnack < 5; session.Character.MaxSnack++)
            {
                if (session.Character.Hp <= 0)
                {
                    _regenerateDisposable?.Dispose();
                    session.Character.SnackHp = 0;
                    session.Character.SnackMp = 0;
                    session.Character.SnackAmount = 0;
                    return;
                }

                var hpLoad = (int) session.Character.HPLoad();
                var mpLoad = (int) session.Character.MPLoad();

                var buffRc = session.Character.GetBuff(BCardType.CardType.LeonaPassiveSkill,
                                 (byte) AdditionalTypes.LeonaPassiveSkill.IncreaseRecoveryItems)[0] / 100D;

                var hpAmount = session.Character.SnackHp + (int) (session.Character.SnackHp * buffRc);
                var mpAmount = session.Character.SnackMp + (int) (session.Character.SnackMp * buffRc);

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
                                                      (byte) AdditionalTypes.DarkCloneSummon.ConvertRecoveryToDamage)[0];

                if (convertRecoveryToDamage)
                {
                    session.Character.Hp -= hpAmount;

                    if (session.Character.Hp < 1)
                    {
                        session.Character.Hp = 1;
                    }

                    if (hpAmount > 0)
                    {
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateDm(hpAmount));
                    }
                }
                else
                {
                    session.Character.Hp += hpAmount;

                    if (hpAmount > 0)
                    {
                        session.CurrentMapInstance?.Broadcast(session, session.Character.GenerateRc(hpAmount));
                    }
                }

                session.Character.Mp += mpAmount;

                foreach (var mate in session.Character.Mates.Where(s => s.IsTeamMember && s.IsAlive))
                {
                    hpLoad = mate.HpLoad();
                    mpLoad = mate.MpLoad();

                    hpAmount = session.Character.SnackHp;
                    mpAmount = session.Character.SnackMp;

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

                    if (hpAmount > 0)
                    {
                        session.CurrentMapInstance?.Broadcast(session, mate.GenerateRc(hpAmount));
                    }
                }

                if (session.IsConnected)
                {
                    session.SendPacket(session.Character.GenerateStat());

                    if (session.Character.Mates.Any(m => m.IsTeamMember && m.IsAlive))
                    {
                        session.SendPackets(session.Character.GeneratePst());
                    }

                    Thread.Sleep(1800);
                }
                else
                {
                    return;
                }
            }

            session.Character.SnackAmount = 0;
        }
    }
}