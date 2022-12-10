using BackEnd.Helpers;
using BackEnd.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace BackEnd.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        // ConFigration To Authenticate To cloudinary
        public PhotoService(IOptions<CloudinarySetting>  config)
        {
            var acc = new Account
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

             _cloudinary = new Cloudinary(acc);
        }

        // Add Image To cloudinary
        public async Task<ImageUploadResult> AddImageAsync(IFormFile file)
        {
            var UploadReslut = new ImageUploadResult();

            if(file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var UploadParam = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };

                UploadReslut = await _cloudinary.UploadAsync(UploadParam);
            }

            return UploadReslut;
        }

        // Remove Image From cloudinary
        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            var DeletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(DeletionParams);

            return result;

        }
    }
}
