using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfNotifierClient
{
    /// <summary>
    /// Interaction logic for IntervalReportWindow.xaml
    /// </summary>
    public partial class IntervalReportWindow : Window
    {
        public IntervalReportWindow()
        {
            InitializeComponent();
        }

        private void showReport_Click(object sender, RoutedEventArgs e)
        {
            var connection = new DbConnection();

            DateTime? selectedDate = startTime.SelectedDate;
            var pasTime = selectedDate.Value;
            DgTrxInfo.ItemsSource = null;
            DgTrxInfo.ItemsSource = connection.SelectFromDb(pasTime, DateTime.Now);

        }

    }
}
