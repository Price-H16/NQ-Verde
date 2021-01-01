using AutoMapper;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using System;
using System.Collections.Generic;

namespace OpenNos.DAL.DAO.Base
{
    public abstract class MappingBaseDao<TEntity, TDto> : IMappingBaseDAO
    where TDto : MappingBaseDTO
    {
        #region Members

        protected readonly IMapper _mapper;
        protected readonly IDictionary<Type, Type> _mappings = new Dictionary<Type, Type>();

        #endregion

        #region Instantiation

        protected MappingBaseDao(IMapper mapper) => _mapper = mapper;

        #endregion
    }
}