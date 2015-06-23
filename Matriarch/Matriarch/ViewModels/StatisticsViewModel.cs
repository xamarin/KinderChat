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
	public class MyRegDate
	{
		public string Title { get; set; }
		public string Detail { get; set; }
	}
	public class StatisticsViewModel : BaseViewModel
	{
		public StatisticsViewModel()
		{
			Title = "Registrations";
			RegDates = new ObservableCollection<MyRegDate>();
		}


		public ObservableCollection<MyRegDate> RegDates { get; set; }



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

				RegDates.Clear();
				foreach(var reg in await adminManager.TotalRegistrations())
				{
					RegDates.Add(new MyRegDate
						{
							Title = new DateTime(reg.Year, reg.Month, reg.Day).ToString("D"),
							Detail = reg.Total.ToString()
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
