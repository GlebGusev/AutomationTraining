using System;
using Allure.NUnit.Attributes;
using Initialize;
using NUnit.Framework;
using OpenQA.Selenium.Support.PageObjects;

namespace Task_70.Tests
{
    [TestFixture]
    [AllureSuite("Task_70 tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class Task_70 : TestBase
    {
        private Locators.PageFactory _locators;
        private readonly Uri _startPage = new Uri("https://www.tut.by/");
        private readonly string _username = "seleniumtests@tut.by";
        private readonly string _password = "123456789zxcvbn";

        [OneTimeSetUp]
        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();
            //new AssemblyConfiguration().UpdateAllureConfig();
            _locators = PageFactory.InitElements<Locators.PageFactory>(Driver);
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
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
            LaunchBrowser(_startPage);

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
            LoginToAccount();
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
            LoginToAccount();

            //Logout
            _locators.PerformLogOut();

            //Validate user logged out
            _locators.AssertLoggedOut();
        }

        private void LoginToAccount()
        {
            //Open start page
            LaunchBrowser(_startPage);

            //Enter credentials
            _locators.PerformAutorization(_username, _password);

            //Validate user logged in
            _locators.AssertLoggedIn();
        }
    }
}