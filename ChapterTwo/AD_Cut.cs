using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChapterTwo
{
    public static class AD_Cut
    {
        public static TBin Cut<TBin>(double value, double[] bins, TBin[] labels)
        {
            if (bins.Length != labels.Length)
                throw new ArgumentException("Bins and labels must have the same length.");

            for (int i = 0; i < bins.Length; i++)
            {
                double lower = i == 0 ? double.NegativeInfinity : bins[i - 1];
                double upper = bins[i];

                if (value >= lower && value < upper)
                    return labels[i];
            }

            // Handle last bin (inclusive of infinity)
            if (value >= bins[^1])
                return labels[^1];

            throw new ArgumentOutOfRangeException(nameof(value));
        }
    }
}
