using System.Text.Json.Serialization;
using NovillusPath.Domain.Enums;

namespace NovillusPath.Application.DTOs.ContentBlock;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(TextContentDto), typeDiscriminator: nameof(ContentBlockType.Text))]
[JsonDerivedType(typeof(VideoContentDto), typeDiscriminator: nameof(ContentBlockType.Video))]
public abstract class ContentBlockDto
{
    public Guid Id { get; init; }
    public int Order { get; init; }

    [JsonIgnore]
    public ContentBlockType Type { get; protected set; } // Discriminator for client
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
