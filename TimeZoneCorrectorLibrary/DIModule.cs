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
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new MongoDbSettings() 
            { ConnectionString = "mongodb://localhost:27017",DatabaseName = "atlas"})
                .As<IMongoDbSettings>();
            builder.RegisterGeneric(typeof(MongoRepository<>))
                .As(typeof(IMongoRepository<>))
                .SingleInstance();
        }
    }
}
