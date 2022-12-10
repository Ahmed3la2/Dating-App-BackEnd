using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BackEnd.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddImageAsync(IFormFile file);

        Task<DeletionResult> DeleteImageAsync(string publicId);
    }
}
