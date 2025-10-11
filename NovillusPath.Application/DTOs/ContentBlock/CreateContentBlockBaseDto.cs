using System.Text.Json.Serialization;

namespace NovillusPath.Application.DTOs.ContentBlock;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(CreateTextContentDto), typeDiscriminator: nameof(ContentBlockType.Text))]
[JsonDerivedType(typeof(CreateVideoContentDto), typeDiscriminator: nameof(ContentBlockType.Video))]
public abstract class CreateContentBlockBaseDto
{
    public ContentBlockType Type { get; set; }
    public int Order { get; set; }
}
