using PharmacyApp.ViewModels;
using ScottPlot;
using ScottPlot.Plottables;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PharmacyApp.Views.Pages
{
    public partial class DashboardPage : UserControl
    {
        public DashboardPage()
        {
            InitializeComponent();
            Loaded += DashboardPage_Loaded;
        }

        private void DashboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is not DashboardViewModel vm)
                return;

            if (vm.RevenueValues == null || vm.RevenueValues.Length == 0)
                return;

            var plot = RevenuePlot.Plot;
            plot.Clear();

            var bars = new Bar[vm.RevenueValues.Length];

            for (int i = 0; i < vm.RevenueValues.Length; i++)
            {
                bars[i] = new Bar
                {
                    Position = i,
                    Value = vm.RevenueValues[i],
                    FillColor = Colors.CadetBlue,
                    LineColor = Colors.CadetBlue
                };
            }

            plot.Add.Bars(bars);

            plot.Add.Bars(bars);

            double[] positions = Enumerable
                .Range(0, vm.RevenueLabels.Length)
                .Select(i => (double)i)
                .ToArray();

            string[] labels = vm.RevenueLabels
                .Select(l => l ?? "")
                .ToArray();

            plot.Axes.Bottom.TickGenerator =
                new ScottPlot.TickGenerators.NumericManual(positions, labels);

            plot.Title("Doanh thu");

            plot.Axes.Left.Label.Text = "VNĐ";
            plot.Axes.Bottom.Label.Text = "Thời gian";

            plot.Axes.Bottom.TickLabelStyle.Rotation = 45;
            plot.Axes.Bottom.TickLabelStyle.FontSize = 12;

            RevenuePlot.Refresh();
        }
    }
}
