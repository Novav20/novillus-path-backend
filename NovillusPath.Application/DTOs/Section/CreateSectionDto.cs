namespace NovillusPath.Application.DTOs.Section;

public class CreateSectionDto
{
    public required string Title { get; set; }
    public int? Order { get; set; } // allowing for server-side logic to determine the order if not provided.
}
