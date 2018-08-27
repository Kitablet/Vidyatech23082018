using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FISE_API.Services
{

    public class BlobService
    {
        static CloudStorageAccount _account;
        static bool _accountSet = false;
        public BlobService()
        {
            string accountName = System.Configuration.ConfigurationManager.AppSettings["BLOB"].ToString();
            string keyVal = System.Configuration.ConfigurationManager.AppSettings["KEY"].ToString();
            _accountSet = CloudStorageAccount.TryParse(String.Format("DefaultEndpointsProtocol=https;" + "AccountName={0};AccountKey={1}", accountName, keyVal), out _account);
        }
        
        public string GetSASUri(string blobName, string policyName=null)
        {
            if (_accountSet)
            {
                try
                {
                    string BLOBContainer = System.Configuration.ConfigurationManager.AppSettings["BLOBContainer"].ToString();
                    var container = _account.CreateCloudBlobClient().GetContainerReference(BLOBContainer);
                    container.CreateIfNotExists();
                    var blob = container.GetBlobReference(blobName);

                    var sas = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
                    {
                        Permissions = SharedAccessBlobPermissions.Read,
                        SharedAccessExpiryTime = DateTime.UtcNow + TimeSpan.FromMinutes(1)
                    });
                    return blob.Uri.AbsoluteUri + sas;
                }
                catch (StorageException e)
                {
                    throw;
                }
            }
            else return "The API has not been configurated yet";
        }
        void CreateSharedAccessPolicy(CloudBlobClient blobClient, CloudBlobContainer container,
            string policyName)
        {
            //Create a new shared access policy and define its constraints.
            SharedAccessBlobPolicy sharedPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List |
                    SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create | SharedAccessBlobPermissions.Delete
            };

            //Get the container's existing permissions.
            BlobContainerPermissions permissions = container.GetPermissions();

            //Add the new policy to the container's permissions, and set the container's permissions.
            permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
            container.SetPermissions(permissions);
        }

    }
}