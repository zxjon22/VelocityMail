﻿using System;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Threading;
using netDumbster.smtp;

namespace VelocityMail.Tests
{
    public abstract class AbstractMailTest : IDisposable
    {
        /// <summary>
        /// Creates and initialises a fake smtp server on a unique port
        /// for each test.
        /// </summary>
        public AbstractMailTest()
        {
            // To ensure 'fallback' templates are tested correctly, all tests
            // run in English, regardless of your current locale.
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            // Generate a new port for each test so they can all be run
            // in parallel.
            this.smtpPort = Interlocked.Increment(ref portSeed);
            this.smtpServer = SimpleSmtpServer.Start(this.smtpPort);
        }

        /// <summary>
        /// Disposes of the fake smtp server at the end of the test
        /// </summary>
        public void Dispose()
        {
            this.smtpServer.Stop();
        }

        /// <summary>
        /// System.Mail.SmtpClient holds open connections to the smtp server
        /// for a while so we need to generate a fresh port for each
        /// test.
        /// </summary>
        private static int portSeed = 25250;

        /// <summary>
        /// Port the SMTP server is running on for the current test.
        /// </summary>
        private int smtpPort;

        /// <summary>
        /// netDumbster test server
        /// </summary>
        protected SimpleSmtpServer smtpServer;

        /// <summary>
        /// Gets the path on disk to the folder containing the
        /// assets.
        /// </summary>
        protected string AssetsDir
        {
            get
            {
                var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                return Path.Combine(path, "Assets");
            }
        }

        /// <summary>
        /// Creates a new SmtpClient configured to send to the correct port
        /// for the current test
        /// </summary>
        /// <returns>New SmtpClient</returns>
        public SmtpClient CreateSmtpClient()
        {
            return new SmtpClient("localhost", this.smtpPort);
        }

        /// <summary>
        /// Gets the path to the specified asset on disk
        /// </summary>
        /// <param name="assetName">File name of the asset to get the path to.</param>
        /// <returns>Full path to the asset</returns>
        public string GetAssetPath(string assetName)
        {
            return Path.Combine(this.AssetsDir, assetName);
        }
    }
}
