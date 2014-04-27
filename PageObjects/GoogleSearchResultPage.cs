using AcceptanceTesting.Common;
using OpenQA.Selenium;

namespace AcceptanceTesting.PageObjects
{
    public class GoogleSearchResultPage : BasePage
    {
        public IWebElement FirstLink { get { return FindElementByCss("#rso > li > div > div > h3 > a"); } }

        public GoogleSearchResultPage(IWebDriver driver) : base(driver)
        {
        }
    }
}