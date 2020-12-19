using Newtonsoft.Json;
using OpenNos.Domain;
using OpenNos.GameObject.Networking;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OpenNos.GameObject.Helpers
{

    [JsonObject]
    internal interface IEmbedDimension
    {
        #region Properties

        [JsonProperty("height")]
        int Height { get; set; }

        [JsonProperty("width")]
        int Width { get; set; }

        #endregion
    }

    [JsonObject]
    internal interface IEmbedIconProxyUrl
    {
        #region Properties

        [JsonProperty("proxy_icon_url")]
        string ProxyIconUrl { get; set; }

        #endregion
    }

    [JsonObject]
    internal interface IEmbedIconUrl
    {
        #region Properties

        [JsonProperty("icon_url")]
        string IconUrl { get; set; }

        #endregion
    }

    [JsonObject]
    internal interface IEmbedProxyUrl
    {
        #region Properties

        [JsonProperty("proxy_url")]
        string ProxyUrl { get; set; }

        #endregion
    }

    [JsonObject]
    internal interface IEmbedUrl
    {
        #region Properties

        [JsonProperty("url")]
        string Url { get; set; }

        #endregion
    }

    public static class DiscordWebhookHelper
    {
        #region Members

        private static readonly HttpClient _httpClient;
        private static readonly string _webhookUrlBazar;
        private static readonly string _webhookUrl;
        private static readonly string _webhookUrlLog;
        private static readonly string _webhookUrlLogPVP;
        private static readonly string _webhookUrlEventRaid;
        private static string _webhookUrlEventRaidEnd;
        private static readonly string _webhookUrlEvent;
        private static readonly string _webhookUrlEventSay;


        #endregion

        #region Instantiation

        static DiscordWebhookHelper()
        {
            _httpClient = new HttpClient();
            _webhookUrlBazar = "https://discordapp.com/api/webhooks/782399319637622794/ykKvGUHgh0R8KLmEGqWRqnfDiMQ41IzG9Fo9iW1bcW2sJXSJpsbMKrNLbDcdBgqHt3M4";
            _webhookUrl = "https://discordapp.com/api/webhooks/760122073363513354/MQkt4p7fLzXmqDgq6Ya7DR0T5pPwfh1enjPeXqyKYDNnYiCy43HsDOT_hQ_RbUvLaCDn";
            _webhookUrlLog = "https://discordapp.com/api/webhooks/782398928057925643/E_gHM80vFhq9m_0BYZ4InXKFp57MNFywkqiQhoIrywLliHJuZ-Vp3jgjPVrL4DcwHYXZ";
            _webhookUrlLogPVP = "https://discordapp.com/api/webhooks/782398928057925643/E_gHM80vFhq9m_0BYZ4InXKFp57MNFywkqiQhoIrywLliHJuZ-Vp3jgjPVrL4DcwHYXZ";
            _webhookUrlEventRaidEnd = "https://discordapp.com/api/webhooks/760122073363513354/MQkt4p7fLzXmqDgq6Ya7DR0T5pPwfh1enjPeXqyKYDNnYiCy43HsDOT_hQ_RbUvLaCDn";
            _webhookUrlEventRaid = "https://discordapp.com/api/webhooks/760122073363513354/MQkt4p7fLzXmqDgq6Ya7DR0T5pPwfh1enjPeXqyKYDNnYiCy43HsDOT_hQ_RbUvLaCDn";
            _webhookUrlEvent = "https://discordapp.com/api/webhooks/760122073363513354/MQkt4p7fLzXmqDgq6Ya7DR0T5pPwfh1enjPeXqyKYDNnYiCy43HsDOT_hQ_RbUvLaCDn";
            _webhookUrlEventSay = "https://discordapp.com/api/webhooks/760122073363513354/MQkt4p7fLzXmqDgq6Ya7DR0T5pPwfh1enjPeXqyKYDNnYiCy43HsDOT_hQ_RbUvLaCDn";
        }



        #endregion

        #region Methods

        internal static Embed GenerateEmbed(string reason, string username, string adminName, DateTime dateEnd, PenaltyType penalty)
        {
            int color = 0;
            switch (penalty)
            {
                case PenaltyType.Banned:
                    color = 16711680;
                    break;
                case PenaltyType.Muted:
                    color = 16772608;
                    break;
                case PenaltyType.Warning:
                    color = 16754432;
                    break;

                case PenaltyType.BlockExp:
                    color = 47871;
                    break;

                case PenaltyType.BlockFExp:
                    color = 34047;
                    break;

                case PenaltyType.BlockRep:
                    color = 13500671;
                    break;
            }
            List<EmbedField> fields = new List<EmbedField>
            {
                new EmbedField { Name = "User:", Value = "```" + username + "```" },
                new EmbedField { Name = "Punished for:", Value = "```" + reason + "```" },
                new EmbedField { Name = "Punished till:", Value = "```" + dateEnd.ToString() + "```" }
            };
            return new Embed
            {
                Title = "Penalty Type:",
                Description = "```" + penalty + "```",
                Color = color,
                Author = new EmbedAuthor { Name = adminName },
                Fields = fields
            };
        }

        public static async Task<HttpResponseMessage> DiscordEventNosBazar(string message)
        {
            StringContent msg =
                    new StringContent(
                        JsonConvert.SerializeObject(new WebhookObject
                        {
                            Content = "NosBazar" + " New item on Bazaar!:\n```" + message + "```",
                        }), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(_webhookUrlBazar, msg).ConfigureAwait(false);
        }

        public static async Task<HttpResponseMessage> DiscordEventRaid(string message)
        {
            StringContent msg =
                    new StringContent(
                        JsonConvert.SerializeObject(new WebhookObject
                        {
                            Content = "Raid" + " Open:\n```" + message + "```" + "||http://i.epvpimg.com/JJeXaab.jpg ||",
                        }), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(_webhookUrlEventRaid, msg).ConfigureAwait(false);
        }

        public static async Task<HttpResponseMessage> DiscordEventRaidEnd(string message)
        {
            StringContent msg =
                    new StringContent(
                        JsonConvert.SerializeObject(new WebhookObject
                        {
                            Content = "Raid" + " Completed:\n```" + message + "```",
                        }), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(_webhookUrlEventRaidEnd, msg).ConfigureAwait(false);
        }
        public static async Task<HttpResponseMessage> DiscordEventFamily(string message)
        {
            StringContent msg =
                    new StringContent(
                        JsonConvert.SerializeObject(new WebhookObject
                        {
                            Content = "Family" + " Discord Info:\n```" + message + "```",
                        }), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(_webhookUrl, msg).ConfigureAwait(false);
        }

        public static async Task<HttpResponseMessage> DiscordEvent(string message)
        {
            StringContent msg =
                    new StringContent(
                        JsonConvert.SerializeObject(new WebhookObject
                        {
                            Content = "Admin" + " Discord Info:\n```" + message + "```",
                        }), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(_webhookUrl, msg).ConfigureAwait(false);
        }

        public static async Task<HttpResponseMessage> DiscordEventSay(string message)
        {
            StringContent msg =
                    new StringContent(
                        JsonConvert.SerializeObject(new WebhookObject
                        {
                            Content = "Admin" + " Discord Info:\n```" + message + "```",
                        }), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(_webhookUrlEventSay, msg).ConfigureAwait(false);
        }

        public static async Task<HttpResponseMessage> DiscordEventT(string message)
        {
            StringContent msg =
                    new StringContent(
                        JsonConvert.SerializeObject(new WebhookObject
                        {
                            Content = "Event" + " Discord Info:\n```" + message + "```",
                        }), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(_webhookUrlEvent, msg).ConfigureAwait(false);
        }

        public static async Task<HttpResponseMessage> DiscordEventlog(string message)
        {
            StringContent msg =
                    new StringContent(
                        JsonConvert.SerializeObject(new WebhookObject
                        {
                            Content = "AdminLog" + " Discord Info:\n```" + message + "```",
                        }), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(_webhookUrlLog, msg).ConfigureAwait(false);
        }
        public static async Task<HttpResponseMessage> DiscordEventlogPVP(string message)
        {
            StringContent msg =
                    new StringContent(
                        JsonConvert.SerializeObject(new WebhookObject
                        {
                            Content = "Score" + " ServerScore:\n```" + message + "```",
                        }), Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(_webhookUrlLogPVP, msg).ConfigureAwait(false);
        }




        #endregion
    }

    [JsonObject]
    internal class Embed : IEmbedUrl
    {
        #region Properties

        [JsonProperty("author")]
        public EmbedAuthor Author { get; set; }

        [JsonProperty("color")]
        public int Color { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("fields")]
        public List<EmbedField> Fields { get; set; } = new List<EmbedField>();

        [JsonProperty("footer")]
        public EmbedFooter Footer { get; set; }

        [JsonProperty("image")]
        public EmbedImage Image { get; set; }

        [JsonProperty("provider")]
        public EmbedProvider Provider { get; set; }

        [JsonProperty("thumbnail")]
        public EmbedThumbnail Thumbnail { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset? TimeStamp { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = "rich";

        public string Url { get; set; }

        [JsonProperty("video")]
        public EmbedVideo Video { get; set; }

        #endregion
    }

    [JsonObject]
    internal class EmbedAuthor : EmbedUrl, IEmbedIconUrl, IEmbedIconProxyUrl
    {
        #region Properties

        public string IconUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public string ProxyIconUrl { get; set; }

        #endregion
    }

    [JsonObject]
    internal class EmbedField
    {
        #region Properties

        [JsonProperty("inline")]
        public bool Inline { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        #endregion
    }

    [JsonObject]
    internal class EmbedFooter : IEmbedIconUrl, IEmbedIconProxyUrl
    {
        #region Properties

        public string IconUrl { get; set; }

        public string ProxyIconUrl { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        #endregion
    }

    [JsonObject]
    internal class EmbedImage : EmbedProxyUrl, IEmbedDimension
    {
        #region Properties

        public int Height { get; set; }

        public int Width { get; set; }

        #endregion
    }

    [JsonObject]
    internal class EmbedProvider : EmbedUrl
    {
        #region Properties

        [JsonProperty("name")]
        public string Name { get; set; }

        #endregion
    }

    [JsonObject]
    internal abstract class EmbedProxyUrl : EmbedUrl, IEmbedProxyUrl
    {
        #region Properties

        public string ProxyUrl { get; set; }

        #endregion
    }

    [JsonObject]
    internal class EmbedThumbnail : EmbedProxyUrl, IEmbedDimension
    {
        #region Properties

        public int Height { get; set; }

        public int Width { get; set; }

        #endregion
    }

    [JsonObject]
    internal abstract class EmbedUrl : IEmbedUrl
    {
        #region Properties

        public string Url { get; set; }

        #endregion
    }

    [JsonObject]
    internal class EmbedVideo : EmbedUrl, IEmbedDimension
    {
        #region Properties

        public int Height { get; set; }

        public int Width { get; set; }

        #endregion
    }

    [JsonObject]
    internal class WebhookObject
    {
        #region Properties

        [JsonProperty("avatar_url")] public string AvatarUrl { get; set; }

        [JsonProperty("content")] public string Content { get; set; }

        [JsonProperty("embeds")] public List<Embed> Embeds { get; set; } = new List<Embed>();

        [JsonProperty("tts")] public bool IsTTS { get; set; }

        [JsonProperty("username")] public string Username { get; set; }

        #endregion
    }
}