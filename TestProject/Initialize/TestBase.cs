using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Allure.Commons;
using Initialize.TestOptions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace Initialize
{
    //[Parallelizable]
    public abstract class TestBase : AllureReport
    {
        public RunSourceOptions _runSource = RunSourceOptions.SeleniumGrid;
        public BrowserOptions _browser = BrowserOptions.Chrome;
        
        public static IWebDriver Driver; //TODO static???
        
        public CIOptions _ci = CIOptions.NA;
        public static readonly string ResultFolder = Path.Combine(TryGetSolutionDirectoryInfo().FullName, @"TestResults\");
        public static readonly string ScreenshotFolder = Path.Combine(ResultFolder, @"Screenshots\");
        public static readonly string FailedFolder = Path.Combine(ScreenshotFolder, @"Failed\");
        public static readonly string TargetPath = Path.Combine(ResultFolder, @"FinalResult\", DateTime.Now.ToString("yyyy-MM-dd"));
        public readonly string[] _resultFolders = Directory.GetDirectories(ResultFolder);
        //there is now way to update allure config on runtime (settings are applied on build)
        public const string LocalAllure = @"C:\Users\GlebGusev\Desktop\AutomationTraining\TestProject\TestResults\FinalResult";
        public const string JenkinsAllure = @"C:\Program Files (x86)\Jenkins\workspace\Run Nunit\allure-results";
        public const string TeamCityAllure = @"C:\TeamCity\buildAgent\work\357d8625a94da553\TestProject\TestResults\FinalResult";
        
        [SetUp]
        public virtual void TestSetup()
        {
            AllureLifecycle.Instance.RunStep(() =>
            {
                TestContext.Progress.WriteLine($"Test \"{TestExecutionContext.CurrentContext.CurrentTest.FullName}\" is starting...");
            });
        }

        [TearDown]
        public virtual void TestCleanup()
        {
            ScreenshotOnFailure();
            AllureLifecycle.Instance.RunStep(() =>
            {
                TestContext.Progress.WriteLine(
                    $"Test {TestExecutionContext.CurrentContext.CurrentTest.FullName}\" is stopping...");
            });
        }


        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            if (!Directory.Exists(ScreenshotFolder)) Directory.CreateDirectory(ScreenshotFolder);
            if (!Directory.Exists(FailedFolder)) Directory.CreateDirectory(FailedFolder);
            if (!Directory.Exists(TargetPath)) Directory.CreateDirectory(TargetPath);
            if (_ci == CIOptions.Jenkins && !Directory.Exists(JenkinsAllure)) Directory.CreateDirectory(JenkinsAllure);

            SetupRunSourceAndBrowser(_runSource, _browser, _ci);

            Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
        }

        [OneTimeTearDown]
        public virtual void OneTimeTearDown()
        {
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
            if (_ci != CIOptions.NA)
            {
                var files = Directory.GetFiles(TargetPath);
                string destFile = null;
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    if (_ci == CIOptions.Jenkins) destFile = Path.Combine(JenkinsAllure, fileName);
                    if (_ci == CIOptions.TeamCity) destFile = Path.Combine(TeamCityAllure, fileName);
                    File.Copy(file, destFile, true);
                }
            }

            Driver.Quit();
            KillDriver(_browser);
        }

        private void ScreenshotOnFailure()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                var sreenshotName = MakeScreenshot(FailedFolder);
                Thread.Sleep(5000); //to save image
                AllureLifecycle.Instance.AddAttachment(FailedFolder + sreenshotName + ".png");
            }
        }

        public void KillDriver(BrowserOptions browser)
        {
            var driverProcess = new Process[] { };
            switch (browser)
            {
                case BrowserOptions.Chrome:
                    driverProcess = Process.GetProcessesByName("chromedriver");
                    break;
                case BrowserOptions.FireFox:
                    driverProcess = Process.GetProcessesByName("geckodriver");
                    break;
                case BrowserOptions.IE:
                    driverProcess = Process.GetProcessesByName("IEDriverServer");
                    break;
                case BrowserOptions.Edge:
                    driverProcess = Process.GetProcessesByName("MicrosoftWebDriver");
                    break;
            }
            if (driverProcess.Length > 0)
            {
                foreach (var driver in driverProcess)
                {
                    driver.Kill();
                }
            }
        }

        public string MakeScreenshot(string saveTo)
        {
            var ss = ((ITakesScreenshot)Driver).GetScreenshot();

            var testArguments = TestContext.CurrentContext.Test.Arguments;
            var argumentsString = string.Empty;

            if (testArguments.Length > 0)
            {
                var arg = testArguments.First();
                var type = arg.GetType();

                var prop = type.GetProperty("Values");
                var value = prop.GetValue(arg, null) as Dictionary<string, string>.ValueCollection;
                var count = value.Count;

                for (var i = 0; i < count; i++)
                {
                    argumentsString += "_" + Regex.Replace(value.ElementAt(i), @"[^0-9a-zA-Z]+", "");
                }

                //if (!type.IsGenericType)
                //{
                //    foreach (var argument in testArguments)
                //    {
                //        argumentsString += argumentsString + "_" + argument;
                //    }
                //}
                //else
                //{
                //    var prop = type.GetProperty("Values");
                //    var value = prop.GetValue(arg, null) as Dictionary<string,string>.ValueCollection;

                //    for (var i = 0; i < count; i++)
                //    {
                //        argumentsString += argumentsString + "_" + value.ElementAt(i);
                //    }
                //}
            }

            var sreenshotName = TestContext.CurrentContext.Test.MethodName + argumentsString + "_" + DateTime.Now.ToString("MMddyyyy_hhmmss");

            var screenshotFile = Path.Combine(Path.Combine(TryGetSolutionDirectoryInfo().FullName, saveTo)
                , sreenshotName + ".png");
            ss.SaveAsFile(screenshotFile, ScreenshotImageFormat.Png);

            TestContext.AddTestAttachment(screenshotFile, "My Screenshot");

            return sreenshotName;
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

        public void LaunchBrowser(Uri startPage, bool takeAShot = false)
        {
            Driver.Navigate().GoToUrl(startPage);

            //var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(20));
            //wait.Until(condition => Driver.Url.Contains(startPage.ToString()));
            
            if (takeAShot) MakeScreenshot(ScreenshotFolder);
        }

        public void SetupRunSourceAndBrowser(RunSourceOptions runSource, BrowserOptions browser, CIOptions ci)
        {
            Uri hubUri = null;
            string driverPath = null;
            var capabilities = new DesiredCapabilities();
            capabilities.SetCapability("username", "glebabee");
            capabilities.SetCapability("accessKey", "ea9def77-f807-4247-9cfd-bb98c26d45b4");

            switch (runSource)
            {
                case RunSourceOptions.Local:
                    driverPath = GetDriverFolder();
                    break;
                case RunSourceOptions.SauceLabs:
                    hubUri = new Uri("https://ondemand.saucelabs.com/wd/hub");
                    break;
                case RunSourceOptions.SeleniumGrid:
                    switch (ci)
                    {
                        case CIOptions.NA:
                            hubUri = new Uri("http://10.10.22.138:4445/wd/hub");
                            break;
                        case CIOptions.Jenkins:
                            hubUri = new Uri("http://10.10.22.138:4444/wd/hub");
                            break;
                        case CIOptions.TeamCity:
                            hubUri = new Uri("http://10.10.22.138:4445/wd/hub");
                            break;
                    }
                    break;
            }

            switch (browser)
            {
                case BrowserOptions.Chrome:
                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("--start-maximized");
                    switch (runSource)
                    {
                        case RunSourceOptions.Local:
                            Driver = new ChromeDriver(driverPath, chromeOptions);
                            break;
                        case RunSourceOptions.SauceLabs:
                            capabilities.SetCapability("browserName", "Chrome");
                            capabilities.SetCapability("version", "40.0");
                            capabilities.SetCapability("platform", "Linux");
                            Driver = new RemoteWebDriver(hubUri, capabilities);
                            break;
                        case RunSourceOptions.SeleniumGrid:
                            Driver = new RemoteWebDriver(hubUri, chromeOptions);
                            break;
                    }
                    break;
                case BrowserOptions.FireFox:
                    var fireFoxOptions = new FirefoxOptions();
                    switch (runSource)
                    {
                        case RunSourceOptions.Local:
                            Driver = new FirefoxDriver(driverPath, fireFoxOptions);
                            Driver.Manage().Window.Maximize();
                            break;
                        case RunSourceOptions.SauceLabs:
                            capabilities.SetCapability("browserName", "Firefox");
                            capabilities.SetCapability("version", "39.0");
                            capabilities.SetCapability("platform", "Windows 8.1");
                            Driver = new RemoteWebDriver(hubUri, capabilities);
                            break;
                        case RunSourceOptions.SeleniumGrid:
                            Driver = new RemoteWebDriver(hubUri, fireFoxOptions);
                            Driver.Manage().Window.Maximize();
                            break;
                    }
                    break;
                case BrowserOptions.IE:
                    var ieOptions = new InternetExplorerOptions();
                    switch (runSource)
                    {
                        case RunSourceOptions.Local:
                            Driver = new InternetExplorerDriver(driverPath, ieOptions);
                            Driver.Manage().Window.Maximize();
                            break;
                        case RunSourceOptions.SauceLabs:
                            capabilities.SetCapability("browserName", "InternetExplorer");
                            capabilities.SetCapability("version", "11");
                            capabilities.SetCapability("platform", "Windows 10");
                            Driver = new RemoteWebDriver(hubUri, capabilities);
                            break;
                        case RunSourceOptions.SeleniumGrid:
                            Driver = new RemoteWebDriver(hubUri, ieOptions);
                            break;
                    }
                    break;
                case BrowserOptions.Edge:
                    var edgeOptions = new EdgeOptions();
                    switch (runSource)
                    {
                        case RunSourceOptions.Local:
                            Driver = new EdgeDriver(driverPath, edgeOptions);
                            Driver.Manage().Window.Maximize();
                            break;
                        case RunSourceOptions.SauceLabs:
                            capabilities.SetCapability("browserName", "MicrosoftEdge");
                            capabilities.SetCapability("version", "16.16299");
                            capabilities.SetCapability("platform", "Windows 10");
                            Driver = new RemoteWebDriver(hubUri, capabilities);
                            break;
                        case RunSourceOptions.SeleniumGrid:
                            Driver = new RemoteWebDriver(hubUri, edgeOptions);
                            break;
                    }
                    break;
            }
        }
    }
}
