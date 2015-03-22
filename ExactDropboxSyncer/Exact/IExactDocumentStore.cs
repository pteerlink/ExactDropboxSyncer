using System;
using ExactDropboxSyncer.Dropbox;

namespace ExactDropboxSyncer.Exact
{
	public interface IExactDocumentStore
	{
        int GetDivisionId();
        Guid Add(IDropboxFile file);
        void Update(Guid key, IDropboxFile file);
        void Delete(Guid key);
        bool ContainsDocument(Guid key);
	}
}
