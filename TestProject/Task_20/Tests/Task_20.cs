using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Task_20.Tests
{
    [TestFixture]
    public class Task_20
    {
        private IWebDriver driver;
        private Locators.Locators _locators;
        private readonly Uri _startPage = new Uri("https://www.tut.by/");
        private readonly string _username = "seleniumtests@tut.by";
        private readonly string _password = "123456789zxcvbn";

        [SetUp]
        public void TestSetup()
        {
            driver = new ChromeDriver(chromeDriverDirectory: Initialize.Initialize.GetDriverFolder());
            driver.Manage().Window.Maximize();
            _locators = new Locators.Locators();
        }

        [Test]
        public void LoginTutBy_CorrectCredentials_Successfull()
        {
            //Open start page
            Initialize.Initialize.LaunchBrowser(driver, _startPage);

            //Enter credentials
            driver.FindElement(_locators.enterLink).Click();
            
            var userNameInput = driver.FindElement(_locators.userNameInput);
            userNameInput.Clear();
            userNameInput.SendKeys(_username);

            var passwordInput = driver.FindElement(_locators.passwordInput);
            passwordInput.Clear();
            passwordInput.SendKeys(_password);

            //Assert forgot password link displays
            Assert.True(driver.FindElement(_locators.forgotPasswordLink).Displayed, "Forgot password link is not displayed");

            //Login
            driver.FindElement(_locators.enterButton).Click();

            //Validate user logged in
            driver.FindElement(_locators.loggedInSpan).Click();

            Assert.AreEqual("Личный кабинет", driver.FindElement(_locators.personalRoomText).Text);
        }

        [TearDown]
        public void TestCleanup()
        {
            driver.FindElement(_locators.logOutLink).Click();
            driver.Quit();
        }
    }
}
