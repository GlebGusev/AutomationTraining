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
        private Locators.PageObject _locators;
        private readonly Uri _startPage = new Uri("https://www.tut.by/");
        private readonly string _username = "seleniumtests@tut.by";
        private readonly string _password = "123456789zxcvbn";

        [SetUp]
        public void TestSetup()
        {
            driver = new ChromeDriver(chromeDriverDirectory: Initialize.Initialize.GetDriverFolder());
            driver.Manage().Window.Maximize();
            _locators = new Locators.PageObject(driver);
        }

        [Test]
        public void LoginTutBy_CorrectCredentials_Successfull()
        {
            //Open start page
            driver.Navigate().GoToUrl(_startPage);

            //Enter credentials
            _locators.PerformAutorization(_username, _password);

            //Validate user logged in
            _locators.AssertLoggedIn();
        }

        [Test]
        public void LogoutTutBy_ClickLogoutLink_Successfull()
        {
            //Login
            LoginTutBy_CorrectCredentials_Successfull();

            //Logout
            _locators.ClickLogoutLink();

            //Validate user logged out
            _locators.AssertLoggedOut();
        }

        [TearDown]
        public void TestCleanup()
        {
            driver.Quit();
        }
    }
}