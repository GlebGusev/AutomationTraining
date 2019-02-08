using OpenQA.Selenium;
using NUnit.Framework;

namespace Task_70.Locators
{
    public class PageObject
    {
        public By enterLink = By.XPath("//a[contains(text(),'Войти')]");

        public By forgotPasswordLink = By.PartialLinkText("не помню");

        public By frameWindow = By.Id("top_bar_helper");

        public By userNameInput = By.Name("login");

        public By passwordInput = By.Name("password");

        public By enterButton = By.ClassName("auth__enter");

        public By personalRoomText = By.TagName("strong");

        public By loggedInSpan = By.CssSelector(".uname");

        public By logOutLink = By.LinkText("Выйти");

        private IWebDriver driver;

        public PageObject(IWebDriver driver)
        {
            this.driver = driver;
        }

        public PageObject ClickEnterLink()
        {
            driver.FindElement(enterLink).Click();
            return this;
        }

        public PageObject PopulateUsername(string username)
        {
            driver.FindElement(userNameInput).SendKeys(username);
            return this;
        }

        public PageObject PopulatePassword(string password)
        {
            driver.FindElement(passwordInput).SendKeys(password);
            return this;
        }

        public PageObject ClickLoginButton()
        {
            driver.FindElement(enterButton).Click();
            return this;
        }

        public PageObject ClickLoggedInSpan()
        {
            driver.FindElement(loggedInSpan).Click();
            return this;
        }

        public PageObject ClickLogoutLink()
        {
            driver.FindElement(logOutLink).Click();
            return this;
        }

        public PageObject PerformAutorization(string username, string password)
        {
            ClickEnterLink();
            PopulateUsername(username);
            PopulatePassword(password);
            ClickLoginButton();
            return this;
        }

        public void AssertLoggedIn()
        {
            ClickLoggedInSpan();

            Assert.AreEqual("Личный кабинет", driver.FindElement(personalRoomText).Text);
        }

        public void AssertLoggedOut()
        {
            Assert.IsNotNull(driver.FindElement(enterLink));
        }
    }
}