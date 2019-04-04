using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Allure.NUnit.Attributes;
using Initialize;
using Initialize.DataDriven;
using NUnit.Framework;
using Task_Final.Locators;

namespace Task_Final.Task_Final
{
    [TestFixture]
    [AllureSuite("Task_Final tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class Task_Final : TestBase
    {
        private GmailLocators _locators;
        private readonly Uri _startPage = new Uri("https://mail.google.com");
        private Dictionary<string, string> _credentials;

        [OneTimeSetUp]
        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();
            //new AssemblyConfiguration().UpdateAllureConfig();
            _locators = new GmailLocators();
            _credentials = new AssemblyConfiguration().GetCredentialsFromDataFile();
        }

        [Test]
        [TestCaseSource(typeof(TestData), "LoginCredentials")]
        [AllureTest]
        [AllureSubSuite("LoginGmail")]
        [AllureSeverity(Allure.Commons.Model.SeverityLevel.Normal)]
        [AllureLink("7")]
        [AllureOwner("Gleb")]
        public void LoginGmail_CorrectCredentials_Successfull(IDictionary<string, string> parameters)
        {
            _locators.PopulateLogin(_startPage, parameters["user"], parameters["password"]);
            
            Assert.AreEqual(bool.Parse(parameters["expectedResult"]), Driver.FindElement(_locators.NewEmailButton).Displayed, "Inbox page is not displayed");
        }

        [Test]
        [TestCaseSource(typeof(TestData), "LogoutCredentials")]
        [AllureTest]
        [AllureSubSuite("LoginGmail")]
        [AllureSeverity(Allure.Commons.Model.SeverityLevel.Normal)]
        [AllureLink("7")]
        [AllureOwner("Gleb")]
        public void LogoutGmail_CorrectCredentials_Successfull(IDictionary<string, string> parameters)
        {
            _locators.PopulateLogin(_startPage, parameters["user"], parameters["password"]);

            Assert.AreEqual(bool.Parse(parameters["expectedResult"]), Driver.FindElement(_locators.NewEmailButton).Displayed, "Inbox page is not displayed");

            _locators.LogOut();

            Assert.AreEqual(bool.Parse(parameters["expectedResult"]), Driver.FindElement(_locators.ProfileIdentifier).Displayed, "Inbox page is not displayed");
        }

        [Test]
        [AllureTest]
        [AllureSubSuite("Emails")]
        [AllureSeverity(Allure.Commons.Model.SeverityLevel.Normal)]
        [AllureLink("8")]
        [AllureOwner("Gleb")]
        public void Gmail_SendEmail_Delivered()
        {
            var subject = new Random().Next(000000, 999999).ToString();
            var body = "Your password";
            var senderName = "Test1 Test1";

            _locators.PopulateLogin(_startPage, _credentials.ElementAt(0).Key, _credentials.ElementAt(0).Value);
            _locators.SendEmail(_credentials.ElementAt(1).Key, subject, body);
            _locators.LogOut();

            Thread.Sleep(5000); //give email service some time to deliver email

            _locators.PopulateLogin(_startPage, _credentials.ElementAt(1).Key, _credentials.ElementAt(1).Value);
            _locators.AssertEmailExistInTable(EmailFolderOptions.Inbox, new List<string> { senderName, subject });
        }

        [Test]
        [AllureTest]
        [AllureSubSuite("Emails")]
        [AllureSeverity(Allure.Commons.Model.SeverityLevel.Normal)]
        [AllureLink("8")]
        [AllureOwner("Gleb")]
        public void GmailSentFolder_SendEmail_Displayed()
        {
            var subject = new Random().Next(000000, 999999).ToString();
            var body = "Test Email";
            var receiverName = "glebgusev";

            _locators.PopulateLogin(_startPage, _credentials.ElementAt(1).Key, _credentials.ElementAt(1).Value);
            _locators.SendEmail(_credentials.ElementAt(2).Key, subject, body);

            _locators.NavigateToTheFolder(EmailFolderOptions.Sent);
            _locators.AssertEmailExistInTable(EmailFolderOptions.Sent, new List<string> { receiverName, subject });
        }

        [Test]
        [AllureTest]
        [AllureSubSuite("Emails")]
        [AllureSeverity(Allure.Commons.Model.SeverityLevel.Normal)]
        [AllureLink("8")]
        [AllureOwner("Gleb")]
        public void GmailTrashFolder_DeleteEmail_Displayed()
        {
            var subject = new Random().Next(000000, 999999).ToString();
            var body = "Email to remove";
            var receiverName = "glebgusev";

            _locators.PopulateLogin(_startPage, _credentials.ElementAt(2).Key, _credentials.ElementAt(2).Value);
            _locators.SendEmail(_credentials.ElementAt(0).Key, subject, body);

            _locators.NavigateToTheFolder(EmailFolderOptions.Sent);
            _locators.RemoveEmail(new List<string> { receiverName, subject });

            _locators.NavigateToTheFolder(EmailFolderOptions.Trash);
            _locators.AssertEmailExistInTable(EmailFolderOptions.Trash, new List<string> { subject });
        }

        [TearDown]
        public override void TestCleanup()
        {
            base.TestCleanup();
            _locators.LogOut();
        }
    }
}