using System;
using System.IO;
using System.Net;

using NUnit.Framework;

namespace Tests.Core.Net
{
    [TestFixture]
    public class NetTests
    {
        // TODO: Test certificate errors. How?
        // URL we expect to always be up.
        private const string KnownURL = "http://example.com/";

        [Test]
        [Category("Online")]
        public void DownloadThrowsOnInvalidURL()
        {
            // Download should throw an exception on an invalid URL.
            Assert.Throws<WebException>(
                delegate
                {
                    CKAN.Net.Download("cheese sandwich");
                });
        }

        [Test]
        [Category("Online")]
        public void DownloadReturnsSavefileNameAndSavefileExists()
        {
            // Two-argument test, should save to the file we supply
            string savefile = "example.txt";
            string downloaded = CKAN.Net.Download(KnownURL, savefile);
            Assert.AreEqual(downloaded, savefile);
            Assert.That(File.Exists(savefile));
            File.Delete(savefile);
        }

        [Test]
        [Category("Online")]
        public void SingleArgumentDownloadSavesToTemporaryFile()
        {
            string downloaded = CKAN.Net.Download(KnownURL);
            Assert.That(File.Exists(downloaded));
            File.Delete(downloaded);
        }

        [Test]
        [Category("FlakyNetwork"), Category("Online")]
        public void SpaceDockSSL()
        {
            string file = CKAN.Net.Download("https://spacedock.info/mod/132/Contract%20Reward%20Modifier/download/2.1");
            if (!File.Exists(file))
            {
                throw new Exception("File not downloaded");
            }
            File.Delete(file);
        }
    }
}
