using System;

namespace KinderChat
{
	public class SignUpValue
	{
		public SignUpIdentity IdentityType { get; set; }

		string signUpValue;
		public string Value {
			get {
				return signUpValue;
			}
			set {
				if (string.IsNullOrWhiteSpace (value)) {
					signUpValue = string.Empty;
					return;
				}

				signUpValue = value.Trim ();
			}
		}
	}
}

