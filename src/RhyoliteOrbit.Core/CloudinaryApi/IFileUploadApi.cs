using System.IO;
using System.Threading.Tasks;
using Abp.Domain.Services;

namespace RhyoliteOrbit.CloudinaryApi;

public interface IFileUploadApi: IDomainService
{
    Task<string> UploadFileStream(Stream dataFileStream, string fileName, string fileExtension, string folderName);

    Task<string> SingleUpload(string base64Image, string folderName);
}