using ApiContracts.Comments;

namespace ApiContracts.Posts
{
    //  Create
    public class CreatePostDto
    {
        public required string Title { get; set; }
        public required string Body { get; set; }
        public required int AuthorUserId { get; set; }
    }

    // Read / Details 
    public class PostDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Body { get; set; }
        public required int AuthorUserId { get; set; }

        // single author name for display
        public string? AuthorName { get; set; }

        // media attachments for this post
        public List<PostMediaDto> Media { get; set; } = new();

        // likes
        public int LikeCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }

        // full comment list (optional)
        public List<CommentDto> Comments { get; set; } = new();
    }

    public class PostMediaDto
    {
        public required string Url { get; set; }
        public required string MediaType { get; set; }
    }

    public class PostLikeDto
    {
        public required int UserId { get; set; }
    }

    public class LikeStatusDto
    {
        public int LikeCount { get; set; }
        public bool IsLiked { get; set; }
    }

    // Update
    public class UpdatePostDto
    {
        public required string Title { get; set; }
        public required string Body { get; set; }
    }

    // List / Summary 
    public class PostSummaryDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? AuthorName { get; set; }
        public bool HasMedia { get; set; }
        public int LikeCount { get; set; }
        public bool IsLiked { get; set; }
    }

    // Query Parameters 
    public class PostQueryParameters
    {
        public string? TitleContains { get; set; }
        public int? AuthorUserId { get; set; }
    }
}
