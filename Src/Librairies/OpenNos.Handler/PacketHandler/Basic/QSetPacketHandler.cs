using System.Linq;
using NosTale.Packets.Packets.ClientPackets;
using NosTale.Packets.Packets.CustomPackets;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.GameObject;

namespace OpenNos.Handler.PacketHandler.Basic
{
    public class QSetPacketHandler : IPacketHandler
    {
        #region Instantiation

        public QSetPacketHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void SetQuicklist(QSetPacket qSetPacket)
        {
            short data1 = 0, data2 = 0, type = qSetPacket.Type, q1 = qSetPacket.Q1, q2 = qSetPacket.Q2;
            if (qSetPacket.Data1.HasValue) data1 = qSetPacket.Data1.Value;

            if (qSetPacket.Data2.HasValue) data2 = qSetPacket.Data2.Value;

            switch (type)
            {
                case 0:
                case 1:

                    // client says qset 0 1 3 2 6 answer -> qset 1 3 0.2.6.0
                    Session.Character.QuicklistEntries.RemoveAll(n =>
                        n.Q1 == q1 && n.Q2 == q2
                                   && (Session.Character.UseSp ? n.Morph == Session.Character.SpInstance.Item.Morph : n.Morph == 0));
                    Session.Character.QuicklistEntries.Add(new QuicklistEntryDTO
                    {
                        CharacterId = Session.Character.CharacterId,
                        Type = type,
                        Q1 = q1,
                        Q2 = q2,
                        Slot = data1,
                        Pos = data2,
                        Morph = Session.Character.UseSp ? (short) Session.Character.SpInstance.Item.Morph : (short) 0
                    });
                    Session.SendPacket($"qset {q1} {q2} {type}.{data1}.{data2}.0");
                    break;

                case 2:

                    // DragDrop / Reorder qset type to1 to2 from1 from2 vars -> q1 q2 data1 data2
                    var qlFrom = Session.Character.QuicklistEntries.SingleOrDefault(n =>
                        n.Q1 == data1 && n.Q2 == data2
                                      && (Session.Character.UseSp ? n.Morph == Session.Character.SpInstance.Item.Morph  : n.Morph == 0));
                    if (qlFrom != null)
                    {
                        var qlTo = Session.Character.QuicklistEntries.SingleOrDefault(n =>
                            n.Q1 == q1 && n.Q2 == q2 && (Session.Character.UseSp
                                ? n.Morph == Session.Character.SpInstance.Item.Morph 
                                : n.Morph == 0));
                        qlFrom.Q1 = q1;
                        qlFrom.Q2 = q2;
                        if (qlTo == null)
                        {
                            // Put 'from' to new position (datax)
                            Session.SendPacket(
                                $"qset {qlFrom.Q1} {qlFrom.Q2} {qlFrom.Type}.{qlFrom.Slot}.{qlFrom.Pos}.0");

                            // old 'from' is now empty.
                            Session.SendPacket($"qset {data1} {data2} 7.7.-1.0");
                        }
                        else
                        {
                            // Put 'from' to new position (datax)
                            Session.SendPacket(
                                $"qset {qlFrom.Q1} {qlFrom.Q2} {qlFrom.Type}.{qlFrom.Slot}.{qlFrom.Pos}.0");

                            // 'from' is now 'to' because they exchanged
                            qlTo.Q1 = data1;
                            qlTo.Q2 = data2;
                            Session.SendPacket($"qset {qlTo.Q1} {qlTo.Q2} {qlTo.Type}.{qlTo.Slot}.{qlTo.Pos}.0");
                        }
                    }

                    break;

                case 3:

                    // Remove from Quicklist
                    Session.Character.QuicklistEntries.RemoveAll(n =>
                        n.Q1 == q1 && n.Q2 == q2
                                   && (Session.Character.UseSp ? n.Morph == Session.Character.SpInstance.Item.Morph  : n.Morph == 0));
                    Session.SendPacket($"qset {q1} {q2} 7.7.-1.0");
                    break;

                default:
                    return;
            }
        }
        public void ChangeLockCode(ChangeLockCodePacket packet)
        {
            if (packet != null)
            {
                if (packet.Lock == "Disable" || packet.Lock == "disable")
                {
                    Session.Character.LockCode = null;
                }
                else
                {
                    Session.Character.LockCode = CryptographyBase.Sha512(packet.Lock);
                }
            }
        }
        #endregion
    }
}