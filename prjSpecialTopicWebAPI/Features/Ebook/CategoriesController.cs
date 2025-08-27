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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HierarchicalCategoryDto>>> GetCategories()
        {
            // [重大修改] 更新查詢邏輯
            var categoriesWithBooks = await _context.EBookCategories
                .AsNoTracking()
                // [核心修改] 擴充 Where 條件
                .Where(c =>
                    // 條件一：這個分類自己本身底下有書
                    c.EBookMains.Any(b => b.IsAvailable) ||
                    // OR (或)
                    // 條件二：這個分類的任何一個子分類(InverseParentCategory)底下有書
                    c.InverseParentCategory.Any(child => child.EBookMains.Any(b => b.IsAvailable))
                )
                .OrderBy(c => c.CategoryId)
                .Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName,
                    c.ParentCategoryId
                })
                .ToListAsync();

            // --- 以下的階層重組邏輯完全維持不變 ---
            var result = new List<HierarchicalCategoryDto>();
            var categoryLookup = categoriesWithBooks
                .ToDictionary(c => c.CategoryId, c => new HierarchicalCategoryDto
                {
                    Id = c.CategoryId,
                    Name = c.CategoryName
                });

            foreach (var categoryData in categoriesWithBooks)
            {
                if (categoryData.ParentCategoryId.HasValue && categoryLookup.ContainsKey(categoryData.ParentCategoryId.Value))
                {
                    var parent = categoryLookup[categoryData.ParentCategoryId.Value];
                    if (!parent.Children.Any(child => child.Id == categoryData.CategoryId))
                    {
                        parent.Children.Add(new CategoryOptionDto { Id = categoryData.CategoryId, Name = categoryData.CategoryName });
                    }
                }
                else
                {
                    if (!result.Any(r => r.Id == categoryData.CategoryId))
                    {
                        result.Add(categoryLookup[categoryData.CategoryId]);
                    }
                }
            }

            result.RemoveAll(parent =>
                parent.Children.Count == 0 &&
                !categoriesWithBooks.Any(c => c.CategoryId == parent.Id && c.ParentCategoryId == null)
            );

            return Ok(result);
        }
    }
}
