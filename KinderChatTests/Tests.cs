using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Android;
using Xamarin.UITest.Queries;
using System.Threading;

namespace KinderChat
{
    [TestFixture]
    public class Tests
    {
        AndroidApp app;

        [SetUp]
        public void BeforeEachTest()
        {
            // TODO: If the Android app being tested is included in the solution then open
            // the Unit Tests window, right click Test Apps, select Add App Project
            // and select the app projects that should be tested.
            app = ConfigureApp
                .Android
                .ApiKey(Key)
                .ApkFile("Kinder.apk")
                .StartApp();
        }

		[Test]
		public void Test1()
		{
			app.Repl ();
		}

        [Test]
        public void A_SigningUp()
        {
            app.Screenshot("The app started");

            app.Tap(x => x.Id("pink"));
            app.Screenshot("Select pink");

            app.Tap(x => x.Id("signup"));
            app.WaitForElement(x => x.Marked("Get Started"));
            app.Screenshot("Start the sign up process");

            app.ClearText(x => x.Id("identifier"));
            app.EnterText(x => x.Id("identifier"), "xamarin@testcloud");

            if (!(app.Query(x => x.Id("nickname")).Any()))
            {
                app.Back();
            }
            app.EnterText(x => x.Id("nickname"), "Test");

            if (!((app.Query(x => x.Id("continue_button")).Any())))
            {
                app.Back();
            }
            app.WaitForElement(x => x.Id("continue_button"));
            if (app.Query(x => x.Id("continue_button")).First().Enabled == true)
                Assert.Fail();
            app.Screenshot("Continue button should be disabled");

            app.ClearText(x => x.Id("identifier"));
            app.EnterText(x => x.Id("identifier"), "xamarin@testcloud.com");
            if (!((app.Query(x => x.Id("continue_button")).Any())))
            {
                app.Back();
            }
            app.Screenshot("Continue button is enabled now");

            app.Tap(x => x.Id("continue_button"));
            if (!((app.Query(x => x.Id("ping")).Any())))
            {
                app.Back();
            }
            app.WaitForElement(x => x.Id("pin"));
            if (!((app.Query(x => x.Id("continue_button")).Any())))
            {
                app.Back();
            }
            app.Screenshot("On the pin page");

            app.EnterText(x => x.Id("pin"), "000000");
            if (!((app.Query(x => x.Id("continue_button")).Any())))
            {
                app.Back();
            }
            app.Screenshot("Pin is entered");

            app.Tap(x => x.Id("continue_button"));
            app.WaitForElement(x => x.Id("message"));
            app.Screenshot("Logged into the app");

            app.Tap(x => x.Marked("OK"));
            app.WaitForElement(x => x.Id("avatar"));

            app.Screenshot("Home page is open");
        }

        [Test]
        public void B_Messaging()
        {
            app.Tap(x => x.Id("signup"));
            app.WaitForElement(x => x.Marked("Get Started"));
            app.ClearText(x => x.Id("identifier"));
           
            app.EnterText(x => x.Id("nickname"), "Test");
            if (!((app.Query(x => x.Id("continue_button")).Any())))
            {
                app.Back();
            }
            app.ClearText(x => x.Id("identifier"));
            app.EnterText(x => x.Id("identifier"), "xamarin@testcloud.com");
            if (!((app.Query(x => x.Id("continue_button")).Any())))
            {
                app.Back();
            }
            app.Tap(x => x.Id("continue_button"));
            if (!((app.Query(x => x.Id("ping")).Any())))
            {
                app.Back();
            }
            app.WaitForElement(x => x.Id("pin"));
            if (!((app.Query(x => x.Id("continue_button")).Any())))
            {
                app.Back();
            }
            app.EnterText(x => x.Id("pin"), "000000");
            if (!((app.Query(x => x.Id("continue_button")).Any())))
            {
                app.Back();
            }
            app.Tap(x => x.Id("continue_button"));
            app.WaitForElement(x => x.Id("message"));
            app.Tap(x => x.Marked("OK"));
            app.WaitForElement(x => x.Id("avatar"));

            app.Screenshot("Home page is open");

            app.Tap(x => x.Text("Chats"));
            app.WaitForElement(x => x.Id("fab"));
            app.Screenshot("Chat page open");

            app.Tap(x => x.Id("fab"));
            app.WaitForElement(x => x.Id("cancel"));
            app.Screenshot("User selection");

            app.Tap(x => x.Id("contact_photo"));
            app.WaitForElement(x => x.Id("input_text"));
            app.Screenshot("Messaging page open");

            app.EnterText(x => x.Id("input_text"), "Yesterday all my troubles seemed so far away");
            app.Tap(x => x.Id("send_button"));
            try
            {
                app.WaitForElement(x => x.Id("message"), timeout: TimeSpan.FromSeconds(5));
                app.Tap(x => x.Id("button1"));
            }
            catch
            {
            }
            app.Screenshot("Message sent!");

            app.Tap(x => x.Marked("Navigate up"));
            app.WaitForElement(x => x.Id("fab"));
            app.Screenshot("Back to main messaging page");

            app.Tap(x => x.Id("contact_photo").Index(1));
            app.EnterText(x => x.Id("input_text"), "We all live in a Yellow Submarine");
            app.Tap(x => x.Id("send_button"));
            try
            {
                app.WaitForElement(x => x.Id("message"), timeout: TimeSpan.FromSeconds(5));
                app.Tap(x => x.Id("button1"));
            }
            catch
            {
            }
            app.Screenshot("New message sent!");

            app.Tap(x => x.Marked("Navigate up"));
            app.WaitForElement(x => x.Id("fab"));
            app.Screenshot("Back to main messaging page");

            app.WaitForElement(x => x.Raw("* {text CONTAINS 'We all live'}"));
            app.WaitForElement(x => x.Raw("* {text CONTAINS 'Yesterday all m'}"));
            app.Screenshot("Messages are confirmed sent");
        }

        [Test]
        public void C_ChangingFriends()
        {
            SigningUp();
        }

        [Test]
        public void D_PasswordSwitching()
        {
            SigningUp();
        }

        public void SigningUp()
        {
            app.Screenshot("The app started");

            app.Tap(x => x.Id("signup"));
            app.WaitForElement(x => x.Marked("Get Started"));
            app.Screenshot("Start the sign up process");
        }


		private const string Key ="5f69a913df44f025d0ada18c6d17bf32";
    }
}

