using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.CDI
{
    class Statistics
    {
        /// <summary> 
        /// Tested against STDEV - standard deviation function in Google Sheets
        /// Standard Deviation 
        /// </summary>
        public static double StandardDeviation(double[] X, double xMean)
        {
            double sum = 0;
            foreach(double x in X)
            {
                double value = x - xMean;
                sum += value * value;
            }
            return Math.Sqrt(sum / (X.Length - 1));
        }

        /// <summary> 
        /// Tested against COVAR - covariance function in Google Sheets
        /// Covariance of X and Y 
        /// </summary>
        public static double Covariance(double[] X, double[] Y, double xMean, double yMean)
        {
            double sum = 0;
            double n = X.Length;
            for (int i = 0; i < n; ++i)
                sum += (X[i] - xMean) * (Y[i] - yMean);

            return sum / (n - 1);
        }


        /// <summary> 
        /// Tested against CORREL - correlation function in Google Sheets
        /// Pearson Correlation Coefficient = SQRT(RSquared) 
        /// Covariance(x,y)/(stdev(x)*stdev(y))
        /// </summary>
        public static double Correlation(double[] X, double[] Y)
        {
            if ((X.Length == 0) || (X.Length != Y.Length))
                return 0;

            double xMean = X.Average();
            double yMean = Y.Average();

            double covariance = Covariance(X, Y, xMean, yMean);

            double xStdDev = StandardDeviation(X, xMean);
            double yStdDev = StandardDeviation(Y, yMean);
            return covariance / (xStdDev * yStdDev);
        }

        /// <summary> 
        /// Optimized version of PearsonCorrelationCoefficient
        /// RSquared = PearsonCorrelationCoefficient^2 
        /// </summary>
        public static double RSquared(double?[] X, double[] Y)
        {
            if ((X.Length == 0) || (X.Length != Y.Length))
                return 0;

            double n = X.Length;
            double sumX = 0, sumY = 0, sumXX = 0, sumYY = 0, sumXY = 0;
            double x;
            for(int i = 0; i < n; ++i)
            {
                if (!X[i].HasValue)
                    continue;
                x = X[i].Value;
                sumX += x;
                sumY += Y[i];
                sumXX += x * x;
                sumYY += Y[i] * Y[i];
                sumXY += x * Y[i];
            }

            double XMean = sumX / n;
            double YMean = sumY / n;
            double numerator = sumXY / n - XMean * YMean;
            numerator *= numerator;

            double denominator = (sumXX / n - XMean * XMean) * (sumYY / n - YMean * YMean);
            return denominator != 0 ? numerator / denominator : 0.0;
        }

    }
}
