using MongoDB.Driver;

namespace MongoDBExcample
{
    public class Repository
    {
        private readonly IMongoCollection<Category> catgDb;
        private readonly IMongoCollection<Product> prDb;


        public Repository(IMongoDatabase mongoDatabase)
        {
            var c = new MongoClient(mongoDatabase.ConnectionString);
            var db = c.GetDatabase(mongoDatabase.DatabaseName);
            catgDb=db.GetCollection<Category>(mongoDatabase.CategoryCollectionName);
            prDb=db.GetCollection<Product>(mongoDatabase.ProductCollectionName);
        }

        public void AddCategory(Category c)
        {
            catgDb.InsertOne(c);
        }

        public Category getCategory(string id)
        {
           return catgDb.Find(m=>m.Id==id)?.FirstOrDefault();
        }

        public void AddProduct(Product c)
        {
            prDb.InsertOne(c);
        }

        public Product getPr(string id)
        {
            return prDb.Find(m => m.Id == id)?.FirstOrDefault();
        }
    }
}
