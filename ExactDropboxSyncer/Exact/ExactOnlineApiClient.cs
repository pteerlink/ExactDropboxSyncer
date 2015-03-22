using ExactOnline.Client.Sdk.Controllers;

namespace ExactDropboxSyncer.Exact
{
    public class ExactOnlineApiClient : IExactOnlineApi
    {
        private readonly ExactOnlineClient exactOnlineClient;

        public ExactOnlineApiClient(ExactOnlineClient exactOnlineClient)
        {
            this.exactOnlineClient = exactOnlineClient;
        }

        public IExactOnlineQuery<T> For<T>() where T : class
        {
            return new ExactOnlineApiQuery<T>(exactOnlineClient.For<T>());
        }

        public int GetDivision()
        {
            return exactOnlineClient.GetDivision();
        }
    }
}