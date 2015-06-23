using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using KinderChat.ServerClient;
using KinderChat.ServerClient.Entities;
using KinderChat.ServerClient.Ws.Proxy;
using KinderChat.Services.Messages;

namespace KinderChat
{
	public static class App
	{
		public const string InsightsKey = "27a02f75ddc98a035428dad91c70cf577b067895";

		public static void Init()
		{
			ServiceContainer.Register<IAuthenticationManager>(()=>new AuthenticationManager());
			ServiceContainer.Register<SignUpViewModel> (() => new SignUpViewModel ());
			ServiceContainer.Register<ConversationsViewModel> (() => new ConversationsViewModel ());
			ServiceContainer.Register<FriendsViewModel> (() => new FriendsViewModel ());
			ServiceContainer.Register<ProfileViewModel> (() => new ProfileViewModel ());
			ServiceContainer.Register<PointsViewModel> (() => new PointsViewModel ());
            ServiceContainer.Register<IDataManager>(() => new DataManager());
            ServiceContainer.Register<IMessageRepository>(() => (DataManager)ServiceContainer.Resolve<IDataManager>());
			ServiceContainer.Register<Logger> (() => new Logger ());
			ServiceContainer.Register<ICryptoService> (() => new RsaAndAesHybridCryptoService ());

            
            ServiceContainer.Register<ConnectionManager>(() => new ConnectionManager(ServiceContainer.Resolve<ILiveConnection>(), new SettingsToCredentialsProviderAdapter()));
            ServiceContainer.Register<MessagingService>(() => new MessagingService(ServiceContainer.Resolve<ConnectionManager>()));
            ServiceContainer.Register<GroupChatsService>(() => new GroupChatsService(ServiceContainer.Resolve<ConnectionManager>()));

            ServiceContainer.Register<MessagesManager>(() => new MessagesManager(
                ServiceContainer.Resolve<MessagingService>(),
                ServiceContainer.Resolve<ConnectionManager>(),
                ServiceContainer.Resolve<IMessageRepository>(),
                ServiceContainer.Resolve<ICryptoService>(),
                ServiceContainer.Resolve<FriendsViewModel>()));

            ServiceContainer.Register<TypingService>(() => new TypingService(ServiceContainer.Resolve<MessagingService>()));
			if (Settings.FirstRun)
				InitKinderTasks ().Wait ();

		}

		public static void Logout()
		{
			ServiceContainer.Clear ();
			Init ();
		}

		public static Task InitKinderTasks ()
		{
			return Task.WhenAll (
				DataManager.AddOrSaveKinderTaskAsync (new KinderTask {
					Name = "Ate Broccoli",
					Points = 50,
					IconName = "taskIconBrocolli"
				}),
				DataManager.AddOrSaveKinderTaskAsync (new KinderTask {
					Name = "Brushed Teeth",
					Points = 20,
					IconName = "taskIconToothbrush"
				}),
				DataManager.AddOrSaveKinderTaskAsync (new KinderTask {
					Name = "Cleaned Room",
					Points = 150,
					IconName = "taskIconVacuumCleaner"
				}),
				DataManager.AddOrSaveKinderTaskAsync (new KinderTask {
					Name = "Finished Homework",
					Points = 100,
					IconName = "taskIconFinishedHomework"
				})
			);
		}

