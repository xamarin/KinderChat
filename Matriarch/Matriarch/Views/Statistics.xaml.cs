using Matriarch.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Matriarch
{
  public partial class Statistics : ContentPage
  {
	  StatisticsViewModel viewModel;
    public Statistics()
    {
      InitializeComponent();
	  Icon = "stats.png";
	  BindingContext = viewModel = new StatisticsViewModel();
    }


	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (viewModel.IsBusy || viewModel.RegDates.Count > 0)
			return;

		viewModel.ExecuteLoadStatsCommand();
	}
  }
}
