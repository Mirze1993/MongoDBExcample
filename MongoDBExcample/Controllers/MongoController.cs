using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MongoDBExcample.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class MongoController : ControllerBase
    {
        Repository repository;

        public MongoController(IMongoDatabase mongoDatabase)
        {
            repository=new Repository(mongoDatabase);
        }

        [HttpGet]
        public Category GetById(string id)
        {
            return repository.getCategory(id);   
        }

        [HttpPost]
        public void AddCategory(Category category)
        {
            repository.AddCategory(category);
        }

        [HttpGet]
        public Product GetByIdPr(string id)
        {
            return repository.getPr(id);
        }

        [HttpPost]
        public void Addproduct(Product p)
        {
            repository.AddProduct(p);
        }
    }
}
