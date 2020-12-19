using System;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class CharacterEditHandler : IPacketHandler
    {
        #region Instantiation

        public CharacterEditHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void CharacterEdit(CharacterEditPacket characterEditPacket)
        {
            if (characterEditPacket != null)
            {
                Session.AddLogsCmd(characterEditPacket);
                if (characterEditPacket.Property != null && !string.IsNullOrEmpty(characterEditPacket.Data))
                {
                    var propertyInfo = Session.Character.GetType().GetProperty(characterEditPacket.Property);
                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(Session.Character,
                            Convert.ChangeType(characterEditPacket.Data, propertyInfo.PropertyType));
                        ServerManager.Instance.ChangeMap(Session.Character.CharacterId);
                        Session.Character.Save();
                        Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("DONE"),
                            10));
                    }
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(CharacterEditPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}