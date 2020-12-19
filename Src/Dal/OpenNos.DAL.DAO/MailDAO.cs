﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Helpers;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using OpenNos.Data.Enums;
using OpenNos.Mapper.Mappers;

namespace OpenNos.DAL.DAO
{
    public class MailDAO : IMailDAO
    {
        #region Methods

        public DeleteResult DeleteById(long mailId)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var mail = context.Mail.First(i => i.MailId.Equals(mailId));

                    if (mail != null)
                    {
                        context.Mail.Remove(mail);
                        context.SaveChanges();
                    }

                    return DeleteResult.Deleted;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return DeleteResult.Error;
            }
        }

        public SaveResult InsertOrUpdate(ref MailDTO mail)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var mailId = mail.MailId;
                    var entity = context.Mail.FirstOrDefault(c => c.MailId.Equals(mailId));

                    if (entity == null)
                    {
                        mail = insert(mail, context);
                        return SaveResult.Inserted;
                    }

                    mail.MailId = entity.MailId;
                    mail = update(entity, mail, context);
                    return SaveResult.Updated;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return SaveResult.Error;
            }
        }

        public IEnumerable<MailDTO> LoadAll()
        {
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<MailDTO>();
                foreach (var mail in context.Mail)
                {
                    var dto = new MailDTO();
                    MailMapper.ToMailDTO(mail, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        public MailDTO LoadById(long mailId)
        {
            try
            {
                using (var context = DataAccessHelper.CreateContext())
                {
                    var dto = new MailDTO();
                    if (MailMapper.ToMailDTO(context.Mail.FirstOrDefault(i => i.MailId.Equals(mailId)), dto))
                        return dto;

                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IEnumerable<MailDTO> LoadSentByCharacter(long characterId)
        {
            //Where(s => s.SenderId == CharacterId && s.IsSenderCopy && MailList.All(m => m.Value.MailId != s.MailId))
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<MailDTO>();
                foreach (var mail in context.Mail.Where(s => s.SenderId == characterId && s.IsSenderCopy).Take(40))
                {
                    var dto = new MailDTO();
                    MailMapper.ToMailDTO(mail, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        public IEnumerable<MailDTO> LoadSentToCharacter(long characterId)
        {
            //s => s.ReceiverId == CharacterId && !s.IsSenderCopy && MailList.All(m => m.Value.MailId != s.MailId)).Take(50)
            using (var context = DataAccessHelper.CreateContext())
            {
                var result = new List<MailDTO>();
                foreach (var mail in context.Mail.Where(s => s.ReceiverId == characterId && !s.IsSenderCopy).Take(40))
                {
                    var dto = new MailDTO();
                    MailMapper.ToMailDTO(mail, dto);
                    result.Add(dto);
                }

                return result;
            }
        }

        private static MailDTO insert(MailDTO mail, OpenNosContext context)
        {
            try
            {
                var entity = new Mail();
                MailMapper.ToMail(mail, entity);
                context.Mail.Add(entity);
                context.SaveChanges();
                if (MailMapper.ToMailDTO(entity, mail)) return mail;

                return null;
            }
            catch (DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                foreach (var validationError in validationErrors.ValidationErrors)
                    // raise a new exception nesting the current instance as InnerException
                    Logger.Error(
                        new InvalidOperationException($"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}",
                            raise));
                return null;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        private static MailDTO update(Mail entity, MailDTO respawn, OpenNosContext context)
        {
            if (entity != null)
            {
                MailMapper.ToMail(respawn, entity);
                context.SaveChanges();
            }

            if (MailMapper.ToMailDTO(entity, respawn)) return respawn;

            return null;
        }

        #endregion
    }
}