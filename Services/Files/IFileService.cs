using static Sounds_New.Utils.Utilites;

namespace Sounds_New.Services.Files
{
    public interface IFileService
    {
        public Task<string?> GetImageSignedUrl(string slug, FileKind relatedTo, string? username);
    }
}