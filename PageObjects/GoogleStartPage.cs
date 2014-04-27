using AcceptanceTesting.Common;
using OpenQA.Selenium;

namespace AcceptanceTesting.PageObjects
{
    public class GoogleStartPage : BasePage
    {
        public IWebElement Input { get { return FindElementByCss("#gbqfq"); } }

        public GoogleStartPage(IWebDriver driver) : base(driver)
        {
            OpenPage("http://google.com");
        }

        public GoogleSearchResultPage EnterSearchText(string arg)
        {
            Input.SendKeys(arg);

            return new GoogleSearchResultPage(Driver);
        }
    }
}