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
        private readonly string[] _resultFolders = Directory.GetDirectories(ResultFolder);
        private const string JenkinsAllure = @"C:\Program Files (x86)\Jenkins\workspace\Run Nunit\allure-results";

        [SetUp]
        public virtual void TestSetup()
        {
            Driver.Quit();
            KillDriver();
            AllureLifecycle.Instance.RunStep(() =>
            {
                TestContext.Progress.WriteLine($"Test \"{TestExecutionContext.CurrentContext.CurrentTest.FullName}\" is starting...");
            });
            options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            Driver = new RemoteWebDriver(new Uri("http://10.10.22.95:4444/wd/hub"), options);
            //Driver = new ChromeDriver(GetDriverFolder(), options);
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

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            KillDriver();

            //Merge all Allure json files in one folder
            var targetPath = Path.Combine(ResultFolder, @"FInalResult\", DateTime.Now.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);

            foreach (var folder in _resultFolders)
            {
                if (folder.Contains("_result"))
                {
                    var filesInFolder = Directory.GetFiles(folder);
                    foreach (var file in filesInFolder)
                    {
                        var fileName = Path.GetFileName(file);
                        var destFile = Path.Combine(targetPath, fileName);
                        File.Copy(file, destFile, true);
                    }
                }
            }

            //Copy to Jenkins allure result
            if(!Directory.Exists(JenkinsAllure)) Directory.CreateDirectory(JenkinsAllure);
            var files = Directory.GetFiles(targetPath);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(@"C:\Program Files (x86)\Jenkins\workspace\Run Nunit\allure-results", fileName);
                File.Copy(file, destFile, true);
            }
        }

        private void ScreenshotOnFailure()
        {
            if(TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
                MakeScreenshot(Driver, @"TestResults\Screenshots\Failed\");
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

            MakeScreenshot(driver, @"TestResults\Screenshots\");
        }
    }
}
