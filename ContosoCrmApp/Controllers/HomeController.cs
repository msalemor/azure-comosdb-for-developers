using ContosoCrm.Common.Models;
using ContosoCrm.DataAccess.Interfaces;
using ContosoCrmApp.Models;
using Microsoft.AspNetCore.Mvc;
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

        public HomeController(IDocumentDbHelper<Contact> repo, IConfiguration config)
        {
            Repository = repo;
            Configuration = config;
            Repository.Initialize(Configuration[Constants.DatabaseId], Configuration[Constants.CollectionId]);
        }

        public async Task<IActionResult> Index()
        {
            // Execute a cross partition query
            var result = await Repository.GetItemsAsync(c => true);
            var list = result.Item4.ToList().OrderBy(c => c.LastName);
            ViewBag.TotalRUs = result.Item1;
            ViewBag.ReadEndpoint = result.Item2;
            ViewBag.WriteEndpoint = result.Item3;
            return View(list);
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
