using Matriarch.Helpers;
using KinderChat.ServerClient;
using KinderChat.ServerClient.Entities;
using KinderChat.ServerClient.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Matriarch.ViewModels
{
	public class MyAvatar
	{
		public string Count { get; set; }
		public string Url { get; set; }

		public string Location { get; set; }
	}
	public class StatisticsAvatarsViewModel : BaseViewModel
	{


		private int count;
		public int Count
		{
			get { return count; }
			set { SetProperty(ref count, value); }
		}

		public ObservableCollection<MyAvatar> Avatars { get; set; }

		public StatisticsAvatarsViewModel()
		{
			Avatars = new ObservableCollection<MyAvatar>();
			Title = "Avatars";
		}


		ICommand loadStatsCommand;
		public ICommand LoadStatsCommand
		{
			get { return loadStatsCommand ?? (loadStatsCommand = new Command(() => ExecuteLoadStatsCommand())); }
		}

		public async Task ExecuteLoadStatsCommand()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				var adminManager = new AdminManager(Settings.AccessToken);
				var avatarManager = new AvatarManager(Settings.AccessToken);
				Count = await adminManager.TotalAvatarCount();
				var avas = await avatarManager.GetStaticAvatars();
				Avatars.Clear();
				var items = await adminManager.PopularAvatars();
				foreach(var avatar in items.OrderByDescending(a => a.Count))
				{
					var myava = avas.FirstOrDefault(a => a.Id == avatar.AvatarId);

					if (myava == null)
						continue;

					Avatars.Add(new MyAvatar
						{
							Count = avatar.Count.ToString(),
							Location = myava.Location,
							Url = EndPoints.BaseUrl + myava.Location
						});
				}
			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				page.DisplayAlert("Error", "Unable to load.", "OK"); ;
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}
