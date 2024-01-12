using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace VideoManager.Model
{
    class HttpClientService
    {
        private static readonly HttpClientService instance = new HttpClientService();
        private readonly HttpClient httpClient;

        private HttpClientService()
        {
            httpClient = new HttpClient();
        }

        public static HttpClientService Instance
        {
            get { return instance; }
        }

        public async Task<string> Get(string apiUrl)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return $"Error: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<Stream> GetStreamAsync(string apiUrl)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStreamAsync();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async void UploadToServer(string filePath, string apiUrl)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //API endpoint for video upload
                    using (var content = new MultipartFormDataContent())
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        // Create a stream content from the file stream
                        var fileContent = new StreamContent(fileStream);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                        // Add the file content to the multipart form data content
                        content.Add(fileContent, "file", Path.GetFileName(filePath));

                        // Send the request to the server
                        HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                        // Checking if the upload was successful
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Video uploaded successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Error uploading video. Status Code: {response.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
