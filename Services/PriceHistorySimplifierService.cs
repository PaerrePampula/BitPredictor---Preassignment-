using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// The API does not give you data by day if your timestamps are close enough, so they need to be preprocessed
/// a bit before they can be checked.
/// </summary>
namespace BitPredictor.Services
{
    public static class PriceHistorySimplifierService
    {
        /// <summary>
        /// Return a list of coin values where each timestamp is from a different day.
        /// May not be actually from 00:00:00, but the closest timestamp to that time.
        /// </summary>
        /// <param name="listOfValues"></param>
        /// <returns></returns>
        public static List<List<double>> getPerDayCoinValues(List<List<double>> listOfValues)
        {
            List<List<double>> coinPricePerDayList = new List<List<double>>();
            coinPricePerDayList.Add(listOfValues[0]);
            int dayCount = 1;
            for (int i = 0; i < listOfValues.Count; i++)
            {
                if (listOfValues[i][0] > coinPricePerDayList[dayCount-1][0] + 86400000)
                {
                    coinPricePerDayList.Add(listOfValues[i]);
                    dayCount++;
                }
            }
            return coinPricePerDayList;
        }
        public static Dictionary<long, double> CreateHashes(List<List<double>> listOfValues)
        {
            Dictionary<long, double> hashedValues = new Dictionary<long, double>();
            for (int i = 0; i < listOfValues.Count; i++)
            {
                hashedValues.Add((long)listOfValues[i][0], (double)listOfValues[i][1]);
            }
            return hashedValues;
        }
    }
}
