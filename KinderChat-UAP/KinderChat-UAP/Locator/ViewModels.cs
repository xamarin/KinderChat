using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Autofac;
using KinderChat_UAP.Common;
using KinderChat_UAP.ViewModels;

namespace KinderChat_UAP.Locator
{
    public class ViewModels
    {
        public ViewModels()
        {
            if (DesignMode.DesignModeEnabled)
            {
                App.Container = AutoFacConfiguration.Configure();
            }
        }

        public static LoginPageViewModel LoginPageVm => App.Container.Resolve<LoginPageViewModel>();
    }
}
