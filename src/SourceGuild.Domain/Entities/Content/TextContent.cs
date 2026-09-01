using SourceGuild.Domain.Common;

namespace SourceGuild.Domain.Entities.Content;

public class TextContent : ContentBlock
{
    private TextContent() { }

    internal static Result<TextContent> Create(Guid lessonId, string text, int order)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Result<TextContent>.Failure(Error.Validation("TextContent.Empty", "El texto del bloque de contenido no puede estar vacío."));

        var content = new TextContent
        {
            Id = Guid.NewGuid(),
            LessonId = lessonId,
            Text = text.Trim(),
            Order = order,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return Result<TextContent>.Success(content);
    }

    public string Text { get; private set; } = string.Empty;

    public Result UpdateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Result.Failure(Error.Validation("TextContent.Empty", "El texto del bloque de contenido no puede estar vacío."));

        Text = text.Trim();
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}