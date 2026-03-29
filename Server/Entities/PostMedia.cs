namespace Entities;

public class PostMedia
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; } = null!;

    public required string Url { get; set; }
    public required string MediaType { get; set; }
}
