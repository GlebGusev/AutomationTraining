using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Task_50.Tests
{
    [TestFixture]
    public class Task_50_herokuapp
    {
        private IWebDriver driver;
        private WebDriverWait _wait;
        private Locators.HerokuappLocators _locators;
        private Uri _startPageAlert = new Uri("https://the-internet.herokuapp.com/javascript_alerts");
        private Uri _startPageFrame = new Uri("https://the-internet.herokuapp.com/iframe");
        private readonly string[] _textToEnter = {"Hello ","World!"};

        [SetUp]
        public void TestSetup()
        {
            KillDriver();
            driver = new ChromeDriver(chromeDriverDirectory: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Drivers\"));
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            driver.Manage().Window.Maximize();
            _locators = new Locators.HerokuappLocators();
        }

        [Test]
        public void Herokuapp_AddTextToFrame_Added()
        {
            //Open start page
            driver.Navigate().GoToUrl(_startPageFrame);

            //FInd Frame and clear
            var frameWindow = driver.FindElement(_locators.frameWindow);
            driver.SwitchTo().Frame(frameWindow);

            var editArea = driver.FindElement(_locators.frameBody);
            editArea.Clear();

            //Enter text in Frame
            editArea.SendKeys(_textToEnter[0]);

            driver.SwitchTo().DefaultContent();
            driver.FindElement(_locators.boldButton).Click();

            driver.SwitchTo().Frame(frameWindow);
            editArea.SendKeys(_textToEnter[1]);

            driver.SwitchTo().DefaultContent();
            driver.FindElement(_locators.boldButton).Click();

            //Validate text in Frame
            driver.SwitchTo().Frame(frameWindow);
            var editorText = Regex.Replace(editArea.Text, @"[^\u0000-\u007F]+", string.Empty);

            Assert.AreEqual(_textToEnter[0] + _textToEnter[1], editorText);
            Assert.AreEqual(_textToEnter[1], editArea.FindElement(_locators.strongText).Text);
        }

        [Test]
        public void Herokuapp_Alert_Added()
        {
            //Open start page
            driver.Navigate().GoToUrl(_startPageAlert);

            //Accept Alert
            IsElementDisplayed(_locators.clickForJSAlertButton);
            driver.FindElement(_locators.clickForJSAlertButton).Click();
            driver.SwitchTo().Alert().Accept();

            Assert.AreEqual("You successfuly clicked an alert", driver.FindElement(_locators.resultMessage).Text);
        }

        [Test]
        public void Herokuapp_Confirm_Added()
        {
            //Open start page
            driver.Navigate().GoToUrl(_startPageAlert);

            //Cancel Confirm
            IsElementDisplayed(_locators.clickForJSConfirmButton);
            var confirmButton = driver.FindElement(_locators.clickForJSConfirmButton);
            confirmButton.Click();
            driver.SwitchTo().Alert().Dismiss();

            Assert.AreEqual("You clicked: Cancel", driver.FindElement(_locators.resultMessage).Text);

            //Accept Confirm
            confirmButton.Click();
            driver.SwitchTo().Alert().Accept();

            Assert.AreEqual("You clicked: Ok", driver.FindElement(_locators.resultMessage).Text);
        }

        [Test]
        public void Herokuapp_Prompt_Added()
        {
            //Open start page
            driver.Navigate().GoToUrl(_startPageAlert);

            //Cancel Prompt
            IsElementDisplayed(_locators.clickForJSPromptButton);
            var promptButton = driver.FindElement(_locators.clickForJSPromptButton);
            promptButton.Click();
            driver.SwitchTo().Alert().Dismiss();

            Assert.AreEqual("You entered: null", driver.FindElement(_locators.resultMessage).Text);

            //Accept Prompt with no value
            promptButton.Click();
            driver.SwitchTo().Alert().Accept();

            Assert.AreEqual("You entered:", driver.FindElement(_locators.resultMessage).Text);

            //Accept Prompt with no value
            promptButton.Click();
            driver.SwitchTo().Alert().SendKeys("a");
            driver.SwitchTo().Alert().Accept();

            Assert.AreEqual("You entered: a", driver.FindElement(_locators.resultMessage).Text);
        }
        
        [TearDown]
        public void TestCleanup()
        {
            driver.Quit();
            KillDriver();
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

        private void IsElementDisplayed(By locator)
        {
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            _wait.PollingInterval = TimeSpan.FromSeconds(1);
            var elementDisplayed = _wait.Until(condition =>
            {
                try
                {
                    return driver.FindElement(locator).Enabled;
                }
                catch (NullReferenceException)
                {
                    return false;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
            });

            if (!elementDisplayed) Assert.Fail("Element with is not found by locator: {0}", locator);
        }
    }
}
