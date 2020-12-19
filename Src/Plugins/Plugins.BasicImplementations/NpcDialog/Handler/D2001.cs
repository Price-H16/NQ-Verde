using System.Linq;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D2001 : INpcDialogAsyncHandler
    {
        public long HandledId => 2001;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           switch (packet.Type)
                        {
                            case 1: // Pajama
                                {
                                    if (Session.Character.MapInstance.Npcs.Any(s => s.NpcVNum == 932))
                                    {
                                        /**
                                         * TODO:
                                         *
                                         * Session.Character.GiftAdd(900, 1);
                                         *
                                         */

                                        return;
                                    }
                                }
                                break;

                            case 2: // SP 1
                                {
                                    if (Session.Character.MapInstance.Npcs.Any(s => s.NpcVNum == 933))
                                    {
                                        /**
                                         * TODO:
                                         *
                                         * switch (Session.Character.Class)
                                         * {
                                         *     case ClassType.Swordsman:
                                         *         Session.Character.GiftAdd(901, 1);
                                         *         break;
                                         *     case ClassType.Archer:
                                         *         Session.Character.GiftAdd(903, 1);
                                         *         break;
                                         *     case ClassType.Magician:
                                         *         Session.Character.GiftAdd(905, 1);
                                         *         break;
                                         * }
                                         *
                                         */

                                        return;
                                    }
                                }
                                break;

                            case 3: // SP 2
                                {
                                    if (Session.Character.MapInstance.Npcs.Any(s => s.NpcVNum == 934))
                                    {
                                        /**
                                         * TODO:
                                         *
                                         * switch (Session.Character.Class)
                                         * {
                                         *     case ClassType.Swordsman:
                                         *         Session.Character.GiftAdd(902, 1);
                                         *         break;
                                         *     case ClassType.Archer:
                                         *         Session.Character.GiftAdd(904, 1);
                                         *         break;
                                         *     case ClassType.Magician:
                                         *         Session.Character.GiftAdd(906, 1);
                                         *         break;
                                         * }
                                         *
                                         */

                                        return;
                                    }
                                }
                                break;
                        }
        }
    }
}