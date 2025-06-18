namespace NovillusPath.Application.DTOs.Lesson;

public class UpdateLessonDto
{
    public string? Title { get; set; }
    public int? Order { get; set; }
    // Updating content blocks is complex:
    // - Add new blocks?
    // - Update existing blocks? (Need Id)
    // - Delete blocks not in the list?
    // - Re-order blocks?
    // This often requires a more sophisticated DTO or multiple endpoints.
    // For now, let's assume we might allow updating Title/Order.
    // Full content block updates might come later or via dedicated content block endpoints.
    // For "basic CRUD" on Day 2, we might simplify this to just allow updating lesson properties,
    // and handle content block management via separate calls to content block endpoints.
    // OR, we can try to handle it. A common DTO for updating a list might include:
    // public List<UpdateContentBlockDtoWithId> ContentBlocksToUpdate { get; set; } = new();
    // public List<CreateContentBlockDto> ContentBlocksToAdd { get; set; } = new();
    // public List<Guid> ContentBlockIdsToDelete { get; set; } = new();

    // For simplicity for Day 2, let's focus UpdateLessonDto on lesson's own properties for now.
    // We'll manage content blocks via their own endpoints or enhance this later.
}
