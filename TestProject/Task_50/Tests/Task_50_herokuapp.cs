using System;
using System.Text.RegularExpressions;
using Allure.NUnit.Attributes;
using Initialize;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Task_50.Tests
{
    [TestFixture]
    [AllureSuite("Task_50_hero tests")]
    public class Task_50_herokuapp : TestBase
    {
        private Locators.HerokuappLocators _locators;
        private Uri _startPageAlert = new Uri("https://the-internet.herokuapp.com/javascript_alerts");
        private Uri _startPageFrame = new Uri("https://the-internet.herokuapp.com/iframe");
        private readonly string[] _textToEnter = {"Hello ","World!"};

        [SetUp]
        public override void TestSetup()
        {
            base.TestSetup();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            _locators = new Locators.HerokuappLocators();
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
            Initialize.Initialize.LaunchBrowser(_driver, _startPageFrame);

            //FInd Frame and clear
            var frameWindow = _driver.FindElement(_locators.frameWindow);
            _driver.SwitchTo().Frame(frameWindow);

            var editArea = _driver.FindElement(_locators.frameBody);
            editArea.Clear();

            //Enter text in Frame
            editArea.SendKeys(_textToEnter[0]);

            _driver.SwitchTo().DefaultContent();
            _driver.FindElement(_locators.boldButton).Click();

            _driver.SwitchTo().Frame(frameWindow);
            editArea.SendKeys(_textToEnter[1]);

            _driver.SwitchTo().DefaultContent();
            _driver.FindElement(_locators.boldButton).Click();

            //Validate text in Frame
            _driver.SwitchTo().Frame(frameWindow);
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
            Initialize.Initialize.LaunchBrowser(_driver, _startPageAlert);

            //Accept Alert
            _locators.WaitForElementDisplayed(_driver, _locators.clickForJSAlertButton);
            _driver.FindElement(_locators.clickForJSAlertButton).Click();
            _driver.SwitchTo().Alert().Accept();

            Assert.AreEqual("You successfuly clicked an alert", _driver.FindElement(_locators.resultMessage).Text);
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
            Initialize.Initialize.LaunchBrowser(_driver, _startPageAlert);

            //Cancel Confirm
            _locators.WaitForElementDisplayed(_driver, _locators.clickForJSConfirmButton);
            var confirmButton = _driver.FindElement(_locators.clickForJSConfirmButton);
            confirmButton.Click();
            _driver.SwitchTo().Alert().Dismiss();

            Assert.AreEqual("You clicked: Cancel", _driver.FindElement(_locators.resultMessage).Text);

            //Accept Confirm
            confirmButton.Click();
            _driver.SwitchTo().Alert().Accept();

            Assert.AreEqual("You clicked: Ok", _driver.FindElement(_locators.resultMessage).Text);
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
            Initialize.Initialize.LaunchBrowser(_driver, _startPageAlert);

            //Cancel Prompt
            _locators.WaitForElementDisplayed(_driver, _locators.clickForJSPromptButton);
            var promptButton = _driver.FindElement(_locators.clickForJSPromptButton);
            promptButton.Click();
            _driver.SwitchTo().Alert().Dismiss();

            Assert.AreEqual("You entered: null", _driver.FindElement(_locators.resultMessage).Text);

            //Accept Prompt with no value
            promptButton.Click();
            _driver.SwitchTo().Alert().Accept();

            Assert.AreEqual("You entered:", _driver.FindElement(_locators.resultMessage).Text);

            //Accept Prompt with no value
            promptButton.Click();
            _driver.SwitchTo().Alert().SendKeys("a");
            _driver.SwitchTo().Alert().Accept();

            Assert.AreEqual("You entered: a", _driver.FindElement(_locators.resultMessage).Text);
        }
    }
}
