using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace Task_70.Locators
{
    public class PageFactory
    {
        [FindsBy(How=How.XPath, Using = "//a[contains(text(),'Войти')]")]
        IWebElement enterLink;

        [FindsBy(How = How.PartialLinkText, Using = "не помню")]
        IWebElement forgotPasswordLink;

        [FindsBy(How = How.Id, Using = "top_bar_helper")]
        IWebElement frameWindow;

        [FindsBy(How = How.Name, Using = "login")]
        IWebElement userNameInput;

        [FindsBy(How = How.Name, Using = "password")]
        IWebElement passwordInput;

        [FindsBy(How = How.ClassName, Using = "auth__enter")]
        IWebElement enterButton;

        [FindsBy(How = How.TagName, Using = "strong")]
        IWebElement personalRoomText;

        [FindsBy(How = How.CssSelector, Using = ".uname")]
        IWebElement loggedInSpan;

        [FindsBy(How = How.LinkText, Using = "Выйти")]
        IWebElement logOutLink;

        private IWebDriver driver;

        public PageFactory(IWebDriver driver)
        {
            this.driver = driver;
        }

        public void ClickEnterLink()
        {
            enterLink.Click();
        }

        public void PopulateUsername(string username)
        {
            userNameInput.SendKeys(username);
        }

        public void PopulatePassword(string password)
        {
            passwordInput.SendKeys(password);
        }

        public void ClickLoginButton()
        {
            enterButton.Click();
        }

        public void ClickLoggedInSpan()
        {
            loggedInSpan.Click();
        }

        public void ClickLogoutLink()
        {
            logOutLink.Click();
        }

        public void PerformAutorization(string username, string password)
        {
            ClickEnterLink();
            PopulateUsername(username);
            PopulatePassword(password);
            ClickLoginButton();
        }

        public void AssertLoggedIn()
        {
            ClickLoggedInSpan();

            Assert.AreEqual("Личный кабинет", personalRoomText.Text);
        }

        public void AssertLoggedOut()
        {
            Assert.IsNotNull(enterLink);
        }
    }
}