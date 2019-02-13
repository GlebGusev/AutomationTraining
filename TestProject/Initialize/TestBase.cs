using System;
using System.Diagnostics;
using System.IO;
using Allure.Commons;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Initialize
{
    public abstract class TestBase : AllureReport
    {
        public IWebDriver Driver;
        private static readonly string ResultFolder = Path.Combine(Initialize.TryGetSolutionDirectoryInfo().FullName, @"TestResults\");
        private readonly string[] _resultFolders = Directory.GetDirectories(ResultFolder);
        private static readonly string TargetPath = Path.Combine(ResultFolder, @"FInalResult\", DateTime.Now.ToString("yyyy-MM-dd"));

        [SetUp]
        public virtual void TestSetup()
        {
            KillDriver();
            AllureLifecycle.Instance.RunStep(() =>
            {
                TestContext.Progress.WriteLine($"Test \"{TestExecutionContext.CurrentContext.CurrentTest.FullName}\" is starting...");
            });
            Driver = new ChromeDriver(chromeDriverDirectory: Initialize.GetDriverFolder());
            Driver.Manage().Window.Maximize();
        }

        [TearDown]
        public void TestCleanup()
        {
            ScreenshotOnFailure();
            Driver.Quit();
            AllureLifecycle.Instance.RunStep(() =>
            {
                TestContext.Progress.WriteLine(
                    $"Test {TestExecutionContext.CurrentContext.CurrentTest.FullName}\" is stopping...");
            });
            KillDriver();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            //Merge all Allure json files in one folder
            if (!Directory.Exists(TargetPath)) Directory.CreateDirectory(TargetPath);

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
        }

        private void ScreenshotOnFailure()
        {
            if(TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
                Initialize.MakeScreenshot(Driver, @"TestResults\Screenshots\Failed\");
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
    }
}
