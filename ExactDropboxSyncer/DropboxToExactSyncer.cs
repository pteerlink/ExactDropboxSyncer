using System.Linq;
using ExactDropboxSyncer.Dropbox;
using ExactDropboxSyncer.Exact;
using ExactDropboxSyncer.Model;

namespace ExactDropboxSyncer
{
	public class DropboxToExactSyncer : ISyncer
	{
		private readonly IDropboxFileProvider dropboxFileProvider;
		private readonly IExactDocumentStore documentStore;
		private readonly ISyncInfoRepository syncInfoRepository;

        public DropboxToExactSyncer(IDropboxFileProvider dropboxFileProvider, IExactDocumentStore documentStore, ISyncInfoRepository syncInfoRepository)
		{
			this.dropboxFileProvider = dropboxFileProvider;
			this.documentStore = documentStore;
			this.syncInfoRepository = syncInfoRepository;
		}

		#region Public methods
		public void SyncFiles()
		{
			var allDropboxFiles = dropboxFileProvider.GetFiles().ToList();
            foreach (var file in allDropboxFiles) {
				SyncFile(file);
			}

            var deletedFiles = syncInfoRepository.GetAll(dropboxFileProvider.GetAccountId(), documentStore.GetDivisionId())
                .Where(a => allDropboxFiles.Any(b => b.FilePath == a.DropboxPath) == false).ToList();

			foreach (var fileSyndInfo in deletedFiles) {
				DeleteFile(fileSyndInfo);
			}
		}
		#endregion

		#region Private methods

		private void SyncFile(IDropboxFile file)
		{
			var fileSyncInfo = syncInfoRepository.GetByDropboxPath(dropboxFileProvider.GetAccountId(), documentStore.GetDivisionId(), file.FilePath);

		    if (fileSyncInfo == null)
		    {
                InsertFile(file);
		    }
		    else if (documentStore.ContainsDocument(fileSyncInfo.ExactId) == false)
		    {
		        syncInfoRepository.Remove(fileSyncInfo);
                InsertFile(file);
		    }
			else if (fileSyncInfo.LastModified < file.Modified)
			{
                UpdateFile(file, fileSyncInfo);
			}
		}

		private void InsertFile(IDropboxFile file)
		{
			var newId = documentStore.Add(file);
			syncInfoRepository.Add(new SyncInfo
			{
                ExactDivisionId = documentStore.GetDivisionId(),
                ExactId = newId,
                DropboxAccountId = dropboxFileProvider.GetAccountId(),
                DropboxPath = file.FilePath,
				LastModified = file.Modified
			});
		}

        private void UpdateFile(IDropboxFile file, SyncInfo syncInfo)
		{
			documentStore.Update(syncInfo.ExactId, file);
			syncInfo.LastModified = file.Modified;
			syncInfoRepository.Update(syncInfo);
		}

        private void DeleteFile(SyncInfo syncInfo)
        {
            documentStore.Delete(syncInfo.ExactId);
            syncInfoRepository.Remove(syncInfo);
        } 

		#endregion
	}
}
