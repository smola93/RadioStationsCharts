using System.Collections.Generic;

namespace RadioStationsCharts
{
    public class ChartsScraping
    {
        public string Station { get; set; }
        public string ChartsName { get; set; }
        public List<Charts> Charts { get; set; }
    }

    public class Charts
    {
        public int Position { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
    }
}
