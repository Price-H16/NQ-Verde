using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ChickenAPI.Enums;
using NosTale.Packets.Packets.CommandPackets;
using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Extension;

namespace OpenNos.Handler.PacketHandler.Command
{
    public class HelpHandler : IPacketHandler
    {
        #region Instantiation

        public HelpHandler(ClientSession session)
        {
            Session = session;
        }

        #endregion

        #region Properties

        public ClientSession Session { get; }

        #endregion

        #region Methods

        public void Command(HelpPacket helpPacket)
        {
            Session.AddLogsCmd(helpPacket);
            // get commands
            /*  var assemblies = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes());
  
              var classes = assemblies.Where(s => s.IsClass && s.Namespace == "OpenNos.Packets.CommandPackets" && 
              s.GetCustomAttribute<PacketHeaderAttribute>()?.Authority <= Session.Account.Authority).ToList();*/

            // get commands
            var classes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t =>
                t.IsClass && t.Namespace == "NosTale.Packets.Packets.CommandPackets"
                          && (((PacketHeaderAttribute) Array.Find(t.GetCustomAttributes(true),
                              ca => ca.GetType().Equals(typeof(PacketHeaderAttribute))))?.Authorities)
                          .Any(c => Session.Account.Authority.Equals(c)
                                    || Session.Account.Authority.Equals(AuthorityType.Administrator)
                                    || c.Equals(AuthorityType.User) && Session.Account.Authority >= AuthorityType.User
                                    || c.Equals(AuthorityType.MOD) && Session.Account.Authority >= AuthorityType.MOD &&
                                    Session.Account.Authority <= AuthorityType.BA
                                    || c.Equals(AuthorityType.SMOD) &&
                                    Session.Account.Authority >= AuthorityType.SMOD &&
                                    Session.Account.Authority <= AuthorityType.BA
                                    || c.Equals(AuthorityType.TGM) && Session.Account.Authority >= AuthorityType.TGM
                                    || c.Equals(AuthorityType.GM) && Session.Account.Authority >= AuthorityType.GM
                                    || c.Equals(AuthorityType.SGM) && Session.Account.Authority >= AuthorityType.SGM
                                    || c.Equals(AuthorityType.GA) && Session.Account.Authority >= AuthorityType.GA
                                    || c.Equals(AuthorityType.TM) && Session.Account.Authority >= AuthorityType.TM
                                    || c.Equals(AuthorityType.CM) && Session.Account.Authority >= AuthorityType.CM
                          )).ToList();
            var messages = new List<string>();
            foreach (var type in classes)
            {
                var classInstance = Activator.CreateInstance(type);
                var classType = classInstance.GetType();
                var method = classType.GetMethod("ReturnHelp");
                if (method != null)
                {
                    messages.Add(method.Invoke(classInstance, null).ToString());
                }
                else
                {
                    var identification = type.GetCustomAttribute<PacketHeaderAttribute>()?.Identification;
                    messages.Add(identification != null && identification.Length > 0
                        ? identification[0]
                        : string.Empty);
                }
            }

            // send messages
            messages.Sort();
            if (helpPacket.Contents == "*" || string.IsNullOrEmpty(helpPacket.Contents))
            {
                Session.SendPacket(Session.Character.GenerateSay("-------------Commands Info-------------", 11));
                foreach (var message in messages) Session.SendPacket(Session.Character.GenerateSay(message, 12));
            }
            else
            {
                Session.SendPacket(Session.Character.GenerateSay("-------------Command Info-------------", 11));
                foreach (var message in messages.Where(s =>
                    s.IndexOf(helpPacket.Contents, StringComparison.OrdinalIgnoreCase) >= 0))
                    Session.SendPacket(Session.Character.GenerateSay(message, 12));
            }

            Session.SendPacket(Session.Character.GenerateSay("-----------------------------------------------", 11));
        }

        #endregion
    }
}