namespace TeamSupport.ServiceLibrary
{
    public interface IWebClient
    {
        string UploadString(string uri, string method = "", string contentType = "", string data = "");
    }
}