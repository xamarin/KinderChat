using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using KinderChat_UAP.ViewModels;

namespace KinderChat_UAP.Common
{
    public class AutoFacConfiguration
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            // Register View Models
            builder.RegisterType<LoginPageViewModel>().SingleInstance();
            builder.RegisterType<MainPage>();
            return builder.Build();
        }
    }
}
