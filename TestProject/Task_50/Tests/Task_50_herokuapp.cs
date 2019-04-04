using System;
using System.Text.RegularExpressions;
using Allure.NUnit.Attributes;
using Initialize;
using NUnit.Framework;

namespace Task_50.Tests
{
    [TestFixture]
    [AllureSuite("Task_50_hero tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class Task_50_herokuapp : TestBase
    {
        private Locators.HerokuappLocators _locators;
        private Uri _startPageAlert = new Uri("https://the-internet.herokuapp.com/javascript_alerts");
        private Uri _startPageFrame = new Uri("https://the-internet.herokuapp.com/iframe");
        private readonly string[] _textToEnter = {"Hello ","World!"};

        [OneTimeSetUp]
        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();
            //new AssemblyConfiguration().UpdateAllureConfig();
            _locators = new Locators.HerokuappLocators();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        }

        [Test]
        [AllureTest]
        [AllureSubSuite("FrameTests")]
        [AllureSeverity(Allure.Commons.Model.SeverityLevel.Minor)]
        [AllureLink("4")]
        [AllureOwner("Gleb")]
        public void Herokuapp_AddTextToFrame_Added()
        {
            //Open start page
            LaunchBrowser(_startPageFrame);

            //FInd Frame and clear
            var frameWindow = Driver.FindElement(_locators.frameWindow);
            Driver.SwitchTo().Frame(frameWindow);

            var editArea = Driver.FindElement(_locators.frameBody);
            editArea.Clear();

            //Enter text in Frame
            editArea.SendKeys(_textToEnter[0]);

            Driver.SwitchTo().DefaultContent();
            Driver.FindElement(_locators.boldButton).Click();

            Driver.SwitchTo().Frame(frameWindow);
            editArea.SendKeys(_textToEnter[1]);

            Driver.SwitchTo().DefaultContent();
            Driver.FindElement(_locators.boldButton).Click();

            //Validate text in Frame
            Driver.SwitchTo().Frame(frameWindow);
            var editorText = Regex.Replace(editArea.Text, @"[^\u0000-\u007F]+", string.Empty);

            Assert.AreEqual(_textToEnter[0] + _textToEnter[1], editorText);
            Assert.AreEqual(_textToEnter[1], editArea.FindElement(_locators.strongText).Text);
        }

        [Test]
        [AllureTest]
        [AllureSubSuite("AlertTests")]
        [AllureSeverity(Allure.Commons.Model.SeverityLevel.Minor)]
        [AllureLink("5")]
        [AllureOwner("Gleb")]
        public void Herokuapp_Alert_Added()
        {
            //Open start page
            LaunchBrowser(_startPageAlert);

            //Accept Alert
            _locators.WaitForElementDisplayed(_locators.clickForJSAlertButton);
            Driver.FindElement(_locators.clickForJSAlertButton).Click();
            Driver.SwitchTo().Alert().Accept();

            Assert.AreEqual("You successfuly clicked an alert", Driver.FindElement(_locators.resultMessage).Text);
        }

        [Test]
        [AllureTest]
        [AllureSubSuite("AlertTests")]
        [AllureSeverity(Allure.Commons.Model.SeverityLevel.Minor)]
        [AllureLink("5")]
        [AllureOwner("Gleb")]
        public void Herokuapp_Confirm_Added()
        {
            //Open start page
            LaunchBrowser(_startPageAlert);

            //Cancel Confirm
            _locators.WaitForElementDisplayed(_locators.clickForJSConfirmButton);
            var confirmButton = Driver.FindElement(_locators.clickForJSConfirmButton);
            confirmButton.Click();
            Driver.SwitchTo().Alert().Dismiss();

            Assert.AreEqual("You clicked: Cancel", Driver.FindElement(_locators.resultMessage).Text);

            //Accept Confirm
            confirmButton.Click();
            Driver.SwitchTo().Alert().Accept();

            Assert.AreEqual("You clicked: Ok", Driver.FindElement(_locators.resultMessage).Text);
        }

        [Test]
        [AllureTest]
        [AllureSubSuite("AlertTests")]
        [AllureSeverity(Allure.Commons.Model.SeverityLevel.Minor)]
        [AllureLink("5")]
        [AllureOwner("Gleb")]
        public void Herokuapp_Prompt_Added()
        {
            //Open start page
            LaunchBrowser(_startPageAlert);

            //Cancel Prompt
            _locators.WaitForElementDisplayed(_locators.clickForJSPromptButton);
            var promptButton = Driver.FindElement(_locators.clickForJSPromptButton);
            promptButton.Click();
            Driver.SwitchTo().Alert().Dismiss();

            Assert.AreEqual("You entered: null", Driver.FindElement(_locators.resultMessage).Text);

            //Accept Prompt with no value
            promptButton.Click();
            Driver.SwitchTo().Alert().Accept();

            Assert.AreEqual("You entered:", Driver.FindElement(_locators.resultMessage).Text);

            //Accept Prompt with no value
            promptButton.Click();
            Driver.SwitchTo().Alert().SendKeys("a");
            Driver.SwitchTo().Alert().Accept();

            Assert.AreEqual("You entered: a", Driver.FindElement(_locators.resultMessage).Text);
        }
    }
}
