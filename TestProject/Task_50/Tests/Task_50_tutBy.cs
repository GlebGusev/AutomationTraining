using System;
using System.Threading;
using Allure.NUnit.Attributes;
using Initialize;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Task_50.Tests
{
    [TestFixture]
    [AllureSuite("Task_50 tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class Task_50_tutBy : TestBase
    {
        private Locators.TutByLocators _locators;
        private readonly Uri _startPage = new Uri("https://www.tut.by/");
        private readonly string _username = "seleniumtests@tut.by";
        private readonly string _password = "123456789zxcvbn";

        [SetUp]
        public override void TestSetup()
        {
            base.TestSetup();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            _locators = new Locators.TutByLocators();
        }

        [Test]
        [AllureTest]
        [AllureSubSuite("LoginTutBy")]
        [AllureSeverity(Allure.Commons.Model.SeverityLevel.Minor)]
        [AllureLink("2")]
        [AllureOwner("Gleb")]
        public void LoginTutBy_CorrectCredentialsAndWait_Successfull()
        {
            //Open start page
            LaunchBrowser(Driver, _startPage);
            Thread.Sleep(3000); //Sleep is like explisit wait, but it will block run till all time specified.

            //Enter credentials
            Driver.FindElement(_locators.enterLink).Click();

            var userNameInput = Driver.FindElement(_locators.userNameInput);
            userNameInput.Clear();
            userNameInput.SendKeys(_username);

            var passwordInput = Driver.FindElement(_locators.passwordInput);
            passwordInput.Clear();
            passwordInput.SendKeys(_password);

            //Login
            Driver.FindElement(_locators.enterButton).Click();

            //Validate user logged in
            _locators.WaitForElementDisplayed(Driver, _locators.loggedInSpan);
            Driver.FindElement(_locators.loggedInSpan).Click();

            Assert.True(Driver.FindElement(_locators.personalRoomText).Displayed);
        }

        [Test]
        [TestCase("seleniumtests@tut.by", "123456789zxcvbn", ExpectedResult = true)]
        [TestCase("seleniumtests2@tut.by", "123456789zxcvbn", ExpectedResult = true)]
        [AllureTest]
        [AllureSubSuite("LoginTutBy")]
        [AllureSeverity(Allure.Commons.Model.SeverityLevel.Minor)]
        [AllureLink("3")]
        [AllureOwner("Gleb")]
        public bool LoginTutBy_DDT_Successfull(string userName, string password)
        {
            //Open start page
            LaunchBrowser(Driver, _startPage);
            Thread.Sleep(3000); //Sleep is like explisit wait, but it will block run till all time specified.

            //Enter credentials
            Driver.FindElement(_locators.enterLink).Click();

            var userNameInput = Driver.FindElement(_locators.userNameInput);
            userNameInput.Clear();
            userNameInput.SendKeys(userName);

            var passwordInput = Driver.FindElement(_locators.passwordInput);
            passwordInput.Clear();
            passwordInput.SendKeys(password);

            //Login
            Driver.FindElement(_locators.enterButton).Click();

            //Validate user logged in
            _locators.WaitForElementDisplayed(Driver, _locators.loggedInSpan);
            Driver.FindElement(_locators.loggedInSpan).Click();

            return Driver.FindElement(_locators.personalRoomText).Displayed;
        }
    }
}
