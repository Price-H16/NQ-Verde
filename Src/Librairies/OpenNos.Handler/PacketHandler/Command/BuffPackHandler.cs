using System.Collections.Generic;
using ChickenAPI.Enums.Game.Buffs;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class BuffPackHandler : IPacketHandler
    {
        #region Instantiation

        public BuffPackHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void BuffPack(BuffPackPacket buffpackPacket)
        {
            if (buffpackPacket != null)
            {
                Session.AddLogsCmd(buffpackPacket);
                Session.Character.DisableBuffs(new List<BuffType> {BuffType.Bad});

                //ADD some buffs
                Session.Character.AddBuff(new Buff(155, Session.Character.Level), Session.Character.BattleEntity);
                Session.Character.AddBuff(new Buff(153, Session.Character.Level), Session.Character.BattleEntity);
                Session.Character.AddBuff(new Buff(151, Session.Character.Level), Session.Character.BattleEntity);
                Session.Character.AddBuff(new Buff(116, Session.Character.Level), Session.Character.BattleEntity);
                Session.Character.AddBuff(new Buff(117, Session.Character.Level), Session.Character.BattleEntity);
                Session.Character.AddBuff(new Buff(139, Session.Character.Level), Session.Character.BattleEntity);
                Session.Character.AddBuff(new Buff(140, Session.Character.Level), Session.Character.BattleEntity);
                Session.Character.AddBuff(new Buff(72, Session.Character.Level), Session.Character.BattleEntity);
                Session.Character.AddBuff(new Buff(74, Session.Character.Level), Session.Character.BattleEntity);
                Session.Character.AddBuff(new Buff(89, Session.Character.Level), Session.Character.BattleEntity);
                Session.Character.AddBuff(new Buff(91, Session.Character.Level), Session.Character.BattleEntity);

                //effect cuz it looks cooler
                Session.CurrentMapInstance?.Broadcast(
                    StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 7567),
                    Session.Character.PositionX, Session.Character.PositionY);

                // Static buff
                if (!Session.Character.Buff.ContainsKey(131))
                {
                    Session.Character.AddStaticBuff(new StaticBuffDTO {CardId = 131});
                    Session.CurrentMapInstance?.Broadcast(Session.Character.GeneratePairy());
                }

                Session.SendPacket(Session.Character.GenerateSay("You can rule the world now!", 10));
            }
        }

        #endregion
    }
}