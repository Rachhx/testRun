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
using System.Globalization;
using System.Diagnostics;

namespace AstraTestWpf
{
    /// <summary>
    /// Interaction logic for MainTest.xaml
    /// </summary>
    public partial class MainTest : Window
    {
        public MainTest()
        {
            InitializeComponent();
        }
        private void Next_Click(object sender, RoutedEventArgs e) => next(0);

        private void next(int sensorIndex)
        {
            var color1 = color.IsChecked ?? false; // with the skeleton one (coloured)
            var tracking = track.IsChecked ?? false; // with the actual image (from camera)
            nextup(sensorIndex, color1, tracking);
        }

        private void nextup(int sensorIndex, bool color1 , bool tracking)
        {
            try
            {
                // connecting with the device (orbbec astra) 
                var connectionString = "device/sensor" + sensorIndex.ToString(CultureInfo.InvariantCulture);
                var set = Astra.StreamSet.Open(connectionString);
                var sensorViewModel = new SensorViewModel(Properties.Settings.Default, Dispatcher, set, color1, tracking);

                // switch/redirect to the next page
                var wnd = new test123(sensorViewModel) { Owner = this };
                wnd.Title += " #" + sensorIndex;
                wnd.Show();
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Cannot open depth sensor #{sensorIndex}:\r\n{exc.Message}", "Error");
            }
        }

      
    }
    }

