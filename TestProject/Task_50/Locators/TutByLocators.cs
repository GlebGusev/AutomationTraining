﻿using Initialize;
using OpenQA.Selenium;

namespace Task_50.Locators
{
    public class TutByLocators : MapBase
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
    }
}