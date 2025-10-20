using System;
using System.Linq;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Collections.Generic;
using System.Windows.Forms;
using ChapterTwo;
using ScottPlot;


namespace HandsOnMachineLearning.WindowsForm.ChapterTwo
{
    internal class AB_Histogram
    {
        public void CreateHistogram(List<HousingRecord> housingData)
        {
            // Get data (e.g., MedianIncome)
            var values = housingData
                .Where(h => h.MedianIncome.HasValue)
                .Select(h => h.MedianIncome.Value)
                .ToList();

            if (values.Count == 0) return;

            int binCount = 50;
            double min = values.Min();
            double max = values.Max();
            double binWidth = (max - min) / binCount;

            // Initialize bins
            var binCounts = new int[binCount];
            foreach (var val in values)
            {
                int binIndex = (int)((val - min) / binWidth);
                if (binIndex >= binCount) binIndex = binCount - 1; // Handle max value
                if (binIndex >= 0) binCounts[binIndex]++;
            }

            // Create HistogramItems
            var histogramItems = new List<HistogramItem>();
            for (int i = 0; i < binCount; i++)
            {
                double binStart = min + i * binWidth;
                double binEnd = binStart + binWidth;
                histogramItems.Add(new HistogramItem(binStart, binEnd, binCounts[i], i));
            }

            // Create plot
            var plotModel = new PlotModel { Title = "Housing Data Histogram" };

            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Median Income" });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Frequency", Minimum = 0 });

            // ✅ Correct usage of HistogramSeries
            var histogramSeries = new HistogramSeries
            {
                ItemsSource = histogramItems,
                FillColor = OxyColors.SteelBlue,
                StrokeColor = OxyColors.Transparent
            };

            plotModel.Series.Add(histogramSeries);

            // Display
            var form = new Form { Width = 1200, Height = 800, Text = "Histogram" };
            var plotView = new OxyPlot.WindowsForms.PlotView { Dock = DockStyle.Fill, Model = plotModel };
            form.Controls.Add(plotView);
            form.ShowDialog();
        }



        public void CreateHistogramSimple(List<object> housingData)
        {
            if (housingData == null || housingData.Count == 0) { return; }


            var first = housingData.First();

            var properties = first.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (!IsNumeric(property.PropertyType))
                {
                    continue;
                }
                double[] incomes = housingData
                    .Where(h => property.GetValue(h) != null) // check for null
                    .Select(h => Convert.ToDouble(property.GetValue(h)))
                    .ToArray();

                if (incomes.Length == 0) continue;

                var formsPlot = new ScottPlot.WinForms.FormsPlot
                {
                    Dock = DockStyle.Fill
                };

                var plt = formsPlot.Plot;
                var histogram = ScottPlot.Statistics.Histogram.WithBinCount(50, incomes);
                plt.Add.Histogram(histogram);

                plt.Title("Housing Data Histogram");
                plt.XLabel(property.Name);
                plt.YLabel("Frequency");

                var form = new Form
                {
                    Size = new System.Drawing.Size(600, 400),
                    Text = property.Name,
                };
                form.Controls.Add(formsPlot);
                form.AutoScroll = true;
                form.Show();
            }



        }

        private static readonly HashSet<Type> NumericTypes = new()
        {
            typeof(int), typeof(long), typeof(short), typeof(byte),
            typeof(uint), typeof(ulong), typeof(ushort), typeof(sbyte),
            typeof(float), typeof(double), typeof(decimal)
        };

        public static bool IsNumeric(Type type)
        {
            if (type == null) return false;
            var underlying = Nullable.GetUnderlyingType(type) ?? type;
            return NumericTypes.Contains(underlying);
        }
    }
}


