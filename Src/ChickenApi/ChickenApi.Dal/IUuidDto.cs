using System;

namespace ChickenAPI.DAL
{
    public interface IUuidDto : IDto
    {
        #region Properties

        Guid Id { get; set; }

        #endregion
    }
}