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
	public class FlaggedUsersViewModel : BaseViewModel
	{

		public Flag Flag { get; set; }
		ObservableCollection<Grouping<string, Flag>> flagsGrouped;

		public ObservableCollection<Grouping<string, Flag>> FlagsGrouped
		{
			get { return flagsGrouped; }
			private set { flagsGrouped = value; }
		}


		ObservableCollection<Grouping<string, User>> usersGrouped;

		public ObservableCollection<Grouping<string, User>> UsersGrouped
		{
			get { return usersGrouped; }
			private set { usersGrouped = value; }
		}

		public FlaggedUsersViewModel()
		{
			Title = "Flagged Users";
			flagsGrouped = new ObservableCollection<Grouping<string, Flag>>();
			usersGrouped = new ObservableCollection<Grouping<string, User>>();
		}

		ICommand loadFlagsCommand;
		public ICommand LoadFlagsCommand
		{
			get { return loadFlagsCommand ?? (loadFlagsCommand = new Command(() => ExecuteLoadFlagsCommand())); }
		}

		public async Task ExecuteLoadFlagsCommand()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				
				var userManager = new UserManager(Settings.AccessToken);
			
				var flags = await userManager.GetFlags();
				//Use linq to sorty our monkeys by name and then group them by the new name sort property
				var sorted = from flag in flags
							 orderby flag.AlertLevel
							 group flag by flag.AlertLevel.ToString() into flagGroup
							 select new Grouping<string, Flag>(flagGroup.Key, flagGroup);

				//create a new collection of groups
				FlagsGrouped = new ObservableCollection<Grouping<string, Flag>>(sorted);
				OnPropertyChanged("FlagsGrouped");
			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				page.DisplayAlert("Error", "Unable to load flags.", "OK"); ;
			}
			finally
			{
				IsBusy = false;
			}
		}

		ICommand loadUsersCommand;
		public ICommand LoadUsersCommand
		{
			get { return loadUsersCommand ?? (loadUsersCommand = new Command(() => ExecuteLoadUsersCommand())); }
		}

		public async Task ExecuteLoadUsersCommand()
		{

			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				var adminManager = new AdminManager(Settings.AccessToken);


				var users = await adminManager.FlagUserList(Flag.Id);
				//Use linq to users by name and then group them by the new name sort property
				var sorted = from user in users
							 orderby user.NickName
							 group user by (user.NickName.Length == 0 ? "?" : user.NickName[0].ToString()) into userGroup
							 select new Grouping<string, User>(userGroup.Key, userGroup);

				//create a new collection of groups
				UsersGrouped = new ObservableCollection<Grouping<string, User>>(sorted);
				OnPropertyChanged("UsersGroupeds");
			}
			catch (Exception ex)
			{
				var page = new ContentPage();
				page.DisplayAlert("Error", "Unable to load users.", "OK"); ;
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}
