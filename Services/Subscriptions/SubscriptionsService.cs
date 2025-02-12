using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sounds_New.Db;
using Sounds_New.DTO;
using Sounds_New.Models;

namespace Sounds_New.Services.Subscriptions
{
    public class SubscriptionsService(SoundsContext context) : ISubscriptionsService
    {
        private readonly SoundsContext _context = context;

        public async Task<DefaultMethodResponseDTO> SubscribeTo(int userToBeSubscribedToId, string currentUserName)
        {
            var userToBeSubscribedTo = await _context.Users.FindAsync(userToBeSubscribedToId);
            if (userToBeSubscribedTo == null)
            {
                return new DefaultMethodResponseDTO
                {
                    IsOk = false,
                    Message = "Пользователь не найден",
                    StatusCode = 404
                };
            }

            var currentUser = await _context.Users.FirstAsync(u => u.Username == currentUserName);
            if (currentUser == null)
            {
                return new DefaultMethodResponseDTO
                {
                    IsOk = false,
                    Message = "Ошибка авторизации",
                    StatusCode = 401
                };
            }

            var isAlreadySubscribed = await _context.Subscriptions
                    .Where(s => s.User == userToBeSubscribedTo)
                    .Where(s => s.Subscriber.Id == currentUser.Id)
                    .FirstOrDefaultAsync();

            if (isAlreadySubscribed != null)
            {
                return new DefaultMethodResponseDTO
                {
                    IsOk = false,
                    Message = $"Вы уже подписаны на {userToBeSubscribedTo.Username}!",
                    StatusCode = 400
                };
            }

            var newSubscription = new Subscription
            {
                User = userToBeSubscribedTo,
                Subscriber = currentUser
            };
            var isCreated = await _context.Subscriptions.AddAsync(newSubscription);

            await _context.SaveChangesAsync();

            return new DefaultMethodResponseDTO
            {
                IsOk = true,
                Message = "Ok",
                StatusCode = 204
            };
        }

        public async Task<DefaultMethodResponseDTO> UnsubscribeFrom(int userToBeUnsubscribedFromId, string currentUserName)
        {
            var userToBeUnsubscribedFrom = await _context.Users.FindAsync(userToBeUnsubscribedFromId);
            if (userToBeUnsubscribedFrom == null)
            {
                return new DefaultMethodResponseDTO
                {
                    IsOk = false,
                    Message = "Пользователь не найден",
                    StatusCode = 404
                };
            }

            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == currentUserName);
            if (currentUser == null)
            {
                return new DefaultMethodResponseDTO
                {
                    IsOk = false,
                    Message = "Ошибка авторизации",
                    StatusCode = 401
                };
            }

            var subscription = await _context.Subscriptions
                .Where(s => s.Subscriber.Id == currentUser.Id)
                .Where(s => s.User.Id == userToBeUnsubscribedFrom.Id)
                .FirstOrDefaultAsync();
            if (subscription == null)
            {
                return new DefaultMethodResponseDTO
                {
                    IsOk = false,
                    Message = "Подписка не существует",
                    StatusCode = 400
                };
            }

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();

            return new DefaultMethodResponseDTO
            {
                IsOk = true,
                Message = "Ok",
                StatusCode = 204
            };
        }

        public async Task<List<User>?> GetSubscribersByUserId(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

            var subscribers = new List<User>();
            // var subscriptions = await _context.Subscriptions
            //     .Where(s => s.User.Id == user.Id)
            //     .ToListAsync();

            var subscriptions = await _context.Subscriptions
                .Where(s => s.User.Id == user.Id)
                .Include(s => s.Subscriber)
                .ToListAsync();

            subscriptions.ForEach(s => subscribers.Add(s.Subscriber));

            return subscribers;
        }
    }
}