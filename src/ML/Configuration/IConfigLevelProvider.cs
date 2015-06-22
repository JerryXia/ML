using System.Collections.Generic;

namespace ML.Configuration
{
    public interface IConfigLevelProvider
    {
        List<ConfigLevelItem> GetConfigLevels();
    }
}
