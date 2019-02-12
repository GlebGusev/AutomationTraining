using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Initialize
{
    public static class Initialize
    {
        public static DirectoryInfo TryGetSolutionDirectoryInfo()
        {
            var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }
            
            return directory;
        }

        public static string GetDriverFolder()
        {
            var solutionPath = TryGetSolutionDirectoryInfo().FullName;

            return Path.Combine(solutionPath, @"Drivers\");
        }

        public static void MakeScreenshot(IWebDriver driver, string saveTo)
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

        public static void LaunchBrowser(IWebDriver driver, Uri startPage)
        {
            driver.Navigate().GoToUrl(startPage);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(condition => driver.Url.Contains(startPage.ToString()));

            MakeScreenshot(driver, @"TestResults\Screenshots\");
        }
    }
}
