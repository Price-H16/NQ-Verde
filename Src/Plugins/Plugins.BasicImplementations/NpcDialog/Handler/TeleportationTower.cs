using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class TeleportationTower : INpcDialogAsyncHandler
    {
        public long HandledId => 661;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Gold >= 50000)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RAID_KEEPER"), 0));
                    Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(o =>
                    {
                        Session.Character.Gold -= 50000;
                        Session.SendPacket(Session.Character.GenerateGold());
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 170, 85, 70);
                    });

                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_GOLD"), 10));
                    return;
                }

            }
        }
    }

    public class TeleportationTower2 : INpcDialogAsyncHandler
    {
        public long HandledId => 662;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Gold >= 10000)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RAID_KEEPER"), 0));
                    Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(o =>
                    {
                        Session.Character.Gold -= 10000;
                        Session.SendPacket(Session.Character.GenerateGold());
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 179, 138, 122);
                    });

                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_GOLD"), 10));
                    return;
                }

            }
        }
    }

    public class TeleportationTower3 : INpcDialogAsyncHandler
    {
        public long HandledId => 710;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 55)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RAID_KEEPER"), 0));
                    Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(o =>
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 86, 14, 6);
                    });

                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("NEED_LEVEL_55"), 10));
                    return;
                }

            }
        }
    }

    public class TeleportationTower4 : INpcDialogAsyncHandler
    {
        public long HandledId => 711;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 55)
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RAID_KEEPER"), 0));
                    Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(o =>
                    {
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId, 1, 81, 152);
                    });

                }
                else
                {
                    Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 10));
                    return;
                }

            }
        }
    }

}