using System.Linq;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class AddQuestHandler : IPacketHandler
    {
        #region Instantiation

        public AddQuestHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void AddQuest(AddQuestPacket addQuestPacket)
        {
            if (addQuestPacket != null)
            {
                Session.AddLogsCmd(addQuestPacket);

                if (ServerManager.Instance.Quests.Any(q => q.QuestId == addQuestPacket.QuestId))
                {
                    Session.Character.AddQuest(addQuestPacket.QuestId);
                    return;
                }

                Session.SendPacket(Session.Character.GenerateSay("This Quest doesn't exist", 11));
            }
        }

        #endregion
    }
}