using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // [修改] 請務必加入這一行！
using prjSpecialTopicWebAPI.Features.Ebook.DTOs;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Features.Ebook
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly TeamAProjectContext _context;

        public CategoriesController(TeamAProjectContext context)
        {
            _context = context;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HierarchicalCategoryDto>>> GetCategories()
        {
            // 1. 找出所有底下有 "上架中" 書籍的分類
            var categoriesWithBooks = await _context.EBookCategories
                .AsNoTracking()
                .Where(c => c.EBookMains.Any(b => b.IsAvailable)) // [核心] 只選擇有書的分類
                .OrderBy(c => c.CategoryId)
                .Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName,
                    c.ParentCategoryId
                })
                .ToListAsync();

            // 2. 在記憶體中將扁平列表重建成階層結構
            var result = new List<HierarchicalCategoryDto>();
            var categoryLookup = categoriesWithBooks
                .ToDictionary(c => c.CategoryId, c => new HierarchicalCategoryDto
                {
                    Id = c.CategoryId,
                    Name = c.CategoryName
                });

            foreach (var categoryData in categoriesWithBooks)
            {
                // 如果是子分類，就把它加到對應的父分類的 Children 列表中
                if (categoryData.ParentCategoryId.HasValue && categoryLookup.ContainsKey(categoryData.ParentCategoryId.Value))
                {
                    var parent = categoryLookup[categoryData.ParentCategoryId.Value];
                    parent.Children.Add(new CategoryOptionDto { Id = categoryData.CategoryId, Name = categoryData.CategoryName });
                }
                // 如果是父分類 (或沒有父分類)，就把它加到最外層結果中
                else
                {
                    // 確保不重複加入
                    if (!result.Any(r => r.Id == categoryData.CategoryId))
                    {
                        result.Add(categoryLookup[categoryData.CategoryId]);
                    }
                }
            }

            return Ok(result);
        }
    }
}
