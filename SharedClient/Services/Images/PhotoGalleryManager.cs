using System;
using System.Threading.Tasks;

namespace KinderChat
{
    public class PhotoGalleryManager
    {
        private readonly IPhotoGalleryService photoGallery;
        private const int MaxImageWidth = 1280;
        private const int MaxImageHeight = 768;
        private const int MaxThumbnailWidth = 400;
        private const int MaxThumbnailHeight = 400;

        public PhotoGalleryManager(IPhotoGalleryService photoGallery)
        {
            this.photoGallery = photoGallery;
        }

        public async Task<Photo> PickAndPrepare(bool fromCamera = false)
        {
            var photoBytes = await photoGallery.PickAsync(fromCamera, MaxImageWidth, MaxImageHeight, MaxThumbnailWidth, MaxThumbnailHeight);
            if (photoBytes == null)
                return null;

            var id = await UploadToCloudAsync(photoBytes.Original);

            return new Photo(id, photoBytes.Thumbnail);
        }

        private Task<string> UploadToCloudAsync(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
