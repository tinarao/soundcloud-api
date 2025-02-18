namespace Sounds_New.Services.SignedUrl
{
    // Main goal of this thing here to manage signed urls created for file access
    public interface ISignedUrlService
    {
        public Task<string> CreateSignedUrl(string filepath);
        public Task<string?> GetFilePathByUrl(string url);
    }
}