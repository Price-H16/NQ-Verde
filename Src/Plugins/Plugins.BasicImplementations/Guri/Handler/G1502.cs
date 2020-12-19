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
    public class G1502 : IGuriHandler
    {
        public long GuriEffectId => 1502;

        public async Task ExecuteAsync(ClientSession Session, GuriEvent e)
        {
            if (e.Type == 1502)
            {
                short relictVNum = 0;
                if (e.Argument == 10000)
                {
                    relictVNum = 1878;
                }
                else if (e.Argument == 30000)
                {
                    relictVNum = 1879;
                }
                if (relictVNum > 0 && Session.Character.Inventory.CountItem(relictVNum) > 0)
                {
                    var roll = DAOFactory.RollGeneratedItemDAO.LoadByItemVNum(relictVNum);
                    IEnumerable<RollGeneratedItemDTO> rollGeneratedItemDtos = roll as IList<RollGeneratedItemDTO> ?? roll.ToList();
                    if (!rollGeneratedItemDtos.Any())
                    {
                        Logger.Warn(string.Format(Language.Instance.GetMessageFromKey("NO_HANDLER_RELICT"), GetType(), relictVNum));
                        return;
                    }
                    var probabilities = rollGeneratedItemDtos.Sum(s => s.Probability);
                    var rnd = 0;
                    var rnd2 = ServerManager.RandomNumber(0, probabilities);
                    var currentrnd = 0;
                    foreach (var rollitem in rollGeneratedItemDtos.OrderBy(s => ServerManager.RandomNumber()))
                    {
                        sbyte rare = 0;
                        if (rollitem.IsRareRandom)
                        {
                            rnd = ServerManager.RandomNumber(0, 100);

                            for (var j = 7; j >= 0; j--)
                            {
                                if (rnd < ItemHelper.RareRate[j])
                                {
                                    rare = (sbyte)j;
                                    break;
                                }
                            }
                            if (rare < 1)
                            {
                                rare = 1;
                            }
                        }

                        if (rollitem.Probability == 10000)
                        {
                            Session.Character.GiftAdd(rollitem.ItemGeneratedVNum, rollitem.ItemGeneratedAmount, (byte)rare, design: rollitem.ItemGeneratedDesign);
                            continue;
                        }
                        currentrnd += rollitem.Probability;
                        if (currentrnd < rnd2)
                        {
                            continue;
                        }
                        Session.Character.GiftAdd(rollitem.ItemGeneratedVNum, rollitem.ItemGeneratedAmount, (byte)rare, design: rollitem.ItemGeneratedDesign);//, rollitem.ItemGeneratedUpgrade);
                        break;
                    }
                    Session.Character.Inventory.RemoveItemAmount(relictVNum);
                    Session.Character.Gold -= e.Argument;
                    Session.SendPacket(Session.Character.GenerateGold());
                    Session.SendPacket("shop_end 1");
                }
            }
        }
    }
} 