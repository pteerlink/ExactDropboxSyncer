using System;
using System.Collections.Generic;
using ExactDropboxSyncer.Exact;
using ExactOnline.Client.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ExactDropboxSyncer.Tests
{
    [TestClass]
    public class ExactEntityServiceTest
    {
        private ExactEntityService<Document> exactEntityService;
        private Mock<IExactOnlineQuery<Document>> exactOnlineQueryMock;
        private Mock<IExactOnlineApi> exactOnlineApiMock;
            
        [TestInitialize]
        public void Initialize()
        {
            exactOnlineQueryMock = new Mock<IExactOnlineQuery<Document>>();
            exactOnlineApiMock = new Mock<IExactOnlineApi>();
            exactOnlineApiMock.Setup(a => a.For<Document>()).Returns(exactOnlineQueryMock.Object);
            exactEntityService = new ExactEntityService<Document>(exactOnlineApiMock.Object);
        }

        [TestMethod]
        public void Create_ReturnsNewObject()
        {
            var originalDocument = new Document();
            var insertTestMockQuery = new ExactOnlineQueryInsertTestMock<Document> {
                NewEntity = new Document { ID = Guid.NewGuid() }
            };
            exactOnlineApiMock.Setup(a => a.For<Document>()).Returns(insertTestMockQuery);

            var newDocument = exactEntityService.Create(originalDocument);

            Assert.AreSame(insertTestMockQuery.NewEntity, newDocument);
        }

        [TestMethod]
        public void Create_DoesNotChangeOriginalObject()
        {
            var originalGuid = Guid.NewGuid();
            var originalDocument = new Document { ID = originalGuid };

            var insertTestMockQuery = new ExactOnlineQueryInsertTestMock<Document>
            {
                NewEntity = new Document { ID = Guid.NewGuid() }
            };
            exactOnlineApiMock.Setup(a => a.For<Document>()).Returns(insertTestMockQuery);

            exactEntityService.Create(originalDocument);

            Assert.AreEqual(originalDocument.ID, originalGuid);
        }

        [TestMethod]
        public void Create_CallsInsertOnQuery()
        {
            var document = new Document();
            exactEntityService.Create(document);
            exactOnlineQueryMock.Verify(query => query.Insert(ref document), Times.Once);
        }

        [TestMethod]
        public void Delete_CallsDeleteOnQuery()
        {
            var document = new Document();
            exactEntityService.Delete(document);
            exactOnlineQueryMock.Verify(query => query.Delete(document), Times.Once);
        }
        [TestMethod]
        public void Update_CallsUpdateOnQuery()
        {
            var document = new Document();
            exactEntityService.Update(document);
            exactOnlineQueryMock.Verify(query => query.Update(document), Times.Once);
        }
        [TestMethod]
        public void Get_CallsGetEntityOnQuery()
        {
            var guid = Guid.NewGuid();
            exactEntityService.Get(guid);
            exactOnlineQueryMock.Verify(query => query.GetEntity(guid), Times.Once);
        }

        class ExactOnlineQueryInsertTestMock<T> : IExactOnlineQuery<T> where T : class
        {
            public T NewEntity;

            public bool Insert(ref T entity)
            {
                entity = NewEntity;
                return true;
            }

            #region not implemented

            public IExactOnlineQuery<T> Select(string[] fields)
            {
                throw new NotImplementedException();
            }

            public IExactOnlineQuery<T> Select(string field)
            {
                throw new NotImplementedException();
            }

            public IExactOnlineQuery<T> Where(string name)
            {
                throw new NotImplementedException();
            }

            public IList<T> Get()
            {
                throw new NotImplementedException();
            }

            public bool Delete(T document)
            {
                throw new NotImplementedException();
            }

            public bool Update(T document)
            {
                throw new NotImplementedException();
            }

            public T GetEntity(Guid guid)
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}