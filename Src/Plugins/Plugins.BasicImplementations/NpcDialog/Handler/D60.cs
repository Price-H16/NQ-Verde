using System;
using System.Linq;
using System.Threading.Tasks;
using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject._NpcDialog;
using OpenNos.GameObject._NpcDialog.Event;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace Plugins.BasicImplementations.NpcDialog.Handler
{
    public class D60 : INpcDialogAsyncHandler
    {
        public long HandledId => 60;

        public async Task Execute(ClientSession Session, NpcDialogEvent packet)
        {
           var npc = packet.Npc;
           if (!Session.Character.CanUseNosBazaar())
           {
               Session.SendPacket(UserInterfaceHelper.GenerateInfo(Language.Instance.GetMessageFromKey("INFO_BAZAAR")));
               return;
           }

           MedalType medalType = 0;
           var time = 0;

           var medal = Session.Character.StaticBonusList.Find(s => s.StaticBonusType == StaticBonusType.BazaarMedalGold || s.StaticBonusType == StaticBonusType.BazaarMedalSilver);

           if (medal != null)
           {
               time = (int)(medal.DateEnd - DateTime.Now).TotalHours;

               switch (medal.StaticBonusType)
               {
                   case StaticBonusType.BazaarMedalGold:
                       medalType = MedalType.Gold;
                       break;

                   case StaticBonusType.BazaarMedalSilver:
                       medalType = MedalType.Silver;
                       break;
               }
           }

           Session.SendPacket($"wopen 32 {(byte)medalType} {time}");
        }
    }
}