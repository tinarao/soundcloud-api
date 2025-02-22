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
        public Task<List<Track>> GetTracksByUser(string userSlug, string? ctxUsername);
        public Task<UpdateTrackDataStatus> UpdateTrackData(UpdateTrackDataDTO dto, int id);
        public Task<DefaultMethodResponseDTO> UpdateTrackPrimaryData(UpdateTrackPrimaryDataDTO dto, string slug, string username);
        public Task<DefaultMethodResponseDTO> DeleteTrack(int trackId, string username);
        public Task<DefaultMethodResponseDTO> ChangeTrackVisibility(string slug, string username, bool newIsPublic);
        public Task<DefaultMethodResponseDTO> IncreaseTrackListens(string slug);
    }
}