using System.Collections.Generic;

namespace ExactDropboxSyncer.Exact
{
    public interface IExactOnlineQuery<T> where T : class
    {
        IExactOnlineQuery<T> Select(string[] fields);
        IExactOnlineQuery<T> Select(string field);
        IExactOnlineQuery<T> Where(string name);
        IList<T> Get();

        bool Insert(ref T document);
        bool Delete(T document);
        bool Update(T document);

        T GetEntity(System.Guid guid);

    }
}