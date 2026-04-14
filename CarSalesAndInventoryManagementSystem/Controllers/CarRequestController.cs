using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CarSalesAndInventoryManagementSystem.Context;
using CarSalesAndInventoryManagementSystem.Models;

namespace CarSalesAndInventoryManagementSystem.Controllers
{

    public class CarRequestController : Controller
    {

        private readonly CarSales_Inventory _dbContext;

        public CarRequestController()
        {

            _dbContext = new CarSales_Inventory();

        }
      protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserID"] == null)
            {
                filterContext.Result = RedirectToAction("Login", "User");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
        // GET: /CarRequest/MyCarRequests
        public ActionResult MyCarRequests()
        {
            var userId = (int)Session["UserID"];
            var myRequests = _dbContext.CarRequests
                .Where(r => r.UserID == userId)
                .OrderByDescending(r => r.RequestDate)
                .ToList();
            return View(myRequests);
        }
        // GET: /CarRequest/Edit/{id}
       public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarRequest carRequest = _dbContext.CarRequests.Find(id);
            if (carRequest == null)
            {
                return HttpNotFound();
            }
            //SECURITY CHECK
            var userId = (int)Session["UserID"];
            if (carRequest.UserID != userId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            }
            return View(carRequest);
        }

        // POST: /CarRequest/Edit/{id}
        [HttpPost]

        public ActionResult Edit(CarRequest carRequest)
        {
         
            var userId = (int)Session["UserID"];
            var originalRequest = _dbContext.CarRequests.AsNoTracking().FirstOrDefault(r => r.CarRequestID == carRequest.CarRequestID);
            if (originalRequest == null || originalRequest.UserID != userId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            // allow edit if the status is "New"

            if (originalRequest.Status != "New")
            {
                ModelState.AddModelError("", "This request can no longer be edited as it is already being processed.");
                return View(carRequest);
            }

            if (ModelState.IsValid)

            {
    
                var entry = _dbContext.Entry(carRequest);
                entry.State = EntityState.Modified;
                entry.Property(e => e.UserID).IsModified = false;
                entry.Property(e => e.RequestDate).IsModified = false;
                entry.Property(e => e.Status).IsModified = false;
                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Your request has been updated successfully.";
                return RedirectToAction("MyCarRequests");

            }
            return View(carRequest);

        }

        // GET: /CarRequest/Delete/{id}

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarRequest carRequest = _dbContext.CarRequests.Find(id);
            if (carRequest == null)
            {
                return HttpNotFound();
            }
         
            var userId = (int)Session["UserID"];
            if (carRequest.UserID != userId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(carRequest);
        }

        // POST: /CarRequest/Delete/{id}

        [HttpPost, ActionName("Delete")]

        public ActionResult DeleteConfirmed(int id)
        {

            CarRequest carRequest = _dbContext.CarRequests.Find(id);
    

            var userId = (int)Session["UserID"];
            if (carRequest == null || carRequest.UserID != userId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            _dbContext.CarRequests.Remove(carRequest);
            _dbContext.SaveChanges();
            TempData["SuccessMessage"] = "Your request has been successfully deleted.";
            return RedirectToAction("MyCarRequests");

        }

    }

}

