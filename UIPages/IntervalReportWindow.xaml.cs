using System;
using System.Windows;

namespace WpfNotifierClient.UIPages
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
            var end = endTime.SelectedDate != null ? endTime.SelectedDate.Value.AddDays(1) : DateTime.Now.AddDays(1);
            DgTrxInfo.ItemsSource = null;
            DgTrxInfo.ItemsSource = connection.SelectTrxFromDb(start, end);
        }

    }
}
