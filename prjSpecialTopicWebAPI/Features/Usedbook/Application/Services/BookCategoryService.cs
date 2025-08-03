using AutoMapper;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.Repositories;
using prjSpecialTopicWebAPI.Features.Usedbook.Infrastructure.UnitOfWork;

namespace prjSpecialTopicWebAPI.Usedbook.Application.Services
{
    public class BookCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly BookCategoryRepository _bookCategoryRepository;
        private readonly ILogger<BookCategoryService> _logger;

        public BookCategoryService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            BookCategoryRepository bookCategoryRepository,
            ILogger<BookCategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _bookCategoryRepository = bookCategoryRepository;
            _mapper = mapper;
            _logger = logger;
        }
    }
}
