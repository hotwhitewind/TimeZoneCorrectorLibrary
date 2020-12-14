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
            try
            {
                return _mongoRepository.FindOne(c => c.CountryName == "Andorra");
            }
            catch (TimeoutException ex)
            {
                return null;
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<string> GetCountries()
        {
            try
            {
                return _mongoRepository.AsQueryable()?.Select(c => c.CountryName).ToList();
            }
            catch(TimeoutException ex)
            {
                return null;
            }
            catch(MongoDB.Driver.MongoException ex)
            {
                return null;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public List<string> GetStates(string countryName)
        {
            try
            {
                return _mongoRepository.FindOne(c => c.CountryName == countryName)
                    .States?.Select(c => c.StateName)
                    .ToList();
            }
            catch (TimeoutException ex)
            {
                return null;
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<string> GetCities(string countryName)
        {
            try
            {
                return _mongoRepository.FindOne(c => c.CountryName == countryName)
                    .Cities?.Select(c => c.CityName)
                    .ToList();
            }
            catch (TimeoutException ex)
            {
                return null;
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<string> GetCities(string countryName, string stateName, string districtName)
        {
            try
            {
                if (string.IsNullOrEmpty(stateName) || stateName == "--")
                {
                    return _mongoRepository.FindOne(c => c.CountryName == countryName)
                       .Cities?.Select(c => c.CityName).ToList();
                }
                else if(string.IsNullOrEmpty(districtName) || districtName == "--")
                {
                    return _mongoRepository.FindOne(c => c.CountryName == countryName)
                        .States?.Find(c => c.StateName == stateName)
                        .Cities?.Select(c => c.CityName).ToList();
                }
                else
                {
                    return _mongoRepository.FindOne(c => c.CountryName == countryName)
                        .States?.Find(c => c.StateName == stateName)
                        .Districts?.Find(c => c.DistrictName == districtName)
                        .Cities?.Select(c => c.CityName).ToList();
                }
            }
            catch (TimeoutException ex)
            {
                return null;
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public City GetCity(string countryName, string stateName, string districtName, string cityName)
        {
            try
            {
                if (string.IsNullOrEmpty(stateName) && string.IsNullOrEmpty(districtName))
                {
                    return _mongoRepository.FindOne(c => c.CountryName == countryName)
                        .Cities?.Find(c => c.CityName == cityName);
                }
                else if(string.IsNullOrEmpty(districtName))
                {
                    return _mongoRepository.FindOne(c => c.CountryName == countryName)
                        .States?.Find(c => c.StateName == stateName)
                        .Cities?.Find(c => c.CityName == cityName);
                }
                else
                {
                    return _mongoRepository.FindOne(c => c.CountryName == countryName)
                        .States?.Find(c => c.StateName == stateName)
                        .Districts?.Find(c => c.DistrictName == districtName)
                        .Cities?.Find(c => c.CityName == cityName);
                }
            }
            catch (TimeoutException ex)
            {
                return null;
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DateTime ConvertToUtcFromCustomTimeZone(string timezone, DateTime datetime)
        {
            DateTimeZone zone = DateTimeZoneProviders.Tzdb[timezone];
            var localtime = LocalDateTime.FromDateTime(datetime);
            var zonedtime = localtime.InZoneLeniently(zone);
            return zonedtime.ToInstant().InZone(zone).ToDateTimeUtc();
        }

        public Country GetCountryInfo(string countryName)
        {
            try
            {
                return _mongoRepository.FindOne(c => c.CountryName == countryName);
            }
            catch (TimeoutException ex)
            {
                return null;
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<string> GetDistricts(string countryName, string stateName)
        {
            try
            {
                return _mongoRepository.FindOne(c => c.CountryName == countryName)
                    .States?.Find(c => c.StateName == stateName)
                    .Districts.Select(c => c.DistrictName)
                    .ToList();
            }
            catch (TimeoutException ex)
            {
                return null;
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
