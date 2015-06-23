using System.Collections.Generic;

namespace ML.Configuration
{
    public class DefaultConfigLevelProvider : IConfigLevelProvider
    {
        private List<ConfigLevelItem> list;

        public DefaultConfigLevelProvider()
        {
            list = new List<ConfigLevelItem>();
            list.Add(new ConfigLevelItem(1, "Debug"));
            list.Add(new ConfigLevelItem(2, "ExtraDebug"));
            list.Add(new ConfigLevelItem(3, "Test"));
            list.Add(new ConfigLevelItem(4, "ExtraTest"));
            list.Add(new ConfigLevelItem(5, "PreRelease"));
            list.Add(new ConfigLevelItem(6, "ExtraRelease"));
            list.Add(new ConfigLevelItem(7, "Release"));
        }

        public List<ConfigLevelItem> GetConfigLevels()
        {
            return list;
        }
    }
}
