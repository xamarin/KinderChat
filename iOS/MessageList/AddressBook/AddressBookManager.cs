using System;

using AddressBook;
using AddressBookUI;
using UIKit;
using Foundation;
using ObjCRuntime;

namespace KinderChat.iOS
{
	public class AddressBookManager : NSObject, IABPeoplePickerNavigationControllerDelegate
	{
		public event EventHandler<PersonEventArgs> EmailPicked;

		public ABPeoplePickerNavigationController PeoplePicker { get; private set; }

		public AddressBookManager ()
		{
			PeoplePicker = new ABPeoplePickerNavigationController ();
			PeoplePicker.Delegate = this;

			// The people picker will only display the person's name, image and email properties in ABPersonViewController.
			PeoplePicker.DisplayedProperties.Add (ABPersonProperty.Email);

			// The people picker will enable selection of persons that have at least one email address.
			if(PeoplePicker.RespondsToSelector(new Selector("setPredicateForEnablingPerson:")))
				PeoplePicker.PredicateForEnablingPerson = NSPredicate.FromFormat ("emailAddresses.@count > 0");

			// The people picker will select a person that has exactly one email address and call peoplePickerNavigationController:didSelectPerson:,
			// otherwise the people picker will present an ABPersonViewController for the user to pick one of the email addresses.
			if(PeoplePicker.RespondsToSelector(new Selector("setPredicateForSelectionOfPerson:")))
				PeoplePicker.PredicateForSelectionOfPerson = NSPredicate.FromFormat ("emailAddresses.@count = 1");
		}

		// iOS7 and below
//		void HandleSelectPerson (object sender, ABPeoplePickerSelectPersonEventArgs e)
//		{
//			var peoplePicker = (ABPeoplePickerNavigationController)sender;
//
//			e.Continue = false;
//			using (ABMultiValue<string> emails = e.Person.GetEmails ())
//				e.Continue = emails.Count == 1;
//
//			if (!e.Continue) {
//				peoplePicker.DismissViewController (true, null);
//				RaiseEmailPicked (PersonFormatter.GetPickedEmail (e.Person));
//			}
//		}

		// iOS8+
//		void HandleSelectPerson2 (object sender, ABPeoplePickerSelectPerson2EventArgs e)
//		{
//			RaiseEmailPicked (PersonFormatter.GetPickedEmail (e.Person));
//		}

		// iOS7 and below
//		void HandlePerformAction (object sender, ABPeoplePickerPerformActionEventArgs e)
//		{
//			var peoplePicker = (ABPeoplePickerNavigationController)sender;
//
//			RaiseEmailPicked (PersonFormatter.GetPickedEmail (e.Person, e.Identifier));
//			peoplePicker.DismissViewController (true, null);
//
//			e.Continue = false;
//		}

//		// iOS8+
//		void HandlePerformAction2 (object sender, ABPeoplePickerPerformAction2EventArgs e)
//		{
//			RaiseEmailPicked (PersonFormatter.GetPickedEmail (selectedPerson, identifier));
//		}

		void RaiseEmailPicked(string email)
		{
			var handler = EmailPicked;
			if (handler != null)
				handler (PeoplePicker, new PersonEventArgs { Email = email });
		}

		// iOS8 +
		[Export ("peoplePickerNavigationController:didSelectPerson:")]
		void DidSelectPerson (ABPeoplePickerNavigationController peoplePicker, ABPerson selectedPerson)
		{
			RaiseEmailPicked (PersonFormatter.GetPickedEmail (selectedPerson));
		}

		// iOS8 +
		[Export ("peoplePickerNavigationController:didSelectPerson:property:identifier:")]
		void DidSelectPersonProperty (ABPeoplePickerNavigationController peoplePicker, ABPerson selectedPerson, int propertyId, int identifier)
		{
			Console.WriteLine (identifier);
			RaiseEmailPicked (PersonFormatter.GetPickedEmail (selectedPerson, identifier));
		}

		// iOS7 and below
		[Export ("peoplePickerNavigationController:shouldContinueAfterSelectingPerson:")]
		bool ShouldContinue (ABPeoplePickerNavigationController peoplePicker, ABPerson selectedPerson)
		{
			bool shouldcontinue = false;
			using (ABMultiValue<string> emails = selectedPerson.GetEmails ())
				shouldcontinue = emails.Count == 1;

			if (!shouldcontinue) {
				peoplePicker.DismissViewController (true, null);
				RaiseEmailPicked (PersonFormatter.GetPickedEmail (selectedPerson));
			}

			return shouldcontinue;
		}

		// iOS7 and below
		[Export ("peoplePickerNavigationController:shouldContinueAfterSelectingPerson:property:identifier:")]
		bool ShouldContinue (ABPeoplePickerNavigationController peoplePicker, ABPerson selectedPerson, int propertyId, int identifier)
		{
			RaiseEmailPicked (PersonFormatter.GetPickedEmail (selectedPerson, identifier));
			peoplePicker.DismissViewController (true, null);

			return false;
		}

		[Export ("peoplePickerNavigationControllerDidCancel:")]
		void Cancelled (ABPeoplePickerNavigationController peoplePicker)
		{
			peoplePicker.DismissViewController (true, null);
		}
	}
}