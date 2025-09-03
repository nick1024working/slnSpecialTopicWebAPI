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
        CreateMap<UsedBook, UpdateBookPayloadDto>();
        CreateMap<UsedBook, EditBookDto>();
        // Detail
        CreateMap<UsedBookDetailQueryResult, PublicUsedBookDetailDto>();
        CreateMap<UsedBookDetailQueryResult, AdminUsedBookDetailDto>();
        // ListItem
        CreateMap<PublicBookListItemQueryResult, PublicBookListItemDto>();
        CreateMap<UserBookListItemQueryResult, UserBookListItemDto>();
        CreateMap<AdminBookListItemQueryResult, AdminBookListItemDto>();


        // BookCategory 轉換
        CreateMap<CreateCategoryRequest, BookCategory>();
        CreateMap<BookCategoryQueryResult, BookCategoryDto>();

        // BookSaleTag 轉換
        CreateMap<CreateSaleTagRequest, BookSaleTag>();
        CreateMap<BookSaleTagQueryResult, BookSaleTagDto>();

        // UserBookOrder 轉換
        CreateMap<CreateOrderRequest, UsedBookOrder>();
        CreateMap<UserOrderListItemQueryResult, UserOrderListItemDto>();
        CreateMap<AdminOrderListItemQueryResult, AdminOrderListItemDto>();

    }
}
