using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using NosTale.Packets.Packets.ClientPackets;
using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;
using OpenNos.Master.Library.Client;

namespace OpenNos.Handler.BasicPacket.CharScreen
{
    internal class SelectCharacterPacketHandler : IPacketHandler
    {
        #region Instantiation

        public SelectCharacterPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        public void SelectCharacter(SelectPacket selectPacket)
        {
            try
            {
                #region Validate Session

                if (Session?.Account == null
                    || Session.HasSelectedCharacter)
                {
                    return;
                }

                #endregion

                #region Load Character

                var characterDTO = DAOFactory.CharacterDAO.LoadBySlot(Session.Account.AccountId, selectPacket.Slot);

                if (characterDTO == null)
                {
                    return;
                }

                var character = new Character(characterDTO);

                #endregion
                
                #region Check If Character is Saving

                // i'll enable this when i find the timeouts error
                
                //int saveTimes = 0;
                //while (ServerManager.Instance.IsCharacterSaving(character.AccountId))
                //{
                //    saveTimes++;
                //    Thread.Sleep(1000);
                //    if (saveTimes >= 15)
                //    {
                //        return;
                //    }
                //}

                #endregion
                
                #region Unban Character

                if (ServerManager.Instance.BannedCharacters.Contains(character.CharacterId))
                {
                    ServerManager.Instance.BannedCharacters.RemoveAll(s => s == character.CharacterId);
                }

                #endregion

                #region Initialize Character

                character.Initialize();

                character.MapInstanceId = ServerManager.GetBaseMapInstanceIdByMapId(character.MapId);
                character.PositionX = character.MapX;
                character.PositionY = character.MapY;
                character.Authority = Session.Account.Authority;

                Session.SetCharacter(character);

                #endregion

                #region Limitations

                // If there are > 4 accounts connected, kick this session.
                if (CommunicationServiceClient.Instance.RetrieveOnlineCharacters(character.CharacterId).Count() > 4)
                {
                    Session.Disconnect();
                }

                #endregion

                #region Load General Logs

                character.GeneralLogs = new ThreadSafeGenericList<GeneralLogDTO>();
                character.GeneralLogs.AddRange(DAOFactory.GeneralLogDAO.LoadByAccount(Session.Account.AccountId)
                    .Where(s => s.LogType == "DailyReward" || s.CharacterId == character.CharacterId).ToList());

                #endregion

                #region Reset SpPoint

                if (!Session.Character.GeneralLogs.Any(s => s.Timestamp == DateTime.Now && s.LogData == "World" && s.LogType == "Connection"))
                {
                    Session.Character.SpAdditionPoint += (int)(Session.Character.SpPoint / 100D * 20D);
                    Session.Character.SpPoint = 10000;
                }

                #endregion

                #region Other Character Stuffs

                Session.Character.Respawns =
                    DAOFactory.RespawnDAO.LoadByCharacter(Session.Character.CharacterId).ToList();
                Session.Character.StaticBonusList = DAOFactory.StaticBonusDAO
                    .LoadByCharacterId(Session.Character.CharacterId).ToList();
                Session.Character.LoadInventory();
                Session.Character.LoadQuicklists();
                Session.Character.GenerateMiniland();

                #endregion

                #region Quests

                //if (!DAOFactory.CharacterQuestDAO.LoadByCharacterId(Session.Character.CharacterId).Any(s => s.IsMainQuest)
                //    && !DAOFactory.QuestLogDAO.LoadByCharacterId(Session.Character.CharacterId).Any(s => s.QuestId == 1997))
                //{
                //    CharacterQuestDTO firstQuest = new CharacterQuestDTO
                //    {
                //        CharacterId = Session.Character.CharacterId,
                //        QuestId = 1997,
                //        IsMainQuest = true
                //    };

                //    DAOFactory.CharacterQuestDAO.InsertOrUpdate(firstQuest);
                //}

                DAOFactory.CharacterQuestDAO.LoadByCharacterId(Session.Character.CharacterId).ToList()
                    .ForEach(qst => Session.Character.Quests.Add(new CharacterQuest(qst)));

                #endregion

                #region Title

                DAOFactory.CharacterTitleDAO.LoadByCharacterId(Session.Character.CharacterId).ToList().ForEach(s =>
                {
                    Session.Character.Title.Add(s);
                });

                #endregion

                #region Fix Partner Slots

                if (character.MaxPartnerCount < 3)
                {
                    character.MaxPartnerCount = 3;
                }

                #endregion

                #region Load Mates

                DAOFactory.MateDAO.LoadByCharacterId(Session.Character.CharacterId).ToList().ForEach(s =>
                {
                    var mate = new Mate(s)
                    {
                        Owner = Session.Character
                    };

                    mate.GenerateMateTransportId();
                    mate.Monster = ServerManager.GetNpcMonster(s.NpcMonsterVNum);

                    Session.Character.Mates.Add(mate);
                });

                #endregion

                #region Load Permanent Buff

                Session.Character.LastPermBuffRefresh = DateTime.Now;

                #endregion

                #region CharacterLife

                Session.Character.Life = Observable.Interval(TimeSpan.FromMilliseconds(300))
                    .Subscribe(x => Session.Character.CharacterLife());

                #endregion

                #region Load Amulet

                Observable.Timer(TimeSpan.FromSeconds(1))
                    .Subscribe(o =>
                    {
                        var amulet =
                            Session.Character.Inventory.LoadBySlotAndType((byte)EquipmentType.Amulet,
                                InventoryType.Wear);

                        if (amulet?.ItemDeleteTime != null || amulet?.DurabilityPoint > 0)
                        {
                            Session.Character.AddBuff(new Buff(62, Session.Character.Level),
                                    Session.Character.BattleEntity);
                        }
                    });

                #endregion

                #region Load Static Buff

                foreach (var staticBuff in DAOFactory.StaticBuffDAO.LoadByCharacterId(Session.Character.CharacterId))
                {
                    if (staticBuff.CardId != 319 /* Wedding */)
                    {
                        Session.Character.AddStaticBuff(staticBuff);
                    }
                }

                #endregion

                #region RemoveAllDupedShell

                foreach (var e in DAOFactory.ItemInstanceDAO.LoadByCharacterId(Session.Character.CharacterId))
                {
                    var count = DAOFactory.ShellEffectDAO.LoadByEquipmentSerialId(e.EquipmentSerialId)
                        .Where(s => !s.IsRune).Count();

                    if (count > 15)
                    {
                        e.ShellRarity = null;
                        DAOFactory.ShellEffectDAO.DeleteByEquipmentSerialId(e.EquipmentSerialId);
                    }
                }

                #endregion

                #region Enter the World

                Session.Character.GeneralLogs.Add(new GeneralLogDTO
                {
                    AccountId = Session.Account.AccountId,
                    CharacterId = Session.Character.CharacterId,
                    IpAddress = Session.IpAddress,
                    LogData = "World",
                    LogType = "Connection",
                    Timestamp = DateTime.Now
                });

                Session.Character.LastPulse = DateTime.Now;

                Session.SendPacket("OK");


                CommunicationServiceClient.Instance.ConnectCharacter(ServerManager.Instance.WorldId, character.CharacterId);


                character.Channel = ServerManager.Instance;

                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error("Failed selecting the character.", ex);
            }
            finally
            {
                // Suspicious activity detected -- kick!
                if (Session != null && ((!Session.HasSelectedCharacter || Session.Character == null) || (CommunicationServiceClient.Instance.RetrieveOnlineCharacters(Session.Character.CharacterId).Count() >= 5)))
                {
                    Session.Disconnect();
                }
            }
        }

        #endregion
    }
}