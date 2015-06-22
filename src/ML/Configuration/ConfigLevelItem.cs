using System;
using System.Collections.Generic;

namespace ML.Configuration
{
    public sealed class ConfigLevelItem : IComparer<ConfigLevelItem>
    {
        public int Index { get; private set; }
        public string ConfigName { get; private set; }

        public ConfigLevelItem(int index, string configName)
        {
            if (index < 1)
            {
                throw new ArgumentException("index is not invalid");
            }

            if (String.IsNullOrWhiteSpace(configName))
            {
                throw new ArgumentNullException("configName can not be null or empty");
            }

            this.Index = index;
            this.ConfigName = configName;
        }

        public int Compare(ConfigLevelItem x, ConfigLevelItem y)
        {
            return x.Index - y.Index;
        }

    }
}
