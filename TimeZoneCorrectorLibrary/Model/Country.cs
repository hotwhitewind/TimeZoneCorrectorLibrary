using System;
using System.Collections.Generic;
using System.Text;
using TimeZoneCorrectorLibrary.Abstraction;

namespace ConvertGeoNamesDBToMongoDB.Models
{
    [BsonCollection("countries")]
    public class Country : Document
    {
        public string CountryName { get; set; }
        public string CountryISOCode { get; set; }
        public List<State> States { get; set; }
        public List<City> Cities { get; set; }
    }
}
