using SourceGuild.Application.DTOs.ContentBlock;

namespace SourceGuild.Application.DTOs.Lesson;

public record CreateLessonDto
{
    public required string Title { get; init; }
    public int? Order { get; init; }
    public List<CreateContentBlockBaseDto> ContentBlocks { get; init; } = [];
}
