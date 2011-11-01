using Microsoft.WindowsAzure.StorageClient;

namespace Two10.Azure.Arr
{
    public class Log : TableServiceEntity
    {

        public string Role { get; set; }
        public string Message { get; set; }

    }
}
