using Microsoft.EntityFrameworkCore;
using Sounds_New.Db;
using Sounds_New.DTO;
using Sounds_New.Models;

namespace Sounds_New.Services.Users
{
    public class UserService(SoundsContext context) : IUserService
    {
        private readonly SoundsContext _context = context;

        public async Task<DefaultMethodResponseDTO> ChangeUserAvatar(IFormFile newAvatar, string username)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return new DefaultMethodResponseDTO
                {
                    IsOk = false,
                    StatusCode = 404,
                    Message = "Пользователь не найден"
                };
            }

            var avatarFileName = $"{Guid.NewGuid()}_{newAvatar.FileName}";
            var avatarFilePath = Path.Combine(uploadsFolder, avatarFileName);

            using var stream = new FileStream(avatarFilePath, FileMode.Create);
            await newAvatar.CopyToAsync(stream);

            user.AvatarFilePath = avatarFileName;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new DefaultMethodResponseDTO
            {
                IsOk = true,
                StatusCode = 200,
                Message = "Успешно"
            };
        }

        public async Task<User?> GetUserBySlug(string slug)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Slug == slug);
            return user;
        }

        public async Task<DefaultMethodResponseDTO> SetUserLinks(SetUserLinksDTO dto, string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return new DefaultMethodResponseDTO
                {
                    IsOk = false,
                    StatusCode = 404,
                    Message = "Пользователь не найден"
                };
            }

            user.Links = dto.Links;
            await _context.SaveChangesAsync();
            return new DefaultMethodResponseDTO
            {
                IsOk = true,
                StatusCode = 200,
                Message = "Обновлено"
            };
        }

        public async Task<UserPrimaryDataDTO?> GetUserPrimaryDataBySlug(string slug)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Slug == slug);

            if (user == null)
            {
                return null;
            }

            var tracksCount = await _context
                .Tracks
                .Include(t => t.User)
                .Where(t => t.IsPublic == true)
                .CountAsync(t => t.User.Slug == slug);


            var subscribersCount = await _context
                .Subscriptions
                .Include(s => s.User)
                .CountAsync(u => u.User.Slug == slug);

            var totalListens = await _context
                .Tracks
                .Include(t => t.User)
                .Where(t => t.IsPublic)
                .Where(t => t.User.Slug == slug)
                .SumAsync(t => t.Listens);

            return new UserPrimaryDataDTO
            {
                Id = user.Id,
                Username = user.Username,
                Bio = user.Bio,
                CreatedAt = user.CreatedAt,
                AvatarFilePath = user.AvatarFilePath,
                BannerFilePath = user.BannerFilePath,
                Links = user.Links,
                SubscribersCount = subscribersCount,
                TracksCount = tracksCount,
                Slug = user.Slug,
                TotalListens = totalListens
            };
        }

        public async Task<UserStatisticDTO> GetUserStatistics(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                var result = new UserStatisticDTO
                {
                    IsOk = false,
                    Message = "Пользователь не найден",
                    StatusCode = 404,
                };
                return result;
            }

            var tracks = await _context.Tracks.AsNoTracking().Where(t => t.UserId == user.Id).ToListAsync();
            var subscribers = await _context.Subscriptions.AsNoTracking().Where(s => s.User.Id == user.Id).ToListAsync();

            var overallLikes = 0;
            var overallListens = 0;
            var publicTracksCount = 0;
            var subscribersCount = subscribers.Count;
            var tracksCount = tracks.Count;

            foreach (var track in tracks)
            {
                overallLikes += track.Likes;
                overallListens += track.Listens;

                if (track.IsPublic)
                {
                    publicTracksCount += 1;
                }
            }

            float likesPerListen = (overallLikes == 0 || overallListens == 0) ? 0 : overallListens / overallLikes;
            float listensPerTrack = (overallListens == 0 || tracksCount == 0) ? 0 : overallListens / tracksCount;
            float likesPerSubscriber = (overallLikes == 0 || subscribersCount == 0) ? 0 : overallLikes / subscribersCount;
            float listensPerSubscriber = (overallListens == 0 || subscribersCount == 0) ? 0 : overallListens / subscribersCount;
            float likesPerTrack = (overallLikes == 0 || tracksCount == 0) ? 0 : overallLikes / tracksCount;

            return new UserStatisticDTO
            {
                IsOk = true,
                Message = "Успешно",
                StatusCode = 200,
                OverallListens = overallListens,
                OverallLikes = overallLikes,
                TracksCount = tracksCount,
                PublicTracksCount = publicTracksCount,
                LikesPerListen = likesPerListen,
                ListensPerTrack = listensPerTrack,
                SubscribersCount = subscribersCount,
                LikesPerSubscriber = likesPerSubscriber,
                ListensPerSubscriber = listensPerSubscriber,
                LikesPerTrack = likesPerTrack,
            };
        }
    }
}