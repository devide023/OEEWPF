using System;
using System.Diagnostics;
using System.Windows;
using LBJOEE.Tools;
using LBJOEE.Services;
using Prism.Ioc;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
namespace LBJOEE.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IContainerExtension _container; 
        private readonly SBXXService _sbxxservice;

        public MainWindow(SBXXService sBXXService, IContainerExtension container)
        {
            InitializeComponent();
            _container = container;
            _sbxxservice = sBXXService;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            string path = Process.GetCurrentProcess().MainModule.FileName;
            var list = _sbxxservice.GetDYGX().OrderBy(t=>t.seq);
            // 居中对齐风格
            Style stylem = new Style(typeof(TextBlock));
            Setter setm = new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            stylem.Setters.Add(setm);
            foreach (var item in list)
            {
                DataGridTextColumn col = new DataGridTextColumn()
                {
                    Header = item.txt,
                    Width = (double)item.width,
                    MinWidth = (double)50,
                    MaxWidth = (double)200,
                    Binding = new Binding(item.colname),
                    ElementStyle = stylem
                };
                DataGrid_His.Columns.Add(col);
            }
        }

        private void main_window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        

    }
}
