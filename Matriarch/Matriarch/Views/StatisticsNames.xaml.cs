using Matriarch.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Matriarch.Views
{
	public partial class StatisticsNames : ContentPage
	{
		StatisticsNamesViewModel viewModel;
		public StatisticsNames()
		{
			InitializeComponent();
			Icon = "stats.png";
			BindingContext = viewModel = new StatisticsNamesViewModel();
		}


		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (viewModel.IsBusy || viewModel.Names.Count > 0)
				return;

			viewModel.ExecuteLoadStatsCommand();
		}
	}
}
