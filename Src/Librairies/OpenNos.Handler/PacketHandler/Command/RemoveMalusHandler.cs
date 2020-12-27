using System.Collections.Generic;
using ChickenAPI.Enums.Game.Buffs;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    internal class RemoveMalusHandler : IPacketHandler
    {
        #region Instantiation

        public RemoveMalusHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        /// <summary>
        ///     $RemoveMalus packet
        /// </summary>
        /// <param name="removebuffPacket"></param>
        public void RemoveBuff(RemoveMalusPacket removebuffPacket) // by went
        {
            if (removebuffPacket != null)
            {
                Session.AddLogsCmd(removebuffPacket);
                var bufftodisable = new List<BuffType> {BuffType.Bad}; // REMOVE MALUSES
                Session.Character.DisableBuffs(bufftodisable);
                Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"), 10));
            }
        }
    }
}