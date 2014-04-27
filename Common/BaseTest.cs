using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AcceptanceTesting.Common
{
    /// <summary>
    /// Базовый класс UI тестов
    /// </summary>
    [Timeout(30000)]
    [TestFixture("Chrome")]
    public class BaseTest
    {
        #region Fields

        public IWebDriver Driver { get; private set; }

        public readonly string BrowserName;

        private readonly string screenshotsDir = ConfigurationManager.AppSettings["ScreenshotsDir"];

        private readonly DateTime starTime = DateTime.Now;

        private int screenshotNumber;

        #endregion

        #region Ctor

        public BaseTest(string browserName)
        {
            BrowserName = browserName;
        }

        #endregion

        #region Public methods

        public void Step(string message, Action action)
        {
            LogMessage(string.Format("Выполняется кейс: '{0}'", message));

            try
            {
                action();
            }
            catch (Exception)
            {
                DoScreenShotOnError(message);
                throw;
            }
        }

        public void ReloadDriver()
        {
            try
            {
                Driver.Quit();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception: " + ex);
            }

            Driver = BrowserFactory.GetLocalDriver(BrowserName);
            LogMessage(" [Перезагрузили драйвер Selenium-а]");
        }
        
        #endregion

        #region Public methods - config test

        /// <summary>
        /// Настройка перед запуском тест кейса
        /// </summary>
        [TestFixtureSetUp]
        public void FixtureBaseSetup()
        {
            Driver = BrowserFactory.GetLocalDriver(BrowserName);
            FixtureSetup();
        }

        /// <summary>
        /// Окончание тест кейса
        /// </summary>
        [TestFixtureTearDown]
        public void FixtureBaseTearDown()
        {
            FixtureTearDown();
            try
            {
                Driver.Quit();
            }
            catch (Exception ex)
            {
                LogMessage("Exception: " + ex);
            }
        }

        /// <summary>
        /// Настройка перед запуском каждого теста в тест кейсе
        /// </summary>
        [SetUp]
        public void BaseSetup()
        {
            Setup();
        }

        /// <summary>
        /// Выполнение действий по завершению каждого теста в тест кейсе
        /// </summary>
        [TearDown]
        public void BaseTearDown()
        {
            TearDown();
        }

        #endregion

        #region Protected methods for config

        /// <summary>
        /// Метод для настройки начального состояния системы ДО запуска теста.
        /// </summary>
        protected virtual void Setup()
        {
        }

        /// <summary>
        /// Метод приведения системы после выполнения теста.
        /// </summary>
        protected virtual void TearDown()
        {
        }

        /// <summary>
        /// Метод для настройки начального состояния системы ДО запуска тест кейса
        /// </summary>
        protected virtual void FixtureSetup()
        {
        }

        /// <summary>
        /// Метод для настройки начального состояния системы ДО запуска тест кейса
        /// </summary>
        protected virtual void FixtureTearDown()
        {
        }

        #endregion

        #region Private helpers

        private string SaveScreenShot(string screenMark, string testCaseDescription)
        {
            var screenshotSavePath = GetScreenshotSavePath();
            var screenshotFileName = GetScreenshotFileName();

            if (!Directory.Exists(screenshotSavePath))
            {
                try
                {
                    Directory.CreateDirectory(screenshotSavePath);
                }
                catch(Exception ex)
                {
                    LogMessage(ex.ToString());
                }
            }

            var ss = ((ITakesScreenshot) Driver).GetScreenshot();
            var fullpath = string.Format(@"{0}\{1}_{2}", screenshotSavePath, screenMark, screenshotFileName);
            var fullFileName = string.Format("{0}.jpg", fullpath);


            File.WriteAllText(string.Format("{0}\\readme.txt", screenshotSavePath), testCaseDescription);
            File.WriteAllText(string.Format("{0}\\page.html", screenshotSavePath), Driver.PageSource);

            ss.SaveAsFile(fullFileName, ImageFormat.Jpeg);
            return fullFileName;
        }

        private string GetScreenshotFileName()
        {
            screenshotNumber++;
            return string.Format("screen_{0}", screenshotNumber);
        }

        /// <summary>
        /// Фикс для получения имени теста - при запуске тестов на teamcity TestContext.CurrentContext == null
        /// </summary>
        /// <returns></returns>
        private string GetTestMethodName()
        {
            try
            {
                return TestContext.CurrentContext.Test.Name;
            }
            catch
            {
                var stackTrace = new StackTrace();
                foreach (var stackFrame in stackTrace.GetFrames())
                {
                    MethodBase methodBase = stackFrame.GetMethod();
                    Object[] attributes = methodBase.GetCustomAttributes(
                                              typeof(TestAttribute), false);
                    if (attributes.Length >= 1)
                    {
                        return methodBase.Name;
                    }
                }
                return "Not called from a test method";
            }
        }

        private string GetScreenshotSavePath()
        {
            return string.Format("{0}{1:yyyy_MM_dd_HHmm}\\{2}", screenshotsDir, starTime, GetTestMethodName());
        }

        private static void LogMessage(string message)
        {
            Trace.WriteLine(message);
        }

        private void DoScreenShotOnError(string message)
        {
            // записали скриншот в файл
            var result = SaveScreenShot("ERR", message);
            
            // В файл лога добавляем имя скриншота
            LogMessage(string.Format("Скриншот с ошибкой для кейса [{0}] сохранён - путь {1}", message, result));
        }

        #endregion
    }
}