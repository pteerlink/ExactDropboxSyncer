using System;

namespace ExactDropboxSyncer.Exact
{
    public class ExactEntityService<T> : IExactEntityService<T> where T : class
    {
        private readonly IExactOnlineApi api;

        public ExactEntityService(IExactOnlineApi api)
        {
            this.api = api;
        }

        public T Create(T entity)
        {
            var newEntity = entity;
            GetQuery().Insert(ref newEntity);
            return newEntity;
        }

        public bool Update(T entity)
        {
            return GetQuery().Update(entity);
        }

        public bool Delete(T entity)
        {
            return GetQuery().Delete(entity);
        }

        public T Get(Guid guid)
        {
            return GetQuery().GetEntity(guid);
        }

        protected IExactOnlineQuery<T> GetQuery()
        {
            return api.For<T>();
        }
    }
}
