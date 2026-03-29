using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiContracts.Posts;
using BlazorApp.Services.Interfaces;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp.Services.HttpPostService
{
    public class HttpPostService : IPostService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public HttpPostService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ✅ Create a new post
        public async Task<PostDto> CreatePostAsync(CreatePostDto request)
        {
            var response = await _httpClient.PostAsJsonAsync("posts", request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(body);

            return JsonSerializer.Deserialize<PostDto>(body, _json)!;
        }

        public async Task<PostDto> CreatePostWithMediaAsync(CreatePostDto request, IEnumerable<IBrowserFile> files)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(request.Title), "Title");
            content.Add(new StringContent(request.Body), "Body");
            content.Add(new StringContent(request.AuthorUserId.ToString()), "AuthorUserId");

            foreach (var file in files ?? Enumerable.Empty<IBrowserFile>())
            {
                var streamContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 50_000_000));
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(streamContent, "Files", Path.GetFileName(file.Name));
            }

            var response = await _httpClient.PostAsync("posts/upload", content);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception(body);

            return JsonSerializer.Deserialize<PostDto>(body, _json)!;
        }

        // ✅ Get post by ID
        public async Task<PostDto> GetPostByIdAsync(int postId, int? currentUserId = null)
        {
            var url = $"posts/{postId}";
            if (currentUserId.HasValue)
                url += $"?currentUserId={currentUserId.Value}";

            var post = await _httpClient.GetFromJsonAsync<PostDto>(url);
            return post ?? throw new Exception($"Post with ID {postId} not found");
        }

        // ✅ Update an existing post
        public async Task UpdatePostAsync(int postId, UpdatePostDto request)
        {
            var response = await _httpClient.PutAsJsonAsync($"posts/{postId}", request);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception(body);
            }
        }

        // ✅ Delete a post
        public async Task DeletePostAsync(int postId)
        {
            var response = await _httpClient.DeleteAsync($"posts/{postId}");
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception(body);
            }
        }

        public async Task<LikeStatusDto> ToggleLikeAsync(int postId, int userId)
        {
            var response = await _httpClient.PostAsJsonAsync($"posts/{postId}/likes", new PostLikeDto { UserId = userId });
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception(body);

            return JsonSerializer.Deserialize<LikeStatusDto>(body, _json)!;
        }

        public async Task<LikeStatusDto> GetLikeStatusAsync(int postId, int? userId = null)
        {
            var url = $"posts/{postId}/likes";
            if (userId.HasValue)
                url += $"?userId={userId.Value}";

            var status = await _httpClient.GetFromJsonAsync<LikeStatusDto>(url);
            return status ?? new LikeStatusDto { LikeCount = 0, IsLiked = false };
        }

        // ✅ Get posts with optional filters (titleContains, authorUserId)
        public async Task<List<PostSummaryDto>> GetPostsAsync(PostQueryParameters queryParameters)
        {
            var url = "posts";

            var queryParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(queryParameters.TitleContains))
                queryParts.Add($"titleContains={Uri.EscapeDataString(queryParameters.TitleContains)}");
            if (queryParameters.AuthorUserId.HasValue)
                queryParts.Add($"authorUserId={queryParameters.AuthorUserId.Value}");

            if (queryParts.Count > 0)
                url += "?" + string.Join("&", queryParts);

            var posts = await _httpClient.GetFromJsonAsync<List<PostSummaryDto>>(url);
            return posts ?? new List<PostSummaryDto>();
        }

        // ✅ Get all posts (no filters)
        public async Task<List<PostSummaryDto>> GetAllPostsAsync()
        {
            var posts = await _httpClient.GetFromJsonAsync<List<PostSummaryDto>>("posts");
            return posts ?? new List<PostSummaryDto>();
        }
    }
}
