using Microsoft.EntityFrameworkCore;
using Sounds_New.Db;
using Sounds_New.Services.SignedUrl;
using static Sounds_New.Utils.Utilites;

namespace Sounds_New.Services.Files
{
    public class FileService(SoundsContext context, ISignedUrlService signedUrlService) : IFileService
    {
        private readonly SoundsContext _context = context;
        private readonly ISignedUrlService _signedUrlService = signedUrlService;

        public async Task<string?> GetImageSignedUrl(string slug, FileKind kind, string? username)
        {
            string? url = null;

            switch (kind)
            {
                case FileKind.UserAvatar:
                    url = await GetUserAvatarFileUrl(slug);
                    break;
                case FileKind.TrackArtwork:
                    url = await GetTrackArtworkFileUrl(slug, username);
                    break;
                case FileKind.TrackAudio:
                    url = await GetAudioFileUrl(slug, username);
                    break;
            }

            return url;
        }

        private async Task<string?> GetUserAvatarFileUrl(string slug)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Slug == slug);
            if (user == null || user.AvatarFilePath == null)
            {
                return null;
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            string filePath = Path.Combine(uploadsFolder, user.AvatarFilePath);

            var signedUrl = await _signedUrlService.CreateSignedUrl(filePath);
            return signedUrl;
        }

        private async Task<string?> GetTrackArtworkFileUrl(string slug, string? ctxUsername)
        {
            var track = await _context.Tracks.Include(t => t.User).FirstOrDefaultAsync(t => t.Slug == slug);
            if (track == null)
            {
                return null;
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            string filePath = Path.Combine(uploadsFolder, track.ImageFilePath);

            if (track.IsPublic)
            {
                var signedUrl = await _signedUrlService.CreateSignedUrl(filePath);
                if (signedUrl == null)
                {
                    return null;
                }

                return signedUrl;
            }

            if (!track.IsPublic && ctxUsername == null)
            {
                return null;
            }

            if (!track.IsPublic && track.User.Username != ctxUsername)
            {
                return null;
            }

            var aSignedUrl = await _signedUrlService.CreateSignedUrl(filePath);
            return aSignedUrl;
        }

        private async Task<string?> GetAudioFileUrl(string slug, string? ctxUsername)
        {
            var track = await _context.Tracks.Include(t => t.User).FirstOrDefaultAsync(t => t.Slug == slug);
            if (track == null)
            {
                return null;
            }

            if (!track.IsPublic && ctxUsername == null)
            {
                return null;
            }

            if (!track.IsPublic && track.User.Username != ctxUsername)
            {
                return null;
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            string filePath = Path.Combine(uploadsFolder, track.AudioFilePath);

            var signedUrl = await _signedUrlService.CreateSignedUrl(filePath);
            return signedUrl;
        }
    }
}