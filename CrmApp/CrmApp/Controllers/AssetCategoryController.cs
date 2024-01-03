using CrmApp.Models;
using CrmApp.Models.Entities;
using CrmApp.ViewModel.AssetCategoryViewModels;
using CrmApp.ViewModel.AssetTypeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CrmApp.Controllers
{
    [Authorize(Roles = "admin, varlık kategori")]

    public class AssetCategoryController : Controller
    {
        private readonly CrmAppDbContext _context;

        public AssetCategoryController(CrmAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AssetCategoryCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssetCategoryCreate(AssetCategoryCreateViewModel  model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var result = _context.AssetCategories.Where(x => x.Name == model.Name).FirstOrDefault();
            if (result != null)
            {
                TempData["messageError"] = "Bu kayıt daha önce yapılmış. Lütfen yeni kayıt için tekrar deneyiniz.";

                return View();
            }
            AssetCategory assetCategory = new AssetCategory()
            {
                Code = model.Code,
                Name = model.Name.ToUpper(),
                Description = model.Description
            };

            await _context.AddAsync(assetCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AssetCategoryList));
        }

        public IActionResult AssetCategoryList()
        {
            var result = _context.AssetCategories.ToList();
            var listOfAssetCategory = result.Select(x => new AssetCategoryListViewModel()
            {
                Code = x.Code,
                Name = x.Name,
                Description = x.Description
            }).OrderBy(x=> x.Name).ToList();
            return View(listOfAssetCategory);
        }
    }
}
