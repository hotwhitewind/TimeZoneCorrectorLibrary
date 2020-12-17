using ConvertGeoNamesDBToMongoDB.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeZoneCorrectorLibrary.Abstraction
{
    public interface ITimeZoneCorrector
    {
        Country ConnectionToDBTest();
        List<string> GetCountries();
        Task<List<string>> GetStates(string countryName);
        Task<Country> GetCountryInfo(string countryName);
        Task<List<string>> GetCities(string countryName);
        Task<List<string>> GetCities(string countryName, string stateName, string districtName);
        Task<List<string>> GetDistricts(string countryName, string stateName);
        Task<City> GetCity(string countryName, string stateName, string districtName, string cityName);
        bool ConvertToUtcFromCustomTimeZone(string timezone, DateTime datetime, out DateTime outdatetime);
    }
}
