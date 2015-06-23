using System;
using System.Windows.Input;
using System.Threading.Tasks;
using KinderChat.ServerClient;
using KinderChat.ServerClient.Managers;
using System.Collections.Generic;
using KinderChat.ServerClient.Ws.Proxy;

namespace KinderChat
{
	/// <summary>
	/// Sign up view model, handling view state and sending of pin,
	/// confirmation, and validation of email/sms pin.
	/// </summary>
	public class SignUpViewModel : BaseViewModel
	{
	    private readonly IAuthenticationManager authenticationService;

	    public SignUpViewModel ()
	    {
			this.authenticationService = App.AuthenticationManager;
			nickName = Settings.NickName;
	    }

	    SignUpIdentity idenity = SignUpIdentity.Email;
		public SignUpIdentity Identity
		{
			get { return idenity; }
			set {
				SetProperty (ref idenity, value); 
			}
		}

		string identifier = string.Empty;
		public string Identifier
		{
			get { return identifier; }
			set {
				value = value.Trim ();
				SetProperty (ref identifier, value); 
				CheckRegisterEnabled ();
			}
		}

		string nickName = string.Empty;
		public string NickName
		{
			get { return nickName; }
			set {
				value = value.Trim ();
				SetProperty (ref nickName, value); 
				CheckRegisterEnabled ();
			}
		}

		private void CheckRegisterEnabled()
		{
			//valid email address here
			if (Identity == SignUpIdentity.Email)
				RegisterEnabled = identifier.IsValidEmail () && nickName.Length > 0;
			else
				RegisterEnabled = identifier.IsValidPhoneNumber() && nickName.Length > 0;
		}

		string pin = string.Empty;
		public string Pin
		{
			get { return pin; }
			set {
				value = value.Trim ();
				SetProperty (ref pin, value); 
				ValidatePinEnabled = pin.IsValidPin();
			}
		}

		bool validatePinEnabled;
		public const string ValidatePinEnabledPropertyName = "ValidatePinEnabled";
		public bool ValidatePinEnabled
		{
			get { return validatePinEnabled; }
			set { SetProperty (ref validatePinEnabled, value); }
		}

		bool canProgress;
		public const string CanProgressPropertyName = "CanProgress";
		public bool CanProgress
		{
			get { return canProgress; }
			set { SetProperty (ref canProgress, value); }
		}


		bool registerEnabled;
		public const string RegisterEnabledPropertyName = "RegisterEnabled";
		public bool RegisterEnabled
		{
			get { return registerEnabled; }
			set { SetProperty (ref registerEnabled, value); }
		}

		ICommand registerCommand;
		public ICommand RegisterCommand
		{
			get { return registerCommand ?? (registerCommand = new RelayCommand (() => ExecuteRegisterCommand ())); }
		}

		public async Task ExecuteRegisterCommand ()
		{
			if (App.FakeSignup) {
				CanProgress = true;
				OnPropertyChanged ("CanProgress");
				Settings.Email = Identifier;
				Settings.NickName = NickName;
				Settings.UserDeviceId = "test";
				Settings.UserDeviceLoginId = "test";
				return;
			}

			if (IsBusy)
				return;

			CanProgress = false;

			using (BusyContext ()) {
				using (App.Logger.TrackTimeContext ("RegisterUser")) {
					try {
						CanProgress = await authenticationService.GetTokenEmail (Identifier);
						if (CanProgress) {
							Settings.Email = Identifier;
							Settings.NickName = NickName;
						}
					} catch (Exception ex) {
						App.MessageDialog.SendToast ("We are sorry something went wrong, please try again.");
						App.Logger.Report (ex);
					}
				}
			}
		}

		ICommand validatePinCommand;
		public ICommand ValidatePinCommand
		{
			get { return validatePinCommand ?? (validatePinCommand = new RelayCommand (()=>ExecuteValidatePinCommand())); }
		}

		public async Task ExecuteValidatePinCommand ()
		{
			if (App.FakeSignup) {
				await App.InitDatabase();
				CanProgress = true;
				OnPropertyChanged ("CanProgress");
				return;
			}

			if (IsBusy)
				return;

			CanProgress = false;

			using (BusyContext ()) {
				using (App.Logger.TrackTimeContext ("FinalizeSignup")) {
					try {
						App.CryptoService.GenerateKeys ();

						var pk = Convert.ToBase64String (App.CryptoService.PublicKey);
						var token = await authenticationService.CreateUserDeviceViaEmail (Settings.Email, Pin, pk, nickName);
						if (token == null || token.AccessToken == null || token.DeviceId == null) {
							App.MessageDialog.SendToast ("Invalid PIN, please try again");
							return;
						}
						// Initial "AccessToken" is actually the devices password, and needed to get new tokens
						Settings.DevicePassword = token.AccessToken;
						Settings.UserDeviceLoginId = token.DeviceLoginId;
						Settings.UserDeviceId = token.DeviceId;

						var finalToken = await authenticationService.Authenticate (Settings.UserDeviceLoginId, Settings.DevicePassword);
						if (finalToken == null || finalToken.AccessToken == null) {

							Settings.AccessToken = string.Empty;
							Settings.UserDeviceId = string.Empty;
							Settings.UserDeviceLoginId = string.Empty;
							App.MessageDialog.SendToast ("Invalid PIN, please try again");
							return;
						}

						Settings.AccessToken = finalToken.AccessToken;
						var nextTime = DateTime.UtcNow.AddSeconds (finalToken.ExpiresIn).Ticks;
						Settings.KeyValidUntil = nextTime;
						//need to store when to refresh

						var userManager = new UserManager (Settings.AccessToken);
						var user = await userManager.GetUser (Settings.Email);

						if (user == null || user.Devices == null) {
							Settings.AccessToken = string.Empty;
							Settings.UserDeviceId = string.Empty;
							Settings.UserDeviceLoginId = string.Empty;
							App.MessageDialog.SendToast ("Issue in registration, please try again.");
							return;
						}

						Settings.MyId = user.Id;
						Settings.Avatar = user.Avatar.Location;

						#pragma warning disable 4014
						// This is like "fire and forget" we are not interested in result
						// also we are not interested in possible exceptions
						ServiceContainer.Resolve<ConnectionManager> ().TryKeepConnectionAsync (); //we don't need to await it
						#pragma warning disable restore

						App.Logger.Track ("PickTheme", new Dictionary<string, string> {
							{ "nickname", Settings.NickName },
							{ "avatar", user.Avatar.Location },
							{ "theme", Settings.AppTheme == AppTheme.Blue ? "blue" : "pink" }
						});
						CanProgress = true;
					} catch (Exception ex) {
						Settings.AccessToken = string.Empty;
						Settings.UserDeviceId = string.Empty;
						Settings.UserDeviceLoginId = string.Empty;
						App.Logger.Report (ex);
						App.MessageDialog.SendToast ("Invalid PIN, please try again");
					}
				}
			}
		}
	}
}

