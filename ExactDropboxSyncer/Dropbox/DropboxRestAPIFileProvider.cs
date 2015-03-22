using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DropboxRestAPI;
using DropboxRestAPI.Models.Core;

namespace ExactDropboxSyncer.Dropbox
{
    public class DropboxRestAPIFileProvider : IDropboxFileProvider
    {

	    private IClient client;
	    private IClient Client
	    {
		    get
		    {
				if (client == null) 
					client = new Client(new Options { AccessToken = oAuthTokenProvider.GetAccessToken() });
			    return client;
		    }
		}

		private readonly IDropboxOAuthIAccessTokenProvider oAuthTokenProvider;

        public DropboxRestAPIFileProvider(IDropboxOAuthIAccessTokenProvider oAuthTokenProvider)
        {
            if (oAuthTokenProvider == null)
                throw new ArgumentNullException("oAuthTokenProvider");

	        this.oAuthTokenProvider = oAuthTokenProvider;
        }

	    public long GetAccountId()
        {
            var accountInfoTask = Client.Core.Accounts.AccountInfoAsync();
            Task.WaitAll(accountInfoTask);
            return accountInfoTask.Result.uid;
        }

        public IEnumerable<IDropboxFile> GetFiles()
        {
            var task = Client.Core.Metadata.MetadataAsync("/ExactOnline");
            Task.WaitAll(task);
            var root = task.Result;
            return GetFilesRecursive(root);
        }

        private IEnumerable<IDropboxFile> GetFilesRecursive(MetaData fileSystemEntry)
        {
            if (fileSystemEntry.is_dir)
            {
                if (fileSystemEntry.contents == null)
                {
                    var task = Client.Core.Metadata.MetadataAsync(fileSystemEntry.path);
                    Task.WaitAll(task);
                    fileSystemEntry = task.Result;
                }

                foreach (var child in fileSystemEntry.contents)
                {
                    foreach (var file in GetFilesRecursive(child))
                    {
                        yield return file;
                    }
                }
            }
            else
            {
                yield return new DropboxRestAPIFile(fileSystemEntry, client);
            }
        }
    }
}