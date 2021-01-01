using OpenNos.Core;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;
using System;
using System.Reactive.Linq;

namespace OpenNos.GameObject.Event.ACT7
{
    public static class Act7Ship
    {
        #region Methods

        public static void Run(ClientSession session)
        {
            //SendMsg(session, 60);
            //Observable.Timer(TimeSpan.FromSeconds(10)).Subscribe(o =>
            //{
            //    SendMsg(session, 50);
            //});

            //Observable.Timer(TimeSpan.FromSeconds(20)).Subscribe(o =>
            //{
            //    SendMsg(session, 40);
            //});

            //Observable.Timer(TimeSpan.FromSeconds(30)).Subscribe(o =>
            //{
            //    SendMsg(session, 30);
            //});

            //Observable.Timer(TimeSpan.FromSeconds(40)).Subscribe(o =>
            //{
            //    SendMsg(session, 20);
            //});

            Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(o => { SendMsg(session, 5); });

            Observable.Timer(TimeSpan.FromSeconds(15)).Subscribe(o =>
            {
                session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("MAURICE_SHIP"), 0));

                ServerManager.Instance.ChangeMap(session.Character.CharacterId, 2631, 7, 46);
            });
        }

        private static void SendMsg(this ClientSession session, byte sec)
        {
            session.SendPacket(
                UserInterfaceHelper.GenerateMsg(
                    string.Format(Language.Instance.GetMessageFromKey("MAURICE_SHIP_SEC"), sec), 0));
        }

        #endregion
    }
}