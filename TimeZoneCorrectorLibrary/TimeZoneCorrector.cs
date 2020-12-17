using System;
using NodaTime;
using System.IO;
using System.Collections.Generic;
using TimeZoneCorrectorLibrary.Abstraction;
using ConvertGeoNamesDBToMongoDB.Models;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace TimeZoneCorrectorLibrary
{
    //http://download.geonames.org/export/dump/
    //https://github.com/dr5hn/countries-states-cities-database

    public class TimeZoneCorrector : ITimeZoneCorrector
    {
        private readonly IMongoRepository<Country> _mongoRepository;
        private readonly ILogger<TimeZoneCorrector> _logger;

        public TimeZoneCorrector(IMongoRepository<Country> mongoRepository, ILogger<TimeZoneCorrector> logger)
        {
            _mongoRepository = mongoRepository;
            _logger = logger;  
        }

        public Country ConnectionToDBTest()
        {
            try
            {
                return _mongoRepository.FindOne(c => c.CountryName == "Andorra");
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Error connection to data base test (timeout)");
                return null;
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                _logger.LogError(ex, "Error connection to data base test (mongo)");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connection to data base test");
                return null;
            }
        }

        public List<string> GetCountries()
        {
            try
            {
                return _mongoRepository.AsQueryable()?.Select(c => c.CountryName).OrderBy(c => c).ToList();
            }
            catch(TimeoutException ex)
            {
                _logger.LogError(ex, "Error get countries (timeout)");
                return null;
            }
            catch(MongoDB.Driver.MongoException ex)
            {
                _logger.LogError(ex, "Error get countries (mongo exception)");
                return null;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error get countries");
                return null;
            }
        }

        public async Task<List<string>> GetStates(string countryName)
        {
            try
            {
                Country country = await _mongoRepository.FindOneAsync(c => c.CountryName == countryName);
                return country.States?.Select(c => c.StateName)
                .OrderBy(c => c)
                .ToList();
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Error get states (timeout)");
                return null;
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                _logger.LogError(ex, "Error get states (mongo exception)");

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error get states");
                return null;
            }
        }

        public async Task<List<string>> GetCities(string countryName)
        {
            try
            {
                Country country = await _mongoRepository.FindOneAsync(c => c.CountryName == countryName);
                return country
                    .Cities?.Select(c => c.CityName)
                    .OrderBy(c => c)
                    .ToList();
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Error get cities (timeout)");
                return null;
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                _logger.LogError(ex, "Error get cities (mongo)");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error get cities ");
                return null;
            }
        }

        public async Task<List<string>> GetCities(string countryName, string stateName, string districtName)
        {
            try
            {
                if (string.IsNullOrEmpty(stateName) || stateName == "--")
                {
                    Country country = await _mongoRepository.FindOneAsync(c => c.CountryName == countryName);
                    return country
                       .Cities?.Select(c => c.CityName)
                       .OrderBy(c => c)
                       .ToList();
                }
                else if(string.IsNullOrEmpty(districtName) || districtName == "--")
                {
                    Country country = await _mongoRepository.FindOneAsync(c => c.CountryName == countryName);
                    return country
                        .States?.Find(c => c.StateName == stateName)
                        .Cities?.Select(c => c.CityName)
                        .OrderBy(c => c)
                        .ToList();
                }
                else
                {
                    Country country = await _mongoRepository.FindOneAsync(c => c.CountryName == countryName);
                    return country
                        .States?.Find(c => c.StateName == stateName)
                        .Districts?.Find(c => c.DistrictName == districtName)
                        .Cities?.Select(c => c.CityName)
                        .OrderBy(c => c)
                        .ToList();
                }
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Error get cities (timeout)");
                return null;
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                _logger.LogError(ex, "Error get cities (mongo)");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error get cities");
                return null;
            }
        }

        public async Task<City> GetCity(string countryName, string stateName, string districtName, string cityName)
        {
            try
            {
                if (string.IsNullOrEmpty(stateName) && string.IsNullOrEmpty(districtName))
                {
                    Country country = await _mongoRepository.FindOneAsync(c => c.CountryName == countryName);
                    return country
                        .Cities?.Find(c => c.CityName == cityName);
                }
                else if(string.IsNullOrEmpty(districtName))
                {
                    Country country = await _mongoRepository.FindOneAsync(c => c.CountryName == countryName);
                    return country
                        .States?.Find(c => c.StateName == stateName)
                        .Cities?.Find(c => c.CityName == cityName);
                }
                else
                {
                    Country country = await _mongoRepository.FindOneAsync(c => c.CountryName == countryName);
                    return country
                        .States?.Find(c => c.StateName == stateName)
                        .Districts?.Find(c => c.DistrictName == districtName)
                        .Cities?.Find(c => c.CityName == cityName);
                }
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Error get city (timeout)");

                return null;
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                _logger.LogError(ex, "Error get city (mongo)");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error get city");
                return null;
            }
        }

        public bool ConvertToUtcFromCustomTimeZone(string timezone, DateTime datetime, out DateTime outdatetime)
        {
            outdatetime = datetime;
            try
            {
                DateTimeZone zone = DateTimeZoneProviders.Tzdb[timezone];
                var localtime = LocalDateTime.FromDateTime(datetime);
                var zonedtime = localtime.InZoneLeniently(zone);
                outdatetime = zonedtime.ToInstant().InZone(zone).ToDateTimeUtc();
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error in ConvertToUtcFromCustomTimeZone");
                return false;
            }
        }

        public async Task<Country> GetCountryInfo(string countryName)
        {
            try
            {
                Country country = await _mongoRepository.FindOneAsync(c => c.CountryName == countryName);
                country.States?.Sort((c, f) => c.StateName.CompareTo(f.StateName));
                country.Cities?.Sort((c, f) => c.CityName.CompareTo(f.CityName));
                country.States?.ForEach(c => {
                    c.Districts?.Sort((f, g) => f.DistrictName.CompareTo(g.DistrictName));
                    c.Cities?.Sort((f, g) => f.CityName.CompareTo(g.CityName));
                    c.Districts?.ForEach(x =>
                    {
                        x.Cities?.Sort((f, g) => f.CityName.CompareTo(g.CityName));
                    });
                });
                return country;
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Error get country info (timeout)");
                return null;
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                _logger.LogError(ex, "Error get country info (mongo)");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error get country info");
                return null;
            }
        }

        public async Task<List<string>> GetDistricts(string countryName, string stateName)
        {
            try
            {
                Country country = await _mongoRepository.FindOneAsync(c => c.CountryName == countryName);
                return country
                    .States?.Find(c => c.StateName == stateName)
                    .Districts.Select(c => c.DistrictName)
                    .ToList();
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Error get district (timeout)");
                return null;
            }
            catch (MongoDB.Driver.MongoException ex)
            {
                _logger.LogError(ex, "Error get district (mongo)");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error get district");
                return null;
            }
        }
    }
}
