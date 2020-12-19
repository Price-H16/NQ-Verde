using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChickenAPI.Core.Events;
using Discord;
using Discord.Webhook;
using OpenNos.Domain;
using OpenNos.Master.Library.Data;

namespace Plugins.DiscordWebhook
{
  public class DiscordWebHookNotifier : IPlayerNotifier
  {
        private readonly DiscordWebhookNotifierConfig _conf;
        private readonly SerializableWorldServer _worldInfos;
        private readonly DiscordWebhookClient _client;
        private string _username;

        public DiscordWebHookNotifier(DiscordWebhookNotifierConfig conf, SerializableWorldServer worldInfos, DiscordWebhookClient client)
        {
            _conf = conf;
            _worldInfos = worldInfos;
            _client = client;
            _username = $"Bot - :Channel:";
        }

        public async Task NotifyAllAsync(NotifiableEventType notifiable)
        {
            var eventName = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { IconUrl = _conf.IconUrl }, 
                Title = _conf.Title[notifiable], 
                Description = _conf.Messages[notifiable], 
                Color = Color.Orange, 
                ThumbnailUrl = _conf.Images[notifiable], 
                Timestamp = DateTimeOffset.Now, 
                Footer = new EmbedFooterBuilder { IconUrl = _conf.IconUrl }
            };
                
            await _client.SendMessageAsync(
                avatarUrl: _conf.IconUrl, 
                username: _username, 
                embeds: new List<Embed> { eventName.Build() }
            );
        }

        public async Task NotifyAllAsync(NotifiableEventType notifiable, params object[] objs)
        {
            var eventName = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder { IconUrl = _conf.IconUrl }, 
                Title = _conf.Title[notifiable],
                Description = string.Format(_conf.Messages[notifiable], objs),
                Color = _conf.Colors[notifiable],
                ImageUrl = _conf.Images[notifiable], 
                ThumbnailUrl = _conf.Thumbnails[notifiable], 
                Timestamp = DateTimeOffset.Now, 
                Footer = new EmbedFooterBuilder { IconUrl = _conf.IconUrl }
            };
               
            await _client.SendMessageAsync(
                avatarUrl: _conf.IconUrl,
                username: _username, 
                embeds: new List<Embed> { eventName.Build() }
                );
        }
    }
}