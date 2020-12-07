using System;
using NodaTime;
using System.IO;
using System.Collections.Generic;
using TimeZoneCorrectorLibrary.Abstraction;
using ConvertGeoNamesDBToMongoDB.Models;
using System.Linq;

namespace TimeZoneCorrectorLibrary
{
    //http://download.geonames.org/export/dump/
    //https://github.com/dr5hn/countries-states-cities-database

    public class TimeZoneCorrector : ITimeZoneCorrector
    {
        private readonly IMongoRepository<Country> _mongoRepository;

        public TimeZoneCorrector(IMongoRepository<Country> mongoRepository)
        {
            _mongoRepository = mongoRepository;
        }

        public Country ConnectionToDBTest()
        {
            return _mongoRepository.FindOne(c => c.CountryName == "Andorra");
        }

        public List<string> GetAllCountries()
        {
            return _mongoRepository.AsQueryable().Select(c => c.CountryName).ToList();
        }

        public List<string> GetAllStateByCountry(string countryName)
        {
            return _mongoRepository.FindOne(c => c.CountryName == countryName)
                .States?.Select(c => c.StateName)
                .ToList();
        }

        public List<string> GetAllCitiesByCountry(string countryName)
        {
            return _mongoRepository.FindOne(c => c.CountryName == countryName)
                .Cities?.Select(c => c.CityName)
                .ToList();
        }

        public List<string> GetAllCitiesByState(string countryName, string stateName)
        {
            return _mongoRepository.FindOne(c => c.CountryName == countryName)
                .States?.Find(c => c.StateName == stateName)
                .Cities?.Select(c => c.CityName).ToList();
        }

        public City GetCityByCountryAndState(string countryName, string stateName, string cityName)
        {
            if(string.IsNullOrEmpty(stateName))
            {
                return _mongoRepository.FindOne(c => c.CountryName == countryName)
                    .Cities?.Find(c => c.CityName == cityName);
            }
            else
            {
                return _mongoRepository.FindOne(c => c.CountryName == countryName)
                    .States?.Find(c => c.StateName == stateName)
                    .Cities?.Find(c => c.CityName == cityName);
            }
        }

        public DateTime ConvertToUtcFromCustomTimeZone(string timezone, DateTime datetime)
        {
            DateTimeZone zone = DateTimeZoneProviders.Tzdb[timezone];
            var localtime = LocalDateTime.FromDateTime(datetime);
            var zonedtime = localtime.InZoneLeniently(zone);
            return zonedtime.ToInstant().InZone(zone).ToDateTimeUtc();
        }
    }
}
