using System;
using System.Diagnostics;
using System.Windows;
using LBJOEE.Tools;
using LBJOEE.Services;
namespace LBJOEE.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static bool IsStart = true;
        public MainWindow()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            string path = Process.GetCurrentProcess().MainModule.FileName;
            //AutoStart auto = new AutoStart();
            //auto.SetMeAutoStart(true);
            
        }

        private void main_window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        

    }
}
