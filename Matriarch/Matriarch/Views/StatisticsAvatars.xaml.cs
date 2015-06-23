using Matriarch.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Matriarch.Views
{
	public partial class StatisticsAvatars : ContentPage
	{
		StatisticsAvatarsViewModel viewModel;
		public StatisticsAvatars()
		{
			InitializeComponent();
			Icon = "stats.png";
			BindingContext = viewModel = new StatisticsAvatarsViewModel() ;
		}


		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (viewModel.IsBusy || viewModel.Avatars.Count > 0)
				return;

			viewModel.ExecuteLoadStatsCommand();
		}
	}
}
