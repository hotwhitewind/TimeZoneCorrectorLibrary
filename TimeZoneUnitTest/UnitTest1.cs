using Autofac;
using ConvertGeoNamesDBToMongoDB.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using TimeZoneCorrectorLibrary;
using TimeZoneCorrectorLibrary.Abstraction;

namespace TimeZoneUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        private IContainer container;
        private ITimeZoneCorrector _timeZoneCorrector;

        [TestMethod]
        public void GetAllCountries()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new DIModule()
            {
                ConnectionStringConfig = "mongodb://localhost:27017",
                DatabseNameConfig = "atlas"
            });
            this.container = containerBuilder.Build();
            _timeZoneCorrector = container.Resolve<ITimeZoneCorrector>();
            Assert.IsNotNull(_timeZoneCorrector);
            var countries = _timeZoneCorrector.GetCountries();
            Assert.IsNotNull(countries);
            Assert.IsTrue(countries.Count != 0);
        }

        [TestMethod]
        public async Task GetStateByCountry()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new DIModule()
            {
                ConnectionStringConfig = "mongodb://localhost:27017",
                DatabseNameConfig = "atlas"
            });
            this.container = containerBuilder.Build();
            _timeZoneCorrector = container.Resolve<ITimeZoneCorrector>();
            Assert.IsNotNull(_timeZoneCorrector);
            var states = await _timeZoneCorrector.GetStates("Russia");
            Assert.IsNotNull(states);
            Assert.IsTrue(states.Count != 0);
        }

        [TestMethod]
        public async Task GetCitiesByCountryAndState()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new DIModule()
            {
                ConnectionStringConfig = "mongodb://localhost:27017",
                DatabseNameConfig = "atlas"
            });
            this.container = containerBuilder.Build();
            _timeZoneCorrector = container.Resolve<ITimeZoneCorrector>();
            Assert.IsNotNull(_timeZoneCorrector);
            var cities = await _timeZoneCorrector.GetCities("Russia", "Moscow", null);
            Assert.IsNotNull(cities);
            Assert.IsTrue(cities.Count != 0);
        }

        [TestMethod]
        public async Task GetCityByCountryAndState()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new DIModule()
            {
                ConnectionStringConfig = "mongodb://localhost:27017",
                DatabseNameConfig = "atlas"
            });
            this.container = containerBuilder.Build();
            _timeZoneCorrector = container.Resolve<ITimeZoneCorrector>();
            Assert.IsNotNull(_timeZoneCorrector);
            var city = await _timeZoneCorrector.GetCity("Russia", "Moscow", null, "Moscow");
            Assert.IsNotNull(city);
            Assert.IsTrue(city.CityName == "Moscow");

            var city1 = await _timeZoneCorrector.GetCity("Ukraine", "Sumy", null, "Sumy");
            Assert.IsNotNull(city1);
            Assert.IsTrue(city1.CityName == "Sumy");
        }

        [TestMethod]
        public void TestDBConnection()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new DIModule()
            {
                ConnectionStringConfig = "mongodb://localhost:27017",
                DatabseNameConfig = "atlas"
            });
            this.container = containerBuilder.Build();
            _timeZoneCorrector = container.Resolve<ITimeZoneCorrector>();
            var find = _timeZoneCorrector.ConnectionToDBTest();
            Assert.IsNotNull(find);
            Assert.AreEqual(find.CountryName, "Andorra");
        }

        [TestMethod]
        public async Task TestTimeZoneCorrector()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new DIModule()
            {
                ConnectionStringConfig = "mongodb://localhost:27017",
                DatabseNameConfig = "atlas"
            });
            this.container = containerBuilder.Build();
            _timeZoneCorrector = container.Resolve<ITimeZoneCorrector>();
            Assert.IsNotNull(_timeZoneCorrector);
            var city1 = await _timeZoneCorrector.GetCity("Ukraine", "Sumy", null, "Sumy");
            Assert.IsNotNull(city1);
            Assert.IsTrue(city1.CityName == "Sumy");

            Assert.AreEqual(city1.TimeZone, "Europe/Kiev");
            _timeZoneCorrector.ConvertToUtcFromCustomTimeZone(city1.TimeZone, 
                new System.DateTime(1979, 03, 12, 3, 19, 00), out DateTime t1);
            _timeZoneCorrector.ConvertToUtcFromCustomTimeZone(city1.TimeZone, 
                new System.DateTime(2004, 03, 12, 3, 19, 00), out DateTime t2);
            Assert.AreEqual(t1, new System.DateTime(1979, 03, 12, 0, 19, 0));
            Assert.AreEqual(t2, new System.DateTime(2004, 03, 12, 1, 19, 0));

            var city2 = await _timeZoneCorrector.GetCity("Uzbekistan", "Toshkent Shahri", null, "Tashkent");
            Assert.IsNotNull(city2);
            Assert.IsTrue(city2.CityName == "Tashkent");

            Assert.AreEqual(city2.TimeZone, "Asia/Tashkent");
            _timeZoneCorrector.ConvertToUtcFromCustomTimeZone(city2.TimeZone, 
                new System.DateTime(1979, 03, 12, 8, 19, 00), out DateTime t3);
            _timeZoneCorrector.ConvertToUtcFromCustomTimeZone(city2.TimeZone, 
                new System.DateTime(2004, 03, 12, 8, 19, 00), out DateTime t4);
            Assert.AreEqual(t3, new System.DateTime(1979, 03, 12, 2, 19, 0));
            Assert.AreEqual(t4, new System.DateTime(2004, 03, 12, 3, 19, 0));

        }
    }
}
