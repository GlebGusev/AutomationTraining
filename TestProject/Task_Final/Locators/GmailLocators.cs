using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Initialize;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Task_Final.Locators
{
    public class GmailLocators : MapBase
    {
        //login pages
        public By ProgressBar = By.Id("initialView");

        public By UserNameInput = By.Id("identifierId");

        public By IdentifierNextButton = By.Id("identifierNext");

        public By PasswordInput = By.Name("password");
        
        public By PasswordNextButton = By.Id("passwordNext"); 

        public By ProfileIdentifier = By.Id("profileIdentifier");
        
        public By ChooseAccountList = By.CssSelector("ul[class='FPFGmf']");

        public By ChooseAccountListItems = By.CssSelector("ul[class='FPFGmf'] > li > div");
        
        //mail application
        public By SignOutOptionsHyperlink = By.CssSelector("a[href*='https://accounts.google.com/SignOutOptions']");

        public By LogoutHyperLink = By.CssSelector("a[href*='https://accounts.google.com/Logout']");

        public By InboxhHyperlink = By.CssSelector("a[href='https://mail.google.com/mail/u/0/#inbox']");

        public By SentEmailHyperlink = By.CssSelector("a[href='https://mail.google.com/mail/u/0/#sent']");

        public By TrashHyperlink = By.CssSelector("a[href='https://mail.google.com/mail/u/0/#trash']");

        public By CollapseMoreOptionsPane = By.CssSelector("span[class*='J-Ke n4 ah9']");

        public By NewEmailForm = By.ClassName("AD");

        public By NewEmailButton = By.CssSelector("div[class='T-I J-J5-Ji T-I-KE L3']");

        public By ReceiversPane = By.CssSelector("div[class='aoD hl']");

        public By NewEmailTo = By.CssSelector("textarea[name='to']");

        public By NewEmailSubject = By.Name("subjectbox");

        public By NewEmailBody = By.CssSelector("div[class='Am Al editable LW-avf']");

        public By NewEmailSendButton = By.CssSelector("div[class='J-J5-Ji btA']");

        public By EmailSentMessagePane = By.CssSelector("div.vh span.bAq");
        
        public By EmailsTable = By.CssSelector("div[class*='BltHke nH oy8Mbf'][role = 'main'] div[class*='ae4']:not([style='display: none;']) table[class='F cf zt']"); //does it change everydayy??

        public By EmailCheckBox = By.CssSelector("div[role=checkbox]");
        
        public By RemoveEmailPane = By.CssSelector("div[class='D E G-atb']:not([style='display: none;']) div[act='10']");

        //methods
        public void PopulateLogin(Uri url, string user, string password)
        {
            driver.Navigate().GoToUrl(url);

            //var progressBar = driver.FindElement(ProgressBar); // id=initialView area-busy=true

            // because driver does not navigate to the same url on the next step (redirect from https://mail.google.com to https://www.google.com)
            var profilesDropDown = driver.FindElements(ProfileIdentifier);
            if (profilesDropDown.Count > 0)
            {
                if (!driver.Url.Contains("https://accounts.google.com/ServiceLogin/signinchooser?"))
                {
                    profilesDropDown.First().Click();

                    //WaitForElementAttributeNotExist(ProgressBar, "area-busy");
                    Thread.Sleep(1000);
                }

                var accountsList = driver.FindElements(ChooseAccountListItems);
                accountsList[accountsList.Count-2].Click(); // penultimate item in the list - Change account
            }
            WaitForElementDisplayed(IdentifierNextButton);
            Thread.Sleep(500);

            driver.FindElement(UserNameInput).SendKeys(user);
            driver.FindElement(IdentifierNextButton).Click();

            WaitForElementDisplayed(PasswordNextButton);
            Thread.Sleep(500);

            driver.FindElement(PasswordInput).SendKeys(password);
            driver.FindElement(PasswordNextButton).Click();

            WaitForElementDisplayed(NewEmailButton);
            Thread.Sleep(500);
        }

        public void LogOut()
        {
            var signOutOptions =  driver.FindElements(SignOutOptionsHyperlink);
            if (signOutOptions.Count > 0)
            {
                driver.FindElement(SignOutOptionsHyperlink).Click();

                driver.FindElement(LogoutHyperLink).Click();

                if(isAlertPresent()) Driver.SwitchTo().Alert().Accept();

                WaitForElementDisplayed(ProfileIdentifier);
            }
            else Console.WriteLine("User is already signed out or webcontrol properties were changed");
        }

        public void SendEmail(string receiver, string subject, string body)
        {
            driver.FindElement(NewEmailButton).Click();

            WaitForElementDisplayed(NewEmailTo);

            driver.FindElement(NewEmailTo).SendKeys(receiver);
            driver.FindElement(NewEmailSubject).SendKeys(subject);
            driver.FindElement(NewEmailBody).SendKeys(body);
            driver.FindElement(NewEmailSendButton).Click();

            WaitForElementDisplayed(EmailSentMessagePane);
        }

        public void NavigateToTheFolder(string folder)
        {
            //var landingParameter = string.Format("/{0}", driver.Url.Split('/').Last());
            //if (landingParameter == folder) return;

            switch (folder)
            {
                case EmailFolderOptions.Inbox:
                    driver.FindElement(InboxhHyperlink).Click();
                    break;
                case EmailFolderOptions.Sent:
                    driver.FindElement(SentEmailHyperlink).Click();
                    break;
                case EmailFolderOptions.Trash:
                    driver.FindElement(CollapseMoreOptionsPane).Click();
                    driver.FindElement(TrashHyperlink).Click();
                    break;
                default:
                    Assert.Fail("Folder is not suppoted. Please add to Task_Final.Locators.GmailLocators.EmailFolderOptions.cs and update all places were it used");
                    break;
            }
            Thread.Sleep(3000); // to give a time for DOM to change styles, using attributes validation will add complicated and hardly supported logic
        }

        public void RemoveEmail(List<string> searchValues)
        {
            var table = driver.FindElement(EmailsTable);
            var row = GetRow(table, searchValues);

            row.FindElement(EmailCheckBox).Click();
            WaitForElementDisplayed(RemoveEmailPane);
            driver.FindElement(RemoveEmailPane).Click();

            //WaitForElementDisplayed(EmailSentMessagePane);
        }

        public void AssertEmailExistInTable(string folder, List<string> searchValues)
        {
            switch (folder)
            {
                case EmailFolderOptions.Inbox:
                    Assert.True(driver.Url.Contains(EmailFolderOptions.Inbox));
                    break;
                case EmailFolderOptions.Sent:
                    Assert.True(driver.Url.Contains(EmailFolderOptions.Sent));
                    break;
                case EmailFolderOptions.Trash:
                    Assert.True(driver.Url.Contains(EmailFolderOptions.Trash));
                    break;
                default: Assert.Fail("Folder is not suppoted. Please add to Task_Final.Locators.GmailLocators.EmailFolderOptions.cs and update all places were it used");
                    break;
            }

            var table = driver.FindElement(EmailsTable);
            var row = GetRow(table, searchValues);
            
            Assert.NotNull(row, "Row is not found with the search values: {0}", string.Join(", ", searchValues));
        }

        public IWebElement GetRow(IWebElement table, List<string> searchValues)
        {
            var tableRows = new List<IWebElement>(table.FindElements(By.TagName("tr")));
            foreach (var row in tableRows)
            {
                if (searchValues.All(x => row.Text.Contains(x))) return row;
            }

            return null;
        }
    }

    public class EmailFolderOptions
    {
        public const string Inbox = "/#inbox";
        public const string Sent = "/#sent";
        public const string Trash = "/#trash";  
    }
}