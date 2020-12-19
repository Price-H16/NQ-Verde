using System;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class AddShellEffectHandler : IPacketHandler
    {
        #region Instantiation

        public AddShellEffectHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void AddShellEffect(AddShellEffectPacket addShellEffectPacket)
        {
            if (addShellEffectPacket != null)
            {
                Session.AddLogsCmd(addShellEffectPacket);
                try
                {
                    var instance =
                        Session.Character.Inventory.LoadBySlotAndType(addShellEffectPacket.Slot,
                            InventoryType.Equipment);
                    if (instance != null)
                        instance.ShellEffects.Add(new ShellEffectDTO
                        {
                            EffectLevel = (ShellEffectLevelType) addShellEffectPacket.EffectLevel,
                            Effect = addShellEffectPacket.Effect,
                            Value = addShellEffectPacket.Value,
                            EquipmentSerialId = instance.EquipmentSerialId
                        });
                }
                catch (Exception)
                {
                    Session.SendPacket(Session.Character.GenerateSay(AddShellEffectPacket.ReturnHelp(), 10));
                }
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay(AddShellEffectPacket.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}