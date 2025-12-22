namespace SourceGuild.Domain.Entities.Content;

public class TextContent : ContentBlock
{
    public required string Text { get; set; } = string.Empty;
    
}
