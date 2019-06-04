using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;

namespace AstraTestWpf
{
    /// <summary>
    /// Interaction logic for test123.xaml
    /// </summary>
    public partial class test123 : Window
    {
        public test123()
        {
            InitializeComponent();
       
        } 
        internal test123(SensorViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ViewModel?.Dispose();
            Owner?.Activate();
        }
        private SensorViewModel ViewModel => DataContext as SensorViewModel;


        private void CopyDepthImageToClipboard(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(ViewModel?.DepthImageSource);
        }
        private void CopyColorImageToClipboard(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(ViewModel?.ColorImageSource);
        }
        
        private static void CopyToClipboard(BitmapSource bitmap)
        {
            if(bitmap == null)
            {
                Console.Beep();
            }
            else
            {
                Clipboard.SetImage(bitmap);
            }
        }

        private void press_click(object sender, RoutedEventArgs e)
        {
            result.Text = "hello";
        }
    }
}
