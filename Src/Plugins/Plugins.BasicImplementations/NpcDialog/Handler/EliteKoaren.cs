﻿using OpenNos.Core;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using System.Threading.Tasks;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class EliteKoaren : INpcDialogAsyncHandler
    {
        #region Properties

        public long HandledId => 628;

        #endregion

        #region Methods

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
            var npc = packet.Npc;
            if (npc != null)
            {
                if (Session.Character.Level >= 45)
                {
                    Session.Character.AddQuest(412, false);
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