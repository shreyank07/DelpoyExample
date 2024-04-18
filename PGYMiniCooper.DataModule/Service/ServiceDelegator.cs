using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGYMiniCooper.DataModule.Model;
using System.Threading;
using PGYMiniCooper.DataModule.Interface;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace PGYMiniCooper.DataModule.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceDelegator
    {
        public ServiceDelegator()
        {
            Register();
        }

        private void Register()
        {
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddSingleton<IApiService, ApiService>()
                .AddSingleton<IDataProvider, DataProvider>()
                .BuildServiceProvider());
        }
    }
}
