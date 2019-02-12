using System.Diagnostics;
using Allure.Commons;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Initialize
{
    public abstract class TestBase : AllureReport
    {
        public IWebDriver _driver;
        
        [SetUp]
        public virtual void TestSetup()
        {
            KillDriver();
            var t = AllureLifecycle.Instance.ResultsDirectory;
            AllureLifecycle.Instance.RunStep(() =>
            {
                TestContext.Progress.WriteLine($"Test \"{TestExecutionContext.CurrentContext.CurrentTest.FullName}\" is starting...");
            });
            _driver = new ChromeDriver(chromeDriverDirectory: Initialize.GetDriverFolder());
            _driver.Manage().Window.Maximize();
        }

        [TearDown]
        public void TestCleanup()
        {
            ScreenshotOnFailure();
            _driver.Quit();
            AllureLifecycle.Instance.RunStep(() =>
            {
                TestContext.Progress.WriteLine(
                    $"Test {TestExecutionContext.CurrentContext.CurrentTest.FullName}\" is stopping...");
            });
            var t = AllureLifecycle.Instance.ResultsDirectory;
            KillDriver();
        }

        private void ScreenshotOnFailure()
        {
            if(TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
                Initialize.MakeScreenshot(_driver, @"TestResults\Screenshots\Failed\");
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
