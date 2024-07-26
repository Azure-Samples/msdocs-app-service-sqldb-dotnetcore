using DotNetCoreSqlDb.Models;
using DotNetCoreSqlDb.Repository;
using DotNetCoreSqlDb.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreSqlDb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOFWork _unitOfWork;
        public CategoryController(IUnitOFWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upsert(Category category)
        {
            if (category.Id == 0)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }

        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.Category.GetAll() });
        }
        #endregion
    }
}
