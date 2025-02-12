using Sounds_New.DTO;
using Sounds_New.Models;

namespace Sounds_New.Services.Subscriptions
{
    public interface ISubscriptionsService
    {
        Task<DefaultMethodResponseDTO> SubscribeTo(int userToBeSubscribedToId, string currentUserName);
        Task<DefaultMethodResponseDTO> UnsubscribeFrom(int userToBeUnsubscribedFromId, string currentUserName);

        Task<List<User>?> GetSubscribersByUserId(int id);
    }
}