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

namespace CheckInWeb.Controllers
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
        public ActionResult Create([Bind(Include = "ID,firstName,lastName,telNum,email,contactEmail1,contactEmail2,contactEmail3,contactEmail4,location,returnTime,message,subscribe")] UserCheckIn userCheckIn)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (userCheckIn.returnTime.CompareTo(DateTime.Now) < 0)
                    {
                        userCheckIn.returnTime = userCheckIn.returnTime.AddDays(1);
                    }
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

            //First Name and yes they gave us those fucking values to use
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
            
            /*
            // fname=evan&lname=chen&email=evan.c1995@gmail.com
            string post = "82512_109811pi_82512_109811=evan&82512_109813pi_82512_109813=evan2&82512_109815pi_82512_109815=evan.c1995@gmail.com";
            var encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(post);
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();
            WebResponse response = request.GetResponse();
            String result;
            StreamReader sr = new StreamReader(response.GetResponseStream());
            
            result = sr.ReadToEnd();
            sr.Close();
            
            System.Diagnostics.Debug.WriteLine("\nResponse received was :\n{0}", result);
            */
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
                        "<td>" + userCheckIn.returnTime + "</td>" +
                    "</tr> " +
                "</table> ";

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
        public ActionResult Edit([Bind(Include = "ID,firstName,lastName,telNum,email,contactEmail1,contactEmail2,contactEmail3,contactEmail4,location,returnTime,message,subscribe")] UserCheckIn userCheckIn)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userCheckIn).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userCheckIn);
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
