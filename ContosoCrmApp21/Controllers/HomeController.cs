using ContosoCrm.Common21.Models;
using ContosoCrm.DataAccess21.Interfaces;
using ContosoCrmApp21;
using ContosoCrmApp21.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoCrmApp.Controllers
{
    public class HomeController : Controller
    {
        readonly IDocumentDbHelper<Contact> Repository;
        readonly IConfiguration Configuration;

        public HomeController(IDocumentDbHelper<Contact> repo, IConfiguration config, IDocumentDbHelper<Company> companyRepository)
        {
            Configuration = config;

            Repository = repo;            
            Repository.Initialize(Configuration[Constants.DatabaseId], Configuration[Constants.CollectionId], partitionKey: Configuration[Constants.CollectionPartionKey]);
        }

        public async Task<IActionResult> Index()
        {
            // Use Cosmos DB for session
            await HttpContext.Session.LoadAsync();
            var data = HttpContext.Session.GetString("data");
            if (data == null)
            {
                HttpContext.Session.SetString("data", "someValue");
            }

            var readCharge = CosmosCache.LastReadCharge;
            var lastWriteCharge = CosmosCache.LastWriteCharge;

            // Execute a cross partition query
            // Demo: compare getting all the properties vs not all the properties
            var result = await Repository.GetItemsAsync(c => true,
                c => new Contact { LastName = c.LastName, FirstName = c.FirstName, Email = c.Email, Phone = c.Phone, Company = c.Company, ContactType= c.ContactType });
            //var result = await Repository.GetItemsAsync(c => true);
            ViewBag.TotalRUs = result.Item1;
            ViewBag.ReadEndpoint = result.Item2;
            ViewBag.WriteEndpoint = result.Item3;
            ViewBag.ConsistencyLevel = result.Item4;
            return View(result.Item5.ToList().OrderBy(c => c.LastName));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
