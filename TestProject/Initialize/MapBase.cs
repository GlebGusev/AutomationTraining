using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Initialize
{
    public abstract class MapBase : TestBase
    {
        private WebDriverWait _wait;

        public bool WaitForElementDisplayed(IWebDriver driver, By locator)
        {
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
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
            catch (NullReferenceException)
            {
                return false;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            };
        }
    }
}
