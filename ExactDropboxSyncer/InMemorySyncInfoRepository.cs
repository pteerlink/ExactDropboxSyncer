using System.Collections.Generic;
using System.Linq;
using ExactDropboxSyncer.Model;

namespace ExactDropboxSyncer
{
	public class InMemorySyncInfoRepository : ISyncInfoRepository
	{
        private readonly IList<SyncInfo> data;

        public InMemorySyncInfoRepository(IList<SyncInfo> data)
        {
            this.data = data;
        }

	    public InMemorySyncInfoRepository()
		{
            data = new List<SyncInfo>();
		}

        public IEnumerable<SyncInfo> GetAll(long dropboxAccountId, int exactDivisionId)
		{
            return data.Where(a => a.DropboxAccountId == dropboxAccountId && a.ExactDivisionId == exactDivisionId);
		}

        public SyncInfo GetByDropboxPath(long dropboxAccountId, int exactDivisionId, string dropboxPath)
        {
            return GetAll(dropboxAccountId, exactDivisionId).FirstOrDefault(a => a.DropboxPath == dropboxPath);
		}

        public bool Add(SyncInfo syncInfo)
        {
            data.Add(syncInfo);
			return true;
		}

        public bool Update(SyncInfo syncInfo)
        {
			return true;
		}

        public bool Remove(SyncInfo syncInfo)
		{
            data.Remove(syncInfo);
			return true;
		}
    }
}
