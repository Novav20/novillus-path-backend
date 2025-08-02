using Moq;
using AutoMapper;
using NovillusPath.Application.DTOs.Lesson;
using NovillusPath.Application.DTOs.ContentBlock;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Application.Interfaces.Common;
using NovillusPath.Application.Services;
using NovillusPath.Domain.Entities;
using NovillusPath.Domain.Entities.Content;
using NovillusPath.Domain.Enums;

namespace NovillusPath.Test;

public class LessonServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<ISectionRepository> _sectionRepoMock = new();
    private readonly Mock<ILessonRepository> _lessonRepoMock = new();

    public LessonServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.SectionRepository).Returns(_sectionRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.LessonRepository).Returns(_lessonRepoMock.Object);
    }

    [Fact]
    public async Task CreateLessonAsync_MapsContentBlocks_Correctly()
    {
        // Arrange
        var sectionId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();
        var section = new Section
        {
            Id = sectionId,
            Title = "Test Section",
            Course = new Course
            {
                InstructorId = instructorId
            }
        };
        var createLessonDto = new CreateLessonDto
        {
            Title = "Test Lesson",
            ContentBlocks =
            [
                new CreateTextContentDto { Text = "Hello", Order = 0, Type = ContentBlockType.Text },
                new CreateVideoContentDto { VideoUrl = "http://video", Order = 1, DurationMinutes = 5, Type = ContentBlockType.Video }
            ]
        };
        _sectionRepoMock.Setup(r => r.GetSectionWithCourseAsync(sectionId, It.IsAny<CancellationToken>())).ReturnsAsync(section);
        _currentUserServiceMock.Setup(u => u.UserId).Returns(instructorId);
        _currentUserServiceMock.Setup(u => u.IsInRole("Admin")).Returns(false);
        _lessonRepoMock
            .Setup(r => r.ListAsync<int>(
                It.IsAny<System.Linq.Expressions.Expression<Func<Lesson, bool>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Lesson, int>>>(),
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _lessonRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Lesson>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Lesson lesson, CancellationToken _) => lesson);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Setup mapping for lesson and content blocks
        _mapperMock.Setup(m => m.Map<Lesson>(It.IsAny<CreateLessonDto>())).Returns(new Lesson { Title = createLessonDto.Title, ContentBlocks = [] });
        _mapperMock.Setup(m => m.Map<TextContent>(It.IsAny<CreateContentBlockBaseDto>())).Returns((CreateContentBlockBaseDto dto) => new TextContent { Text = ((CreateTextContentDto)dto).Text });
        _mapperMock.Setup(m => m.Map<VideoContent>(It.IsAny<CreateContentBlockBaseDto>())).Returns((CreateContentBlockBaseDto dto) => new VideoContent { VideoUrl = ((CreateVideoContentDto)dto).VideoUrl, DurationMinutes = ((CreateVideoContentDto)dto).DurationMinutes });
        _mapperMock.Setup(m => m.Map<LessonDto>(It.IsAny<Lesson>())).Returns(new LessonDto { Title = createLessonDto.Title, Status = "Draft" });

        var service = new LessonService(_unitOfWorkMock.Object, _mapperMock.Object, _currentUserServiceMock.Object);

        // Act
        var result = await service.CreateLessonAsync(sectionId, createLessonDto, CancellationToken.None);

        // Assert
        _sectionRepoMock.Verify(r => r.GetSectionWithCourseAsync(sectionId, It.IsAny<CancellationToken>()), Times.Once);
        _lessonRepoMock.Verify(r => r.AddAsync(It.IsAny<Lesson>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<TextContent>(It.IsAny<CreateContentBlockBaseDto>()), Times.Once);
        _mapperMock.Verify(m => m.Map<VideoContent>(It.IsAny<CreateContentBlockBaseDto>()), Times.Once);
        Assert.Equal(createLessonDto.Title, result.Title);
    }
}