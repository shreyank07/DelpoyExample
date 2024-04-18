using Microsoft.Win32;
using PGYMiniCooper.CoreModule.ViewModel;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using ProdigyFramework.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PGYMiniCooper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new AnalyzerViewModel();
            ((this.DataContext)as AnalyzerViewModel).HeaderVM.RequestClose += this.Close;
            SessionConfiguration.OnAnimate += UpdateStoryBoard;
        }

        void UpdateStoryBoard(bool val)
        {
            this.Dispatcher.Invoke(() =>
            {
                Storyboard blinkAnimation = TryFindResource("FadeStoryboard") as Storyboard;
                if (blinkAnimation != null)
                {
                    if (val)
                    {
                        blinkAnimation.Begin();
                        TitleTextBlock.Foreground = Brushes.LightGreen;
                    }
                    else
                    {
                        blinkAnimation.Stop();
                        TitleTextBlock.Foreground = Brushes.LightCoral;
                    }
                }
            });
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                //string fileName = MiniCooperDirectoryInfo.CurrentSettingsFile;
                //var configurations = ConfigModel.GetInstance();
                //SerializeObject<ConfigModel>.SerializeData(configurations, fileName);
               
            }
            catch(Exception ex)
            {
                var log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Error(ex.Message);
            }
            finally
            {
                CloseCaptureApp();
                base.OnClosing(e);
            }
        }

        public void CloseCaptureApp()
        {
            Process[] GetPArry = Process.GetProcesses();
            foreach (Process process in GetPArry)
            {
                string ProcessName = process.ProcessName;

                ProcessName = ProcessName.ToLower().Trim();
                if (ProcessName.CompareTo("prodigycapturetrayapplication") == 0)
                {
                    process.Kill();
                    break;
                }
            }
        }

        private void LogoFilePickerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a logo file";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            var viewModel = this.DataContext as AnalyzerViewModel;
            if (op.ShowDialog() == true)
            {
                viewModel.ReportVM.LogoFilePath = op.FileName;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReportAdvancedMenuItem.IsSubmenuOpen = false;
        }
    }
}
