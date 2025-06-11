namespace NovillusPath.Application.DTOs.Section;

public class UpdateSectionDto
{
    public string? Title { get; set; }
    public int? Order { get; set; } // Sections can be reordered (e.g. when new sections are semantically appropriated between specific topics)
}
