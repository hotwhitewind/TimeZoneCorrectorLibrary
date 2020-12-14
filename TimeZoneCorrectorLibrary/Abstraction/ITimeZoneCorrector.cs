using ConvertGeoNamesDBToMongoDB.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeZoneCorrectorLibrary.Abstraction
{
    public interface ITimeZoneCorrector
    {
        Country ConnectionToDBTest();
        List<string> GetCountries();
        List<string> GetStates(string countryName);
        Country GetCountryInfo(string countryName);
        List<string> GetCities(string countryName);
        List<string> GetCities(string countryName, string stateName, string districtName);
        List<string> GetDistricts(string countryName, string stateName);
        City GetCity(string countryName, string stateName, string districtName, string cityName);
        DateTime ConvertToUtcFromCustomTimeZone(string timezone, DateTime datetime);
    }
}
