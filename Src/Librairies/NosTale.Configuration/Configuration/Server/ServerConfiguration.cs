namespace NosTale.Configuration.Configuration.Server
{
    public struct ServerConfiguration
    {
        #region Properties

        public int Act4Port { get; set; }

        public string AuthentificationServiceAuthKey { get; set; }

        public string IPAddress { get; set; }

        public bool LagMode { get; set; }

        // World configuration console ↓
        public string Language { get; set; }

        public int LogerPort { get; set; }

        // Login configuration console ↓
        public int LoginPort { get; set; }

        // Master configuration console ↓

        public string LogKey { get; set; }

        public string MasterAuthKey { get; set; }

        public string MasterIP { get; set; }

        public int MasterPort { get; set; }

        public bool SceneOnCreate { get; set; }

        public string ServerGroupS1 { get; set; }

        public int SessionLimit { get; set; }

        public bool UseOldCrypto { get; set; }

        public bool WorldInformation { get; set; }

        public int WorldPort { get; set; }

        public bool BCardsInArenaTalent { get; set; }

        public bool LockSystem { get; set; }

        #endregion
    }
}