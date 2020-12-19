using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Networking;
using System;

namespace OpenNos.GameObject.Extension
{
    public static class LogsExtension
    {
        public static void AddLogsCmd(this ClientSession e, PacketDefinition cmd)
        {
            string withoutHeaderpacket = string.Empty;
            string[] packet = cmd.OriginalContent.Split(' ');
            for (int i = 1; i < packet.Length; i++)
            {
                withoutHeaderpacket += $" {packet[i]}";
            }

            ServerManager.Instance.CommandsLogs.Add(new LogCommandsDTO
            {
                CharacterId = e.Character.CharacterId,
                Command = cmd.OriginalHeader,
                Data = withoutHeaderpacket,
                IpAddress = e.Account.Authority == AuthorityType.Administrator ? "127.0.0.1" : e.CleanIpAddress, // lmao
                Name = e.Character.Name,
                Timestamp = DateTime.Now
            });
        }
    }
}
