using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Mapper.Mappers;
using System.Collections.Generic;

namespace OpenNos.DAL.DAO
{
    public class BoxItemDAO : IBoxItemDAO
    {
        #region Methods

        public List<BoxItemDTO> LoadAll()
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<BoxItemDTO>();

                foreach (var boxItem in context.BoxItem)
                {
                    var dto = new BoxItemDTO();
                    BoxItemMapper.ToBoxItemDTO(boxItem, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        #endregion
    }
}