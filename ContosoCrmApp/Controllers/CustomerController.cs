using ContosoCrm.Common.Models;
using ContosoCrm.DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoCrmApp.Controllers
{
    public class CustomerController : Controller
    {
        readonly IDocumentDbHelper<Contact> Repository;
        readonly IConfiguration Configuration;
        readonly ContactType DefaultContactType = ContactType.Customer;

        public CustomerController(IDocumentDbHelper<Contact> repo, IConfiguration config)
        {
            Repository = repo;
            Configuration = config;
            Repository.Initialize(Configuration[Constants.DatabaseId], Configuration[Constants.CollectionId], partitionKey: Configuration[Constants.CollectionPartionKey]);
        }

        public async Task<IActionResult> Index()
        {
            var result = await Repository.GetItemsAsync
                (
                    c => c.ContactType == DefaultContactType,
                    c => new Contact
                    {
                        Id = c.Id,
                        Company = c.Company,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Phone = c.Phone,
                        Email = c.Email
                    },
                    DefaultContactType.ToString()
                );
            ViewBag.Area = Constants.CustomerList;
            ViewBag.TotalRUs = result.Item1;
            ViewBag.ReadEndpoint = result.Item2;
            ViewBag.WriteEndpoint = result.Item3;
            ViewBag.ConsistencyLevel = result.Item4;
            return View(result.Item5.ToList().OrderBy(c => c.LastName));
        }

        // GET: Lead/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var result = await Repository.GetItemAsync(id, DefaultContactType.ToString());
            ViewBag.Area = Constants.CustomerArea;
            ViewBag.TotalRUs = result.Item1;
            return View(result.Item2);

        }

        // GET: Lead/Create
        public ActionResult Create()
        {
            ViewBag.Area = Constants.CustomerArea;
            return View(new Contact());
        }

        // POST: Lead/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contact contact)
        {
            try
            {
                // Note: Not necessary. Let CosmosDB set the ID
                contact.Id = null;

                // set the contact type
                contact.ContactType = DefaultContactType;

                // TODO: Add insert logic here
                if (!ModelState.IsValid)
                {
                    throw new Exception();
                }
                Repository.CreateItemAsync(contact);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.Area = Constants.CustomerArea;
                return View(contact);
            }
        }

        // GET: Lead/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var result = await Repository.GetItemAsync(id, DefaultContactType.ToString());
            ViewBag.Area = Constants.CustomerArea;
            ViewBag.TotalRUs = result.Item1;
            return View(result.Item2);
        }

        // POST: Lead/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Contact contact)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    await Repository.UpdateItemAsync(contact.Id, contact);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                ViewBag.Error = "Unable to edit record.";
                ViewBag.Area = Constants.CustomerArea;
                return View(contact);
            }
        }

        // GET: Lead/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var result = await Repository.GetItemAsync(id, DefaultContactType.ToString());
            ViewBag.Area = Constants.CustomerArea;
            ViewBag.TotalRUs = result.Item1;
            return View(result.Item2);
        }

        // POST: Lead/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Contact contact)
        {
            try
            {
                if (!string.IsNullOrEmpty(contact.Id))
                {
                    // TODO: Add delete logic here
                    await Repository.DeleteItemAsync(contact.Id, contact.ContactType.ToString());
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                ViewBag.Area = Constants.CustomerArea;
                ViewBag.Error = "Unable to delete record.";
                return View(contact);
            }
        }
    }
}