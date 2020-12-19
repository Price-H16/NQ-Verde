using System;
using System.Xml.Serialization;
using OpenNos.XMLModel.Events.Quest;
using OpenNos.XMLModel.Objects.Quest;

namespace OpenNos.XMLModel.Models.Quest
{
    [XmlRoot("Definition"),Serializable]
    public class QuestModel
    {
        #region Properties

        public KillObjective[] KillObjectives { get; set; }

        public LootObjective[] LootObjectives { get; set; }

        public short QuestDataVNum { get; set; }

        public QuestGiver QuestGiver { get; set; }

        public short QuestGoalType { get; set; }

        public Reward Reward { get; set; }

        public Script Script { get; set; }

        public WalkObjective WalkObjective { get; set; }

        #endregion
    }
}