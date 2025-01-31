using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minio;
using Minio.DataModel.Args;
namespace Common
{
    public static class MinioHelper
    {
        private static readonly IMinioClient MinioClient = new MinioClient()
                                                                .WithEndpoint("127.0.0.1:9000")
                                                                .WithCredentials("admin", "password")
                                                                .Build();

        public static async Task<string?> GetImageUrl(string bucketName, string objectName, int expiryInSeconds)
        {
            try
            {
                var presignedGetObjectArgs = new PresignedGetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithExpiry(expiryInSeconds);

                return await MinioClient.PresignedGetObjectAsync(presignedGetObjectArgs);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Ошибка при генерации URL: {exception.Message}");
                return null;
            }
        }
    }
}
