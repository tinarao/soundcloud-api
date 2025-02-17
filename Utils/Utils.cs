using System.Net.Mime;
using Sounds_New.Models;

// file: FieldData<NamedTempFile>,
// callback_api_url: String,

namespace Sounds_New.Utils
{
    public class Utilites
    {
        public async static void SendTrackToAnalysisService(Track track)
        {
            var client = new HttpClient();
            var formdata = new MultipartFormDataContent();

            client.BaseAddress = new Uri("http://localhost:4200");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            var trackFilePath = Path.Combine(uploadsFolder, track.AudioFilePath);

            var audioStream = new FileStream(trackFilePath, FileMode.Open);
            var audioContent = new StreamContent(audioStream);
            audioContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data");

            formdata.Add(audioContent, "file", track.Title);
            formdata.Add(new StringContent($"http://localhost:5129/api/track/update-track-data/{track.Id}"), "callback_api_url");

            var response = await client.PostAsync("/generate", formdata);
            response.EnsureSuccessStatusCode();
        }

        public static string? GetIdentityUserName(HttpContext ctx)
        {
            if (ctx.User == null || ctx.User.Identity == null)
            {
                return null;
            }

            return ctx.User.Identity.Name;
        }
    }
}