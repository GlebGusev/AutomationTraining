using Initialize;
using OpenQA.Selenium;

namespace Task_50.Locators
{
    public class HerokuappLocators : MapBase
    {
        public By frameWindow = By.Id("mce_0_ifr");

        public By frameBody = By.Id("tinymce");

        public By strongText = By.TagName("strong");
        
        public By boldButton = By.CssSelector("button>i.mce-ico.mce-i-bold");

        public By clickForJSAlertButton = By.XPath("//button[text()='Click for JS Alert']");

        public By clickForJSConfirmButton = By.XPath("//button[text()='Click for JS Confirm']");

        public By clickForJSPromptButton = By.XPath("//button[text()='Click for JS Prompt']");

        public By resultMessage = By.CssSelector("#result");
    }
}