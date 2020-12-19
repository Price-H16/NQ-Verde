using System.Collections.Generic;
using OpenNos.Data;

namespace OpenNos.DAL.Interface
{
    public interface IScriptedInstanceDAO
    {
        #region Methods

        ScriptedInstanceDTO Insert(ScriptedInstanceDTO scriptedInstance);

        void Insert(List<ScriptedInstanceDTO> scriptedInstances);

        IEnumerable<ScriptedInstanceDTO> LoadByMap(short mapId);

        #endregion
    }
}