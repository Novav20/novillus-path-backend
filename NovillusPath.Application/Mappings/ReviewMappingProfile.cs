using AutoMapper;
using NovillusPath.Application.DTOs.Review;
using NovillusPath.Application.Helpers;
using NovillusPath.Application.Interfaces.Common;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Application.Mappings;

public class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : null))
            .ForMember(dest => dest.UserProfileImageUrl, opt => opt.MapFrom(src => src.User != null ? src.User.ProfilePictureUrl : null))
            .AfterMap((src, dest, context) =>
            {
                if (context.Items.TryGetValue("currentUserService", out var service) && service is ICurrentUserService currentUserService)
                {
                    var canModify = AuthorizationHelper.CanModifyReview(currentUserService, src.UserId);
                    dest.CanEdit = canModify;
                    dest.CanDelete = canModify;
                }
                else
                {
                    // Default values if service is not available
                    dest.CanEdit = false;
                    dest.CanDelete = false;
                }
            });

        CreateMap<CreateReviewDto, Review>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Will be set by service
            .ForMember(dest => dest.User, opt => opt.Ignore())   // Navigation property
            .ForMember(dest => dest.CourseId, opt => opt.Ignore()) // Will be set by service
            .ForMember(dest => dest.Course, opt => opt.Ignore()) // Navigation property
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Default in entity
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); // Default in entity

        CreateMap<UpdateReviewDto, Review>()
            .ForMember(dest => dest.Rating, opt => opt.Condition(src => src.Rating.HasValue))
            .ForMember(dest => dest.Comment, opt => opt.Condition(src => src.Comment != null))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.CourseId, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}
