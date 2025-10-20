using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using SharpCompress.Readers;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Compression;
namespace ChapterTwo
{
    public class AA_HousingDataLoader
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string TarballUrl = "https://github.com/ageron/data/raw/main/housing.tgz";
        private const string TarballPath = "datasets/housing.tgz";
        private const string ExtractedCsvPath = "datasets/housing/housing.csv";

        public async Task<List<HousingRecord>> LoadHousingDataAsync()
        {
            // Create datasets directory if it doesn't exist
            Directory.CreateDirectory("datasets");

            // Download tarball if it doesn't exist
            if (!File.Exists(TarballPath))
            {
                Console.WriteLine("Downloading housing data...");
                using var response = await httpClient.GetAsync(TarballUrl);
                response.EnsureSuccessStatusCode();

                using var fileStream = File.Create(TarballPath);
                await response.Content.CopyToAsync(fileStream);
            }

            // Extract tarball if CSV doesn't exist
            if (!File.Exists(ExtractedCsvPath))
            {
                Console.WriteLine("Extracting housing data...");
                ExtractTarGz(TarballPath, "datasets");
            }

            // Read and return CSV data
            return ReadCsvData(ExtractedCsvPath);
        }

        private void ExtractTarGz(string tarGzFilePath, string extractPath)
        {
            // First decompress .gz
            string tarFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tar");
            try
            {
                using var inputFile = File.OpenRead(tarGzFilePath);
                using var gzipStream = new GZipStream(inputFile, CompressionMode.Decompress);
                using var outputFile = File.Create(tarFilePath);
                gzipStream.CopyTo(outputFile);
            }
            catch (InvalidDataException ex) when (ex.Message.Contains("unknown compression method"))
            {
                // If it's not actually gzipped, treat as regular tar
                tarFilePath = tarGzFilePath;
            }

            // Then extract tar
            ExtractTar(tarFilePath, extractPath);

            // Clean up temporary tar file if we created one
            if (tarFilePath != tarGzFilePath && File.Exists(tarFilePath))
            {
                File.Delete(tarFilePath);
            }
        }

        private void ExtractTar(string tarFilePath, string extractPath)
        {
            using var stream = File.OpenRead(tarFilePath);
            using var reader = SharpCompress.Readers.ReaderFactory.Open(stream);
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    reader.WriteEntryToDirectory(extractPath, new  SharpCompress.Common.ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
            }

        }

        private List<HousingRecord> ReadCsvData(string csvPath)
        {
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture));
            return csv.GetRecords<HousingRecord>().ToList();
        }
    }

    // 									

    public class HousingRecord
    {
        [CsvHelper.Configuration.Attributes.Name("longitude")]
        public float? Longitude { get; set; }

        [Name("latitude")]
        public float? Latitude { get; set; }

        [Name("housing_median_age")]
        public float? HousingMedianAge { get; set; }

        [Name("total_rooms")]
        public float? TotalRooms { get; set; }

        [Name("total_bedrooms")]
        public float? TotalBedrooms { get; set; }

        [Name("population")]
        public float? Population { get; set; }

        [Name("households")]
        public float? Households { get; set; }

        [Name("median_income")]
        public float? MedianIncome { get; set; }

        [Name("ocean_proximity")]
        public string? OceanProximity { get; set; }

        [Name("median_house_value")]
        public float? MedianHouseValue { get; set; }
    }
}
