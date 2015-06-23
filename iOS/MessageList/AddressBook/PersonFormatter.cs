using System;
using AddressBook;
using ObjCRuntime;
using System.Runtime.InteropServices;
using Foundation;
using System.Reflection;

namespace KinderChat.iOS
{
	public static class PersonFormatter
	{
		public static string GetPickedName(ABPerson person)
		{
			string contactName = person.ToString ();
			return string.Format ("Picked {0}", contactName ?? "No Name");
		}

		public static string GetPickedEmail(ABPerson person, int? identifier = null)
		{
			string emailAddress = "no email address";

			using (ABMultiValue<string> emails = person.GetEmails ()) {
				bool emailExists = emails != null && emails.Count > 0;

				if (emailExists) {
					nint index = identifier.HasValue ? emails.GetIndexForIdentifier (identifier.Value) : 0;
					emailAddress = emails [index].Value;
				}
			}

			return emailAddress;
		}

//		[DllImport (Constants.AddressBookLibrary)]
//		extern static IntPtr ABRecordCopyValue (IntPtr record, Int64 /* ABPropertyID = int32_t */ property);
//
//		[DllImport (Constants.AddressBookLibrary)]
//		extern static long ABMultiValueGetCount (IntPtr multiValue);
//
//		[DllImport (Constants.AddressBookLibrary)]
//		extern static long ABMultiValueGetIndexForIdentifier (IntPtr multiValue, int multiValueId);
//
//		[DllImport (Constants.AddressBookLibrary)]
//		extern static IntPtr ABMultiValueCopyValueAtIndex (IntPtr multiValue, long index);
//
//		static string ToString (IntPtr value)
//		{
//			if (value == IntPtr.Zero) {
//				return null;
//			}
//			return Runtime.GetNSObject (value).ToString ();
//		}
//
//		internal static IntPtr ToIntPtr (string value)
//		{
//			if (value == null)
//				return IntPtr.Zero;
//			return new NSString (value).Handle;
//		}
	}
}

