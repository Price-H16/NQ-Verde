using System.Collections.Generic;
using OpenNos.Data;
using OpenNos.Data.Enums;

namespace OpenNos.DAL.Interface
{
    public interface IMailDAO
    {
        #region Methods

        DeleteResult DeleteById(long mailId);

        SaveResult InsertOrUpdate(ref MailDTO mail);

        IEnumerable<MailDTO> LoadAll();

        MailDTO LoadById(long mailId);

        IEnumerable<MailDTO> LoadSentByCharacter(long characterId);

        IEnumerable<MailDTO> LoadSentToCharacter(long characterId);

        #endregion

        //
    }
}