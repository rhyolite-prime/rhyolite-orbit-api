using System.IO;
using System.Threading.Tasks;
using Abp.Domain.Services;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;

namespace RhyoliteOrbit.CloudinaryApi;

public class FileUploadApi: DomainService, IFileUploadApi
{
    private readonly string _cloudinaryName;
    private readonly string _cloudinaryApiKey;
    private readonly string _cloudinaryApiSecret;
    
    public FileUploadApi(IConfiguration configuration)
    {
        _cloudinaryName = configuration["CloudinarySettings:Cloud"];
        _cloudinaryApiKey = configuration["CloudinarySettings:ApiKey"];
        _cloudinaryApiSecret = configuration["CloudinarySettings:ApiSecret"];
    }
     
    public async Task<string> UploadFileStream(Stream dataFileStream,string fileName,string fileExtension)
    {
        Account account = new Account(_cloudinaryName, _cloudinaryApiKey, _cloudinaryApiSecret);
        var cloudinary = new Cloudinary(account);

        if (!string.IsNullOrEmpty(fileName) && dataFileStream.Length > 0)
        {

            var uploadParams = new RawUploadParams()
            {
                File = new FileDescription($"{fileName}{fileExtension}", dataFileStream),
                Folder = "school_manager_repository",
                Overwrite = true,
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            if (uploadResult != null)
            {
                return uploadResult.Url.ToString();
            }
        }

        return string.Empty;

    }

    private async Task<byte[]> GetBytes(Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            var readAsync = await stream.ReadAsync(bytes, 0, bytes.Length);
            await stream.DisposeAsync();
            return bytes;
        }
}