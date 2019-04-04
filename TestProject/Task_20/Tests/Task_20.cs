using System;
using Allure.NUnit.Attributes;
using Initialize;
using NUnit.Framework;

namespace Task_20.Tests
{
    [TestFixture]
    [AllureSuite("Task_20 tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class Task_20 : TestBase
    {
        private Locators.Locators _locators;
        private readonly Uri _startPage = new Uri("https://www.tut.by/");
        private readonly string _username = "seleniumtests@tut.by";
        private readonly string _password = "123456789zxcvbn";

        [OneTimeSetUp]
        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();
            //new AssemblyConfiguration().UpdateAllureConfig();
            _locators = new Locators.Locators();
        }

        [Test]
        [AllureTest]
        [AllureSubSuite("LoginTutBy")]
        [AllureSeverity(Allure.Commons.Model.SeverityLevel.Minor)]
        [AllureLink("1")]
        [AllureOwner("Gleb")]
        public void LoginTutBy_CorrectCredentials_Successfull()
        {
            //Open start page
            LaunchBrowser(_startPage);

            //Enter credentials
            Driver.FindElement(_locators.enterLink).Click();
            
            var userNameInput = Driver.FindElement(_locators.userNameInput);
            userNameInput.Clear();
            userNameInput.SendKeys(_username);

            var passwordInput = Driver.FindElement(_locators.passwordInput);
            passwordInput.Clear();
            passwordInput.SendKeys(_password);

            //Assert forgot password link displays
            Assert.True(Driver.FindElement(_locators.forgotPasswordLink).Displayed, "Forgot password link is not displayed");

            //Login
            Driver.FindElement(_locators.enterButton).Click();

            //Validate user logged in
            Driver.FindElement(_locators.loggedInSpan).Click();

            Assert.AreEqual("Личный кабинет", Driver.FindElement(_locators.personalRoomText).Text);

            Driver.FindElement(_locators.logOutLink).Click();
        }
    }
}
