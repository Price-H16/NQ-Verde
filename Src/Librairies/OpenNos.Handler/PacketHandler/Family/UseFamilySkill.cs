using System;
using System.Linq;
using System.Reactive.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extensions;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;

namespace OpenNos.Handler.PacketHandler.Family
{
    public class UseFamilySkillPacketHandler : IPacketHandler
    {
        #region Instantiation

        public UseFamilySkillPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void UseFamilySkill(FwsPacket fwsPacket)
        {
            if (Session.Character.Family != null && Session.Character.FamilyCharacter != null && Session.Character.Family.FamilySkillMissions.FirstOrDefault(s => s.ItemVNum == fwsPacket.ItemVNum && s.CurrentValue == 1) is FamilySkillMission fsm && fsm != null)
            {
                if (Session.Character.FamilyCharacter.Authority == FamilyAuthority.Familydeputy || Session.Character.FamilyCharacter.Authority == FamilyAuthority.Head)
                {
                    switch (fwsPacket.ItemVNum)
                    {
                        case 9600:
                            ServerManager.Instance.Configuration.FamilyExpBuff = true;
                            ServerManager.Instance.Configuration.TimeExpBuff = DateTime.Now.AddMinutes(60);
                            Observable.Timer(TimeSpan.FromMinutes(60)).Subscribe(x => { ServerManager.Instance.Configuration.FamilyExpBuff = false; });

                            foreach (ClientSession s in ServerManager.Instance.Sessions)
                            {
                                s.Character.AddStaticBuff(new StaticBuffDTO
                                {
                                    CardId = 360,
                                    CharacterId = s.Character.CharacterId,
                                    RemainingTime = (int)(ServerManager.Instance.Configuration.TimeExpBuff - DateTime.Now).TotalSeconds
                                });
                            }
                            ServerManager.Shout($"Family {Session.Character.Family.Name} used Exp Buff at Channel {ServerManager.Instance.ChannelId}");
                            break;
                        case 9601:
                            ServerManager.Instance.Configuration.FamilyGoldBuff = true;
                            ServerManager.Instance.Configuration.TimeGoldBuff = DateTime.Now.AddMinutes(60);
                            Observable.Timer(TimeSpan.FromMinutes(60)).Subscribe(x => { ServerManager.Instance.Configuration.FamilyExpBuff = false; });
                            foreach (ClientSession s in ServerManager.Instance.Sessions)
                            {
                                s.Character.AddStaticBuff(new StaticBuffDTO
                                {
                                    CardId = 361,
                                    CharacterId = s.Character.CharacterId,
                                    RemainingTime = (int)(ServerManager.Instance.Configuration.TimeGoldBuff - DateTime.Now).TotalSeconds
                                });
                            }
                            ServerManager.Shout($"Family {Session.Character.Family.Name} used Gold Buff at Channel {ServerManager.Instance.ChannelId}");
                            break;
                        case 9602:
                            if (ServerManager.Instance.ChannelId != 51) return;

                            //Add filling glaceron bar

                            break;
                        case 9603:
                            if (ServerManager.Instance.ChannelId != 51) return;
                            //Add filling glaceron bar

                            break;

                        default:
                            return;
                    }
                    Session.Character.Family.InsertFamilyLog(FamilyLogType.SkillUse, characterName: Session.Character.Name, itemVNum: (fwsPacket.ItemVNum - 600));
                    fsm.CurrentValue = 0;
                    Session.Character.Family.SaveMission(fsm);
                }
            }
        }

        #endregion
    }
}