﻿using OpenNos.Core;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class Fernon : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 731;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85 && Session.Character.Faction == FactionType.Angel) //  Daily Quest Fernon
                {
                    Session.Character.AddQuest(6402, false);
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_RIGHT_FACTION"), 0));
                }
            }
        }

        #endregion
    }

    public class Fernon2 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 732;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85 && Session.Character.Faction == FactionType.Demon) //  Daily Quest Fernon #2
                {
                    Session.Character.AddQuest(6406, false);
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_RIGHT_FACTION"), 0));
                }
            }
        }

        #endregion
    }

    public class Fernon3 : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 733;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 85) //  Daily Quest Fernon #2
                {
                    Session.Character.AddQuest(6410, false);
                }
                else
                {
                    Session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("TOO_LOW_LVL"), 0));
                }
            }
        }

        #endregion
    }
}