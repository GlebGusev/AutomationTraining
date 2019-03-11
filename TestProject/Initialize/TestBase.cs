using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Allure.Commons;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace Initialize
{
    [Parallelizable]
    public abstract class TestBase : AllureReport
    {
        public IWebDriver Driver;
        private ChromeOptions options;
        private static readonly string ResultFolder = Path.Combine(TryGetSolutionDirectoryInfo().FullName, @"TestResults\");
        private static readonly string ScreenshotFolder = Path.Combine(ResultFolder, @"Screenshots\");
        private static readonly string FailedFolder = Path.Combine(ScreenshotFolder, @"Screenshots\");
        private static readonly string TargetPath = Path.Combine(ResultFolder, @"FinalResult\", DateTime.Now.ToString("yyyy-MM-dd"));
        private readonly string[] _resultFolders = Directory.GetDirectories(ResultFolder);
        private const string JenkinsAllure = @"C:\Program Files (x86)\Jenkins\workspace\Run Nunit\allure-results";
        private const string TeamCityAllure = @"C:\TeamCity\buildAgent\work\357d8625a94da553\TestProject\TestResults\FinalResult";

        [SetUp]
        public virtual void TestSetup()
        {
            AllureLifecycle.Instance.RunStep(() =>
            {
                TestContext.Progress.WriteLine($"Test \"{TestExecutionContext.CurrentContext.CurrentTest.FullName}\" is starting...");
            });

            options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            //Driver = new RemoteWebDriver(new Uri("http://10.10.22.95:4444/wd/hub"), options); //Jenkins
            Driver = new RemoteWebDriver(new Uri("http://10.10.22.95:4445/wd/hub"), options); //TeamCity
            //Driver = new ChromeDriver(GetDriverFolder(), options);

            #region SauceLabs
            //var capabilities = new DesiredCapabilities();
            //capabilities.SetCapability("username", "glebabee");
            //capabilities.SetCapability("accessKey", "ea9def77-f807-4247-9cfd-bb98c26d45b4");

            //capabilities.SetCapability("browserName", "MicrosoftEdge");
            //capabilities.SetCapability("platform", "Windows 10");
            //capabilities.SetCapability("version", "16.16299");

            //capabilities.SetCapability("browserName", "Firefox");
            //capabilities.SetCapability("platform", "Windows 8.1");
            //capabilities.SetCapability("version", "39.0");

            //capabilities.SetCapability("browserName", "Chrome");
            //capabilities.SetCapability("platform", "Linux");
            //capabilities.SetCapability("version", "40.0");

            //Driver = new RemoteWebDriver(new Uri("https://ondemand.saucelabs.com/wd/hub"), capabilities);
            #endregion SauceLabs
        }

        [TearDown]
        public void TestCleanup()
        {
            ScreenshotOnFailure();
            AllureLifecycle.Instance.RunStep(() =>
            {
                TestContext.Progress.WriteLine(
                    $"Test {TestExecutionContext.CurrentContext.CurrentTest.FullName}\" is stopping...");
            });
            Driver.Quit(); //fails test in parallel. need to add driver factory
            KillDriver();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            if (!Directory.Exists(ScreenshotFolder)) Directory.CreateDirectory(ScreenshotFolder);
            if (!Directory.Exists(FailedFolder)) Directory.CreateDirectory(FailedFolder);
            if (!Directory.Exists(TargetPath)) Directory.CreateDirectory(TargetPath);
            //if (!Directory.Exists(JenkinsAllure)) Directory.CreateDirectory(JenkinsAllure);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            KillDriver();

            //Merge all Allure json files in one folder
            foreach (var folder in _resultFolders)
            {
                if (folder.Contains("_result"))
                {
                    var filesInFolder = Directory.GetFiles(folder);
                    foreach (var file in filesInFolder)
                    {
                        var fileName = Path.GetFileName(file);
                        var destFile = Path.Combine(TargetPath, fileName);
                        File.Copy(file, destFile, true);
                    }
                }
            }

            //Copy to Jenkins or TeamCity allure result
            var files = Directory.GetFiles(TargetPath);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                //var destFile = Path.Combine(JenkinsAllure, fileName);
                var destFile = Path.Combine(TeamCityAllure, fileName);
                File.Copy(file, destFile, true);
            }
        }

        private void ScreenshotOnFailure()
        {
            if(TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
                MakeScreenshot(Driver, FailedFolder);
        }

        private void KillDriver()
        {
            var cromeDriver = Process.GetProcessesByName("chromedriver");
            if (cromeDriver.Length > 0)
            {
                foreach (var driver in cromeDriver)
                {
                    driver.Kill();
                }
            }
        }

        public void MakeScreenshot(IWebDriver driver, string saveTo)
        {
            var ss = ((ITakesScreenshot)driver).GetScreenshot();

            var testArguments = TestContext.CurrentContext.Test.Arguments;
            var argumentsString = string.Empty;

            if (testArguments.Length > 0)
            {
                argumentsString = "_";
                foreach (var argument in testArguments)
                {
                    argumentsString += argumentsString + "_" + argument;
                }
            }

            var screenshotFile = Path.Combine(Path.Combine(TryGetSolutionDirectoryInfo().FullName, saveTo)
                , TestContext.CurrentContext.Test.MethodName + argumentsString + "_" + DateTime.Now.ToString("MMddyyyy_hhmmss") + ".png");
            ss.SaveAsFile(screenshotFile, ScreenshotImageFormat.Png);

            TestContext.AddTestAttachment(screenshotFile, "My Screenshot");
        }

        public static DirectoryInfo TryGetSolutionDirectoryInfo()
        {
            var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }

            return directory;
        }

        public string GetDriverFolder()
        {
            var solutionPath = TryGetSolutionDirectoryInfo().FullName;

            return Path.Combine(solutionPath, @"Drivers\");
        }

        public void LaunchBrowser(IWebDriver driver, Uri startPage)
        {
            driver.Navigate().GoToUrl(startPage);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until(condition => driver.Url.Contains(startPage.ToString()));

            MakeScreenshot(driver, ScreenshotFolder);
        }
    }
}
