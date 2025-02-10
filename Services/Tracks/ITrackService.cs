using Sounds_New.DTO;
using Sounds_New.Models;

namespace Sounds_New.Services.Tracks
{
    public enum UpdateTrackDataStatus
    {
        Success,
        TrackNotFound
    }

    public interface ITrackService
    {
        public Task<Track?> GetTrackBySlug(string slug);
        public Task<CreateTrackResultDTO> CreateTrack(CreateTrackDTO dto, string username);
        public Task<List<Track>> GetHotTracks();
        public Task<UpdateTrackDataStatus> UpdateTrackData(UpdateTrackDataDTO dto, int id);
    }
}