		/// <summary>
		/// This is for debug purposes only!
		/// </summary>
		/// <returns>The database.</returns>
		public static async Task InitDatabase()
		{
			await Task.Yield ();

			#if DEBUG

			if(!FakeSignup || !ForceSignup)
				return;
			
			if((await DataManager.GetFriendsAsync()).Count == 0){

				var id = Settings.GenerateTempFriendId();
				await DataManager.AddOrSaveFriendAsync(new Friend
					{
						FriendId = id,
						Name = "Paul",
						Photo="http://www.gravatar.com/avatar.php?gravatar_id=8de583685d3e68f79917ec82253185d8&size=200&default=http%3A%2F%2Fvanillicon.com%2F8de583685d3e68f79917ec82253185d8_200.png",
						AvatarType = AvatarType.User
					});

				id = Settings.GenerateTempFriendId();
				await DataManager.AddOrSaveFriendAsync(new Friend
					{
						FriendId = id,
						Name = "Laurent",
						Photo="http://s.gravatar.com/avatar/749bf0fc447ba1650d2e7e273c6a73c9?s=200",
						AvatarType = AvatarType.User
					});

				id = Settings.GenerateTempFriendId();
				await DataManager.AddOrSaveFriendAsync(new Friend
					{
						FriendId = id,
						Name = "Tomasz",
						Photo="http://s.gravatar.com/avatar/f780d57997526876b0625e517c1e0884?s=200",
						AvatarType = AvatarType.User
					});

				id = Settings.GenerateTempFriendId();
				await DataManager.AddOrSaveFriendAsync(new Friend
					{
						FriendId = id,
						Name = "Brett",
						Photo="http://www.gravatar.com/avatar.php?gravatar_id=cad414faa78a2721c5095aa248cb1f63&size=200&default=http%3A%2F%2Fvanillicon.com%2Fcad414faa78a2721c5095aa248cb1f63_200.png",
						AvatarType = AvatarType.User
					});

				id = Settings.GenerateTempFriendId();
				await DataManager.AddOrSaveFriendAsync(new Friend
					{
						FriendId = id,
						Name = "Frank",
						Photo="http://s.gravatar.com/avatar/3eef89c1710742937cc74bf88da9dff8?s=200",
						AvatarType = AvatarType.User
					});

				id = Settings.GenerateTempFriendId();
				await DataManager.AddOrSaveFriendAsync(new Friend
					{
						FriendId = id,
						Name = "Jon",
						Photo="http://s.gravatar.com/avatar/c6857c59dea4b1adeb32d61b0ee0e8c0?s=200",
						AvatarType = AvatarType.User
					});

				id = Settings.GenerateTempFriendId();
				await DataManager.AddOrSaveFriendAsync(new Friend
					{
						FriendId = id,
						Name = "Stuart",
						Photo="http://s.gravatar.com/avatar/4b5c15250cb89971e0642129fae0473f?s=200",
						AvatarType = AvatarType.User
					});

				id = Settings.GenerateTempFriendId();
				await DataManager.AddOrSaveFriendAsync(new Friend
					{
						FriendId = id, 
						Name = "David",
						Photo="http://www.gravatar.com/avatar.php?gravatar_id=0c38e45d16ca384896dbcb91532fc50a&size=200&default=http%3A%2F%2Fvanillicon.com%2F0c38e45d16ca384896dbcb91532fc50a_200.png",
						AvatarType = AvatarType.User
					});

				id = Settings.GenerateTempFriendId();
				await DataManager.AddOrSaveFriendAsync(new Friend
					{
						FriendId = id,
						Name = "Greg",
						Photo="http://www.gravatar.com/avatar.php?gravatar_id=279b474d14f72e4daa1fc76e6f3c929f&size=200&default=http%3A%2F%2Fvanillicon.com%2F279b474d14f72e4daa1fc76e6f3c929f_200.png",
						AvatarType = AvatarType.User
					});
			}

			List<Message> list = await DataManager.GetLatestMessagesAsync ();
			if(list.Count == 0){

				await DataManager.AddMessageAsync(new Message("Paul", "Hello world"){Sender = -2, Recipient = Settings.MyId, Date = DateTime.UtcNow.AddDays(-1)});
				await DataManager.AddMessageAsync(new Message("Laurent", "Testing...") { Sender = -3, Recipient =  Settings.MyId, Date = DateTime.UtcNow.AddDays(-1) });
				await DataManager.AddMessageAsync(new Message("Tomasz", "Some long meaningless text in order to check something"){Sender = -4, Recipient =  Settings.MyId, Date = DateTime.UtcNow.AddDays(-2) });
				await DataManager.AddMessageAsync(new Message("Brett", "How's it going?") { Sender = -5, Recipient =  Settings.MyId, Date = DateTime.UtcNow.AddDays(-3) });

			}

			var avatars = await DataManager.GetAvatarsAsync();
			if(avatars.Count == 0){
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 0, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-cat-1.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 1, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-cat-2.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 2, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-cat-3.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 3, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-cat-4.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 4, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-cat-5.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 5, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-cat-6.png"});

				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 6, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-lion-1.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 7, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-lion-2.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 8, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-lion-3.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 9, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-lion-4.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 11, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-lion-5.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 12, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-lion-6.png"});


				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 13, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-panda-1.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 14, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-panda-2.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 15, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-panda-3.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 16, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-panda-4.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 17, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-panda-5.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 18, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-panda-6.png"});



				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 19, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-pig-1.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 20, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-pig-2.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 21, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-pig-3.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 22, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-pig-4.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 23, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-pig-5.png"});
				await DataManager.AddAvatarAsync(new AvatarItem{ AvatarId = 24, AvatarType = AvatarType.Animal, Location = "/Images/Avatars/avatar-pig-6.png"});

				Settings.Avatar = "/Images/Avatars/avatar-cat-1.png";
			}

			#endif
		}


