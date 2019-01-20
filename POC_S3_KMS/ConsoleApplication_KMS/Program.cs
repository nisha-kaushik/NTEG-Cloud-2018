using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.KeyManagementService;
using Amazon.Util;
using Amazon.Runtime;
using Amazon.Auth;
using Amazon;
using Amazon.MissingTypes;
using Amazon.Internal;
using Amazon.S3.Encryption;
using Amazon.S3.Model;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using System.IO;
using Amazon.KeyManagementService.Model;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.IO.Compression;
using System.Net.Http;
using Amazon.S3.IO;

namespace ConsoleApplication_KMS
{
    class Program
    {
        //Personal Account
        static readonly string accessKey = "AKIAJSVTB2FWAKHRLSBA";
        static readonly string secretKey = "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";

        //static readonly string accessKey = string.Empty;
        //static readonly string secretKey = string.Empty;

        public static void PutKeyOnS3WithEncryption(string kmsKeyID, string objectKeyName)
        {
            Console.WriteLine("Starting PutKeyOnS3WithEncryption with Customer Key with ID = " + kmsKeyID);
            // Encryption
            // ----------
            var bucketName = "nishatestkms";
            //var kmsKeyID = "2461967c-c660-4914-83e6-b29690949812";
            var objectKey = objectKeyName;
            var objectContent = "myobject content";


            //var options = new CredentialProfileOptions
            //{
            //    AccessKey = "AKIAJSVTB2FWAKHRLSBA",
            //    SecretKey = "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"
            //};
            //var profile = new CredentialProfile("shared_profile", options);
            //profile.Region = RegionEndpoint.USWest1;
            //var sharedFile = new SharedCredentialsFile();
            //sharedFile.RegisterProfile(profile);         

            AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);
            //AmazonS3Config config = new AmazonS3Config();
            //config.ServiceURL = "s3.amazonaws.com";
            //config.RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName("us-east-1");
            //client = new AmazonS3Client(credentials, config);



            var kmsEncryptionMaterials = new EncryptionMaterials(kmsKeyID);
            // CryptoStorageMode.ObjectMetadata is required for KMS EncryptionMaterials
            var config = new AmazonS3CryptoConfiguration()
            {
                StorageMode = CryptoStorageMode.ObjectMetadata
            };

            config.ServiceURL = "s3.amazonaws.com";
            config.RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName("us-east-1");

            using (var client = new AmazonS3EncryptionClient(config, kmsEncryptionMaterials))
            {
                var request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    ContentBody = objectContent
                };
                client.PutObject(request);
            }

