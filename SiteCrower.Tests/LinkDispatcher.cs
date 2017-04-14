using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SiteCrower.Core;

namespace SiteCrower.Tests
{
    [TestClass]
    public class LinkDispatcherTests
    {
        [TestMethod]
        public void LinkDispatcher_GetDomain()
        {
            var dispatcher = new LinkDispatcher("http://sirjordan.net");

            string full = dispatcher.GetDomain("http://sirjordan.net/sub/link.net");
            string relative = dispatcher.GetDomain("/sub/link.net");
            string relative2 = dispatcher.GetDomain("sub/link.net");

            Assert.AreEqual("sirjordan.net", full);
            Assert.AreEqual("sirjordan.net", relative);
            Assert.AreEqual("sirjordan.net", relative2);
        }

        [TestMethod]
        public void LinkDispatcher_DecorateUrl()
        {
            var dispatcher = new LinkDispatcher("http://sirjordan.net");
            
            Assert.AreEqual("http://sirjordan.net/link", dispatcher.DecorateUrl("link"));
            Assert.AreEqual("http://sirjordan.net/link", dispatcher.DecorateUrl("/link"));
            Assert.AreEqual("http://sirjordan.net/link", dispatcher.DecorateUrl("http://sirjordan.net/link"));
        }

        [TestMethod]
        public void LinkDispatcher_IsUrlValid()
        {
            Assert.IsTrue(LinkDispatcher.IsUrlValid("http://sirjordan.net"));
            Assert.IsTrue(LinkDispatcher.IsUrlValid("www.sirjordan.net"));
            Assert.IsTrue(LinkDispatcher.IsUrlValid("example.com"));
            Assert.IsTrue(LinkDispatcher.IsUrlValid("example.com.ph"));
            Assert.IsTrue(LinkDispatcher.IsUrlValid("www.example.com"));
            Assert.IsTrue(LinkDispatcher.IsUrlValid("http://example.com"));
            Assert.IsTrue(LinkDispatcher.IsUrlValid("https://example.com"));
            Assert.IsTrue(LinkDispatcher.IsUrlValid("http://www.example.com"));
            Assert.IsTrue(LinkDispatcher.IsUrlValid("ftp://example.com"));
            Assert.IsTrue(LinkDispatcher.IsUrlValid("example.com/doc"));
            Assert.IsTrue(LinkDispatcher.IsUrlValid("www.example.com/doc"));
            Assert.IsTrue(LinkDispatcher.IsUrlValid("http://example.com/doc"));
            Assert.IsTrue(LinkDispatcher.IsUrlValid("http://example.com/questions/12576252/convert-javascript-regex-to-c-sharp-regex-for-email-validation"));
        }
    }
}