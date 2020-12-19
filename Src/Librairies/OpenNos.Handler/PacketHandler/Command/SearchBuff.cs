using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.GameObject;
using System.Collections.Generic;
using System.Linq;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class SearchBuffHandler : IPacketHandler
    {
        #region Instantiation

        public SearchBuffHandler (ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void SearchBuff(SearchBuff searchItemPacket)
        {
            if (searchItemPacket != null)
            {
                string contents = searchItemPacket.Contents;

                string name = string.Empty;
                byte page = 0;
                if (!string.IsNullOrEmpty(contents))
                {
                    string[] packetsplit = contents.Split(' ');
                    bool withPage = byte.TryParse(packetsplit[0], out page);
                    name = packetsplit.Length == 1 && withPage ? string.Empty : packetsplit.Skip(withPage ? 1 : 0).Aggregate((a, b) => a + ' ' + b);

                }
                    
                IEnumerable<CardDTO> itemlist = DAOFactory.CardDAO.LoadAll().ToArray(); // to fix?
                if (itemlist.Any())
                {
                    foreach (CardDTO item in itemlist)
                    {
                        Session.SendPacket(Session.Character.GenerateSay($"[SearchBuff]Buff: {(string.IsNullOrEmpty(item.Name) ? "none" : item.Name)} VNum: {item.CardId}", 12));

                    }
                }
                else
                {
                    Session.SendPacket(Session.Character.GenerateSay("Buff not found.", 11));

                }
            }
            else
            {
                //    Session.SendPacket(Session.Character.GenerateSay(SearchBuff.ReturnHelp(), 10));
            }
        }

        #endregion
    }
}