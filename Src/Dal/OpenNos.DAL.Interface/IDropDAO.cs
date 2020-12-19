using System.Collections.Generic;
using OpenNos.Data;

namespace OpenNos.DAL.Interface
{
    public interface IDropDAO
    {
        #region Methods

        DropDTO Insert(DropDTO drop);

        void Insert(List<DropDTO> drops);

        List<DropDTO> LoadAll();

        IEnumerable<DropDTO> LoadByMonster(short monsterVNum);

        #endregion
    }
}