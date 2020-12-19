using System;
using OpenNos.Domain;

namespace OpenNos.Data
{
    [Serializable]
    public class ScriptedInstanceDTO
    {
        #region Properties

        public string Label { get; set; }

        public short MapId { get; set; }

        public short PositionX { get; set; }

        public short PositionY { get; set; }

        public int QuestTimeSpaceId { get; set; }

        public string Script { get; set; }

        public short ScriptedInstanceId { get; set; }

        public ScriptedInstanceType Type { get; set; }

        #endregion
    }
}