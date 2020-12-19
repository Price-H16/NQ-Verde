using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public class LogsCommmandsMapper
    {
        #region Methods

        public static bool ToLogsCommmand(LogCommandsDTO input, LogCommands output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }

            output.CommandId = input.CommandId;
            output.Data = input.Data;
            output.Command = input.Command;
            output.CharacterId = input.CharacterId;
            output.IpAddress = input.IpAddress;
            output.Name = input.Name;
            output.Timestamp = input.Timestamp;

            return true;
        }

        public static bool LogsCommmandDTO(LogCommands input, LogCommandsDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }

            output.CommandId = input.CommandId;
            output.Data = input.Data;
            output.Command = input.Command;
            output.CharacterId = input.CharacterId;
            output.IpAddress = input.IpAddress;
            output.Name = input.Name;
            output.Timestamp = input.Timestamp;

            return true;
        }

        #endregion
    }
}