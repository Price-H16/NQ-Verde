//using OpenNos.DAL.EF;
//using OpenNos.Data;

//namespace OpenNos.Mapper.Mappers
//{
//    public class ChatLogMapping
//    {
//        #region Methods

//        public static bool ToChatLog(ChatLogDTO input, ChatLog output)
//        {
//            if (input == null)
//            {
//                output = null;
//                return false;
//            }

//            output.Id = input.Id;
//            output.AccountId = input.Id;
//            output.CharacterId = input.CharacterId;
//            output.CharacterName = input.CharacterName;
//            output.Message = input.Message;
//            output.Type = input.Type;
//            output.DateTime = input.DateTime;
//            output.TargetCharacterName = input.TargetCharacterName;
//            output.TargetCharacterId = input.TargetCharacterId;
//            return true;
//        }

//        public static bool ToChatLogDTO(ChatLog input, ChatLogDTO output)
//        {
//            if (input == null)
//            {
//                output = null;
//                return false;
//            }

//            output.Id = input.Id;
//            output.AccountId = input.Id;
//            output.CharacterId = input.CharacterId;
//            output.CharacterName = input.CharacterName;
//            output.Message = input.Message;
//            output.Type = input.Type;
//            output.DateTime = input.DateTime;
//            output.TargetCharacterName = input.TargetCharacterName;
//            output.TargetCharacterId = input.TargetCharacterId;
//            return true;
//        }

//        #endregion
//    }
//}