using System;
using System.Collections.Generic;
using ExactDropboxSyncer.Exact;
using ExactOnline.Client.Models;
using ExactOnline.Client.Sdk.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace ExactDropboxSyncer.Tests
{
    [TestClass]
    public class ExactDocumentStoreTest
    {
        private Mock<IExactOnlineApi> clientMock;
        private Mock<IExactDocumentAttachmentService> documentAttachmentServiceMock;
        private Mock<IExactDocumentCategoryService> documentCategoryServiceMock;
        private Mock<IExactEntityService<Document>> documentServiceMock;
        private DropboxFileMock fileMock;

        [TestInitialize]
        public void Initialize()
        {
            fileMock = new DropboxFileMock(new byte[]{}, "TestFile.txt", new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            clientMock = GetDefaultExactApiMock();
            documentAttachmentServiceMock = GetDefaultExactDocumentAttachmentServiceMock();
            documentCategoryServiceMock = GetDefaultExactDocumentCategoryServiceMock();
            documentServiceMock = GetDefaultExactDocumentServiceMock();
        }

        [TestMethod]
        public void Add_ReturnsDocumentGuidOnSuccess()
        {
            var expectedGuid = Guid.NewGuid();
            documentServiceMock.Setup(a => a.Create(It.IsAny<Document>())).Returns(new Document() { ID = expectedGuid });

            var result = GetExactDocumentStore().Add(fileMock);

            Assert.AreEqual(expectedGuid, result);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Add_CallsDeleteDocumentWhenDocumentAttachentCreateFails()
        {
            documentAttachmentServiceMock.Setup(a => a.Create(It.IsAny<DocumentAttachment>())).Throws(new Exception());
            GetExactDocumentStore().Add(fileMock);
            documentServiceMock.Verify(m => m.Delete(It.IsAny<Document>()), Times.Once());
        }

        [TestMethod]
        public void ContainsDocument_ReturnsFalseWhenExactEntityServiceGetReturnsNull()
        {
            var guid = Guid.NewGuid();

            documentServiceMock.Setup(a => a.Get(guid)).Returns((Document)null);
            var result = GetExactDocumentStore().ContainsDocument(guid);
            Assert.IsFalse(result);
        }
        [TestMethod]
        public void ContainsDocument_ReturnsFalseWhenExactEntityServiceThrowsNotFoundException()
        {
            var guid = Guid.NewGuid();
            documentServiceMock.Setup(a => a.Get(guid)).Throws(new NotFoundException());
            var result = GetExactDocumentStore().ContainsDocument(guid);
            Assert.IsFalse(result);
        }
        [TestMethod, ExpectedException(typeof(Exception))]
        public void ContainsDocument_ThrowsExceptionWhenExactEntityServiceThrowsException()
        {
            var guid = Guid.NewGuid();

            documentServiceMock.Setup(a => a.Get(guid)).Throws(new Exception());
            var result = GetExactDocumentStore().ContainsDocument(guid);
            
            Assert.IsFalse(result);
        }


        private ExactDocumentStore GetExactDocumentStore()
        {
            var documentService = documentServiceMock.Object;
            var client = clientMock.Object;
            var documentAttachmentService = documentAttachmentServiceMock.Object;
            var documentCategoryService = documentCategoryServiceMock.Object;
            var exactStore = new ExactDocumentStore(client, documentService, documentAttachmentService, documentCategoryService);
            return exactStore;
        }

        #region default mocks

        private Mock<IExactOnlineApi> GetDefaultExactApiMock()
        {
            var mock = new Mock<IExactOnlineApi>();
            mock.Setup(m => m.GetDivision()).Returns(1);
            return mock;
        }

        private Mock<IExactEntityService<Document>> GetDefaultExactDocumentServiceMock()
        {
            var mock = new Mock<IExactEntityService<Document>>();
            mock.Setup(m => m.Create(It.IsAny<Document>())).Returns(new Document());
            mock.Setup(m => m.Delete(It.IsAny<Document>())).Returns(true);
            mock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(It.IsAny<Document>());
            mock.Setup(m => m.Update(It.IsAny<Document>())).Returns(true);
            return mock;
        }

        private Mock<IExactDocumentAttachmentService> GetDefaultExactDocumentAttachmentServiceMock()
        {
            var mock = new Mock<IExactDocumentAttachmentService>();
            mock.Setup(m => m.Create(It.IsAny<DocumentAttachment>())).Returns(new DocumentAttachment());
            mock.Setup(m => m.Delete(It.IsAny<DocumentAttachment>())).Returns(true);
            mock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(new DocumentAttachment());
            mock.Setup(m => m.Update(It.IsAny<DocumentAttachment>())).Returns(true);
            mock.Setup(m => m.GetByDocumentId(It.IsAny<Guid>())).Returns(new List<DocumentAttachment>());
            return mock;
        }

        private Mock<IExactDocumentCategoryService> GetDefaultExactDocumentCategoryServiceMock()
        {
            var mock = new Mock<IExactDocumentCategoryService>();
            var entity = It.IsAny<DocumentCategory>();
            mock.Setup(m => m.Create(entity)).Returns(new DocumentCategory());
            mock.Setup(m => m.Delete(entity)).Returns(true);
            mock.Setup(m => m.Get(It.IsAny<Guid>())).Returns(new DocumentCategory());
            mock.Setup(m => m.Update(entity)).Returns(true);
            mock.Setup(m => m.GetByGuidByCategoryName("Employees")).Returns(Guid.NewGuid());
            return mock;
        }

        #endregion
    }
}
