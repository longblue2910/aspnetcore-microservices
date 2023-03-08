using Contracts.Domains.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Inventory.Product.API.Entities.Abstraction
{
    public abstract class MongoEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public virtual string  Id { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("createdDate")]
        public DateTime CreatedDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement("lastModifiedDate")]
        public DateTime? LastModifiedDate { get; set; }
    }
}
