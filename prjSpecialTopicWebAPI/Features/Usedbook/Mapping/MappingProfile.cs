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
        // BookCategoryGroup 轉換
        CreateMap<CreateBookCategoryGroupRequest, BookCategoryGroup>();
        CreateMap<BookCategoryGroupResult, BookCategoryGroupDto>();

        // BookCategory 轉換
        CreateMap<CreateBookCategoryRequest, BookCategory>();
        CreateMap<BookCategoryResult, BookCategoryDto>();

        // BookSaleTag 轉換
        CreateMap<CreateSaleTagRequest, BookSaleTag>();
        CreateMap<BookSaleTagResult, BookSaleTagDto>();
    }
}
