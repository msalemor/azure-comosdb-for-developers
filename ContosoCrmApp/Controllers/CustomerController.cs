using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoCrm.Common.Models;
using ContosoCrm.DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

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
            Repository.Initialize(Configuration["DatabaseId"], Configuration["CollectionId"]);
        }

        public async Task<IActionResult> Index()
        {
            var result = await Repository.GetItemsAsync(c => c.ContactType == DefaultContactType,
                c => new Contact
                {
                    Id = c.Id,
                    Company = c.Company,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Phone = c.Phone,
                    Email = c.Email
                });
            ViewBag.Area = "Customers";
            ViewBag.TotalRUs = result.Item1;
            return View(result.Item2);
        }

        // GET: Lead/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var result = await Repository.GetItemAsync(id);
            ViewBag.Area = "Customer";
            ViewBag.TotalRUs = result.Item1;
            return View(result.Item2);

        }

        // GET: Lead/Create
        public ActionResult Create()
        {
            ViewBag.Area = "Customer";
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
                ViewBag.Area = "Customer";
                return View(contact);
            }
        }

        // GET: Lead/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Lead/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Lead/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Lead/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}