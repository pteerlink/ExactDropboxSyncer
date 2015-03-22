using System.Collections.Generic;
using ExactDropboxSyncer.Model;

namespace ExactDropboxSyncer
{
	public interface ISyncInfoRepository
	{
		IEnumerable<SyncInfo> GetAll(long dropboxAccountId, int divisionId);
        SyncInfo GetByDropboxPath(long dropboxAccountId, int divisionId, string path);
        bool Add(SyncInfo syncInfo);
        bool Update(SyncInfo syncInfo);
        bool Remove(SyncInfo syncInfo);
	}
}
