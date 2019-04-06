using ContosoCrm.Common21.Models;
using ContosoCrm.DataAccess21.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoCrmSPA.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        readonly IDocumentDbHelper<Contact> Repository;
        readonly IConfiguration Configuration;
        readonly ILogger Logger;

        public ContactsController(IDocumentDbHelper<Contact> repo, IConfiguration config, ILogger<ContactsController> logger)
        {
            Configuration = config;
            Logger = logger;
            Repository = repo;
            Repository.Initialize(Configuration[Constants.DatabaseId], Configuration[Constants.CollectionId], partitionKey: Configuration[Constants.CollectionPartionKey]);
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<Tuple<double, string, string, string, IEnumerable<Contact>>>> Get()
        {

            var result = await Repository.GetItemsAsync(c => true,
                c => new Contact { LastName = c.LastName, FirstName = c.FirstName, Email = c.Email, Phone = c.Phone, Company = c.Company, ContactType = c.ContactType });
            return new Tuple<double, string, string, string, IEnumerable<Contact>>(result.Item1, result.Item2, result.Item3, result.Item4, result.Item5.OrderBy(c => c.LastName));
        }

        // GET api/values/5
        [HttpGet("{contactType}")]
        public async Task<ActionResult<Tuple<double, string, string, string, IEnumerable<Contact>>>> Get(string contactType)
        {
            var result = await Repository.GetItemsAsync
                (
                    c => c.ContactType == (ContactType)Enum.Parse(typeof(ContactType), contactType, true),
                    c => new Contact
                    {
                        Id = c.Id,
                        Company = c.Company,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Phone = c.Phone,
                        Email = c.Email
                    },
                    contactType.ToString()
                );
            return new Tuple<double, string, string, string, IEnumerable<Contact>>(result.Item1, result.Item2, result.Item3, result.Item4, result.Item5.OrderBy(c => c.LastName));
        }

        [HttpGet("{id}/{contactType}")]
        public async Task<ActionResult<Tuple<double, string, string, string, IEnumerable<Contact>>>> Get(string id, string contactType)
        {
            var result = await Repository.GetItemsAsync
                (
                    c => c.ContactType == (ContactType)Enum.Parse(typeof(ContactType), contactType, true) && c.Id==id,
                    null,
                    contactType.ToString()
                );
            return new Tuple<double, string, string, string, IEnumerable<Contact>>(result.Item1, result.Item2, result.Item3, result.Item4, result.Item5.OrderBy(c => c.LastName));
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Contact contact)
        {
            try
            {
                // Note: Not necessary. Let CosmosDB set the ID
                contact.Id = null;

                // TODO: Add insert logic here
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                await Repository.CreateItemAsync(contact);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                throw e;
            }
            return Created(string.Empty, contact);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Contact contact)
        {
            try
            {
                // TODO: Add insert logic here
                if (!ModelState.IsValid || string.IsNullOrEmpty(contact.Id))
                {
                    return BadRequest("Not a valid model");
                }
                await Repository.UpdateItemAsync(contact.Id, contact);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                throw e;
            }
            return Ok();
        }

        // DELETE api/values/5
        [HttpDelete("{id}/{contactType}")]
        public async Task<IActionResult> Delete(string id, string contactType)
        {
            try
            {
                var result = await Repository.GetItemAsync(id, contactType);
                if (result is null || result.Item2 is null)
                {
                    return BadRequest();
                }
                await Repository.DeleteItemAsync(id, contactType);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                throw e;
            }
            return Ok();
        }
    }
}
