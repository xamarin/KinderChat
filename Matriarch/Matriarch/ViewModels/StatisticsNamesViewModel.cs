using Matriarch.Helpers;
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
	public class StatisticsNamesViewModel : BaseViewModel
	{


		public StatisticsNamesViewModel()
		{
			Title = "Names";
			Names = new ObservableCollection<PopularNames>();
		}


		public ObservableCollection<PopularNames> Names { get; set; }



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

				Names.Clear();
				foreach(var name in await adminManager.PopularNames())
				{
					Names.Add(name);
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
