using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class RetrieveStats : INpcDialogAsyncHandler
    {
        public long HandledId => 663;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                Session.SendPacket(npc?.GenerateSay(Language.Instance.GetMessageFromKey("STATS_RETRIEVED"), 10));
                Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(o =>
                {
                    Session.SendPacket(Session.Character.GenerateSay("---------------------------------------------------------------------", 10));
                    Session.SendPacket(Session.Character.GenerateSay($"Player Info: n.{Session.Character.CharacterId} | {Session.Character.Name}", 10));
                    Session.SendPacket(Session.Character.GenerateSay($"Level: {Session.Character.Level} | {Session.Character.LevelXp}", 10));
                    Session.SendPacket(Session.Character.GenerateSay($"JLevel: {Session.Character.JobLevel} | {Session.Character.JobLevelXp}", 10));
                    Session.SendPacket(Session.Character.GenerateSay($"HeroLvl: {Session.Character.HeroLevel} | {Session.Character.HeroXp}", 10));
                    Session.SendPacket(Session.Character.GenerateSay($"Hair Color: {Session.Character.HairColor}", 10));
                    Session.SendPacket(Session.Character.GenerateSay($"Married: {Session.Character.IsMarried}", 10));
                    Session.SendPacket(Session.Character.GenerateSay($"Max mate/partner slots: {Session.Character.MaxMateCount} | {Session.Character.MaxPartnerCount}", 10));
                    Session.SendPacket(Session.Character.GenerateSay($"Available timespaces: {ServerManager.Instance.TimeSpaces.Count}", 10));
                    Session.SendPacket(Session.Character.GenerateSay($"Available raids: {ServerManager.Instance.Raids.Count}", 10));
                    Session.SendPacket(Session.Character.GenerateSay($"A4 Angels %: {ServerManager.Instance.Act4AngelStat.Percentage}", 10));
                    Session.SendPacket(Session.Character.GenerateSay($"A4 Demons %: {ServerManager.Instance.Act4DemonStat.Percentage}", 10));
                    Session.SendPacket(Session.Character.GenerateSay($"Monsters Killed: {Session.Character.MobKillCounter.ToString("###,##0")}", 10));
                    Session.SendPacket(Session.Character.GenerateSay("---------------------------------------------------------------------", 10));

                });
            }
        }
    }
}
