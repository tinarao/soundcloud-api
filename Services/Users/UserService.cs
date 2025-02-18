using Microsoft.EntityFrameworkCore;
using Sounds_New.Db;
using Sounds_New.DTO;
using Sounds_New.Models;

namespace Sounds_New.Services.Users
{
    public class UserService(SoundsContext context) : IUserService
    {
        private readonly SoundsContext _context = context;

        public async Task<User?> GetUserBySlug(string slug)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Slug == slug);
            return user;
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

            float likesPerListen = overallLikes == 0 ? 0 : overallListens / overallLikes;
            float listensPerTrack = overallListens == 0 ? 0 : overallListens / tracksCount;
            float likesPerSubscriber = overallLikes == 0 ? 0 : overallLikes / subscribersCount;
            float listensPerSubscriber = overallListens == 0 ? 0 : overallListens / subscribersCount;
            float likesPerTrack = overallLikes == 0 ? 0 : overallLikes / tracksCount;

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