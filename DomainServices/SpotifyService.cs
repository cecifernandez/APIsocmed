using APISocMed.Interfaces;
using APISocMed.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace APISocMed.DomainServices
{
    public class SpotifyService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        //access appsettings.json
        public SpotifyService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public string GetAuthorizationUrl()
        {
            var clientId = _configuration["Spotify:clientId"]!;
            var clientSecret = _configuration["Spotify:clientSecret"]!;
            var redirectUri = _configuration["Spotify:redirectUri"]!;

            var scopes = "user-read-private user-read-email";
            return $"https://accounts.spotify.com/authorize?client_id={clientId}&response_type=code&redirect_uri={redirectUri}&scope={Uri.EscapeDataString(scopes)}";
        }

        public async Task<SpotifyTokenResponse> ExchangeCodeForToken(string code)
        {
            var clientId = _configuration["Spotify:clientId"]!;
            var clientSecret = _configuration["Spotify:clientSecret"]!;
            var redirectUri = _configuration["Spotify:redirectUri"]!;

            using var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            });

            var response = await client.PostAsync("https://accounts.spotify.com/api/token", content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            var tokenResponse = JsonConvert.DeserializeObject<SpotifyTokenResponse>(responseBody);

            //var tokenData = JsonConvert.DeserializeObject<SpotifyTokenResponse>(responseBody);

            //if (tokenData != null)
            //{
            //    string spotifyUserId = await GetSpotifyUserId(tokenData.AccessToken);

            //    await _userRepository.SaveSpotifyTokensAsync(userId, spotifyUserId, tokenData.RefreshToken);
            //}

            return tokenResponse; 
        }

        public async Task<string> GetUserProfile(string accessToken)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.GetAsync("https://api.spotify.com/v1/me");
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        public async Task<string> GetSpotifyUserId(string accessToken)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.GetAsync("https://api.spotify.com/v1/me");
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            var userData = JsonConvert.DeserializeObject<SpotifyUserResponse>(responseBody);
            return userData!.Id;
        }
    }

}
