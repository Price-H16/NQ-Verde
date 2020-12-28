using Autofac;
using ChickenAPI.Plugins;
using Discord.Webhook;

namespace Plugins.DiscordWebhook
{
    public class DiscordWebhookPlugin : ICorePlugin
    {
        public PluginEnableTime EnableTime => PluginEnableTime.PreContainerBuild;

        public string Name { get; } = nameof(DiscordWebhookPlugin);

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
        }

        public void OnLoad(ContainerBuilder builder)
        {
            var webhook = "https://discordapp.com/api/webhooks/760122073363513354/MQkt4p7fLzXmqDgq6Ya7DR0T5pPwfh1enjPeXqyKYDNnYiCy43HsDOT_hQ_RbUvLaCDn";
            builder.Register(s => new DiscordWebhookClient(webhook));
            builder.Register(s => new DiscordWebHookNotifier(new DiscordWebhookNotifierConfig(), null, s.Resolve<DiscordWebhookClient>()));
        }
    }
}