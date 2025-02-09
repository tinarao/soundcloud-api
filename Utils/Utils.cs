using System.Net.Mime;
using Sounds_New.Models;

// file: FieldData<NamedTempFile>,
// callback_api_url: String,

namespace Sounds_New.Utils
{
    public class Utilites
    {
        /// <summary>
        ///   Sends a track to the analysis service for processing.
        /// </summary>
        /// <param name="track"></param>
        /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
        public async static void SendTrackToAnalysisService(Track track)
        {
            var client = new HttpClient();
            var formdata = new MultipartFormDataContent();

            client.BaseAddress = new Uri("http://localhost:4200");

            var audioStream = new FileStream(track.AudioFilePath, FileMode.Open);
            var audioContent = new StreamContent(audioStream);
            audioContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data");

            formdata.Add(audioContent, "file", track.Title);
            formdata.Add(new StringContent($"http://localhost:5129/api/track/update-track-data/{track.Id}"), "callback_api_url");

            var response = await client.PostAsync("/generate", formdata);
            response.EnsureSuccessStatusCode();
        }
    }
}