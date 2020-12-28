using System.Collections.Generic;
using Discord;
using OpenNos.Domain;

namespace Plugins.DiscordWebhook
{
     public class DiscordWebhookNotifierConfig
    {
        public Dictionary<NotifiableEventType, string> Messages = new Dictionary<NotifiableEventType, string>
        {
            {
                NotifiableEventType.ACT4_RAID_FIRE_STARTED_ANGEL, @"/!\ __**{0}**__ raid has started for __**{1}**__ ! /!\"
            },
            {
                NotifiableEventType.ACT4_RAID_WATER_STARTED_DEMON, @"/!\ __**{0}**__ raid has started for __**{1}**__! /!\"
            },
            { 
                NotifiableEventType.FAMILY_X_HAS_BEEN_CREATED_BY_Y, "Family __**{0}**__ has been created  by __**{1}**__!"
            },
            {
                NotifiableEventType.X_IS_LOOKING_FOR_RAID_MATES, "Player __**{0}**__ is looking members for the Raid __**{1}**__."
            },
            {
                NotifiableEventType.X_TEAM_WON_THE_RAID_Y, "__**{0}**__'s team completed succesfully the raid __**{1}**__."
            },
            {
                NotifiableEventType.ICEBREAKER_STARTS_IN_5_MINUTES, "Icebreaker will begin in 5 minute/s!."
            },
            {
                NotifiableEventType.INSTANT_BATTLE_STARTS_IN_5_MINUTES, "The Instant Combat will start in __**{0}**__ minutes."
            },
            {
                NotifiableEventType.INSTANT_BATTLE_STARTS_IN_1_MINUTE, "The Instant Combat will start in __**{0}**__ minute."
            },
            {
                NotifiableEventType.CHANNEL_ONLINE, "Server Status __**{0}**__!"
            },
        };

        public Dictionary<NotifiableEventType, string> Thumbnails = new Dictionary<NotifiableEventType, string>
        {
            { NotifiableEventType.CHANNEL_ONLINE, "https://cdn.discordapp.com/attachments/605815407093481472/607209589330673694/alewe.png" },
            { NotifiableEventType.ACT4_RAID_WATER_STARTED_DEMON, "https://cdn.discordapp.com/attachments/605815407093481472/607209587925450788/dlewe1.png" },
            { NotifiableEventType.FAMILY_X_HAS_BEEN_CREATED_BY_Y, "https://i.ytimg.com/vi/5sRaVQuhYY4/maxresdefault.jpg" }
        };

        public Dictionary<NotifiableEventType, string> Images = new Dictionary<NotifiableEventType, string>
        {
            { NotifiableEventType.CHANNEL_ONLINE, "https://cdn.discordapp.com/attachments/605815407093481472/607205615143485441/unknown.png" },
            { NotifiableEventType.ACT4_RAID_WATER_STARTED_DEMON, "https://cdn.discordapp.com/attachments/605815407093481472/607212101622038528/calvinas.png" },
            { NotifiableEventType.FAMILY_X_HAS_BEEN_CREATED_BY_Y, "https://i.ytimg.com/vi/5sRaVQuhYY4/maxresdefault.jpg" }
        };
        
        public Dictionary<NotifiableEventType, string> Title = new Dictionary<NotifiableEventType, string>
        {
            { NotifiableEventType.ACT4_RAID_FIRE_STARTED_ANGEL, "ACT 4 RAID Started" },
            { NotifiableEventType.ACT4_RAID_WATER_STARTED_DEMON, "ACT 4 RAID Started" },
            { NotifiableEventType.FAMILY_X_HAS_BEEN_CREATED_BY_Y, "A Family As Been Created" },
            { NotifiableEventType.CHANNEL_ONLINE, "Channel Status" },
            { NotifiableEventType.X_IS_LOOKING_FOR_RAID_MATES, "Someone Is Looking For You !" },
            { NotifiableEventType.X_TEAM_WON_THE_RAID_Y, "Raid Won" },
            { NotifiableEventType.ICEBREAKER_STARTS_IN_5_MINUTES, "IceBreaker" },
            { NotifiableEventType.INSTANT_BATTLE_STARTS_IN_5_MINUTES, "InstantBattle" },
            { NotifiableEventType.INSTANT_BATTLE_STARTS_IN_1_MINUTE, "InstanBattle" },
        };

        public string IconUrl { get; set; } = "https://cdn.discordapp.com/attachments/718147641623707708/780864472923373598/NQ.png";

        public Dictionary<NotifiableEventType, Color> Colors { get; set; } = new Dictionary<NotifiableEventType, Color>
        {
            { NotifiableEventType.ACT4_RAID_FIRE_STARTED_ANGEL, new Color(0xdd5522) },
            { NotifiableEventType.ACT4_RAID_FIRE_STARTED_DEMON, new Color(0xdd5522) },
            { NotifiableEventType.ACT4_RAID_WATER_STARTED_ANGEL, new Color(0x33aadd) },
            { NotifiableEventType.ACT4_RAID_WATER_STARTED_DEMON, new Color(0x33aadd) },
            { NotifiableEventType.FAMILY_X_HAS_BEEN_CREATED_BY_Y, new Color(0x33aadd) },
            { NotifiableEventType.CHANNEL_ONLINE, new Color(0x33aadd) },
        };
    }
}