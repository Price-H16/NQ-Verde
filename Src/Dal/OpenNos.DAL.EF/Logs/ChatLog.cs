//using System;
//using System.ComponentModel.DataAnnotations;
//using OpenNos.Domain;

//namespace OpenNos.DAL.EF
//{
//    public class ChatLog
//    {
//        #region Properties

//        public int AccountId { get; set; }

//        public int CharacterId { get; set; }

//        public string CharacterName { get; set; }

//        public DateTime DateTime { get; set; }

//        [Key] public int Id { get; set; }

//        public string Message { get; set; }

//        public int? TargetCharacterId { get; set; }

//        public string TargetCharacterName { get; set; }

//        public DialogType Type { get; set; }

//        #endregion
//    }
//}