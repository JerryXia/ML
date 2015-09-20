using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.Configuration;

namespace TempTest
{
    public class ConfigurationTest
    {
    }

    [ConfigFileName("txx.xml", null, typeof(TempTestProvider))]
    public class Test1
    {
        public int Id { get; set; }
        public long? Data { get; set; }
        public string Txt { get; set; }
        public DateTime Bi { get; set; }
        public List<Test2> Test2s { get; set; }
        public Test2[] Test2Arr { get; set; }
    }
    public class Test2
    {
        public long Id2 { get; set; }
    }


    public class TempTestProvider : IConfigLevelProvider
    {
        private List<ConfigLevelItem> list;

        public TempTestProvider()
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
