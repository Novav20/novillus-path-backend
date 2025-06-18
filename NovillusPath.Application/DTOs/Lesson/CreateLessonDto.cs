using NovillusPath.Application.DTOs.ContentBlock;

namespace NovillusPath.Application.DTOs.Lesson;

public class CreateLessonDto
{
    public required string Title { get; set; }
    public int? Order { get; set; }
    public List<CreateContentBlockBaseDto> ContentBlocks { get; set; } = [];
}
