using System;
using ExactDropboxSyncer.Dropbox;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExactDropboxSyncer.Tests
{
    [TestClass]
    public class DropboxRestAPIFileProviderTest
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ThrowsExceptionWherOAuthProviderIsNull()
        {
            new DropboxRestAPIFileProvider(null);
        }
    }
}
