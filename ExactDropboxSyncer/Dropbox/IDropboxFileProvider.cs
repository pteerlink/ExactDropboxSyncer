using System.Collections.Generic;

namespace ExactDropboxSyncer.Dropbox
{
    public interface IDropboxFileProvider
	{
        IEnumerable<IDropboxFile> GetFiles();
        long GetAccountId();
	}
}
