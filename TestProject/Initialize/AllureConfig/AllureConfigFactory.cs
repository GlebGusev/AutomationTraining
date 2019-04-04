using System;
using System.Collections.Generic;
using System.Reflection;
using Allure.Commons;
using Initialize.TestOptions;
using NUnit.Framework;

namespace Initialize.AllureConfig
{
    public class AllureConfigFactory : Allure
    {
        public AllureConfig CreateConfig(BrowserOptions browser, CIOptions ci)
        {
            var currentAssembly = Assembly.GetCallingAssembly().GetName().Name;
            string directory = null;
            switch (ci)
            {
                case CIOptions.NA:
                    directory = @"C:/Users/GlebGusev/Desktop/AutomationTraining/TestProject/TestResults/" + currentAssembly + "_results";
                    break;
                case CIOptions.Jenkins:
                    directory = @"C:/Program Files (x86)/Jenkins/workspace/Run Nunit/TestProject/TestResults/" + currentAssembly + "_results";
                    break;
                case CIOptions.TeamCity:
                    directory = "C:/TeamCity/buildAgent/work/357d8625a94da553/TestProject/TestResults/" + currentAssembly + "_results";
                    break;
            }
            return new AllureConfig
            {
                Allure = new Allure
                {
                    AllowEmptySuites = true,
                    AllowLocalHistoryTrend = true,
                    EnableParameters = false,
                    Directory = directory
                },
                Categories = new List<Categories>
                {
                    new Categories { Name = "Problems with locators" , TraceRegex = ".*NoSuchElementException.*" },
                    new Categories { Name = "Problems with DOM", TraceRegex = ".*StaleElementReferenceException.*" },
                    new Categories { Name = "Problems with timeout", MessageRegex = ".*Timed out.*", TraceRegex = ".*" },
                    new Categories { Name = "Broken tests", MatchedStatuses = new List<string> {"broken"}},
                    new Categories { Name = "Ignored tests", MatchedStatuses = new List<string> {"skipped"}},
                    new Categories { Name = "Defected tests", MatchedStatuses = new List<string> {"failed"}},
                    new Categories { Name = "Passed tests", MatchedStatuses = new List<string> {"passed"}}
                },
                Environment = new Environment
                {
                    Browser = browser.ToString(),
                    RunTime = new RunTime
                    {
                        RunDateTIme = DateTime.Now,
                        OS = System.Environment.OSVersion.VersionString,
                        AllureVersion = "Allure.Commons.AllureLifecycle.AllureVersion"
                    }
                }
            };
        }
    }
}
