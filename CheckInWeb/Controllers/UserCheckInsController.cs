using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CheckInCommon;
using System.Net.Mail;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.IO;
using System.Globalization;

namespace CheckInWeb.Controllers
{
    public class UserCheckInsController : Controller
    {
        public const string URL = "http://safetycheckin.cloudapp.net/UserCheckIns/Delete/";

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

        // GET: UserCheckIns/Create
        public ActionResult Create()
        {
            return View();
        }

        // GET: UserCheckIns/Create
        public ActionResult French()
        {
            return View();
        }

        // POST: UserCheckIns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,firstName,lastName,telNum,email,contactEmail1,contactEmail2,contactEmail3,contactEmail4,location,message,subscribe")] UserCheckIn userCheckIn, string offsetName, string inputTime)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string[] splitData = offsetName.Split('-');

                    // DateTime localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);
                    DateTime userDateTime = new DateTime(int.Parse(splitData[0]),int.Parse( splitData[1]), int.Parse(splitData[2]), int.Parse(splitData[3]), int.Parse(splitData[4]),0);

                    DateTime returnTime = DateTime.Parse(inputTime);

                    // roll-over to the next day if the return time is earlier than the current time
                    if (userDateTime.CompareTo(returnTime) > 0)
                    {
                        returnTime = returnTime.AddDays(1);
                    }

                    //DateTime inputTime = new DateTime()
                    int offset = int.Parse(splitData[5]);

                    userCheckIn.returnTime = returnTime.AddMinutes(offset);



                    // convert local time to UTC
                    // userCheckIn.returnTime = TimeZoneInfo.ConvertTimeToUtc(userCheckIn.returnTime, TimeZoneInfo.Local);
                    userCheckIn.inputTime = returnTime.ToString();
                    userCheckIn.secString = GetSecurityString();
                    db.UserCheckIns.Add(userCheckIn);
                    db.SaveChanges();

                    if(userCheckIn.subscribe)
                    {
                        subscribe(userCheckIn);
                    }

                    sendEmail(userCheckIn);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("Key already exists.", e);
                    sendEmail(e.ToString());
                }
                
