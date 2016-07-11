using System;
using System.Globalization;
using System.Threading;
using VelocityMail.Service;

namespace Samples.Localisation
{
    class Program
    {
        /// <summary>
        /// Shows how to create localised templates for different languages. There is a French-specific
        /// template and a fallback template for other languages.
        /// </summary>
        /// <remarks>Run Smtp4Dev (https://github.com/rnwood/smtp4dev) on port 2525 to inspect the
        /// e-mails sent</remarks>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var mailService = new VelocityMailService();

            // German should fallback to the default.
            RunAsUiCulture("de-DE", () =>
            {
                using (var msg = mailService.CreateMailMessage("SimpleTemplate", "testfrom@test.com", "testto@test.com"))
                {
                    msg.Subject = Subjects.TODAYS_SUBJECT;
                    msg.Put("name", "Jonathan");
                    msg.Put("today", DateTime.Today);

                    mailService.Send(msg);
                }
            });

            // Frence has its own template
            RunAsUiCulture("fr-FR", () =>
            {
                using (var msg = mailService.CreateMailMessage("SimpleTemplate", "testfrom@test.com", "testto@test.com"))
                {
                    msg.Subject = Subjects.TODAYS_SUBJECT;
                    msg.Put("name", "Jonathan");
                    msg.Put("today", DateTime.Today);

                    mailService.Send(msg);
                }
            });
        }

        /// <summary>
        /// Runs the action method with the specified UICulture.
        /// </summary>
        /// <param name="langCode">Desired language</param>
        /// <param name="action">Action to run with the specified language</param>
        static void RunAsUiCulture(string langCode, Action action)
        {
            var oldUiCulture = Thread.CurrentThread.CurrentUICulture;
            var oldCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                var newUiCulture = new CultureInfo(langCode);
                Thread.CurrentThread.CurrentUICulture = newUiCulture;
                Thread.CurrentThread.CurrentCulture = newUiCulture;
                action();
            }
            finally
            {
                Thread.CurrentThread.CurrentUICulture = oldUiCulture;
                Thread.CurrentThread.CurrentCulture = oldCulture;
            }
        }
    }
}
