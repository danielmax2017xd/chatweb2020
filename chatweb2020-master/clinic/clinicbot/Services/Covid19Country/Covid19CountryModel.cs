using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace clinicbot.Services.Covid19Country
{
    [Serializable]
    //MODELO
    public class Covid19CountryModel
    {
        public string country { get; set; }
        public CountryInfo countryInfo { get; set; }
        public int cases { get; set; }
        public int todayCases { get; set; }
        public int deaths { get; set; }
        public int todayDeaths { get; set; }
        public int recovered { get; set; }
        public int active { get; set; }
        public int critical { get; set; }
        public double? casesPerOneMillion { get; set; }
        public double? deathsPerOneMillion { get; set; }
    }

    [Serializable]
    public class CountryInfo
    {
        public int? _id { get; set; }
        public string country { get; set; }
        public string iso2 { get; set; }
        public string iso3 { get; set; }
        public double lat { get; set; }
        public double @long { get; set; }
        public string flag { get; set; }
    }
}
