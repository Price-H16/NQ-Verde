using OpenNos.Data;
using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.Interface
{
    public interface IFamilyQuestsDAO
    {
        public IEnumerable<FamilyQuestsDTO> LoadByFamilyAndQuestId(FamilyQuestsDTO log);

        public SaveResult InsertOrUpdate(FamilyQuestsDTO FamilyQuests);

        public IEnumerable<FamilyQuestsDTO> LoadAllByFamilyId(long Id);
    }
}
