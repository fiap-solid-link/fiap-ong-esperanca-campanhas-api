using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace Esperanca.Campanha.Infrastructure.Transparencia.Mongo
{
    [ExcludeFromCodeCoverage]
    internal abstract class BaseDocument
    {
        [BsonId]
        public ObjectId Id { get; init; }
    }
}
