using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class QtPacketHandler : IPacketHandler
    {
        #region Instantiation

        public QtPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void QtPacket(QtPacket qtPacket)
        {
            switch (qtPacket.Type)
            {
                // On Target Dest
                case 1:
                    Session.Character.IncrementQuests(QuestType.GoTo, Session.CurrentMapInstance.Map.MapId,
                        Session.Character.PositionX, Session.Character.PositionY);
                    break;

                // Give Up Quest
                case 3:
                    var charQuest = Session.Character.Quests?.FirstOrDefault(q => q.QuestNumber == qtPacket.Data);
                    if (charQuest == null || charQuest.IsMainQuest) return;
                    Session.Character.RemoveQuest(charQuest.QuestId, true);
                    break;

                // Ask for rewards
                case 4:
                    break;
            }
        }

        #endregion
    }
}