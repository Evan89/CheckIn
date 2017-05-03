using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CheckIn.Models;
using System.Net.Mail;

namespace CheckIn.Controllers
{
    public class UserCheckInsController : Controller
    {
        private CheckInContext db = new CheckInContext();

        // GET: UserCheckIns
        public ActionResult Index()
        {
            return View(db.UserCheckIns.ToList());
        }

        public ActionResult PostPage()
        {
            return View(db.UserCheckIns.ToList());
        }

        // GET: UserCheckIns/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserCheckIn userCheckIn = db.UserCheckIns.Find(id);
            if (userCheckIn == null)
            {
                return HttpNotFound();
            }
            return View(userCheckIn);
        }

        // GET: UserCheckIns/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserCheckIns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,firstName,lastName,telNum,email,contactEmail,location,returnTime,message")] UserCheckIn userCheckIn)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.UserCheckIns.Add(userCheckIn);
                    db.SaveChanges();
                    sendEmail(userCheckIn);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("Key already exists.", e);
                }
                
                return RedirectToAction("PostPage");
            }

            return View(userCheckIn);
        }
        
        private void sendEmail(UserCheckIn userCheckIn)
        {
           
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("noreply@checkinweb.ca");
            mailMessage.To.Add(userCheckIn.email);
            mailMessage.Subject = "You've been checked in!";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = "Hello " + userCheckIn.firstName + "! <br/> <br/>" +
                "You've checked into SafetyLineLoneWorker's free check-in web app. Here are your check-in details.<br/><br/>" +
                "<table>" +
                    "<tr>" +
                        "<td>Full Name</td>" +
                        "<td>" + userCheckIn.firstName + " "+ userCheckIn.lastName+"</td>" +
                    "</tr> " +
                    "<tr>" +
                        "<td>Emergency contact</td>" +
                        "<td>" + userCheckIn.contactEmail + "</td>" +
                    "</tr> " +
                    "<tr>" +
                        "<td>Return time</td>" +
                        "<td>" + userCheckIn.returnTime + "</td>" +
                    "</tr> " +
                "</table> ";
            //mailMessage.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;
            //to authenticate we set the username and password properites on the SmtpClient
            smtp.Credentials = new NetworkCredential("checkinwebapp@gmail.com", "tsunamisolutions");//no need to mention here?

            smtp.Send(mailMessage);

        }
        

        // GET: UserCheckIns/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserCheckIn userCheckIn = db.UserCheckIns.Find(id);
            if (userCheckIn == null)
            {
                return HttpNotFound();
            }
            return View(userCheckIn);
        }

        // POST: UserCheckIns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,firstName,lastName,telNum,email,contactEmail,location,returnTime,message")] UserCheckIn userCheckIn)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userCheckIn).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userCheckIn);
        }

        // GET: UserCheckIns/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserCheckIn userCheckIn = db.UserCheckIns.Find(id);
            if (userCheckIn == null)
            {
                return HttpNotFound();
            }
            return View(userCheckIn);
        }

        // POST: UserCheckIns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserCheckIn userCheckIn = db.UserCheckIns.Find(id);
            db.UserCheckIns.Remove(userCheckIn);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
