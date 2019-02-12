using System;
using Allure.NUnit.Attributes;
using Initialize;
using NUnit.Framework;

namespace Task_20.Tests
{
    [TestFixture]
    [AllureSuite("Task_20 tests")]
    public class Task_20 : TestBase
    {
        private Locators.Locators _locators;
        private readonly Uri _startPage = new Uri("https://www.tut.by/");
        private readonly string _username = "seleniumtests@tut.by";
        private readonly string _password = "123456789zxcvbn";

        [SetUp]
        public override void TestSetup()
        {
            base.TestSetup();
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
            Initialize.Initialize.LaunchBrowser(_driver, _startPage);

            //Enter credentials
            _driver.FindElement(_locators.enterLink).Click();
            
            var userNameInput = _driver.FindElement(_locators.userNameInput);
            userNameInput.Clear();
            userNameInput.SendKeys(_username);

            var passwordInput = _driver.FindElement(_locators.passwordInput);
            passwordInput.Clear();
            passwordInput.SendKeys(_password);

            //Assert forgot password link displays
            Assert.True(_driver.FindElement(_locators.forgotPasswordLink).Displayed, "Forgot password link is not displayed");

            //Login
            _driver.FindElement(_locators.enterButton).Click();

            //Validate user logged in
            _driver.FindElement(_locators.loggedInSpan).Click();

            Assert.AreEqual("Личный кабинет", _driver.FindElement(_locators.personalRoomText).Text);

            _driver.FindElement(_locators.logOutLink).Click();
        }
    }
}
