namespace ExactDropboxSyncer.Exact
{
    public interface IExactOnlineApi
    {
        IExactOnlineQuery<T> For<T>() where T : class;

        int GetDivision();
    }
}
