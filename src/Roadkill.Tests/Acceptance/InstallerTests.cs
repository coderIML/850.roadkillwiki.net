using System;
using System.IO;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Roadkill.Core;

namespace Roadkill.Tests.Acceptance
{
	[TestFixture]
	[Category("Acceptance")]
	public class InstallerTests : AcceptanceTestBase
	{
		[SetUp]
		public void Setup()
		{
			// Switch installed=false in the web.config
			string sitePath = AcceptanceTestsSetup.GetSitePath();
			string webConfigPath = Path.Combine(sitePath, "web.config");
			string fileText = File.ReadAllText(webConfigPath);
			fileText = fileText.Replace("installed=\"true\"", "installed=\"false\"");
			File.WriteAllText(webConfigPath, fileText);

			Driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10)); // for ajax calls
		}

		[Test]
		public void Installation_Page_Should_Display_For_Home_Page()
		{
			// Arrange


			// Act
			Driver.Navigate().GoToUrl(BaseUrl);

			// Assert
			Assert.That(Driver.FindElements(By.CssSelector("div#installer")).Count, Is.EqualTo(1));
		}

		[Test]
		public void Installation_PageShould_Display_For_Login_Page()
		{
			// Arrange
			

			// Act
			Driver.Navigate().GoToUrl(LoginUrl);

			// Assert
			Assert.That(Driver.FindElements(By.CssSelector("div#installer")).Count, Is.EqualTo(1));
		}

		[Test]
		public void Step1_Web_Config_Test_Button_Should_Display_Success_Box_And_Continue_Link()
		{
			// Arrange
			Driver.Navigate().GoToUrl(BaseUrl);

			// Act
			Driver.FindElement(By.CssSelector("input[id=testwebconfig]")).Click();
			Driver.Wait(0.5);

			// Assert
			Assert.That(Driver.FindElement(By.CssSelector("div#webconfig-success")).Displayed, Is.True);
			Assert.That(Driver.FindElement(By.CssSelector(".continue > a")).Displayed, Is.True);
		}

		[Test]
		public void Step1_Web_Config_Test_Button_Should_Display_Error_Box_And_No_Continue_Link_For_Readonly_Webconfig()
		{
			// Arrange
			string sitePath = AcceptanceTestsSetup.GetSitePath();
			string webConfigPath = Path.Combine(sitePath, "web.config");
			File.SetAttributes(webConfigPath, FileAttributes.ReadOnly);
			Driver.Navigate().GoToUrl(BaseUrl);

			// Act
			Driver.FindElement(By.CssSelector("input[id=testwebconfig]")).Click();

			// Assert
			Assert.That(Driver.FindElement(By.CssSelector("div#webconfig-failure")).Displayed, Is.True);
			Assert.That(Driver.FindElement(By.CssSelector(".continue > a")).Displayed, Is.False);
		}

		[Test]
		public void Step2_Connection_Test_Button_Should_Display_Success_Box_For_Good_ConnectionString()
		{
			// Arrange
			Driver.Navigate().GoToUrl(BaseUrl);

			// Act
			Driver.FindElement(By.CssSelector("input[id=testwebconfig]")).Click();
			Driver.Wait(0.5);
			Driver.FindElement(By.CssSelector(".continue > a")).Click();

			SelectElement select = new SelectElement(Driver.FindElement(By.Id("DataStoreType_Name")));
			select.SelectByValue(DataStoreType.SqlServerCe.Name);

			Driver.FindElement(By.Id("ConnectionString")).SendKeys(@"Data Source=|DataDirectory|\roadkill-acceptancetests.sdf");
			Driver.FindElement(By.CssSelector("input[id=testdbconnection]")).Click();
			Driver.Wait(2);

			// Assert
			Assert.That(Driver.FindElement(By.CssSelector("div#connectionsuccess")).Displayed, Is.True);
			Assert.That(Driver.FindElement(By.CssSelector("div#connectionfailure")).Displayed, Is.False);
		}

		[Test]
		public void Step2_Connection_Test_Button_Should_Display_Error_Box_For_Bad_ConnectionString()
		{
			// Arrange
			Driver.Navigate().GoToUrl(BaseUrl);

			// Act
			Driver.FindElement(By.CssSelector("input[id=testwebconfig]")).Click();
			Driver.Wait(0.5);
			Driver.FindElement(By.CssSelector(".continue > a")).Click();

			SelectElement select = new SelectElement(Driver.FindElement(By.Id("DataStoreType_Name")));
			select.SelectByValue(DataStoreType.SqlServerCe.Name);

			Driver.FindElement(By.Id("ConnectionString")).SendKeys(@"Data Source=|DataDirectory|\madeupfilename.sdf");
			Driver.FindElement(By.CssSelector("input[id=testdbconnection]")).Click();
			Driver.Wait(2);

			// Assert
			Assert.That(Driver.FindElement(By.CssSelector("div#connectionfailure")).Displayed, Is.True);
			Assert.That(Driver.FindElement(By.CssSelector("div#connectionsuccess")).Displayed, Is.False);
		}

		[Test]
		public void Step2_Missing_Site_Name_Title_Should_Prevent_Contine()
		{
			// Arrange
			Driver.Navigate().GoToUrl(BaseUrl);

			// Act
			Driver.FindElement(By.CssSelector("input[id=testwebconfig]")).Click();
			Driver.Wait(0.5);
			Driver.FindElement(By.CssSelector(".continue > a")).Click();

			Driver.FindElement(By.Id("SiteName")).Clear();
			Driver.FindElement(By.Id("SiteUrl")).SendKeys("not empty");
			Driver.FindElement(By.Id("ConnectionString")).SendKeys("not empty");
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			// Assert
			Assert.That(Driver.FindElement(By.CssSelector(".formErrorContent")).Displayed, Is.True);
			Assert.That(Driver.FindElement(By.Id("SiteName")).Displayed, Is.True);
		}

		[Test]
		public void Step2_Missing_Site_Url_Should_Prevent_Continue()
		{
			// Arrange
			Driver.Navigate().GoToUrl(BaseUrl);

			// Act
			Driver.FindElement(By.CssSelector("input[id=testwebconfig]")).Click();
			Driver.Wait(0.5);
			Driver.FindElement(By.CssSelector(".continue > a")).Click();

			Driver.FindElement(By.Id("SiteName")).SendKeys("not empty");
			Driver.FindElement(By.Id("SiteUrl")).Clear();
			Driver.FindElement(By.Id("ConnectionString")).SendKeys("not empty");
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			// Assert
			Assert.That(Driver.FindElement(By.CssSelector(".formErrorContent")).Displayed, Is.True);
			Assert.That(Driver.FindElement(By.Id("SiteUrl")).Displayed, Is.True);
		}

		[Test]
		public void Step2_Missing_ConnectionString_Should_Prevent_Contine()
		{
			// Arrange
			Driver.Navigate().GoToUrl(BaseUrl);

			// Act
			Driver.FindElement(By.CssSelector("input[id=testwebconfig]")).Click();
			Driver.Wait(0.5);
			Driver.FindElement(By.CssSelector(".continue > a")).Click();

			Driver.FindElement(By.Id("SiteName")).SendKeys("not empty");
			Driver.FindElement(By.Id("SiteUrl")).SendKeys("not empty");
			Driver.FindElement(By.Id("ConnectionString")).Clear();
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			// Assert
			Assert.That(Driver.FindElement(By.CssSelector(".formErrorContent")).Displayed, Is.True);
			Assert.That(Driver.FindElement(By.Id("ConnectionString")).Displayed, Is.True);
		}

		[Test]
		public void Step3_Missing_Admin_Email_Should_Prevent_Continue()
		{
			// Arrange
			Driver.Navigate().GoToUrl(BaseUrl);

			// Act
			Driver.FindElement(By.CssSelector("input[id=testwebconfig]")).Click();
			Driver.Wait(0.5);
			Driver.FindElement(By.CssSelector(".continue > a")).Click();

			Driver.FindElement(By.Id("SiteName")).SendKeys("not empty");
			Driver.FindElement(By.Id("SiteUrl")).SendKeys("not empty");
			Driver.FindElement(By.Id("ConnectionString")).SendKeys("not empty");
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			Driver.FindElement(By.Id("AdminEmail")).Clear();
			Driver.FindElement(By.Id("AdminPassword")).SendKeys("not empty");
			Driver.FindElement(By.Id("password2")).SendKeys("not empty");
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			// Assert
			Assert.That(Driver.FindElement(By.CssSelector(".formErrorContent")).Displayed, Is.True);
			Assert.That(Driver.FindElement(By.Id("AdminEmail")).Displayed, Is.True);
		}

		[Test]
		public void Step3_Missing_Admin_Password_Should_Prevent_Continue()
		{
			// Arrange
			Driver.Navigate().GoToUrl(BaseUrl);

			// Act
			Driver.FindElement(By.CssSelector("input[id=testwebconfig]")).Click();
			Driver.Wait(0.5);
			Driver.FindElement(By.CssSelector(".continue > a")).Click();

			Driver.FindElement(By.Id("SiteName")).SendKeys("not empty");
			Driver.FindElement(By.Id("SiteUrl")).SendKeys("not empty");
			Driver.FindElement(By.Id("ConnectionString")).SendKeys("not empty");
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			Driver.FindElement(By.Id("AdminEmail")).SendKeys("not empty");
			Driver.FindElement(By.Id("AdminPassword")).Clear();
			Driver.FindElement(By.Id("password2")).SendKeys("not empty");
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			// Assert
			Assert.That(Driver.FindElement(By.CssSelector(".formErrorContent")).Displayed, Is.True);
			Assert.That(Driver.FindElement(By.Id("AdminPassword")).Displayed, Is.True);
		}

		[Test]
		public void Step3_Not_Min_Length_Admin_Password_Should_Prevent_Continue()
		{
			// Arrange
			Driver.Navigate().GoToUrl(BaseUrl);

			// Act
			Driver.FindElement(By.CssSelector("input[id=testwebconfig]")).Click();
			Driver.Wait(0.5);
			Driver.FindElement(By.CssSelector(".continue > a")).Click();

			Driver.FindElement(By.Id("SiteName")).SendKeys("not empty");
			Driver.FindElement(By.Id("SiteUrl")).SendKeys("not empty");
			Driver.FindElement(By.Id("ConnectionString")).SendKeys("not empty");
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			Driver.FindElement(By.Id("AdminEmail")).SendKeys("not empty");
			Driver.FindElement(By.Id("AdminPassword")).SendKeys("1");
			Driver.FindElement(By.Id("password2")).SendKeys("not empty");
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			// Assert
			Assert.That(Driver.FindElement(By.CssSelector(".formErrorContent")).Displayed, Is.True);
			Assert.That(Driver.FindElement(By.Id("AdminPassword")).Displayed, Is.True);
		}

		[Test]
		public void Step3_Missing_Admin_Password2_Should_Prevent_Continue()
		{
			// Arrange
			Driver.Navigate().GoToUrl(BaseUrl);

			// Act
			Driver.FindElement(By.CssSelector("input[id=testwebconfig]")).Click();
			Driver.Wait(0.5);
			Driver.FindElement(By.CssSelector(".continue > a")).Click();

			Driver.FindElement(By.Id("SiteName")).SendKeys("not empty");
			Driver.FindElement(By.Id("SiteUrl")).SendKeys("not empty");
			Driver.FindElement(By.Id("ConnectionString")).SendKeys("not empty");
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			Driver.FindElement(By.Id("AdminEmail")).SendKeys("not empty");
			Driver.FindElement(By.Id("AdminPassword")).SendKeys("not empty");
			Driver.FindElement(By.Id("password2")).Clear();
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			// Assert
			Assert.That(Driver.FindElement(By.CssSelector(".formErrorContent")).Displayed, Is.True);
			Assert.That(Driver.FindElement(By.Id("password2")).Displayed, Is.True);
		}

		[Test]
		public void Step4_Test_Attachments_Folder_Button_With_Existing_Folder_Should_Display_Success_Box()
		{
			// Arrange
			string sitePath = AcceptanceTestsSetup.GetSitePath();
			Guid folderGuid = Guid.NewGuid();
			string attachmentsFolder = Path.Combine(sitePath, "AcceptanceTests", folderGuid.ToString());		
			Directory.CreateDirectory(attachmentsFolder);

			Driver.Navigate().GoToUrl(BaseUrl);

			// Act
			Driver.FindElement(By.CssSelector("input[id=testwebconfig]")).Click();
			Driver.Wait(0.5);
			Driver.FindElement(By.CssSelector(".continue > a")).Click();

			Driver.FindElement(By.Id("SiteName")).SendKeys("not empty");
			Driver.FindElement(By.Id("SiteUrl")).SendKeys("not empty");
			Driver.FindElement(By.Id("ConnectionString")).SendKeys("not empty");
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			Driver.FindElement(By.Id("AdminEmail")).SendKeys("admin@localhost");
			Driver.FindElement(By.Id("AdminPassword")).SendKeys("not empty");
			Driver.FindElement(By.Id("password2")).SendKeys("not empty");
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			Driver.FindElement(By.Id("AttachmentsFolder")).Clear();
			Driver.FindElement(By.Id("AttachmentsFolder")).SendKeys("~/AcceptanceTests/" + folderGuid);
			Driver.FindElement(By.CssSelector("input[id=testattachments]")).Click();
			Driver.Wait(2);

			// Assert
			try
			{
				Assert.That(Driver.FindElement(By.CssSelector("div#attachmentssuccess")).Displayed, Is.True);
				Assert.That(Driver.FindElement(By.CssSelector("div#attachmentsfailure")).Displayed, Is.False);
			}
			finally
			{
				Directory.Delete(attachmentsFolder, true);
			}
		}

		[Test]
		public void Step4_Test_Attachments_Folder_Button_With_Missing_Folder_Should_Display_Failure_Box()
		{
			// Arrange
			string sitePath = AcceptanceTestsSetup.GetSitePath();
			Guid folderGuid = Guid.NewGuid();
			Driver.Navigate().GoToUrl(BaseUrl);

			// Act
			Driver.FindElement(By.CssSelector("input[id=testwebconfig]")).Click();
			Driver.Wait(0.5);
			Driver.FindElement(By.CssSelector(".continue > a")).Click();

			Driver.FindElement(By.Id("SiteName")).SendKeys("not empty");
			Driver.FindElement(By.Id("SiteUrl")).SendKeys("not empty");
			Driver.FindElement(By.Id("ConnectionString")).SendKeys("not empty");
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			Driver.FindElement(By.Id("AdminEmail")).SendKeys("admin@localhost");
			Driver.FindElement(By.Id("AdminPassword")).SendKeys("not empty");
			Driver.FindElement(By.Id("password2")).SendKeys("not empty");
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			Driver.FindElement(By.Id("AttachmentsFolder")).Clear();
			Driver.FindElement(By.Id("AttachmentsFolder")).SendKeys("~/" + folderGuid);
			Driver.FindElement(By.CssSelector("input[id=testattachments]")).Click();
			Driver.Wait(2);

			// Assert
			Assert.That(Driver.FindElement(By.CssSelector("div#attachmentsfailure")).Displayed, Is.True);
			Assert.That(Driver.FindElement(By.CssSelector("div#attachmentssuccess")).Displayed, Is.False);
		}

		[Test]
		public void Navigation_Persists_Field_Values_Correctly()
		{
			// Arrange
			string sitePath = AcceptanceTestsSetup.GetSitePath();
			Guid folderGuid = Guid.NewGuid();
			Driver.Navigate().GoToUrl(BaseUrl);

			// Act
			Driver.FindElement(By.CssSelector("input[id=testwebconfig]")).Click();
			Driver.Wait(0.5);
			Driver.FindElement(By.CssSelector(".continue > a")).Click();

			Driver.FindElement(By.Id("SiteName")).Clear();
			Driver.FindElement(By.Id("SiteName")).SendKeys("Site Name");

			Driver.FindElement(By.Id("SiteUrl")).Clear();
			Driver.FindElement(By.Id("SiteUrl")).SendKeys("Site Url");

			Driver.FindElement(By.Id("ConnectionString")).Clear();
			Driver.FindElement(By.Id("ConnectionString")).SendKeys("Connection String");
			SelectElement select = new SelectElement(Driver.FindElement(By.Id("DataStoreType_Name")));
			select.SelectByValue(DataStoreType.MySQL.Name);

			Driver.FindElement(By.CssSelector("div.continue input")).Click();
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			Driver.FindElement(By.CssSelector("div.previous a")).Click();
			Driver.FindElement(By.CssSelector("div.previous a")).Click();

			// Assert
			Assert.That(Driver.FindElement(By.Id("SiteName")).GetAttribute("value"), Is.EqualTo("Site Name"));
			Assert.That(Driver.FindElement(By.Id("SiteUrl")).GetAttribute("value"), Is.EqualTo("Site Url"));
			Assert.That(Driver.FindElement(By.Id("ConnectionString")).GetAttribute("value"), Is.EqualTo("Connection String"));

			select = new SelectElement(Driver.FindElement(By.Id("DataStoreType_Name")));
			Assert.That(select.SelectedOption.GetAttribute("value"), Is.EqualTo(DataStoreType.MySQL.Name));
		}

		[Test]
		public void All_Steps_With_Minimum_Required_Should_Complete()
		{
			// Arrange
			Driver.Navigate().GoToUrl(BaseUrl);

			//
			// ***Act***
			//

			// step 1
			Driver.FindElement(By.CssSelector("input[id=testwebconfig]")).Click();
			Driver.Wait(0.5);
			Driver.FindElement(By.CssSelector(".continue > a")).Click();

			// step 2
			Driver.FindElement(By.Id("SiteName")).SendKeys("Acceptance tests");
			SelectElement select = new SelectElement(Driver.FindElement(By.Id("DataStoreType_Name")));
			select.SelectByValue(DataStoreType.SqlServerCe.Name);

			Driver.FindElement(By.Id("ConnectionString")).SendKeys(@"Data Source=|DataDirectory|\roadkill-acceptancetests.sdf");
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			// step 3
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			// step 3b
			Driver.FindElement(By.Id("AdminEmail")).SendKeys("admin@localhost");
			Driver.FindElement(By.Id("AdminPassword")).SendKeys("password");
			Driver.FindElement(By.Id("password2")).SendKeys("password");
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			// step 4
			Driver.FindElement(By.CssSelector("div.continue input")).Click();

			//
			// ***Assert***
			//
			Assert.That(Driver.FindElement(By.CssSelector("div#installsuccess h1")).Text, Is.EqualTo("Installation successful"));
			Driver.FindElement(By.CssSelector("div#installsuccess a")).Click();
			LoginAsAdmin();
		}
	}
}
