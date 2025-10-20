namespace ChapterTwo
{

    public static class AC_Shuffle
    {
        private static readonly Random rng = new Random();

        public static (List<T> Train, List<T> Test) ShuffleAndSplit<T>(List<T> data, double testRatio)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (testRatio <= 0 || testRatio >= 1)
                throw new ArgumentException("testRatio must be between 0 and 1 (exclusive).");

            // Create a shuffled copy of the indices
            var indices = Enumerable.Range(0, data.Count).OrderBy(x => rng.Next()).ToList();

            int testSetSize = (int)(data.Count * testRatio);
            var testIndices = new HashSet<int>(indices.Take(testSetSize));

            var trainSet = new List<T>();
            var testSet = new List<T>();

            for (int i = 0; i < data.Count; i++)
            {
                if (testIndices.Contains(i))
                    testSet.Add(data[i]);
                else
                    trainSet.Add(data[i]);
            }

            return (trainSet, testSet);
        }
        public static (List<T> Train, List<T> Test) TrainTestSplitById<T>(
    IReadOnlyList<T> data,
    double testSize,
    Func<T, long> idSelector,
    int? randomSeed = null) // e.g., x => x.Id
        {
            if (testSize <= 0 || testSize >= 1)
                throw new ArgumentException("testSize must be between 0 and 1.");


            var rng = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();
            var shuffled = data.OrderBy(idSelector).ToList();

            int testCount = (int)Math.Round(data.Count * testSize);
            int trainCount = data.Count - testCount;

            var trainSet = shuffled.Take(trainCount).ToList();
            var testSet = shuffled.Skip(trainCount).Take(testCount).ToList();

            return (trainSet, testSet);
        }
    }
}
