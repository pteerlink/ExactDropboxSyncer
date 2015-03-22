using ExactDropboxSyncer.Dropbox;
using ExactDropboxSyncer.EF;
using ExactDropboxSyncer.Exact;

namespace ExactDropboxSyncer.UI
{
    class FileSyncerFactory : ISyncerFactory
    {
        const string exactWebsite = @"https://start.exactonline.nl";
        const string exactClientId = "003abe89-8f78-43e4-96c7-0f93ebf0ccdb";
        const string exactClientSecret = "3tSeD15WrgKt";
        const string dropboxClientId = "tdp13d3ulqpdvxo";
        const string dropboxClientSecret = "qdoh0tu662uirta";

        private readonly IExactOnlineOAuthIAccessTokenProvider exactOnlineOAuthIAccessTokenProvider;
        private readonly IDropboxOAuthIAccessTokenProvider dropboxOAuthIAccessTokenProvider;
        private readonly ISyncInfoRepository syncInfoRepository;

        public FileSyncerFactory()
        {
            syncInfoRepository = new EFSyncInfoRepository();
            exactOnlineOAuthIAccessTokenProvider = new ExactOAuthTokenProvider(exactWebsite, exactClientId, exactClientSecret);
            dropboxOAuthIAccessTokenProvider = new DropboxOAuthTokenProvider(dropboxClientId, dropboxClientSecret);
        }

        public ISyncer GetSyncer()
        {
            var fileSource = new DropboxRestAPIFileProvider(dropboxOAuthIAccessTokenProvider);
            var fileDestination = new ExactDocumentStore(exactWebsite, exactOnlineOAuthIAccessTokenProvider);
            var syncer = new DropboxToExactSyncer(fileSource, fileDestination, syncInfoRepository);
            return syncer;
        }
    }
}
