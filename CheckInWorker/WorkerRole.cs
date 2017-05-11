using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using CheckInCommon;
using System.Data.Entity.Infrastructure;
using System.Net.Mail;

namespace CheckInWorker
{
    

    public class WorkerRole : RoleEntryPoint
    {

        // Time for when the database will check. Time is in seconds
        private int checkEvery { get; set; }

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private SmtpClient smtp;

        public override void Run()
        {
            checkEvery = 60;
            Trace.TraceInformation("CheckInWorker is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("CheckInWorker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("CheckInWorker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("CheckInWorker has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            CheckInContext db = new CheckInContext();

            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");

                DbSqlQuery<UserCheckIn> query = db.UserCheckIns.SqlQuery("SELECT * FROM dbo.UserCheckIns WHERE returnTime <= @p0",DateTime.Now );

                List<UserCheckIn> expiredCheckIns = query.ToList();

                foreach (UserCheckIn checkIn in expiredCheckIns)
                {
                    sendMissingEmail(checkIn);
                    db.UserCheckIns.Remove(checkIn);               
                    db.SaveChanges();
                }

                await Task.Delay(checkEvery * 1000);
            }


        }

        private void sendMissingEmail(UserCheckIn userCheckIn)
        {

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("noreply@checkinweb.ca");
            mailMessage.To.Add(userCheckIn.contactEmail1);

            if (userCheckIn.contactEmail2 != null)
            {
                mailMessage.To.Add(userCheckIn.contactEmail2);
            }

            if (userCheckIn.contactEmail3 != null)
            {
                mailMessage.To.Add(userCheckIn.contactEmail3);
            }

            if (userCheckIn.contactEmail4 != null)
            {
                mailMessage.To.Add(userCheckIn.contactEmail4);
            }

            mailMessage.Subject = userCheckIn.firstName + " " + userCheckIn.lastName + " has not checked back in from " + userCheckIn.location ;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = "Hello <br/> <br/>" 
                + "This is to inform you that " + userCheckIn.firstName + " " + userCheckIn.lastName 
                + " left you as an emergency contact. <br/>" 
                + userCheckIn.firstName + " was expected to check in at " 
                + userCheckIn.returnTime + "<br/>Ya boi missing. <br/> Here is the message " + userCheckIn.firstName + " left.<br/>" + userCheckIn.message;
                
            //mailMessage.IsBodyHtml = true;

            smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;
            //to authenticate we set the username and password properites on the SmtpClient
            smtp.Credentials = new NetworkCredential("checkinwebapp@gmail.com", "tsunamisolutions");//no need to mention here?

            smtp.Send(mailMessage);

        }
    }
}
