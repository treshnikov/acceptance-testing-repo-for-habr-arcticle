using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace AcceptanceTesting.Common
{
    /// <summary>
    /// Базовый класс для всех PageObject страниц
    /// Содержит вспомогательные функции для работы с элементами на странице
    /// </summary>
    public class BasePage
    {
        #region Fields

        protected IWebDriver Driver { get; private set; }

        #endregion

        #region Ctor

        public BasePage(IWebDriver driver)
        {
            Driver = driver;

            Driver.Manage()
                .Timeouts()
                .ImplicitlyWait(
                    TimeSpan.FromSeconds(30.0));

            Driver.Manage().Window.Maximize();
        }

        #endregion

        #region Properties

        public string PageUrl
        {
            get { return Driver.Url; }
        }

        public string TabCaption
        {
            get { return Driver.Title.ToString(CultureInfo.InvariantCulture); }
        }

        #endregion

        #region Public methods

        public void SendKeysToElement(IWebElement element, string keys)
        {
            var act = new Actions(Driver);
            act.MoveToElement(element).Click().SendKeys(keys).Build().Perform();
            Trace.WriteLine(string.Format("Ввели строку: {0}", keys));
        }

        public void SendKeysToElement(IWebElement element, string keys, int offsetX, int offsetY)
        {
            var act = new Actions(Driver);
            act.MoveToElement(element, offsetX, offsetY).Click().SendKeys(keys).Build().Perform();
            Trace.WriteLine(string.Format("Ввели строку: {0}", keys));
        }

        public void OpenPage(string pagePath)
        {
            bool isAlertPresent;

            Driver.Navigate().GoToUrl(pagePath);

            try
            {
                Driver.SwitchTo().Alert();
                isAlertPresent = true;
            }
            catch (Exception)
            {
                isAlertPresent = false;
            }

            if (isAlertPresent)
            {
                Driver.SwitchTo().Alert().Dismiss();
            }
        }

        public IWebElement FindElementByXPath(string xpath)
        {
            IWebElement el;
            try
            {
                el = Driver.FindElement(By.XPath(xpath));
            }
            catch (NoSuchElementException ex)
            {
                throw new NoSuchElementException(
                    string.Format("XPath selector = \"{0}\"\n", xpath),
                    ex);
            }

            return el;
        }

        public IWebElement FindElementById(string id)
        {
            IWebElement el;
            try
            {
                el = Driver.FindElement(By.Id(id));
            }
            catch (NoSuchElementException ex)
            {
                throw new NoSuchElementException(
                    string.Format("ID selector = \"{0}\"\n", id),
                    ex);
            }

            return el;
        }

        public IWebElement FindElementByCss(string css)
        {
            IWebElement el;
            try
            {
                el = Driver.FindElement(By.CssSelector(css));
            }
            catch (NoSuchElementException ex)
            {
                throw new NoSuchElementException(
                    string.Format("CSS selector = \"{0}\"\n", css),
                    ex);
            }

            return el;
        }

        public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath)
        {
            ReadOnlyCollection<IWebElement> el;
            try
            {
                el = Driver.FindElements(By.XPath(xpath));
            }
            catch (NoSuchElementException ex)
            {
                throw new NoSuchElementException(
                    string.Format("XPath selector = \"{0}\"\n", xpath),
                    ex);
            }

            return el;
        }

        public ReadOnlyCollection<IWebElement> FindElementsByCss(string css)
        {
            ReadOnlyCollection<IWebElement> el;
            try
            {
                el = Driver.FindElements(By.CssSelector(css));
            }
            catch (NoSuchElementException ex)
            {
                throw new NoSuchElementException(
                    string.Format("CSS selector = \"{0}\"\n", css),
                    ex);
            }

            return el;
        }

        public IWebElement FindElementByClassName(string sclass)
        {
            IWebElement el;
            try
            {
                el = Driver.FindElement(By.ClassName(sclass));
            }
            catch (NoSuchElementException ex)
            {
                throw new NoSuchElementException(
                    string.Format("Class = \"{0}\"\n", sclass),
                    ex);
            }

            return el;
        }

        public object ExecuteJavaScript(string javaScript, params object[] args)
        {
            var javaScriptExecutor = (IJavaScriptExecutor)Driver;

            return javaScriptExecutor.ExecuteScript(javaScript, args);
        }

        public void ScrollToElement(IWebElement element)
        {
            ExecuteJavaScript("arguments[0].scrollIntoView(true);", element);
        }

        public void ScrollToTop()
        {
            ExecuteJavaScript("window.scrollTo(0,0);");
        }

        public void DoubleClickOnElement(IWebElement element)
        {
            new Actions(Driver).MoveToElement(element).DoubleClick().Build().Perform();
        }

        #endregion
    }
}