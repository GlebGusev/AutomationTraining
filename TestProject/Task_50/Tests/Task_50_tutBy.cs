using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Task_50.Tests
{
    [TestFixture]
    public class Task_50_tutBy
    {
        private IWebDriver driver;
        private WebDriverWait _wait;
        private Locators.TutByLocators _locators;
        private readonly Uri _startPage = new Uri("https://www.tut.by/");
        private readonly string _username = "seleniumtests@tut.by";
        private readonly string _password = "123456789zxcvbn";
        private bool _userLoggedIn;

        [SetUp]
        public void TestSetup()
        {
            KillDriver();
            driver = new ChromeDriver(chromeDriverDirectory: Initialize.Initialize.GetDriverFolder());
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            driver.Manage().Window.Maximize();
            _locators = new Locators.TutByLocators();
        }

        [Test]
        public void LoginTutBy_CorrectCredentialsAndWait_Successfull()
        {
            //Open start page
            Initialize.Initialize.LaunchBrowser(driver, _startPage);
            Thread.Sleep(3000); //Sleep is like explisit wait, but it will block run till all time specified.

            //Enter credentials
            driver.FindElement(_locators.enterLink).Click();

            var userNameInput = driver.FindElement(_locators.userNameInput);
            userNameInput.Clear();
            userNameInput.SendKeys(_username);

            var passwordInput = driver.FindElement(_locators.passwordInput);
            passwordInput.Clear();
            passwordInput.SendKeys(_password);

            //Login
            driver.FindElement(_locators.enterButton).Click();

            //Validate user logged in
            IsElementDisplayed(_locators.loggedInSpan);
            driver.FindElement(_locators.loggedInSpan).Click();

            Assert.True(_userLoggedIn);
        }

        [Test]
        [TestCase("seleniumtests@tut.by", "123456789zxcvbn", ExpectedResult = true)]
        [TestCase("seleniumtests2@tut.by", "123456789zxcvbn", ExpectedResult = true)]
        public bool LoginTutBy_DDT_Successfull(string userName, string password)
        {
            //Open start page
            Initialize.Initialize.LaunchBrowser(driver, _startPage);
            Thread.Sleep(3000); //Sleep is like explisit wait, but it will block run till all time specified.

            //Enter credentials
            driver.FindElement(_locators.enterLink).Click();

            var userNameInput = driver.FindElement(_locators.userNameInput);
            userNameInput.Clear();
            userNameInput.SendKeys(userName);

            var passwordInput = driver.FindElement(_locators.passwordInput);
            passwordInput.Clear();
            passwordInput.SendKeys(password);

            //Login
            driver.FindElement(_locators.enterButton).Click();

            //Validate user logged in
            IsElementDisplayed(_locators.loggedInSpan);
            driver.FindElement(_locators.loggedInSpan).Click();

            return driver.FindElement(_locators.personalRoomText).Displayed;
        }

        [TearDown]
        public void TestCleanup()
        {
            if (_userLoggedIn) driver.FindElement(_locators.logOutLink).Click();
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
                    return driver.FindElement(locator).Displayed;
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
            else _userLoggedIn = true;
        }
    }
}
