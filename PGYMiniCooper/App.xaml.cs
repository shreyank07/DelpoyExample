using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using ProdigyFramework.Helpers;
using ProdigyFramework.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace PGYMiniCooper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                //Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                //Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
                //FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
                //            XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
                //UiDispatcherHelper.Initialize();
                //if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
                //{
                //    MessageBox.Show("Application is already running" + "\n" + "Running application will be closed after clicking OK");
                //    System.Diagnostics.Process.GetCurrentProcess().Kill();
                //    return;
                //}

                //if (File.Exists(MiniCooperDirectoryInfo.CurrentSettingsFile))
                //{
                //    var configuration = ConfigModel.GetInstance();
                //    if (SerializeObject<ConfigModel>.DeserializeData(ref configuration, MiniCooperDirectoryInfo.CurrentSettingsFile))
                //    {
                //        ProdigyFramework.Helpers.CustomExtensions.CopyPropertiesFrom(ConfigModel.GetInstance(), configuration);
                //    }
                //}
            }
            catch (Exception ex)
            {

            }
            base.OnStartup(e);
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            log.Error(e);
            log.Error("[Available RAM] " + new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory);
            log.Error("[ErrorLine] " + e.Exception.StackTrace);
            log.Error("[Name] " + e.Exception.TargetSite.Name);
            log.Error("[MemberType] " + e.Exception.TargetSite.MemberType);
            log.Error("[Message] " + e.Exception.Message);

            e.Handled = true;

            // TODO: Address the root cause for ItemsControl issue.
            if (e.Exception is InvalidOperationException && e.Exception.Message.Contains("An ItemsControl is inconsistent with its items source"))
                return;

            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "PGY-LA-EMBD", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var computerInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();
            log4net.Config.XmlConfigurator.Configure();
            log.Info("--------------    Application Started PGY-LA-EMBD V" +
                0 + "-08Jan2020  -   Operating System: " + computerInfo.OSFullName + " -    " + computerInfo.OSPlatform + " -   " + computerInfo.OSVersion +
                "-  RAM:" + ((double)computerInfo.TotalPhysicalMemory / (double)(1024 * 1024 * 1024)));
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.Error(e);
            Exception ex = e.ExceptionObject as Exception;
            MessageBox.Show(ex.Message, "Uncaught Thread Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
