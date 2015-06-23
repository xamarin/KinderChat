using Matriarch.ViewModels;
using KinderChat.ServerClient.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Matriarch
{
  public partial class FlaggedUsersDetails : ContentPage
  {
	  FlaggedUsersViewModel viewModel;
	public FlaggedUsersDetails(FlaggedUsersViewModel viewModel, Flag flag)
    {
      InitializeComponent();
	  Icon = "badge.png";
		viewModel.Flag = flag;
		Title = flag.AlertLevel.ToString();
	  BindingContext = this.viewModel = viewModel;
    }

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (viewModel.IsBusy || viewModel.UsersGrouped.Count > 0)
			return;

		viewModel.ExecuteLoadUsersCommand();

	}
  }
}
