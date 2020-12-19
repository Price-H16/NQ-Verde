using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public static class FamilyQuestsMapper
    {
        #region Methods

        public static bool ToFamilyQuests(FamilyQuestsDTO input, FamilyQuests output)
        {
            if (input == null)
            {
                return false;
            }

            output.FamilyQuestsId = input.FamilyQuestsId;
            output.FamilyId = input.FamilyId;
            output.QuestType = input.QuestType;
            output.QuestId = input.QuestId;
            output.Do = input.Do;
            output.Date = input.Date;
            output.Count = input.Count;


            return true;
        }

        public static bool ToFamilyQuestsDTO(FamilyQuests input, FamilyQuestsDTO output)
        {
            if (input == null)
            {
                return false;
            }

            output.FamilyQuestsId = input.FamilyQuestsId;
            output.FamilyId = input.FamilyId;
            output.QuestType = input.QuestType;
            output.QuestId = input.QuestId;
            output.Do = input.Do;
            output.Date = input.Date;
            output.Count = input.Count;

            return true;
        }

        #endregion
    }
}