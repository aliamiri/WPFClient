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
            startTime.FlowDirection = FlowDirection.RightToLeft;
            endTime.FlowDirection = FlowDirection.RightToLeft;
        }

        private void showReport_Click(object sender, RoutedEventArgs e)
        {
            var connection = new DbConnection();

            var start = startTime.SelectedDate != null ? startTime.SelectedDate.Value : DateTime.Now;
            var end = endTime.SelectedDate != null ? endTime.SelectedDate.Value : DateTime.Now.AddDays(1);
            DgTrxInfo.ItemsSource = null;
            DgTrxInfo.ItemsSource = connection.SelectFromDb(start, end);
        }

    }
}
