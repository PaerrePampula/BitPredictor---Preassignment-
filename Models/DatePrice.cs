using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BitPredictor.Models
{
    public class DatePrice
    {
        public long Date { get; set; }
        public double Price { get; set; }
        public DatePrice(long date, double price)
        {
            Date = date;
            Price = price;
        }
        public DateTime getDateTime()
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(Date).DateTime;
        }
        public string getPriceToString()
        {
            return Price.ToString("##,0.00");
        }
    }
}