                return RedirectToAction("PostPage");
            }

            return View(userCheckIn);
        }


        private void sendEmail(string s)
        {

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("noreply@checkinweb.ca");
            mailMessage.To.Add("evan.c1995@gmail.com");
            mailMessage.Subject = "You've been checked in!";
            mailMessage.IsBodyHtml = true;

            


            mailMessage.Body = s;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;
            //to authenticate we set the username and password properites on the SmtpClient
            smtp.Credentials = new NetworkCredential("checkinwebapp@gmail.com", "tsunamisolutions");//no need to mention here?

            smtp.Send(mailMessage);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult French([Bind(Include = "ID,firstName,lastName,telNum,email,contactEmail1,contactEmail2,contactEmail3,contactEmail4,location,message,subscribe")] UserCheckIn userCheckIn)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DateTime localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);

                    // roll-over to the next day if the return time is earlier than the current time
                    if (userCheckIn.returnTime.CompareTo(localNow) < 0)
                    {
                        userCheckIn.returnTime = userCheckIn.returnTime.AddDays(1);
                    }

                    // convert local time to UTC
                    userCheckIn.returnTime = TimeZoneInfo.ConvertTimeToUtc(userCheckIn.returnTime, TimeZoneInfo.Local);

                    userCheckIn.secString = GetSecurityString();
                    db.UserCheckIns.Add(userCheckIn);
                    db.SaveChanges();

                    if (userCheckIn.subscribe)
                    {
                        subscribe(userCheckIn);
                    }

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

        // Subscribes the user to our newsletter
        private void subscribe(UserCheckIn userCheckIn)
        {   

            string url = "http://go.pardot.com/l/82512/2017-05-08/d26jpz";
            
            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();

            // Create a new NameValueCollection instance to hold some custom parameters to be posted to the URL.
            NameValueCollection myNameValueCollection = new NameValueCollection();

            //First Name and yes they gave us those values to use
            myNameValueCollection.Add("82512_109811pi_82512_109811", userCheckIn.firstName);

            //Last Name
            myNameValueCollection.Add("82512_109813pi_82512_109813", userCheckIn.lastName);

            //Email
            myNameValueCollection.Add("82512_109815pi_82512_109815", userCheckIn.email);
            
            System.Diagnostics.Debug.WriteLine("\nUploading to {0} ...", url);
            
            // 'The Upload(String,NameValueCollection)' implicitly method sets HTTP POST as the request method.            
            byte[] responseArray = myWebClient.UploadValues(url, myNameValueCollection);

            // Decode and display the response.
            System.Diagnostics.Debug.WriteLine("\nResponse received was :\n{0}", Encoding.ASCII.GetString(responseArray));
        }

        // Sends the confirmation email to the user
        private void sendEmail(UserCheckIn userCheckIn)
        {
           
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("noreply@checkinweb.ca");
            mailMessage.To.Add(userCheckIn.email);
            mailMessage.Subject = "You've been checked in!";
            mailMessage.IsBodyHtml = true;

            string contactEmailTableRows = "<tr>" +
                        "<td>Emergency contact(s)</td>" +
                        "<td>" + userCheckIn.contactEmail1 + "</td>" +
                    "</tr> ";

            // Add additional contact emails if they exists
            if(userCheckIn.contactEmail2 != null)
            {
                contactEmailTableRows += "<tr>" +
                        "<td></td>" +
                        "<td>" + userCheckIn.contactEmail2 + "</td>" +
                    "</tr> ";
            }

            if (userCheckIn.contactEmail3 != null)
            {
                contactEmailTableRows += "<tr>" +
                        "<td></td>" +
                        "<td>" + userCheckIn.contactEmail3 + "</td>" +
                    "</tr> ";
            }

            if (userCheckIn.contactEmail4 != null)
            {
                contactEmailTableRows += "<tr>" +
                        "<td></td>" +
                        "<td>" + userCheckIn.contactEmail4 + "</td>" +
                    "</tr> ";
            }

            mailMessage.Body = "Hello " + userCheckIn.firstName + "! <br/> <br/>" +
                "You've checked into SafetyLineLoneWorker's free check-in web app. Here are your check-in details:<br/><br/>" +
                "<table>" +
                    "<tr>" +
                        "<td>Full Name</td>" +
                        "<td>" + userCheckIn.firstName + " "+ userCheckIn.lastName+"</td>" +
                    "</tr> " +
                    contactEmailTableRows +
                    "<tr>" +
                        "<td>Location</td>" +
                        "<td>" + userCheckIn.location + "</td>" +
                    "</tr> " +
                    "<tr>" +
                        "<td>Return time</td>" +
                        "<td>" + userCheckIn.inputTime + "</td>" +
                    "</tr> " +
                    "<tr>" +
                        "<td>Return time(UTC)</td>" +
                        "<td>" + userCheckIn.returnTime + "</td>" +
                    "</tr> " +
                    "<tr>" +
                        "<td>CUrrent time(UTC)</td>" +
                        "<td>" + DateTime.UtcNow + "</td>" +
                    "</tr> " +
                "</table> " + 
                "<br><a href=\"" + MvcApplication.DOMAIN_URL + "/UserCheckIns/Delete/" + userCheckIn.ID + "/" + userCheckIn.secString + "\">Check-in here when you've returned.</a>";

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;
            //to authenticate we set the username and password properites on the SmtpClient
            smtp.Credentials = new NetworkCredential("checkinwebapp@gmail.com", "tsunamisolutions");//no need to mention here?

            smtp.Send(mailMessage);
        }

        // GET: UserCheckIns/Delete/5/<Security String>
        public ActionResult Delete(int? id, string sec)
        {
            if (id == null || sec == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserCheckIn userCheckIn = db.UserCheckIns.Find(id);

            if (userCheckIn == null || !sec.Equals(userCheckIn.secString))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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

            return RedirectToAction("Create");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // Generates and returns a random, cryptographically safe alpha-numeric string.
        private static string GetSecurityString(int length = 64)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_";
            StringBuilder res = new StringBuilder();

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }
    }
}
