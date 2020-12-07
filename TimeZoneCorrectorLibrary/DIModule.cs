using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using TimeZoneCorrectorLibrary.Abstraction;
using TimeZoneCorrectorLibrary.Repository;

namespace TimeZoneCorrectorLibrary
{
    public class DIModule : Module 
    {
        public string ConnectionStringConfig { get; set; }
        public string DatabseNameConfig { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                return new MongoDbSettings() 
                {
                    ConnectionString = ConnectionStringConfig,
                    DatabaseName = DatabseNameConfig
                };
            }).As<IMongoDbSettings>();

            builder.RegisterGeneric(typeof(MongoRepository<>))
                .As(typeof(IMongoRepository<>))
                .SingleInstance();
            builder.RegisterType<TimeZoneCorrector>().As<ITimeZoneCorrector>();
        }
    }
}
