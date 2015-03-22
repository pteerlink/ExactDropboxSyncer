namespace ExactDropboxSyncer.UI
{
    public interface ISyncerFactory
    {
        ISyncer GetSyncer();
    }
}
