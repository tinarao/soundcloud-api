using Microsoft.EntityFrameworkCore;
using Sounds_New.Db;

namespace Sounds_New.Services.SignedUrl
{
    public class SignedUrlService(SoundsContext context) : ISignedUrlService
    {
        private readonly SoundsContext _context = context;

        /// <summary>
        /// Returns filepath by signed URL.
        /// </summary>
        /// <param name="url">Signed url to access the file.</param>
        /// <returns>Absolute file path in the bucket if the url is valid, <see langword="null"/> otherwise.</returns>
        public async Task<string?> GetFilePathByUrl(string url)
        {
            var record = await _context.SignedUrls.FirstOrDefaultAsync(u => u.Url == url);
            if (record == null)
            {
                return null;
            }

            return record.FilePath;
        }

        /// <summary>
        /// Creates a signed URL for accessing a file from the file bucket.
        /// Call this method ONLY if you sure that user is allowed to access the goal content.
        /// </summary>
        /// <param name="filepath">The path of the file in the bucket, relative to the root of the bucket.</param>
        /// <returns>The signed URL</returns>
        public async Task<string> CreateSignedUrl(string filepath)
        {
            var url = Guid.NewGuid();
            url.ToString();

            var newSignedUrl = new Models.SignedUrl
            {
                FilePath = filepath,
                Url = url.ToString()
            };

            await _context.SignedUrls.AddAsync(newSignedUrl);
            await _context.SaveChangesAsync();

            return newSignedUrl.Url;
        }
    }
}