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

        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.Category.GetAll() });
        }
        #endregion
    }
}
