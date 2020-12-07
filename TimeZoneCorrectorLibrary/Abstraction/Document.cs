using MongoDB.Bson;
using System;

namespace TimeZoneCorrectorLibrary.Abstraction
{
    public abstract class Document : IDocument
    {
        public ObjectId Id { get; set; }

        public DateTime CreatedAt => Id.CreationTime;
    }
}