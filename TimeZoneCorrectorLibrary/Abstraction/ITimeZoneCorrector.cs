using ConvertGeoNamesDBToMongoDB.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeZoneCorrectorLibrary.Abstraction
{
    public interface ITimeZoneCorrector
    {
        Country ConnectionToDBTest();
        List<string> GetAllCountries();
        List<string> GetAllStateByCountry(string countryName);
        List<string> GetAllCitiesByCountry(string countryName);
        List<string> GetAllCitiesByState(string countryName, string stateName);
        City GetCityByCountryAndState(string countryName, string stateName, string cityName);
        DateTime ConvertToUtcFromCustomTimeZone(string timezone, DateTime datetime);
    }
}
