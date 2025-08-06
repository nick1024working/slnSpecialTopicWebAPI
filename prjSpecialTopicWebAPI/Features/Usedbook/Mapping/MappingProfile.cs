using AutoMapper;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Responses;
using prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Results;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // UsedBook 轉換
        CreateMap<CreateBookRequest, UsedBook>();
        CreateMap<UsedBook, EditBookDto>();
        CreateMap<PublicBookListItemQueryResult, PublicBookListItemDto>();
        CreateMap<UserBookListItemQueryResult, UserBookListItemDto>();
        CreateMap<AdminBookListItemQueryResult, AdminBookListItemDto>();

        // BookImage 轉換
        CreateMap<UsedBookImageQueryResult, BookImageDto>();

        // BookCategory 轉換
        CreateMap<CreateBookCategoryRequest, BookCategory>();
        CreateMap<BookCategoryQueryResult, BookCategoryDto>();

        // BookSaleTag 轉換
        CreateMap<CreateSaleTagRequest, BookSaleTag>();
        CreateMap<BookSaleTagQueryResult, BookSaleTagDto>();
    }
}
