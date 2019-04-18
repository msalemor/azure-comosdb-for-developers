using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace ContosoCrmApp21.Controllers
{
    public class ErrorController : Controller
    {
        private ILogger<ErrorController> logger;
        private TelemetryClient telemetryClient;

        public ErrorController(ILogger<ErrorController> logger, TelemetryClient telemetryClient)
        {
            this.logger = logger;
            this.telemetryClient = telemetryClient;
        }

        // GET: Error
        public ActionResult Index()
        {
           
            logger.LogInformation("Activating action Error/Index");
            try
            {
                throw new ApplicationException();
            }
            catch (Exception ex)
            {
                var telemetry = new ExceptionTelemetry(ex);
                telemetry.Properties.Add("Action", "Error/Index");
                telemetryClient.TrackException(telemetry);
            }
            finally
            {
                telemetryClient.Flush();
            }
            return View();            
        }

        // GET: Error/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Error/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Error/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Error/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Error/Edit/5
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

        // GET: Error/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Error/Delete/5
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