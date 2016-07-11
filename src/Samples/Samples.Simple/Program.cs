using System;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using VelocityMail;

namespace Samples.Simple
{
    class Program
    {
        /// <summary>
        /// Simplest use case creating a VelocityEngine manually. You don't really want to do this -
        /// use the VelocityMailService instead!
        /// </summary>
        /// <remarks>Run Smtp4Dev (https://github.com/rnwood/smtp4dev) on port 2525 to inspect the
        /// e-mails sent</remarks>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Create VelocityEngine that looks for templates embedded in the Samples.Simple assembly in
            // the Simple.EmailTemplates namespace.
            var engine = VelocityEngineFactory.Create("Samples.Simple.EmailTemplates", "Samples.Simple");

            using (var msg = new VelocityMailMessage("SimpleTemplate", "testfrom@test.com", "testto@test.com"))
            {
                msg.Subject = "Rsrc: Today's ($today.ToShortDateString()) message";
                msg.Put("name", "Jonathan");
                msg.Put("today", DateTime.Today);

                using (var smtpServer = new SmtpClient())
                {
                    smtpServer.Send(msg.GetMailMessage(engine));
                }
            }

            // Create another VelocityEngine that looks for templates on disk.
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            engine = VelocityEngineFactory.CreateUsingDirectory(path);

            using (var msg = new VelocityMailMessage("DiskTemplate", "testfrom@test.com", "testto@test.com"))
            {
                msg.Subject = "Disk: Today's ($today.ToShortDateString()) message";
                msg.Put("name", "Bill");
                msg.Put("today", DateTime.Today);

                using (var smtpServer = new SmtpClient())
                {
                    smtpServer.Send(msg.GetMailMessage(engine));
                }
            }
        }
    }
}
