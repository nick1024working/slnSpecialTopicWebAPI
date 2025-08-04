using Microsoft.AspNetCore.Mvc;

namespace prjSpecialTopicWebAPI.Features.Ebook
{
    public class EBooksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
