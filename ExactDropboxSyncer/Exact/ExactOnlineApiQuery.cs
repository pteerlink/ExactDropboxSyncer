using System;
using System.Collections.Generic;
using ExactOnline.Client.Sdk.Helpers;

namespace ExactDropboxSyncer.Exact
{
    public class ExactOnlineApiQuery<T> : IExactOnlineQuery<T> where T : class
    {
        private readonly ExactOnlineQuery<T> exactOnlineQuery;

        public ExactOnlineApiQuery(ExactOnlineQuery<T> exactOnlineQuery)
        {
            this.exactOnlineQuery = exactOnlineQuery;
        }

        public IExactOnlineQuery<T> Select(string[] fields)
        {
            return new ExactOnlineApiQuery<T>(exactOnlineQuery.Select(fields));
        }

        public IExactOnlineQuery<T> Select(string field)
        {
            return new ExactOnlineApiQuery<T>(exactOnlineQuery.Select(field));
        }

        public IExactOnlineQuery<T> Where(string filter)
        {
            return new ExactOnlineApiQuery<T>(exactOnlineQuery.Where(filter));
        }
        
        public IList<T> Get()
        {
            return exactOnlineQuery.Get();
        }

        public bool Insert(ref T entity)
        {
            return exactOnlineQuery.Insert(ref entity);
        }

        public bool Delete(T entity)
        {
            return exactOnlineQuery.Delete(entity);
        }

        public bool Update(T entity)
        {
            return exactOnlineQuery.Update(entity);
        }

        public T GetEntity(Guid guid)
        {
            return exactOnlineQuery.GetEntity(guid);
        }
    }
}