using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class BlockPMHandler : IPacketHandler
    {
        #region Instantiation

        public BlockPMHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void BlockPm(BlockPMPacket blockPmPacket)
        {
            Session.AddLogsCmd(blockPmPacket);

            Session.SendPacket(Session.Character.GenerateSay(Language.Instance.GetMessageFromKey(!Session.Character.GmPvtBlock ? "GM_BLOCK_ENABLE" : "GM_BLOCK_DISABLE"),
                    10));
            Session.Character.GmPvtBlock = !Session.Character.GmPvtBlock;
        }

        #endregion
    }
}