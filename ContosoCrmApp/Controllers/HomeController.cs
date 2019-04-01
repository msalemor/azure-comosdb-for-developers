using ContosoCrm.Common.Models;
using ContosoCrm.DataAccess.Interfaces;
using ContosoCrmApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ContosoCrmApp.Controllers
{
    public class HomeController : Controller
    {
        readonly IDocumentDbHelper<Contact> Repository;
        public HomeController(IDocumentDbHelper<Contact> repo)
        {
            Repository = repo;
            Repository.Initialize("ContosoCrm", "Contacts");
        }

        public async Task<IActionResult> Index()
        {
            var result = await Repository.GetItemsAsync(c => 1 == 1);
            ViewBag.TotalRUs = result.Item1;
            return View(result.Item2);
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
