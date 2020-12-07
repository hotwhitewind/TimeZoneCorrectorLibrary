using Autofac;
using ConvertGeoNamesDBToMongoDB.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeZoneCorrectorLibrary;
using TimeZoneCorrectorLibrary.Abstraction;

namespace TimeZoneUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        private IContainer container;
        private IMongoRepository<Country> _mongoRepository;

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
            _mongoRepository = container.Resolve<IMongoRepository<Country>>();
            Assert.IsNotNull(_mongoRepository);
            TimeZoneCorrector timeZoneCorrector = new TimeZoneCorrector(_mongoRepository);
            var countries = timeZoneCorrector.GetAllCountries();
            Assert.IsTrue(countries.Count != 0);
        }

        [TestMethod]
        public void GetStateByCountry()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new DIModule()
            {
                ConnectionStringConfig = "mongodb://localhost:27017",
                DatabseNameConfig = "atlas"
            });
            this.container = containerBuilder.Build();
            _mongoRepository = container.Resolve<IMongoRepository<Country>>();
            Assert.IsNotNull(_mongoRepository);
            TimeZoneCorrector timeZoneCorrector = new TimeZoneCorrector(_mongoRepository);
            var states = timeZoneCorrector.GetAllStateByCountry("Russia");
            Assert.IsNotNull(states);
            Assert.IsTrue(states.Count != 0);
        }

        [TestMethod]
        public void GetCitiesByCountryAndState()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new DIModule()
            {
                ConnectionStringConfig = "mongodb://localhost:27017",
                DatabseNameConfig = "atlas"
            });
            this.container = containerBuilder.Build();
            _mongoRepository = container.Resolve<IMongoRepository<Country>>();
            Assert.IsNotNull(_mongoRepository);
            TimeZoneCorrector timeZoneCorrector = new TimeZoneCorrector(_mongoRepository);
            var cities = timeZoneCorrector.GetAllCitiesByState("Russia", "Moscow");
            Assert.IsNotNull(cities);
            Assert.IsTrue(cities.Count != 0);
        }

        [TestMethod]
        public void GetCityByCountryAndState()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new DIModule()
            {
                ConnectionStringConfig = "mongodb://localhost:27017",
                DatabseNameConfig = "atlas"
            });
            this.container = containerBuilder.Build();
            _mongoRepository = container.Resolve<IMongoRepository<Country>>();
            Assert.IsNotNull(_mongoRepository);
            TimeZoneCorrector timeZoneCorrector = new TimeZoneCorrector(_mongoRepository);
            var city = timeZoneCorrector.GetCityByCountryAndState("Russia", "Moscow", "Moscow");
            Assert.IsNotNull(city);
            Assert.IsTrue(city.CityName == "Moscow");

            var city1 = timeZoneCorrector.GetCityByCountryAndState("Ukraine", "Sumy", "Sumy");
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
            _mongoRepository = container.Resolve<IMongoRepository<Country>>();
            Assert.IsNotNull(_mongoRepository);
            TimeZoneCorrector timeZoneCorrector = new TimeZoneCorrector(_mongoRepository);
            var find = timeZoneCorrector.ConnectionToDBTest();
            Assert.IsNotNull(find);
            Assert.AreEqual(find.CountryName, "Andorra");
        }

        [TestMethod]
        public void TestTimeZoneCorrector()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new DIModule()
            {
                ConnectionStringConfig = "mongodb://localhost:27017",
                DatabseNameConfig = "atlas"
            });
            this.container = containerBuilder.Build();
            _mongoRepository = container.Resolve<IMongoRepository<Country>>();
            Assert.IsNotNull(_mongoRepository);
            TimeZoneCorrector timeZoneCorrector = new TimeZoneCorrector(_mongoRepository);
            var city1 = timeZoneCorrector.GetCityByCountryAndState("Ukraine", "Sumy", "Sumy");
            Assert.IsNotNull(city1);
            Assert.IsTrue(city1.CityName == "Sumy");

            Assert.AreEqual(city1.TimeZone, "Europe/Kiev");
            var t1 = timeZoneCorrector.ConvertToUtcFromCustomTimeZone(city1.TimeZone, new System.DateTime(1979, 03, 12, 3, 19, 00));
            var t2 = timeZoneCorrector.ConvertToUtcFromCustomTimeZone(city1.TimeZone, new System.DateTime(2004, 03, 12, 3, 19, 00));
            Assert.AreEqual(t1, new System.DateTime(1979, 03, 12, 0, 19, 0));
            Assert.AreEqual(t2, new System.DateTime(2004, 03, 12, 1, 19, 0));

            var city2 = timeZoneCorrector.GetCityByCountryAndState("Uzbekistan", "Toshkent Shahri", "Tashkent");
            Assert.IsNotNull(city2);
            Assert.IsTrue(city2.CityName == "Tashkent");

            Assert.AreEqual(city2.TimeZone, "Asia/Tashkent");
            var t3 = timeZoneCorrector.ConvertToUtcFromCustomTimeZone(city2.TimeZone, new System.DateTime(1979, 03, 12, 8, 19, 00));
            var t4 = timeZoneCorrector.ConvertToUtcFromCustomTimeZone(city2.TimeZone, new System.DateTime(2004, 03, 12, 8, 19, 00));
            Assert.AreEqual(t3, new System.DateTime(1979, 03, 12, 2, 19, 0));
            Assert.AreEqual(t4, new System.DateTime(2004, 03, 12, 3, 19, 0));

        }
    }
}
