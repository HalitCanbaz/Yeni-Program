using CrmApp.Models;
using CrmApp.Models.Entities;
using CrmApp.ViewModel.AssetTypeViewModels;
using CrmApp.ViewModel.FaultViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CrmApp.Controllers
{
    [Authorize(Roles = "admin")]

    public class AssetTypeController : Controller
    {
        private readonly CrmAppDbContext _context;

        public AssetTypeController(CrmAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AssetTypeCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssetTypeCreate(AssetTypeCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var result = _context.AssetTypes.Where(x => x.Name == model.Name).FirstOrDefault();
            if (result != null)
            {
                TempData["messageError"] = "Bu kayıt daha önce yapılmış. Lütfen yeni kayıt için tekrar deneyiniz.";

                return View();
            }


            AssetType assetType = new AssetType()
            {
                Code = model.Code,
                Name = model.Name.ToUpper(),
                Description = model.Description
            };

            await _context.AddAsync(assetType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AssetTypeList));
        }

        public IActionResult AssetTypeList()
        {
            var result = _context.AssetTypes.ToList();
            var listOfAssetType = result.Select(x => new AssetTypeListViewmodel()
            {
                Code = x.Code,
                Name = x.Name,
                Description = x.Description
            }).OrderBy(x => x.Name).ToList();
            return View(listOfAssetType);
        }


    }
}
