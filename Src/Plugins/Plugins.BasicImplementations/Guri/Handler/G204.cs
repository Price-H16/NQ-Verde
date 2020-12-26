using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.Core.Extensions;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._Guri;
using OpenNos.GameObject._Guri.Event;
using OpenNos.GameObject.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace Plugins.BasicImplementations.Guri.Handler
{
    public class G204 : IGuriHandler
    {
        public long GuriEffectId => 204;

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 204)
            {
                if (e.Argument == 0 && short.TryParse(e.User.ToString(), out var slot))
                {
                    var shell = Session.Character.Inventory.LoadBySlotAndType(slot, InventoryType.Equipment);
                    if (shell?.ShellEffects.Count == 0 && shell.Upgrade > 0 && shell.Rare > 0 && Session.Character.Inventory.CountItem(1429) >= shell.Upgrade / 10 + shell.Rare)
                    {
             
                        if (!ShellGeneratorHelper.Instance.ShellTypes.TryGetValue(shell.ItemVNum, out var shellType))
                        {
                            // SHELL TYPE NOT IMPLEMENTED
                            return;
                        }

                        /*if (shellType != 8 && shellType != 9)
                        {
                            if (shell.Upgrade < 50)
                            {
                                return;
                            }
                        }*/

                        /*if (shellType == 8 || shellType == 9)
                        {
                            switch (shell.Upgrade)
                            {
                                case 25:
                                case 30:
                                case 40:
                                case 55:
                                case 60:
                                case 65:
                                case 70:
                                case 75:
                                case 80:
                                case 85:
                                    break;

                                default:
                                    Session.Character.Inventory.RemoveItemFromInventory(shell.Id);
                                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("STOP_SPAWNING_BROKEN_SHELL"), 0));
                                    return;
                            }
                        }*/

                        var shellOptions = ShellGeneratorHelper.Instance.GenerateShell(shellType, shell.Rare, shell.Upgrade);

                        /*if (!shellOptions.Any())
                        {
                            Session.Character.Inventory.RemoveItemFromInventory(shell.Id);
                            Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("STOP_SPAWNING_BROKEN_SHELL"), 0));
                            return;
                        }*/

                        shell.ShellEffects.AddRange(shellOptions);

                        DAOFactory.ShellEffectDAO.InsertOrUpdateFromList(shell.ShellEffects, shell.EquipmentSerialId);

                        Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("OPTION_IDENTIFIED"), 0));
                        Session.SendPacket(StaticPacketHelper.GenerateEff(UserType.Player, Session.Character.CharacterId, 3006));
                        Session.Character.Inventory.RemoveItemAmount(1429, shell.Upgrade / 10 + shell.Rare);
                    }
                }
            }
            
        }
    }
} 