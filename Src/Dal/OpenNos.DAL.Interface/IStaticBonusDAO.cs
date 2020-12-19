using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface IStaticBonusDAO
    {
        #region Methods

        void Delete(short bonusToDelete, long characterId);

        /// <summary>
        ///     Inserts new object to database context
        /// </summary>
        /// <param name="staticBonus"></param>
        /// <returns></returns>
        SaveResult InsertOrUpdate(ref StaticBonusDTO staticBonus);

        /// <summary>
        ///     Loads staticBonus by characterid
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        IEnumerable<StaticBonusDTO> LoadByCharacterId(long characterId);

        IEnumerable<short> LoadTypeByCharacterId(long characterId);

        #endregion
    }
}