		public static ICryptoService CryptoService
		{
			get { return ServiceContainer.Resolve<ICryptoService> (); }
		}

		public static IAuthenticationManager AuthenticationManager
		{
			get { return ServiceContainer.Resolve<IAuthenticationManager> (); }
		}

		public static IMessageDialog MessageDialog
		{
			get { return ServiceContainer.Resolve<IMessageDialog> (); }
		}

        public static IUIThreadDispacher UIThreadDispacher
		{
            get { return ServiceContainer.Resolve<IUIThreadDispacher>(); }
		}

		public static Logger Logger
		{
			get { return ServiceContainer.Resolve<Logger> (); }
		}

		public static IDataManager DataManager
		{
			get { return ServiceContainer.Resolve<IDataManager> (); }
		}

	    public static MessagesManager MessagesManager
	    {
            get { return ServiceContainer.Resolve<MessagesManager>(); }
	    }

        public static ConnectionManager ConnectionManager
        {
            get { return ServiceContainer.Resolve<ConnectionManager>(); }
        }

		public static SignUpViewModel SignUpViewModel
		{
			get { return ServiceContainer.Resolve<SignUpViewModel> (); }
		}

		public static ConversationsViewModel ConversationsViewModel
		{
			get { return ServiceContainer.Resolve<ConversationsViewModel> (); }
		}

		public static FriendsViewModel FriendsViewModel
		{
			get { return ServiceContainer.Resolve<FriendsViewModel> (); }
		}

		public static ProfileViewModel ProfileViewModel
		{
			get { return ServiceContainer.Resolve<ProfileViewModel> (); }
		}

		public static PointsViewModel PointsViewModel
		{
			get { return ServiceContainer.Resolve<PointsViewModel> (); }
		}

		public static bool FakeSignup
		{
			get { return false; }
		}

		public static bool ForceSignup
		{
			get { return false; }
		}

        public static IMessageRepository MessageRepository
        {
            get { return ServiceContainer.Resolve<IMessageRepository>(); }
        }
	}

    public class SettingsToCredentialsProviderAdapter : ICredentialsProvider
    {
        public string DeviceId
        {
            get { return Settings.UserDeviceId; }
        }

        public string AccessToken
        {
            get { return Settings.AccessToken; }
        }

        public long UserId
        {
            get { return Settings.MyId; }
        }

        public byte[] PublicKey
        {
            get { return Settings.PublicKey; }
        }
    }
}

