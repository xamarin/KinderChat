using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Media.Plugin;
using Media.Plugin.Abstractions;
using System.IO;
using KinderChat.ServerClient.Managers;
using System.Collections.ObjectModel;
using KinderChat.ServerClient.Entities;
using KinderChat.ServerClient;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace KinderChat
{
	public class ProfileViewModel : BaseViewModel
	{
		readonly IDataManager dataManager;

		public ProfileViewModel ()
		{
			dataManager = App.DataManager;
			Avatar = Settings.Avatar;
			NickName = Settings.NickName;
			LoadingMessage = string.Empty;
		}

		readonly SemaphoreSlim semaphore = new SemaphoreSlim (1);
		readonly BatchUpdateObservableCollectoin<AvatarItem> avatars = new BatchUpdateObservableCollectoin<AvatarItem> ();

		public BatchUpdateObservableCollectoin<AvatarItem> Avatars {
			get { return avatars; }
		}

		public SemaphoreSlim AvatarsSemaphore {
			get {
				return semaphore;
			}
		}

		public string LoadingMessage { get; set; }

		string nickName = string.Empty;
		public const string NickNamePropertyName = "NickName";

		public string NickName {
			get { return nickName; }
			set {
				SetProperty (ref nickName, value); 
			}
		}

		string avatar = string.Empty;
		public const string AvatarPropertyName = "Avatar";

		public string Avatar {
			get { return avatar; }
			set {
				
				SetProperty (ref avatar, value);
				AvatarUrl = avatar;
			}
		}


		public  string EndPoint { get { return EndPoints.BaseUrl; } }

		string avatarUrl = string.Empty;
		public const string AvatarUrlName = "AvatarUrl";

		public string AvatarUrl {
			get { return avatarUrl; }
			set {
				SetProperty (ref avatarUrl, value.StartsWith ("/", StringComparison.OrdinalIgnoreCase) ? (EndPoint + value) : value);
			}
		}

		ICommand loadAvatarsCommand;

		public ICommand LoadAvatarsCommand {
			get { return loadAvatarsCommand ?? (loadAvatarsCommand = new RelayCommand (() => ExecuteLoadAvatarsCommand ())); }
		}

		public async Task ExecuteLoadAvatarsCommand ()
		{
			if (IsBusy)
				return;

			LoadingMessage = "Loading Avatars...";

			using (BusyContext ()) {
				using (App.Logger.TrackTimeContext ("LoadAvatars")) {
					try {
						if (!await RefreshToken ().ConfigureAwait(false))
							return;

						IEnumerable<AvatarItem> avatars = await FetchAvatars ().ConfigureAwait(false);
						await LoadToCollection (avatars).ConfigureAwait (false);
					} catch (Exception ex) {
						App.Logger.Report (ex);
						RaiseNotification ("Unable to load avatars :(");
					}
				}
			}
		}

		async Task<IEnumerable<AvatarItem>> FetchAvatars ()
		{
			List<AvatarItem> avatars = await dataManager.GetAvatarsAsync ().ConfigureAwait (false);
			if (avatars.Count > 0)
				return avatars;

			var staticAvatars = await LoadStaticAvatars ().ConfigureAwait (false);
			foreach (AvatarItem avatar in staticAvatars) {
				await dataManager.AddAvatarAsync (avatar).ConfigureAwait (false);
				avatars.Add (avatar);
			}

			return avatars;
		}

		async Task<IEnumerable<AvatarItem>> LoadStaticAvatars ()
		{
			var avatarManager = new AvatarManager (Settings.AccessToken);
			List<Avatar> staticAvatars = await avatarManager.GetStaticAvatars ().ConfigureAwait (false);

			return staticAvatars.Select (Convert);
		}

		AvatarItem Convert(Avatar avatar)
		{
			return new AvatarItem {
				AvatarId = avatar.Id,
				Location = avatar.Location,
				AvatarType = avatar.Type
			};
		}

		void TryAddCustomAvatar ()
		{
			AvatarItem customAvatar = GetCustomAvatar();
			if(customAvatar != null)
				Avatars.Add(customAvatar);
		}

		AvatarItem GetCustomAvatar ()
		{
			if (string.IsNullOrWhiteSpace (Settings.CustomAvatar))
				return null;

			return new AvatarItem {
				AvatarId = Settings.CustomAvatarId,
				Location = Settings.CustomAvatar,
				AvatarType = AvatarType.User
			};
		}

		async Task LoadToCollection (IEnumerable<AvatarItem> avatars)
		{
			await AvatarsSemaphore.WaitAsync().ConfigureAwait (false);

			using (Avatars.UpdatesBlock ()) {
				Avatars.Clear ();
				Avatars.AddRange (avatars);
				TryAddCustomAvatar ();
			}

			AvatarsSemaphore.Release ();
		}

		ICommand linkToParentCommand;

		public ICommand LinkToParentCommand {
			get { return linkToParentCommand ?? (linkToParentCommand = new RelayCommand (() => ExecuteLinkToParentCommand ())); }
		}

		public void ExecuteLinkToParentCommand ()
		{
			if (IsBusy)
				return;

			App.MessageDialog.AskForString ("Please enter parent's email:", "Link to Parent", email => {
				email = email.Trim ();

				if (!email.IsValidEmail ()) {
					RaiseNotification("Unable to link to parent, please enter valid email.");
					return;
				}

				Settings.LinkedToParent = true;
				Settings.ParentEmail = email;
				RaiseNotification ("Linked to Parent!");
				Settings.KinderPoints = Settings.KinderPoints + 1000;
			});
		}

		ICommand saveProfileCommand;

		public ICommand SaveProfileCommand {
			get { return saveProfileCommand ?? (saveProfileCommand = new RelayCommand (() => ExecuteSaveProfileCommand ())); }
		}

		public async Task ExecuteSaveProfileCommand ()
		{
			if (IsBusy)
				return;

			LoadingMessage = "Saving Profile...";

			using (BusyContext ()) {
				using (App.Logger.TrackTimeContext ("SaveProfile")) {
					try {
						if (string.IsNullOrWhiteSpace (nickName)) {
							RaiseNotification ("Please fill in your nick name.");
							return;
						}

						if (!await RefreshToken ())
							return;

						var userManager = new UserManager (Settings.AccessToken);

						if (Settings.NickName != nickName) {
							var success = await userManager.ChangeNickname (nickName);

							if (!success) {
								RaiseNotification ("Unable to save profile, please try again.");
								return;
							}
						}

						Settings.NickName = nickName;

						await AvatarsSemaphore.WaitAsync();
						var selectedAvatar = Avatars.FirstOrDefault (a => a.Location == avatar);
						AvatarsSemaphore.Release ();

						if (selectedAvatar != null && Settings.Avatar != avatar) {
							var ava = await userManager.SetAvatarFromList (selectedAvatar.AvatarId);
							if (ava == null) {
								RaiseNotification ("Unable to save profile, please try again.");
								return;
							}
						}

						Settings.Avatar = avatar;
						RaiseNotification ("Profile Saved!");
						App.Logger.Track ("SaveProfile", new Dictionary<string, string> {
							{ "nickname", Settings.NickName },
							{ "avatar", Settings.Avatar }
						});
					} catch (Exception ex) {
						App.Logger.Report (ex);
						RaiseError ("Avatar upload failed. Please try again.");
					}
				}
			}
		}

		public async Task UploadPhoto(MediaFile file)
		{
			bool refreshAvatars = false;

			LoadingMessage = "Uploading Photo...";

			using (BusyContext ()) {
				using (App.Logger.TrackTimeContext ("UploadPhoto")) {
					try {
						if (!await RefreshToken ())
							return;

						var userManager = new UserManager (Settings.AccessToken);

						var stream = file.GetStream ();
						var newAvatar = await userManager.AddCustomAvatar (stream);
						if (newAvatar == null)
							return;

						Avatar = newAvatar.Location;
						Settings.Avatar = newAvatar.Location;
						Settings.CustomAvatar = newAvatar.Location;
						Settings.CustomAvatarId = newAvatar.Id;
						App.Logger.Track ("AvatarUploaded");
						OnPropertyChanged (AvatarUrlName);
						refreshAvatars = true;

						Settings.KinderPoints = Settings.KinderPoints + 50;
					} catch (Exception ex) {
						App.Logger.Report (ex);
						RaiseError ("Avatar upload failed. Please try again.");
					}
				}
			}

			if (refreshAvatars)
				await ExecuteLoadAvatarsCommand ();
		}

		public async Task<MediaFile> PickPhoto()
		{
			if (IsBusy)
				return null;

			App.Logger.Track ("PickPhoto");
			MediaFile file = null;

			try {
				if (CrossMedia.Current.IsPickPhotoSupported)
					file = await CrossMedia.Current.PickPhotoAsync ();
				else
					RaiseNotification("Photo gallery is not available.");

				if (file == null)
					return null;

				await UploadPhoto(file);
			} catch (Exception ex) {
				App.Logger.Report (ex);
			}

			return file;
		}

		public async Task<MediaFile> TakePhoto ()
		{
			if (IsBusy)
				return null;

			App.Logger.Track ("TakePhoto");
			MediaFile file = null;

			try {
				if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported) {
					file = await CrossMedia.Current.TakePhotoAsync (new StoreCameraMediaOptions {
						DefaultCamera = CameraDevice.Front,
						Directory = "Kinder",
						Name = "avatar.jpg"
					});
				} else {
					RaiseNotification("Camera is not available");
				}
				if (file == null)
					return null;

				await UploadPhoto(file);
			} catch (Exception ex) {
				App.Logger.Report (ex);
			}

			return file;
		}
	}
}