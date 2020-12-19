using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.DAL.Interface.PropertiesMapping
{
    public interface IModuleMapper<TDTOEntity, TEntity>
    {
        bool ToDTO(TEntity input, TDTOEntity output);
        bool ToEntity(TDTOEntity input, TEntity output);
    }
}
