using System.Text.Json.Serialization;

namespace NovillusPath.Application.DTOs.ContentBlock;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(CreateTextContentDto), typeDiscriminator: nameof(ContentBlockType.Text))]
[JsonDerivedType(typeof(CreateVideoContentDto), typeDiscriminator: nameof(ContentBlockType.Video))]
public abstract record CreateContentBlockBaseDto
{
    public ContentBlockType Type { get; init; }
    public int Order { get; init; }
}
