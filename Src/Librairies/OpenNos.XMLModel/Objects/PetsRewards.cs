using System;
using System.Xml.Serialization;

namespace OpenNos.XMLModel.Objects
{
    [Serializable]
    public class PetsRewards
    {
        #region Properties

        [XmlAttribute] public int MateVnum { get; set; }

        #endregion
    }
}