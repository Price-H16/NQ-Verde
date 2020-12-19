using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._Guri.Event;
using OpenNos.GameObject.Networking;
using System;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class GuriPacketHandler : IPacketHandler
    {
        #region Instantiation

        public GuriPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Guri(GuriPacket guriPacket)
        {
            if (guriPacket == null)
            {
                return;
            }

            if (!guriPacket.Data.HasValue && guriPacket.Type == 10)
            {
                return;
            }
            var packetsplit = guriPacket.OriginalContent.Split(' ', '^');
            /*if (packetsplit[1].ElementAt(0) == '#')
            {
                Session.EmitEvent(new GuriEvent
                {
                    EffectId = guriPacket.Type,
                    Argument = (short)guriPacket.Argument,
                    User = (short)guriPacket.User,
                    Value = guriPacket.Value
                });
                return;
            }*/          
            Session.Character.Event.EmitEvent(new GuriEvent
            {
                Type = guriPacket.Type,
                Argument = guriPacket.Argument,
                Data = guriPacket.Data ?? 0,
                User = guriPacket.User,
                Value = guriPacket.Value
            });
        }

        #endregion
    }
}