using ChapterTwo;
using HandsOnMachineLearning.WindowsForm.ChapterTwo;

namespace HandsOnMachineLearning.WindowsForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ChapterTwo_Histogram();
            //ChapterTwo_Shuffle();

        }
        private async void ChapterTwo_Histogram()
        {

            var housing = await GetHousingData(); // your data source
            var histogram = new AB_Histogram();
            histogram.CreateHistogramSimple(housing);
        }
        private async void ChapterTwo_Shuffle()
        {

            var housing = await GetHousingData(); // your data source
            var(trainSet, testSet) = AC_Shuffle.ShuffleAndSplit<HousingRecord>(housing, 0.2);

            Console.WriteLine($"Total: {housing.Count}, Train: {trainSet.Count}, Test: {testSet.Count}");
        }

        private async Task<List<HousingRecord>> GetHousingData()
        {

            var housingDataLoader = new AA_HousingDataLoader();
            return await housingDataLoader.LoadHousingDataAsync();
        }
    }
}
