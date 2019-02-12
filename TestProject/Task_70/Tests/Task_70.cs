using System;
using Allure.NUnit.Attributes;
using Initialize;
using NUnit.Framework;
using OpenQA.Selenium.Support.PageObjects;

namespace Task_70.Tests
{
    [TestFixture]
    [AllureSuite("Task_70 tests")]
    public class Task_70 : TestBase
    {
        private Locators.PageFactory _locators;
        private readonly Uri _startPage = new Uri("https://www.tut.by/");
        private readonly string _username = "seleniumtests@tut.by";
        private readonly string _password = "123456789zxcvbn";

        [SetUp]
        public override void TestSetup()
        {
            base.TestSetup();
            _locators = PageFactory.InitElements<Locators.PageFactory>(_driver);
        }

        [Test]
        [AllureTest]
        [AllureSubSuite("LoginTutBy")]
        [AllureSeverity(Allure.Commons.Model.SeverityLevel.Normal)]
        [AllureLink("6")]
        [AllureOwner("Gleb")]
        public void Task90_LoginTutByWrongCredentials_Failed()
        {
            var username = "seleniumtests444@tut.by";

            //Open start page
            Initialize.Initialize.LaunchBrowser(_driver, _startPage);

            //Enter credentials
            _locators.PerformAutorization(username, _password);

            //Validate user logged in
            _locators.AssertLoggedIn();
        }

        [Test]
        [AllureTest]
        [AllureSubSuite("LoginTutBy")]
        [AllureSeverity(Allure.Commons.Model.SeverityLevel.Normal)]
        [AllureLink("6")]
        [AllureOwner("Gleb")]
        public void LoginTutBy_CorrectCredentials_Successfull()
        {
            //Open start page
            Initialize.Initialize.LaunchBrowser(_driver, _startPage);

            //Enter credentials
            _locators.PerformAutorization(_username, _password);

            //Validate user logged in
            _locators.AssertLoggedIn();
        }

        [Test]
        [AllureTest]
        [AllureSubSuite("LoginTutBy")]
        [AllureSeverity(Allure.Commons.Model.SeverityLevel.Normal)]
        [AllureLink("6")]
        [AllureOwner("Gleb")]
        public void LogoutTutBy_ClickLogoutLink_Successfull()
        {
            //Login
            LoginTutBy_CorrectCredentials_Successfull();

            //Logout
            _locators.PerformLogOut();

            //Validate user logged out
            _locators.AssertLoggedOut();
        }
    }
}