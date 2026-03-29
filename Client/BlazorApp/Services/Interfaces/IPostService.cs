using System.Collections.Generic;
using System.Threading.Tasks;
using ApiContracts.Posts;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp.Services.Interfaces;

public interface IPostService
{
    Task<PostDto> CreatePostAsync(CreatePostDto request);
    Task<PostDto> CreatePostWithMediaAsync(CreatePostDto request, IEnumerable<IBrowserFile> files);
    Task<PostDto> GetPostByIdAsync(int postId, int? currentUserId = null);
    Task UpdatePostAsync(int postId, UpdatePostDto request);
    Task DeletePostAsync(int postId);
    Task<List<PostSummaryDto>> GetPostsAsync(PostQueryParameters queryParameters);
    Task<List<PostSummaryDto>> GetAllPostsAsync();
    Task<LikeStatusDto> ToggleLikeAsync(int postId, int userId);
    Task<LikeStatusDto> GetLikeStatusAsync(int postId, int? userId = null);
}
