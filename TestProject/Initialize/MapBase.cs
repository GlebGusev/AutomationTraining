using System;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Initialize
{
    public abstract class MapBase : TestBase
    {
        private WebDriverWait _wait;
        public IWebDriver driver = Driver;

        public bool WaitForElementDisplayed(By locator)
        {
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            _wait.PollingInterval = TimeSpan.FromSeconds(1);
            return _wait.Until(condition =>
            {
                try
                {
                    return driver.FindElement(locator).Displayed;
                }
                catch (WebDriverTimeoutException)
                {
                    return false;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
                catch (ElementNotInteractableException)
                {
                    return false;
                }
                catch (ElementNotSelectableException)
                {
                    return false;
                }
                catch (NullReferenceException)
                {
                    return false;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
            });
        }

        //public bool WaitWhileElementHasAttribute(IWebElement element, string attribute)
        //{
        //}

        public bool WaitForElementAttributeHasValue(IWebElement element, string attribute, string value = null)
        {
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            return _wait.Until(condition =>
            {
                try
                {
                    return element.GetAttribute(attribute) == value;
                }
                catch (WebDriverTimeoutException)
                {
                    return false;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
                catch (ElementNotInteractableException)
                {
                    return false;
                }
                catch (ElementNotSelectableException)
                {
                    return false;
                }
                catch (NullReferenceException)
                {
                    return false;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
            });
        }

        public bool WaitForElementAttributeNotExist(By locator, string attribute)
        {
            var st = locator.ToString().Split(':');
            var byOption = st.First();
            var searchOptions = string.Join(":", st.Skip(1));
            switch (byOption)
            {
                case "By.CssSelector":
                    locator = By.CssSelector(string.Format("{0}[{1}]", searchOptions, attribute));
                    break;
                case "By.ClassName":
                    locator = By.CssSelector(string.Format("[class='{0}'][{1}]", searchOptions, attribute));
                    break;
                case "By.Id":
                    locator = By.CssSelector(string.Format("[id='{0}'][{1}]", searchOptions, attribute));
                    break;
                case "By.LinkText":
                    locator = By.CssSelector(string.Format("[text='{0}'][{1}]", searchOptions, attribute));
                    break;
                case "By.Name":
                    locator = By.CssSelector(string.Format("[name='{0}'][{1}]", searchOptions, attribute));
                    break;
                case "By.PartialLinkText":
                    locator = By.CssSelector(string.Format("[text*='{0}'][{1}]", searchOptions, attribute));
                    break;
                case "By.TagName":
                    locator = By.CssSelector(string.Format("{0}[{1}]", searchOptions, attribute));
                    break;
                case "By.XPath":
                    locator = By.XPath(string.Format("{0}[{1}]", searchOptions, attribute));
                    break;
                default: Console.WriteLine("Locator type is not supported. If it is a new type, please update all affected methods");
                    break;
            }
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            _wait.PollingInterval = TimeSpan.FromMilliseconds(100);
            return _wait.Until(condition =>
            {
                try
                {
                    return driver.FindElements(locator).Count < 1;
                }
                catch (WebDriverTimeoutException)
                {
                    return false;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
                catch (ElementNotInteractableException)
                {
                    return false;
                }
                catch (ElementNotSelectableException)
                {
                    return false;
                }
                catch (NullReferenceException)
                {
                    return false;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
            });
        }


        public bool IsElementDisplayed(IWebElement element)
        {
            try
            {
                return element.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (ElementNotInteractableException)
            {
                return false;
            }
            catch (ElementNotSelectableException)
            {
                return false;
            }
            catch (NullReferenceException)
            {
                return false;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        }

        public bool isAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }
    }
}
