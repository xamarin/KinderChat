using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using KinderChat.ServerClient.Managers;
using KinderChat.ServerClient;
using System.Linq;
using KinderChat.Models;
using KinderChat.ServerClient.Entities;
using KinderChat.Services;
using System.Threading;

namespace KinderChat
{
	public class FriendsViewModel : BaseViewModel, IDeviceInfoProvider //TODO: move to separate service
	{
		protected readonly IDataManager DataManager;

		public FriendsViewModel ()
		{
			DataManager = App.DataManager;
			LoadingMessage = string.Empty;
		}

		public string LoadingMessage { get; set; }

		readonly SemaphoreSlim friendsSemaphore = new SemaphoreSlim (1);
		readonly BatchUpdateObservableCollectoin<Friend> friends = new BatchUpdateObservableCollectoin<Friend> ();

		SemaphoreSlim FriendsSemaphore {
			get {
				return friendsSemaphore;
			}
		}

		public BatchUpdateObservableCollectoin<Friend> Friends {
			get { return friends; }
		}

		public async Task ExecuteFlagFriendCommand (long friendId, string name)
		{
			if (IsBusy)
				return;

			using (BusyContext ()) {
				try {
					if (!await RefreshToken ())
						return;

					var userManager = new UserManager (Settings.AccessToken);

					var flagList = await userManager.GetFlags ();

					if (flagList.Count == 0)
						return;

					LoadingMessage = "Loading...";
					App.MessageDialog.SelectOption ("Report Friend For:", flagList.Select (f => f.Description).ToArray (), async (which) => {
						try {
							var firstFlag = flagList [which];
							await userManager.FlagUser ((int)friendId, firstFlag.Id);

							Settings.KinderPoints = Settings.KinderPoints + 10;
							RaiseNotification ("User has been flagged for administrative review.");
						} catch (Exception ex) {
							App.Logger.Report (ex);
							RaiseError ("Unable to flag, please try again.");
						}
					});
				} catch (Exception ex) {
					App.Logger.Report (ex);
					RaiseError ("Unable to flag, please try again.");
				}
			}
		}

		ICommand loadFriendsCommand;

		public ICommand LoadFriendsCommand {
			get { return loadFriendsCommand ?? (loadFriendsCommand = new RelayCommand (() => ExecuteLoadFriendsCommand ())); }
		}

		public async Task ExecuteLoadFriendsCommand (bool checkUpdates = true)
		{
			if (IsBusy)
				return;

			LoadingMessage = "Loading Friends...";
			using (BusyContext ()) {
				try {
					if (!await RefreshToken ().ConfigureAwait(false))
						return;

					List<Friend> friendsCollecion = await DataManager.GetFriendsAsync ().ConfigureAwait (false);

					if (checkUpdates)
						await UpdateFriendInfoAsync (friendsCollecion).ConfigureAwait (false);

					await LoadToCollection (friendsCollecion).ConfigureAwait (false);
				} catch (Exception ex) {
					App.Logger.Report (ex);
					RaiseError ("Unable to gather friends.");
				}
			}
		}

		async Task LoadToCollection (IEnumerable<Friend> friends)
		{
			await FriendsSemaphore.WaitAsync().ConfigureAwait (false);

			using (Friends.UpdatesBlock ()) {
				Friends.Clear ();
				Friends.AddRange (friends);
			}

			FriendsSemaphore.Release ();
		}

		async Task UpdateFriendInfoAsync(IEnumerable<Friend> friends)
		{
			foreach (var f in friends)
				await UpdateFriendInfoAsync (f).ConfigureAwait (false);
		}

	    public async Task UpdateFriendInfoAsync(Friend item)
        {
			if (App.FakeSignup)
				return;
			
            var userManager = new UserManager(Settings.AccessToken);  //should it be cached?
            User user = null;
            if (item.FriendId < 0)
				user = await userManager.GetUser(item.Name).ConfigureAwait (false);
            else
				user = await userManager.GetUser((int)item.FriendId).ConfigureAwait (false);

            if (user != null && user.Devices != null)
            {

				//update messages for newly connected friends)
				if(item.FriendId < 0){
					var messages = await DataManager.GetUnsentMessagesAsync (item.FriendId).ConfigureAwait (false);

					//update each message with new information.
					foreach (var message in messages) {
						message.FriendName = user.NickName;
						message.FriendPhoto = EndPoints.BaseUrl + user.Avatar.Location;
						message.Recipient = user.Id;
						message.RecipientName = user.NickName;
						await DataManager.AddMessageAsync (message).ConfigureAwait (false);
					}
				}

                //user found
                item.FriendId = user.Id;
                item.Name = user.NickName;
                item.Photo = EndPoints.BaseUrl + user.Avatar.Location;
				item.AvatarType = user.Avatar.Type;
				await SaveUserDevices(user).ConfigureAwait (false);
				await DataManager.AddOrSaveFriendAsync(item).ConfigureAwait (false);
            }
	    }

