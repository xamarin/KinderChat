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
  public partial class FlaggedUsers : ContentPage
  {
	  FlaggedUsersViewModel viewModel;
    public FlaggedUsers()
    {
      InitializeComponent();
	  Icon = "badge.png";
	  BindingContext = viewModel = new FlaggedUsersViewModel();

	  list.ItemTapped += (sender, args) =>
		  {
			  if (list.SelectedItem == null)
				  return;

			  Navigation.PushAsync(new FlaggedUsersDetails(viewModel, list.SelectedItem as Flag));

			  list.SelectedItem = null;
		  };
    }

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (viewModel.IsBusy || viewModel.FlagsGrouped.Count > 0)
			return;

		viewModel.ExecuteLoadFlagsCommand();
	}
  }
}
