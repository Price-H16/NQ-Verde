using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.ScriptedInstance
{
    public class GitPacketHandler : IPacketHandler
    {
        #region Instantiation

        public GitPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Git(GitPacket packet)
        {
            var button = Session.CurrentMapInstance.Buttons.Find(s => s.MapButtonId == packet.ButtonId);
            if (button != null)
            {
                if (Session.Character.IsVehicled)
                {
                    Session.SendPacket(
                        Session.Character.GenerateSay(Language.Instance.GetMessageFromKey("CANT_DO_VEHICLED"), 10));
                    return;
                }

                Session.CurrentMapInstance.Broadcast(StaticPacketHelper.Out(UserType.Object, button.MapButtonId));
                button.RunAction();
                Session.CurrentMapInstance.Broadcast(button.GenerateIn());
            }
        }

        #endregion
    }
}