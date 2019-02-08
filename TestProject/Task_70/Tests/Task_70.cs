using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Task_70.Tests
{
    [TestFixture]
    public class Task_70
    {
        private IWebDriver driver;
        private WebDriverWait _wait;
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
            driver.Navigate().GoToUrl(_startPage);

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
            driver.FindElement(_locators.loggedInSpan).Click();

            Assert.AreEqual("Личный кабинет", driver.FindElement(_locators.personalRoomText).Text);
        }

        [Test]
        public void LogoutTutBy_ClickLogoutLink_Successfull()
        {
            //Login
            LoginTutBy_CorrectCredentials_Successfull();

            //Logout
            driver.FindElement(_locators.logOutLink).Click();
        }

        [TearDown]
        public void TestCleanup()
        {
            driver.Quit();
        }
    }
}
