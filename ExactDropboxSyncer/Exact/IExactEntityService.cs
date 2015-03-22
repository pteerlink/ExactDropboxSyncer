using System;

namespace ExactDropboxSyncer.Exact
{
    public interface IExactEntityService<T> where T : class
    {
        T Create(T entity);
        bool Update(T document);
        bool Delete(T document);
        T Get(Guid guid);

    }
}
