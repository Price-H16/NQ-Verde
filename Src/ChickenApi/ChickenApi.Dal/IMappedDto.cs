namespace ChickenAPI.DAL
{
    public interface IMappedDto : IDto
    {
        #region Properties

        int Id { get; set; }

        #endregion
    }
}