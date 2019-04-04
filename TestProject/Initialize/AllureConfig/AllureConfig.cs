using System;
using System.Collections.Generic;
using System.Linq;

namespace Initialize.AllureConfig
{
    public class AllureConfig
    {
        public Allure Allure { get; set; }
        public List<Categories> Categories { get; set; }
        public Environment Environment { get; set; }

        public static AllureConfigFactory Factory = new AllureConfigFactory();
    }

    public class Allure
    {
        public string Directory { get; set; }
        public bool AllowEmptySuites { get; set; }
        public bool EnableParameters { get; set; }
        public bool AllowLocalHistoryTrend { get; set; }
    }

    public class Categories
    {
        public string Name { get; set; }
        public string TraceRegex { get; set; }
        public string MessageRegex { get; set; }
        public List<string> MatchedStatuses { get; set; }
    }

    public class Environment
    {
        public string Browser { get; set; }
        public RunTime RunTime { get; set; }
    }

    public class RunTime
    {
        public DateTime RunDateTIme { get; set; }
        public string OS { get; set; }
        public string AllureVersion { get; set; }
    }
}
