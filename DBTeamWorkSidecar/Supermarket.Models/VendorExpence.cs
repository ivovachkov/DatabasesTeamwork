using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Supermarket.Models
{
    public class VendorExpence
    {
        [BsonIgnore]
        public int Id { get; set; }

        [NotMapped] // Used for not mapping the property to SQL Server
        [BsonId] // Used for MongoDB mapping
        public ObjectId MongoId { get; set; }

        [BsonIgnore]
        public int VendorId { get; set; }

        [NotMapped]
        public string MongoVendor { get; set; }

        public string Month { get; set; }

        public decimal Ammount { get; set; }
    }
}
