using System.Collections.Generic;
namespace BitPredictor.Models
{
    public class BTCPricesData
    {
        //JSON Information from the public API is split into three lists of lists of doubles, the first element of each
        //inset list is the date, the second element is the variable described by property name.
        public List<List<double>> prices { get; set; }
        public List<List<double>> market_caps { get; set; }
        public List<List<double>> total_volumes { get; set; }
    }
}
