using System.Collections.Generic;

namespace NosTale.Configuration.Configuration.Item
{
    public struct UpgradeTattooConfiguration
    {
        // Just a Simple configuration
        public int[] GoldPrice { get; set; }

        public int[] PercentSucess { get; set; }

        public int[] PercentFail { get; set; }

        public int[] PercentDestroyed { get; set; }

        public List<RequiredItem>[] Item { get; set; }
    }
}