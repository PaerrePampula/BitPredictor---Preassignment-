using BitPredictor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitPredictor.Services
{
    /// <summary>
    /// Contains business logic for calculating trends, highest volumes and finding best dates for buying and selling
    /// </summary>
    public class CryptoCalculationService
    {
        /// <summary>
        /// Returns the longest bearish market for a given list of dates and prices
        /// </summary>
        /// <param name="prices"></param>
        /// <returns></returns>
        public int GetLongestBearishTrendForTimes(List<List<double>> prices)
        {
            int longestTrend = 0;
            int currentTrend = 0;
            List<List<double>> pricesPerDayList = PriceHistorySimplifierService.getPerDayCoinValues(prices);
            //There might not be any bearish trending, so save that to 0 to start
            for (int i = 1; i < pricesPerDayList.Count; i++)
            {
                if (pricesPerDayList[i][1] <= pricesPerDayList[i - 1][1])
                {
                    currentTrend++;
                }
                else
                {
                    currentTrend = 0;
                }
                longestTrend = (currentTrend < longestTrend) ? longestTrend : currentTrend;
            }
            return longestTrend;
        }
        /// <summary>
        /// Returns the day from a list of dates and prices during which the volume was the highest.
        /// </summary>
        /// <param name="prices"></param>
        /// <returns></returns>
        public DatePrice GetDatePriceForHighestVolume(List<List<double>> prices)
        {
            //The JSON contains the information for volume in the second index, access this one.
            Dictionary<long, double> pricesByVolumeHashed = PriceHistorySimplifierService.CreateHashes(prices);
            var highestVolumeHash = pricesByVolumeHashed.FirstOrDefault(x => x.Value == pricesByVolumeHashed.Values.Max()).Key;
            DatePrice datePrice = new DatePrice(highestVolumeHash, pricesByVolumeHashed[highestVolumeHash]);
            return datePrice;
        }
        /// <summary>
        /// Returns best dates for buying end selling bitcoin for a given list of dates
        /// The buy day will always predate the sell day
        /// First value returned is the lowest price
        /// Second one is the highest
        /// </summary>
        /// <param name="prices"></param>
        /// <returns></returns>
        public (DatePrice, DatePrice) GetBestDatePricesForBuyingAndSelling(List<List<double>> prices)
        {
            //Important for the algorithm:
            //Even though the user of the time machine can use the time machine
            //to travel through time, keeping the bought bitcoin BEFORE the buying date is a bit
            //impossible, since the information of purchase of the bitcoin would not travel back in time, 
            //as bitcoin is not tangibly physical, and cannot be taken in someones person in a timemachine,
            //what this means is that the buy date MUST be before the SELL date.
            //The assignment mentioned that the side effects of time travel shouldnt be considered,
            //but I am not sure that you would consider a critical error like this to be a thing like that,
            //unless the assignment description is more like a fun larping of some situation.
            //How the algorithm should work:

            //1.Get the prices per day in a new list, one timeframe per day should suffice
            //2.Start off by finding the index where the minimum value (buy day) is found
            //3.Split this off to a new list, this list will contain the days from buyday-end of list
            //4.Find the highest sell day with this list. Remove the lowest price day from the original list.
            //5.If a low to high value is found, cache this information.
            //6.Repeat the process 2-5 until there is nothing to iterate. 
            //7.Compare the members of the collection of day pairs. The days with the highest profit margins are the optimal choices.
            //8.Return the optimal choices.
            //BUT:If the iteration only returned decreasing values, return null, there isnt an optimal day to buy or sell.


            List<List<double>> pricesPerDayList = PriceHistorySimplifierService.getPerDayCoinValues(prices);
            //The possible buy/sell days should be cached to a tuple collection like this.
            List<(DatePrice, DatePrice)> cachedBuySellPairDatePrices = new List<(DatePrice, DatePrice)>();
            //Find the first lowest index first.
            int lowestIndex = findMinimumIndex(pricesPerDayList);
            //Prices only decreased if null, null is returned, no reason to go through the rest of the operation.
            if (lowestIndex < 0) return (null, null);
            //There was an increase in prices at some point if the block below is accessed.
            while (pricesPerDayList.Count > 1)
            {
                //This will be accessed only if the while loop has already ran, it will find the second lowest buy day.
                if (lowestIndex < 0) lowestIndex = findMinimumIndex(pricesPerDayList);
                if (lowestIndex < 0) break;

                List<List<double>> splitList = new List<List<double>>();
                //Get the original list, and split into the list a new list that starts with the lowest value date.
                splitList.AddRange(pricesPerDayList.GetRange(lowestIndex, pricesPerDayList.Count - lowestIndex - 1));
                int maximumIndex = findMaximumIndex(splitList);
                if (maximumIndex != 0)
                {
                    cachedBuySellPairDatePrices.Add((
                        new DatePrice((long)splitList[0][0], splitList[0][1]),
                        new DatePrice((long)splitList[maximumIndex][0], splitList[maximumIndex][1])));
                }
                pricesPerDayList.RemoveAt(lowestIndex);
                lowestIndex = -1;

            }

            //Cache the index and value of the biggest margin
            //You dont need to cache the margin value, but its a tiny optimization not to calculate this when comparing another pair of margins
            double biggestMargin = 0;
            int biggestMarginIndex = 0;

            for (int i = 0; i < cachedBuySellPairDatePrices.Count; i++)
            {
                //Get a new margin by subtracting the sell day from the buy day.
                double newMargin = (cachedBuySellPairDatePrices[i].Item2.Price - cachedBuySellPairDatePrices[i].Item1.Price);
                if (newMargin > biggestMargin)
                {
                    //If a bigger margin was found than the currently cached one, cache the information from this margin
                    biggestMarginIndex = i;
                    biggestMargin = newMargin;
                }
            }
            if (cachedBuySellPairDatePrices.Count < 1)
            {
                return (null, null);
            }
            //Return the cached dateprices with the biggest margin.
            return (cachedBuySellPairDatePrices[biggestMarginIndex].Item1, cachedBuySellPairDatePrices[biggestMarginIndex].Item2);



        }

        
        #region Private methods
        /// <summary>
        /// Finds index of the date that had the minimum value from a list
        /// A return value of -1 means that only decreasing values were found
        /// </summary>
        /// <param name="prices"></param>
        /// <returns></returns>
        int findMinimumIndex(List<List<double>> prices)
        {
            int minimumIndex = 0;
            bool priceOnlyDecreased = true;
            for (int i = 1; i < prices.Count; i++)
            {
                if (prices[minimumIndex][0] > prices[i][0])
                {
                    minimumIndex = i;
                }
                else
                {
                    priceOnlyDecreased = false;
                }
            }
            if (priceOnlyDecreased) return -1;
            else return minimumIndex;
        }
        /// <summary>
        /// Finds index of the date that had the maximum value from a list
        /// </summary>
        /// <param name="prices"></param>
        /// <returns></returns>
        int findMaximumIndex(List<List<double>> prices)
        {
            int maximumIndex = 0;
            for (int i = 0; i < prices.Count; i++)
            {
                if (prices[maximumIndex][1] < prices[i][1])
                {
                    maximumIndex = i;
                }
            }
            return maximumIndex;
        }
        #endregion

    }

}
