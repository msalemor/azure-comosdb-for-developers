using ContosoCrm.Common21.Models;
using ContosoCrm.DataAccess21.Interfaces;
using ContosoCrmApp21;
using ContosoCrmApp21.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Cosmos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoCrmApp.Controllers
{
    public class HomeController : Controller
    {
        private const string CustomerTypes = "customerTypes";
        readonly IDocumentDbHelper<Contact> Repository;
        readonly IDocumentDbHelper<Lookup> LookupRepository;
        readonly IConfiguration Configuration;
        IEnumerable<Lookup> customerTypes;

        public HomeController(IDocumentDbHelper<Contact> repo, IDocumentDbHelper<Lookup> lookup, IConfiguration config, IDocumentDbHelper<Company> companyRepository)
        {
            Configuration = config;
            Repository = repo;
            LookupRepository = lookup;
            Repository.Initialize(Configuration[Constants.DatabaseId], Configuration[Constants.CollectionId], partitionKey: Configuration[Constants.CollectionPartionKey]);
            LookupRepository.Initialize(Configuration[Constants.DatabaseId], "Lookup", partitionKey: "/id");
        }
        private async void LoadLookupAsync()
        {
            var items = await LookupRepository.GetItemsAsync(c=>true);
            if (items.Item5.Count() == 0)
            {
                await LookupRepository.CreateItemAsync(new Lookup { Area = "Type", Key = "Contact", Value = "Contact" });
                await LookupRepository.CreateItemAsync(new Lookup { Area = "Type", Key = "Customer", Value = "Customer" });
                await LookupRepository.CreateItemAsync(new Lookup { Area = "Type", Key = "Lead", Value = "Lead" });
            }
        }

        public async Task<IActionResult> Index()
        {
            // Create the lookup data once
            LoadLookupAsync();

            // Load the session data
            await HttpContext.Session.LoadAsync();

            // Try to get the data from sessions
            var json = HttpContext.Session.GetString(CustomerTypes);
            if (json == null)
            {
                // If it does not exists add it
                customerTypes = (await LookupRepository.GetItemsAsync(c => true)).Item5;
                HttpContext.Session.SetString(CustomerTypes, JsonConvert.SerializeObject(customerTypes));
                ViewData["CustomerTypes"] = null;
            }
            else
            {
                // Deserialize the json back to a .Net object from cache
                customerTypes = JsonConvert.DeserializeObject<IEnumerable<Lookup>>(json);
                ViewData["CustomerTypes"] = customerTypes;
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
