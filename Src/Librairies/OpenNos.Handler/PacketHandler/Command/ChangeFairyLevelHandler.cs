using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Helpers;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class ChangeFairyLevelHandler : IPacketHandler
    {
        #region Instantiation

        public ChangeFairyLevelHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void ChangeFairyLevel(ChangeFairyLevelPacket changeFairyLevelPacket)
        {
            var fairy =
                Session.Character.Inventory.LoadBySlotAndType((byte) EquipmentType.Fairy, InventoryType.Wear);
            if (changeFairyLevelPacket != null)
            {
                Session.AddLogsCmd(changeFairyLevelPacket);
                if (fairy != null)
                {
                    var fairylevel = changeFairyLevelPacket.FairyLevel;
                    fairylevel -= fairy.Item.ElementRate;
                    fairy.ElementRate = fairylevel;
                    fairy.XP = 0;
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(
                        string.Format(Language.Instance.GetMessageFromKey("FAIRY_LEVEL_CHANGED"), fairy.Item.Name),
                        10));
                    Session.SendPacket(Session.Character.GeneratePairy());
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_FAIRY"),
                        10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(ChangeFairyLevelPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}