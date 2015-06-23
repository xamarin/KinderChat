using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Matriarch.Views
{
	public class MainPage : TabbedPage
	{
		public MainPage()
		{
			Title = "Matriarch";
			this.Children.Add(new Statistics());
			this.Children.Add(new StatisticsAvatars());
			this.Children.Add(new StatisticsNames());
			this.Children.Add(new FlaggedUsers());
		}
	}
}
