using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBExcample
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.Int32)]
        public int MyProperty { get; set; }
        public ProductDetal ProductDetal { get; set; }
    }

    public class ProductDetal
    {
        public string Description { get; set; }
    }
}
