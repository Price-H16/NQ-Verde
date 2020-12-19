using System;
using System.Collections.Generic;
using AutoMapper;
using OpenNos.DAL.Interface;
using OpenNos.Data;

namespace OpenNos.DAL.DAO.Base
{
    public abstract class MappingBaseDao<TEntity, TDto> : IMappingBaseDAO
    where TDto : MappingBaseDTO
    {
        #region Members

        protected readonly IDictionary<Type, Type> _mappings = new Dictionary<Type, Type>();
        protected readonly IMapper _mapper;

        protected MappingBaseDao(IMapper mapper) => _mapper = mapper;

        #endregion

    }
}
