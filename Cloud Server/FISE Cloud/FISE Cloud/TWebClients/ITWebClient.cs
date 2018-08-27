namespace FISE_Cloud.TWebClients
{
    public partial interface ITWebClient
    {
        T UploadData<T>(string resourceName, object postData, bool includeContent=true);
        T DownloadData<T>(string resourceName,object param);
    }
}