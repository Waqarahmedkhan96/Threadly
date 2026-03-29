using ApiContracts.Posts;
using ApiContracts.Comments;
using EfcRepositories;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System.IO;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository _posts;
    private readonly ICommentRepository _comments;
    private readonly EfcRepositories.AppContext _context;

    public PostsController(IPostRepository posts, ICommentRepository comments, EfcRepositories.AppContext context)
    {
        _posts = posts;
        _comments = comments;
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> Create([FromBody] CreatePostDto dto)
    {
        var post = new Post { Title = dto.Title, Body = dto.Body, UserId = dto.AuthorUserId };
        var created = await _posts.AddAsync(post);

        var reloaded = await _context.Posts
            .Include(p => p.Media)
            .Include(p => p.Likes)
            .SingleAsync(p => p.Id == created.Id);

        var result = MapPostToDto(reloaded, null, Request);
        return Created($"/posts/{result.Id}", result);
    }

    [HttpPost("upload")]
    public async Task<ActionResult<PostDto>> CreateWithMedia([FromForm] CreatePostForm form)
    {
        var post = new Post { Title = form.Title, Body = form.Body, UserId = form.AuthorUserId };
        var created = await _posts.AddAsync(post);

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var mediaEntities = new List<PostMedia>();

        foreach (var file in form.Files)
        {
            if (file.Length == 0) continue;

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);

            var mediaType = file.ContentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase) ? "video"
                          : file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) ? "image"
                          : "unknown";

            mediaEntities.Add(new PostMedia
            {
                PostId = created.Id,
                Url = $"/uploads/{fileName}",
                MediaType = mediaType
            });
        }

        if (mediaEntities.Count > 0)
        {
            await _context.PostMedias.AddRangeAsync(mediaEntities);
            await _context.SaveChangesAsync();
        }

        var reloaded = await _context.Posts
            .Include(p => p.Media)
            .Include(p => p.Likes)
            .SingleAsync(p => p.Id == created.Id);

        var result = MapPostToDto(reloaded, null, Request);
        return Created($"/posts/{result.Id}", result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetById(int id, [FromQuery] int? currentUserId = null)
    {
        var query = _posts.GetManyAsync()
            .Where(p => p.Id == id)
            .Include(p => p.Media)
            .Include(p => p.Likes);

        var dto = await query
            .Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Body = p.Body,
                AuthorUserId = p.UserId,
                AuthorName = null,
                Media = p.Media.Select(m => new PostMediaDto
                {
                    Url = m.Url,
                    MediaType = m.MediaType
                }).ToList(),
                LikeCount = p.Likes.Count,
                IsLikedByCurrentUser = currentUserId.HasValue && p.Likes.Any(l => l.UserId == currentUserId.Value),
                Comments = new()
            })
            .FirstOrDefaultAsync();

        if (dto is null)
            return NotFound();

        dto.Media = dto.Media
            .Select(m => new PostMediaDto
            {
                Url = NormalizeMediaUrl(m.Url, Request),
                MediaType = NormalizeMediaType(m.MediaType, m.Url)
            })
            .ToList();

        return Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostSummaryDto>>> GetMany(
        [FromQuery] string? titleContains,
        [FromQuery] int? authorUserId)
    {
        var query = _posts.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(titleContains))
        {
            string term = titleContains.ToLower();
            query = query.Where(p => p.Title.ToLower().Contains(term));
        }

        if (authorUserId is not null)
            query = query.Where(p => p.UserId == authorUserId.Value);

        var list = await query
            .Select(p => new PostSummaryDto
            {
                Id = p.Id,
                Title = p.Title,
                AuthorName = null,
                HasMedia = p.Media.Any(),
                LikeCount = p.Likes.Count,
                IsLiked = false
            })
            .ToListAsync();

        return Ok(list);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePostDto dto)
    {
        var existingPost = await _posts.GetSingleAsync(id);
        existingPost.Title = dto.Title;
        existingPost.Body = dto.Body;

        await _posts.UpdateAsync(existingPost);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var commentIds = await _comments.GetManyAsync()
            .Where(c => c.PostId == id)
            .Select(c => c.Id)
            .ToListAsync();

        foreach (var cid in commentIds)
            await _comments.DeleteAsync(cid);

        await _posts.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:int}/likes")]
    public async Task<ActionResult<LikeStatusDto>> ToggleLike(int id, [FromBody] PostLikeDto dto)
    {
        await _posts.GetSingleAsync(id);

        var existing = await _context.PostLikes.FindAsync(id, dto.UserId);
        if (existing is not null)
        {
            _context.PostLikes.Remove(existing);
            await _context.SaveChangesAsync();
            var count = await _context.PostLikes.CountAsync(l => l.PostId == id);
            return Ok(new LikeStatusDto { LikeCount = count, IsLiked = false });
        }

        _context.PostLikes.Add(new PostLike { PostId = id, UserId = dto.UserId });
        await _context.SaveChangesAsync();
        var newCount = await _context.PostLikes.CountAsync(l => l.PostId == id);
        return Ok(new LikeStatusDto { LikeCount = newCount, IsLiked = true });
    }

    [HttpGet("{id:int}/likes")]
    public async Task<ActionResult<LikeStatusDto>> GetLikeStatus(int id, [FromQuery] int? userId = null)
    {
        await _posts.GetSingleAsync(id);
        var count = await _context.PostLikes.CountAsync(l => l.PostId == id);
        var liked = userId.HasValue && await _context.PostLikes.AnyAsync(l => l.PostId == id && l.UserId == userId.Value);
        return Ok(new LikeStatusDto { LikeCount = count, IsLiked = liked });
    }

    private static PostDto MapPostToDto(Post post, int? currentUserId, HttpRequest request)
    {
        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            AuthorUserId = post.UserId,
            AuthorName = null,
            Media = (post.Media ?? new List<PostMedia>())
                .Select(m => new PostMediaDto
                {
                    Url = NormalizeMediaUrl(m.Url, request),
                    MediaType = NormalizeMediaType(m.MediaType, m.Url)
                })
                .ToList(),
            LikeCount = post.Likes?.Count ?? 0,
            IsLikedByCurrentUser = currentUserId.HasValue && (post.Likes?.Any(l => l.UserId == currentUserId.Value) ?? false),
            Comments = new()
        };
    }

    private static string NormalizeMediaUrl(string? url, HttpRequest request)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        if (Uri.TryCreate(url, UriKind.Absolute, out _))
            return url;

        return $"{request.Scheme}://{request.Host}{url}";
    }

    private static string NormalizeMediaType(string? mediaType, string? url)
    {
        if (!string.IsNullOrWhiteSpace(mediaType))
        {
            var lower = mediaType.Trim().ToLowerInvariant();

            if (lower.StartsWith("image/") || lower == "image")
                return "image";
            if (lower.StartsWith("video/") || lower == "video")
                return "video";
        }

        var ext = Path.GetExtension(url ?? string.Empty).ToLowerInvariant();

        return ext switch
        {
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" or ".bmp" or ".svg" => "image",
            ".mp4" or ".webm" or ".ogg" or ".mov" => "video",
            _ => "unknown"
        };
    }

    public class CreatePostForm
    {
        public required string Title { get; set; }
        public required string Body { get; set; }
        public required int AuthorUserId { get; set; }
        public List<IFormFile> Files { get; set; } = new();
    }
}