            Console.WriteLine("Successfully done PutKeyOnS3WithEncryption with Customer Key with ID = " + kmsKeyID);
        }


        public static string GetKeyFromS3WithEncryption(string kmsKeyID, string objectKeyName)
        {
            Console.WriteLine("Starting GetKeyFromS3WithEncryption with Customer Key with ID = " + kmsKeyID);
            // Decryption
            // ----------
            var bucketName = "nishatestkms";
            //var kmsKeyID = "2461967c-c660-4914-83e6-b29690949812";
            var objectKey = objectKeyName;
            string objectContent = null;

            var kmsEncryptionMaterials = new EncryptionMaterials(kmsKeyID);
            // CryptoStorageMode.ObjectMetadata is required for KMS EncryptionMaterials

            var config = new AmazonS3CryptoConfiguration()
            {
                StorageMode = CryptoStorageMode.ObjectMetadata
            };

            AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);

            config.ServiceURL = "s3.amazonaws.com";
            config.RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName("us-east-1");

            using (var client = new AmazonS3EncryptionClient(config, kmsEncryptionMaterials))
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey
                };

                using (var response = client.GetObject(request))
                using (var stream = response.ResponseStream)
                using (var reader = new StreamReader(stream))
                {
                    objectContent = reader.ReadToEnd();
                }
                //}
                // use objectContent
            }
            Console.WriteLine("Successfully done GetKeyFromS3WithEncryption with Customer Key with ID = " + kmsKeyID);
            Console.WriteLine("Object Content = " + objectContent);
            return objectContent;
        }








        public static void PutKeyOnS3(string objectKeyName)
        {
            Console.WriteLine("Starting PutKeyOnS3 ");          
            var bucketName = "pan-dev-clientmetadata";         
            var objectKey = objectKeyName;
            var objectContent = "myobject content";

            AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);          
           
            var config = new AmazonS3CryptoConfiguration()
            {
                StorageMode = CryptoStorageMode.ObjectMetadata
            };
         
            config.RegionEndpoint = Amazon.RegionEndpoint.USEast1;

            using (var client = new AmazonS3Client(credentials,config))
            {
                var request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    ContentBody = objectContent
                };
                client.PutObject(request);
            }

            Console.WriteLine("Successfully done PutKeyOnS3");
        }

        public static void PutFileOnS3(string objectKeyName,string filepath)
        {
            Console.WriteLine("Starting PutFileOnS3 ");
            var bucketName = "pan-dev-clientmetadata";
            var objectKey = objectKeyName;
            var objectContent = "myobject content";

            AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);

            var config = new AmazonS3CryptoConfiguration()
            {
                StorageMode = CryptoStorageMode.ObjectMetadata
            };

            config.RegionEndpoint = Amazon.RegionEndpoint.USEast1;

            using (var client = new AmazonS3Client(credentials, config))
            {
                var request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    //InputStream
                    //FilePath = filepath
                };
                client.PutObject(request);
            }

            Console.WriteLine("Successfully done PutFileOnS3");
        }

        public static void GetFileOnS3(string objectKeyName)
        {
            Console.WriteLine("Starting GetFileOnS3 ");
            var bucketName = "pan-dev-clientmetadata";
            var objectKey = objectKeyName;
            string objectContent = null;

            AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);

            var config = new AmazonS3CryptoConfiguration()
            {
                StorageMode = CryptoStorageMode.ObjectMetadata
            };

            config.RegionEndpoint = Amazon.RegionEndpoint.USEast1;

            using (var client = new AmazonS3Client(credentials, config))
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey
                };

                using (GetObjectResponse response = client.GetObject(request))
                {
                    string dest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), objectKey);
                    if (!File.Exists(dest))
                    {
                        response.WriteResponseStreamToFile(dest);
                        var x = response.ResponseStream;
                    }
                }              
            }

            Console.WriteLine("Successfully done GetFileOnS3");
        }

        public static void DeleteKeyOnS3(string objectKeyName)
        {
            Console.WriteLine("Starting DeleteKeyOnS3 ");
            var bucketName = "pan-dev-clientmetadata";
            var objectKey = objectKeyName;
            var objectContent = "myobject content";

            AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);

            var config = new AmazonS3CryptoConfiguration()
            {
                StorageMode = CryptoStorageMode.ObjectMetadata
            };

            config.RegionEndpoint = Amazon.RegionEndpoint.USEast1;

            using (var client = new AmazonS3Client(credentials, config))
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey
                };
                client.DeleteObject(request);
            }

            Console.WriteLine("Successfully done DeleteKeyOnS3");
        }

        public static void DeleteFolderOnS3(string objectfolderpath)
        {
            Console.WriteLine("Starting DeleteFolderOnS3 ");
            var bucketName = "pan-dev-clientmetadata";
            var objectKey = objectfolderpath;
           

            AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);

            var config = new AmazonS3Config();
          

            config.RegionEndpoint = Amazon.RegionEndpoint.USEast1;

            using (var client = new AmazonS3Client(credentials, config))
            {
                S3DirectoryInfo directoryToDelete = new S3DirectoryInfo(client, bucketName, objectKey);
                directoryToDelete.Delete(false);
            }

           

            Console.WriteLine("Successfully done DeleteFolderOnS3");
        }



        public static string GetKeyOnS3(string objectKeyName)
        {
            Console.WriteLine("Starting GetKeyOnS3 ");
            var bucketName = "pan-dev-clientmetadata";
            var objectKey = objectKeyName;
            string objectContent = null;
         

            AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);

            var config = new AmazonS3CryptoConfiguration()
            {
                StorageMode = CryptoStorageMode.ObjectMetadata
            };

            config.RegionEndpoint = Amazon.RegionEndpoint.USEast1;

            using (var client = new AmazonS3Client(credentials, config))
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey
                };

                using (var response = client.GetObject(request))
                using (var stream = response.ResponseStream)
                using (var reader = new StreamReader(stream))
                {
                    objectContent = reader.ReadToEnd();
                }
            }

            Console.WriteLine("Successfully done GetKeyOnS3 Object Content = " + objectContent);
            return objectContent;

         
        }

      


        public static string CreateCustomerKey()
        {

            AmazonKeyManagementServiceClient client = new AmazonKeyManagementServiceClient(accessKey, secretKey, Amazon.RegionEndpoint.GetBySystemName("us-east-1"));
            Console.WriteLine("Starting Creating External Customer Key");
            var response = client.CreateKey(new CreateKeyRequest
            {
                Origin = "EXTERNAL",
                Description = "ExternalNisha",
                Tags = new List<Amazon.KeyManagementService.Model.Tag> {
                new Amazon.KeyManagementService.Model.Tag {
                TagKey = "CreatedBy",
                TagValue = "NishaUser"
                }
            } // One or more tags. Each tag consists of a tag key and a tag value.
            });

            KeyMetadata keyMetadata = response.KeyMetadata; // An object that contains information about the CMK created by this operation.
            Console.WriteLine("Successfully Created External Customer Key With ID = " + keyMetadata.KeyId);

            return keyMetadata.KeyId;
        }

        public static void ImportCustomerkey(string keyIdToImport)
        {
            Console.WriteLine("Starting Import Customer Key Material for ID = " + keyIdToImport);
            AmazonKeyManagementServiceClient client = new AmazonKeyManagementServiceClient(accessKey, secretKey, Amazon.RegionEndpoint.GetBySystemName("us-east-1"));
            
            var response = client.GetParametersForImport(new GetParametersForImportRequest
            {
                KeyId = keyIdToImport,  //"36a8f447-0d04-4f3d-bcc3-bea68c67e8bb",//"9cfbd369-05cf-4838-ba76-656ac727244e", // The identifier of the CMK for which to retrieve the public key and import token. You can use the key ID or the Amazon Resource Name (ARN) of the CMK.
                WrappingAlgorithm = "RSAES_PKCS1_V1_5",//"RSAES_PKCS1_V1_5", //"RSAES_OAEP_SHA_1", // The algorithm that you will use to encrypt the key material before importing it.
                WrappingKeySpec = "RSA_2048" // The type of wrapping key (public key) to return in the response.
            });

            MemoryStream importToken = response.ImportToken; // The import token to send with a subsequent ImportKeyMaterial request.
            string keyId = response.KeyId; // The ARN of the CMK for which you are retrieving the public key and import token. This is the same CMK specified in the request.
            DateTime parametersValidTo = response.ParametersValidTo; // The time at which the import token and public key are no longer valid.
            MemoryStream publicKey = response.PublicKey; // The public key to use to encrypt the key material before importing it.
            
            // var material = Encrytion(publicKey);
            // var keymaterial = RSAEncrypt("NTEG2018",publicKey);

            // RSA PKCS Encryption by AWS Key received.
             var material = RSA_Encryption.RSAEncryptionFromPublicKey(publicKey);
            //var material = EncryptionMix(publicKey);

            //AES._AESKey = publicKey.ToArray();
            //var material = AES.Encrypt("NTEG2018");

            var response2 = client.ImportKeyMaterial(new ImportKeyMaterialRequest
            {
                EncryptedKeyMaterial = new MemoryStream((material)), // The encrypted key material to import.
                ExpirationModel = "KEY_MATERIAL_DOES_NOT_EXPIRE", // A value that specifies whether the key material expires.
                ImportToken = new MemoryStream(importToken.ToArray()), // The import token that you received in the response to a previous GetParametersForImport request.
                KeyId = keyIdToImport //"36a8f447-0d04-4f3d-bcc3-bea68c67e8bb"//"9cfbd369-05cf-4838-ba76-656ac727244e" // The identifier of the CMK to import the key material into. You can use the key ID or the Amazon Resource Name (ARN) of the CMK.
            });

            Console.WriteLine("Successfully Imported External Customer Key Material With ID = " + keyIdToImport);
        }

        public static void GenerateDataKey(string customerKeyID)
        {
            Console.WriteLine("Starting GenerateDataKey under Customer Key with ID = " + customerKeyID);
            AmazonKeyManagementServiceClient client = new AmazonKeyManagementServiceClient(accessKey, secretKey, Amazon.RegionEndpoint.GetBySystemName("us-east-1"));

            var abc = new Dictionary<string, string>();
            abc.Add("keyId1", "kryvalue");
            var response = client.GenerateDataKey(new GenerateDataKeyRequest
            {
                KeyId = customerKeyID ,// "5f48e9ad-4f0a-40ff-9baa-2e2c6ec61da2", // The identifier of the CMK to use to encrypt the data key. You can use the key ID or Amazon Resource Name (ARN) of the CMK, or the name or ARN of an alias that refers to the CMK.
                KeySpec = "AES_256" ,// Specifies the type of data key to return.
                EncryptionContext = abc
            });

            MemoryStream ciphertextBlob = response.CiphertextBlob; // The encrypted data key.
            string keyId = response.KeyId; // The ARN of the CMK that was used to encrypt the data key.
            MemoryStream plaintext = response.Plaintext; // The unencrypted (plaintext) data key.

            Console.WriteLine("Successfully GenerateDataKey under Customer Key with ID = " + customerKeyID);
        }

        public static void CreataAliasCMK(string targetKeyId, string aliasName)
        {
            Console.WriteLine("Starting Create Alias Customer Key with ID = " + targetKeyId);
            AmazonKeyManagementServiceClient client = new AmazonKeyManagementServiceClient(accessKey, secretKey, Amazon.RegionEndpoint.GetBySystemName("us-east-1"));
            var response = client.CreateAlias(new CreateAliasRequest
            {
                AliasName = "alias/NTEG2018_ExampleAlias", // The alias to create. Aliases must begin with 'alias/'. Do not use aliases that begin with 'alias/aws' because they are reserved for use by AWS.
                TargetKeyId = targetKeyId // "1234abcd-12ab-34cd-56ef-1234567890ab" // The identifier of the CMK whose alias you are creating. You can use the key ID or the Amazon Resource Name (ARN) of the CMK.
            });
            Console.WriteLine("Successfully Created Alias = NTEG2018_ExampleAlias for Customer Key with ID = " + targetKeyId);
        }

       

        static void Main(string[] args)
        {


            //GetKey();
            //  createcustomerkey();
            //generatedatakey();

            //Importcustomerkey();


            //var keyId = CreateCustomerKey();
            //   var keyId = "c6dd0adc-d727-4225-86f9-776f110cc405";
            // CreataAliasCMK(keyId,"testalias");

            //ImportCustomerkey(keyId);

            // PutKeyOnS3WithEncryption(keyId,"MytestdKeyKMS");
            //  var x = GetKeyFromS3WithEncryption(keyId, "MytestdKeyKMS");


            //PutKeyOnS3("Public/");

            PutFileOnS3("Public/_datasets", @"C:\Users\nisha2487\Desktop\Public\_datasets");
            //GetKeyOnS3("66777DE8-BD44-4BD5-872C-0FA39CB5E47D");

            // DeleteKeyOnS3("66777DE8-BD44-4BD5-872C-0FA39CB5E47D");
          //  GetFileOnS3("Public/_datasets/GBP201_NTEG2018-TESTING21MARCH2018/McK/auth0_59815f27945f240950e20759/NTEG2018-Testing21March2018_2018_03_23-22_12.xlsx");
           // GetFileOnS3("Public/_filegroups/abcd.xlsx");

            // Not Working .
            //DeleteFolderOnS3("GBP201_NTEG2018-TESTING21MARCH2018");
            Console.ReadLine();
        }
    }
}
