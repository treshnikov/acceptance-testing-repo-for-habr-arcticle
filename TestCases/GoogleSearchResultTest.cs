using AcceptanceTesting.Common;
using AcceptanceTesting.PageObjects;
using NUnit.Framework;

namespace AcceptanceTesting.TestCases
{
    public class GoogleSearchResultTest : BaseTest
    {
        private GoogleStartPage googlePage;
        private GoogleSearchResultPage googleSearchResultPage;

        public GoogleSearchResultTest(string browserName) : base(browserName)
        {
        }

        [Test]
        public void Google_Should_Find_SMS_Site()
        {
            Step("Ввод запроса 'смс автоматизация'",
                () => googleSearchResultPage = googlePage.EnterSearchText("смс автоматизация"));

            Step("Первая ссылка должна содержать адрес sms-automation.ru",
                () =>
                {
                    var href = googleSearchResultPage.FirstLink.GetAttribute("href");
                    Assert.AreEqual(href, "http://www.sms-automation.ru/");
                });
        }

        protected override void Setup()
        {
            base.Setup();
            Step("Открыть google.com", () => googlePage = new GoogleStartPage(Driver));
        }
    }
}