	    public Task<List<string>> GetUserDevices(long userId)
	    {
	        return DataManager.GetDeviceListAsync(userId);
	    }

	    public async Task<Dictionary<string, string>> GetUserDevicesAndPublicKeys(int userId)
	    {
	        var result = await DataManager.GetFriendPublicKeysAsync(userId);
	        if (result.Count < 1)
            {
                var userManager = new UserManager(Settings.AccessToken);
                var user = await userManager.GetUser(userId);
                return await SaveUserDevices(user);
            }
			return result;
	    }

	    public Task SavePublicKeyForDeviceId(string deviceId, long userId, string value)
	    {
            return DataManager.AddFriendsPublicKeysAsync(new [] { new DeviceInfo { DeviceId = deviceId, PublicKey = value, UserId = (int) userId} });
	    }

	    ICommand searchForFriendCommand;

		public ICommand SearchForFriendCommand {
			get { return searchForFriendCommand ?? (searchForFriendCommand = new RelayCommand<string> (search => ExecuteSearchForFriendCommand (search))); }
		}

		public async Task ExecuteSearchForFriendCommand (string search)
		{
			search.Trim ();

			if (IsBusy)
				return;

			LoadingMessage = "Finding Friend...";

			using (BusyContext ()) {
				using (App.Logger.TrackTimeContext ("FriendSearch")) {
					try {
						App.Logger.Track ("FriendSearch");

						if (!search.IsValidEmail ()) {
							App.MessageDialog.SendToast ("Please enter a valide email address.");
							return;
						}

						if (!await RefreshToken ())
							return;

						var userManager = new UserManager (Settings.AccessToken);

						var result = await userManager.GetUser (search);
						IsBusy = false;

						//did not find anyone
						//add them and send invite
						if (result == null || result.Devices == null) {
							var newFriend = new Friend {
								FriendId = Settings.GenerateTempFriendId (),
								Name = search
							};

							await DataManager.AddOrSaveFriendAsync (newFriend);
							await FriendsSemaphore.WaitAsync();
							Friends.Add (newFriend);
							FriendsSemaphore.Release ();
							await userManager.SendUserInvite (search);
							RaiseNotification ("Your friend hasn't signed up for Kinder Chat yet. We have sent them an invite!", "Friend Request Sent");
							App.Logger.Track ("FriendRequestSent");

							Settings.KinderPoints = Settings.KinderPoints + 60;
							return;
						}

						//did you enter yourself?
						if (result.Email.ToLowerInvariant () == Settings.Email.ToLowerInvariant ()) {
							RaiseNotification ("This is you!");
							return;
						}

						//did you already friend them?
						await FriendsSemaphore.WaitAsync ();
						var alreadyFriend = Friends.FirstOrDefault (f => f.FriendId == result.Id) != null;
						FriendsSemaphore.Release ();

						if (alreadyFriend) {
							RaiseNotification ("Friend already added.");
							return;
						}

						//new friend found and we want to add them.
						var msg = string.Format ("We found {0} do you want to add to friend list?", result.NickName);
						App.MessageDialog.SendConfirmation (msg, "Friend found!",
							async add => {
								if (!add)
									return;

								var newFriend = new Friend {
									FriendId = result.Id,
									Name = result.NickName,
									Photo = EndPoints.BaseUrl + result.Avatar.Location,
									AvatarType = result.Avatar.Type
								};

								await DataManager.AddOrSaveFriendAsync (newFriend);

								await FriendsSemaphore.WaitAsync ();
								Friends.Add (newFriend);
								FriendsSemaphore.Release ();

								RaiseNotification ("Friend added!");
								App.Logger.Track ("FriendAdded");
								Settings.KinderPoints = Settings.KinderPoints + 30;
							});
					} catch (Exception ex) {
						App.Logger.Report (ex);
						RaiseError ("Something has gone wrong, please try to search again.");
					}
				}
			}
		}

        private async Task<Dictionary<string, string>> SaveUserDevices(User user)
        {
            await DataManager.AddFriendsPublicKeysAsync(user.Devices.Select(d =>
                new DeviceInfo { DeviceId = d.DeviceId, PublicKey = d.PublicKey, UserId = user.Id }));
            var result = new Dictionary<string, string>();
            foreach (var userDevice in user.Devices)
            {
                result[userDevice.DeviceId] = userDevice.PublicKey;
            }
            return result;
        }
	}
}

