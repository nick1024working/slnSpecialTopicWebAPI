using System.ComponentModel.DataAnnotations;

namespace prjSpecialTopicWebAPI.Features.Usedbook.Application.DTOs.Requests
{
    public class UpdateBookRequest
    {
        [Display(Name = "賣家所在縣市")]
        [Required(ErrorMessage = "縣市為必填欄位")]
        public int SellerCountyId { get; set; }

        [Display(Name = "賣家所在鄉鎮市區")]
        [Required(ErrorMessage = "鄉鎮市區為必填欄位")]
        public int SellerDistrictId { get; set; }

        [Display(Name = "售價")]
        [Required(ErrorMessage = "售價為必填欄位")]
        [RegularExpression(@"^\d{1,5}$", ErrorMessage = "售價需為整數，不能超過99999")]
        public decimal SalePrice { get; set; }

        [Display(Name = "書名")]
        [Required(ErrorMessage = "書名為必填欄位")]
        [StringLength(50, ErrorMessage = "書名不可超過 50 字")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "作者")]
        [Required(ErrorMessage = "作者為必填欄位")]
        [StringLength(100, ErrorMessage = "作者不可超過 100 字")]
        public string Authors { get; set; } = string.Empty;

        [Display(Name = "主題分類")]
        [Required(ErrorMessage = "主題分類為必填欄位")]
        public int CategoryId { get; set; }

        [Display(Name = "書況評等")]
        [Required(ErrorMessage = "書況為必填欄位")]
        public int ConditionRatingId { get; set; }

        /* --- 描述欄位 --- */

        [Display(Name = "書況描述")]
        [StringLength(100, ErrorMessage = "書況描述長度不可超過 100 字")]
        public string? ConditionDescription { get; set; }

        [Display(Name = "版次／刷次")]
        [StringLength(10, ErrorMessage = "版次／刷次不可超過 10 字")]
        public string? Edition { get; set; }

        [Display(Name = "出版社")]
        [StringLength(50, ErrorMessage = "出版社不可超過 50 字")]
        public string? Publisher { get; set; }

        [Display(Name = "出版日期")]
        public DateOnly? PublicationDate { get; set; }

        [Display(Name = "ISBN")]
        [StringLength(13, ErrorMessage = "ISBN 不可超過 13 字")]
        [RegularExpression(@"^\d{10}(\d{3})$", ErrorMessage = "需為數字")]
        public string? Isbn { get; set; }

        [Display(Name = "裝訂方式")]
        public int? BindingId { get; set; }

        [Display(Name = "語言")]
        public int? LanguageId { get; set; }

        [Display(Name = "頁數")]
        [RegularExpression(@"^\d{1,4}$", ErrorMessage = "頁數需為整數，不能超過9999")]
        public int? Pages { get; set; }

        [Display(Name = "內容分級")]
        [Required(ErrorMessage = "內容分級為必填欄位")]
        public int ContentRatingId { get; set; }

        /* --- 狀態欄位 --- */

        [Display(Name = "是否上架")]
        [Required]
        public bool IsOnShelf { get; set; }
    }
}
