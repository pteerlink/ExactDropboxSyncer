using System;
using System.Collections.Generic;
using System.Linq;
using ExactDropboxSyncer.Dropbox;
using ExactDropboxSyncer.Exact;
using ExactDropboxSyncer.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ExactDropboxSyncer.Tests
{
    [TestClass]
    public class DropboxToExactSyncerTest
    {
        private IList<DropboxFileMock> dropboxFiles;
        private IList<SyncInfo> exactSyncInfos;
        private IList<Guid> exactDocumentGuids;

        private Mock<IDropboxFileProvider> dropboxFileProvider;
        private Mock<ISyncInfoRepository> syncInfoRepositoryMock;
        private Mock<IExactDocumentStore> exactDocumentStore;

        private DropboxToExactSyncer dropboxToExactSyncer;

        #region initialize

        [TestInitialize]
        public void Initialize()
        {
            dropboxFiles = new List<DropboxFileMock>();
            exactDocumentGuids = new List<Guid>();
            exactSyncInfos = new List<SyncInfo>();

            dropboxFileProvider = CreateDefaultDropboxFileProviderMock();
            syncInfoRepositoryMock = CreateDefaultSyncInfoRepositoryMock();
            exactDocumentStore = CreateDefaultExactDocumentStoreMock();

            InsertFile(new DropboxFileMock(new byte[] { }, "Text1.txt", new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)), Guid.NewGuid());
            InsertFile(new DropboxFileMock(new byte[] { }, "Text2.txt", new DateTime(1970, 1, 2, 0, 0, 0, DateTimeKind.Utc)), Guid.NewGuid());
            InsertFile(new DropboxFileMock(new byte[] { }, "Text3.txt", new DateTime(1970, 1, 3, 0, 0, 0, DateTimeKind.Utc)), Guid.NewGuid());

            dropboxToExactSyncer = new DropboxToExactSyncer(dropboxFileProvider.Object, exactDocumentStore.Object, syncInfoRepositoryMock.Object);
        }

        private Mock<IExactDocumentStore> CreateDefaultExactDocumentStoreMock()
        {
            var mock = new Mock<IExactDocumentStore>();
            mock.Setup(m => m.ContainsDocument(It.IsAny<Guid>())).Returns((Guid guid) => exactDocumentGuids.Contains(guid));
            mock.Setup(m => m.GetDivisionId()).Returns(0);
            return mock;
        }

        private Mock<ISyncInfoRepository> CreateDefaultSyncInfoRepositoryMock()
        {
            var mock = new Mock<ISyncInfoRepository>();
            mock.Setup(m => m.Add(It.IsAny<SyncInfo>())).Returns(true);
            mock.Setup(m => m.Remove(It.IsAny<SyncInfo>())).Returns(true);
            mock.Setup(m => m.Update(It.IsAny<SyncInfo>())).Returns(true);
            mock.Setup(m => m.GetAll(It.IsAny<long>(), It.IsAny<int>()))
                .Returns((long dropboxId, int divisionId) => 
                    exactSyncInfos.Where(a => a.DropboxAccountId == dropboxId && a.ExactDivisionId == divisionId)
                );
            mock.Setup(m => m.GetByDropboxPath(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns((long dropboxId, int divisionId, string path) =>
                    exactSyncInfos.FirstOrDefault(a => a.DropboxAccountId == dropboxId && a.ExactDivisionId == divisionId && a.DropboxPath == path));
            return mock;
        }

        private Mock<IDropboxFileProvider> CreateDefaultDropboxFileProviderMock()
        {
            var mock = new Mock<IDropboxFileProvider>();
            mock.Setup(m => m.GetAccountId()).Returns(0);
            mock.Setup(m => m.GetFiles()).Returns(dropboxFiles);
            return mock;
        }

        private void InsertFile(DropboxFileMock file, Guid guid)
        {
            dropboxFiles.Add(file);
            exactDocumentGuids.Add(guid);
            exactSyncInfos.Add(new SyncInfo
            {
                DropboxAccountId = dropboxFileProvider.Object.GetAccountId(),
                DropboxPath = file.FilePath,
                ExactDivisionId = exactDocumentStore.Object.GetDivisionId(),
                ExactId = guid,
                LastModified = file.Modified
            });
        }

        #endregion

        [TestMethod]
        public void SyncFiles_CallsAddOnDocumentStoreWhenDocumentAdded()
        {
            var file = new DropboxFileMock(new byte[]{}, "NewFile.txt", new DateTime(1970, 1, 4, 0, 0, 0, DateTimeKind.Utc));
            dropboxFiles.Add(file);
            dropboxToExactSyncer.SyncFiles();
            exactDocumentStore.Verify(store => store.Add(file), Times.Once);
        }
        [TestMethod]
        public void SyncFiles_CallsAddOnSyncInfoRepositoryWhenDocumentAdded()
        {
            var file = new DropboxFileMock(new byte[] { }, "NewFile.txt", new DateTime(1970, 1, 4, 0, 0, 0, DateTimeKind.Utc));
            dropboxFiles.Add(file);
            dropboxToExactSyncer.SyncFiles();
            syncInfoRepositoryMock.Verify(sync => sync.Add(It.IsAny<SyncInfo>()), Times.Once);
        }

        [TestMethod]
        public void SyncFiles_CallsDeleteOnDocumentStoreWhenDocumentDeleted()
        {
            dropboxFiles.RemoveAt(0);
            dropboxToExactSyncer.SyncFiles();
            exactDocumentStore.Verify(store => store.Delete(exactDocumentGuids[0]), Times.Once);
        }
        [TestMethod]
        public void SyncFiles_CallsDeleteOnOnSyncInfoRepositoryWhenDocumentDeleted()
        {
            dropboxFiles.RemoveAt(0);
            dropboxToExactSyncer.SyncFiles();
            syncInfoRepositoryMock.Verify(sync => sync.Remove(It.IsAny<SyncInfo>()), Times.Once);
        }

        [TestMethod]
        public void SyncFiles_CallsUpdateOnDocumentStoreWhenDocumentModified()
        {
            dropboxFiles[0].Modified = DateTime.Now.AddSeconds(1);
            dropboxToExactSyncer.SyncFiles();
            exactDocumentStore.Verify(store => store.Update(exactDocumentGuids[0], dropboxFiles[0]), Times.Once);
        }
        [TestMethod]
        public void SyncFiles_CallsUpdateOnOnSyncInfoRepositoryWhenDocumentModified()
        {
            dropboxFiles[0].Modified = DateTime.Now.AddSeconds(1);
            dropboxToExactSyncer.SyncFiles();
            syncInfoRepositoryMock.Verify(sync => sync.Update(It.IsAny<SyncInfo>()), Times.Once);
        }

        [TestMethod]
        public void SyncFiles_CallsAddOnDocumentStoreWhenDocumentDeletedInExact()
        {
            exactDocumentGuids.RemoveAt(0);
            dropboxToExactSyncer.SyncFiles();
            exactDocumentStore.Verify(store => store.Add(dropboxFiles[0]), Times.Once);
        }
        [TestMethod]
        public void SyncFiles_CallsRemoveOnSyncInfoProviderWhenDocumentDeletedInExact()
        {
            exactDocumentGuids.RemoveAt(0);
            dropboxToExactSyncer.SyncFiles();
            syncInfoRepositoryMock.Verify(store => store.Remove(exactSyncInfos[0]), Times.Once);
        }
        [TestMethod]
        public void SyncFiles_CallsAddOnSyncInfoProviderWhenDocumentDeletedInExact()
        {
            exactDocumentGuids.RemoveAt(0);
            dropboxToExactSyncer.SyncFiles();
            syncInfoRepositoryMock.Verify(store => store.Remove(It.IsAny<SyncInfo>()), Times.Once);
        }
    }
}
