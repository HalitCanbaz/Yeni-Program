using CrmApp.Models;
using CrmApp.Models.Entities;
using CrmApp.ViewModel.AssetViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace CrmApp.Controllers
{
    [Authorize(Roles = "admin, varlık")]

    public class AssetController : Controller
    {
        private readonly CrmAppDbContext _context;

        public AssetController(CrmAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AssetCreate()
        {
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "NameSurName");
            ViewData["AssetCategoryId"] = new SelectList(_context.AssetCategories, "Id", "Name");
            ViewData["AssetTypeId"] = new SelectList(_context.AssetTypes, "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssetCreate(AssetCreateViewModel model)
        {
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "NameSurName");
            ViewData["AssetCategoryId"] = new SelectList(_context.AssetCategories, "Id", "Name");
            ViewData["AssetTypeId"] = new SelectList(_context.AssetTypes, "Id", "Name");

            if (!ModelState.IsValid)
            {
                return View();
            }

            Asset asset = new Asset()
            {
                Code = model.Code,
                Name = model.Name,
                Description = model.Description,
                AssetCategoryId = model.AssetCategoryId,
                AssetTypeId = model.AssetTypeId,
                AppUserId = model.AppUserId
            };

            await _context.AddAsync(asset);
            await _context.SaveChangesAsync();
            return View();
        }


        public IActionResult AssetList()
        {
            var result = _context.Assets
                .Join(_context.AssetCategories, x => x.AssetCategoryId, y => y.Id, (x, y)
                => new { Assets = x, AssetCategories = y })
                .Join(_context.Users, x => x.Assets.AppUserId, y => y.Id, (x, y)
                => new { x.Assets, x.AssetCategories, Users = y })
                .Join(_context.AssetTypes, x => x.Assets.AssetTypeId, y => y.Id, (x, y)
                => new { x.Assets, x.AssetCategories, x.Users, AssetTypes = y }).ToList();



            var listOfAsset = result.Select(x => new AssetListViewModel()
            {
                Id = x.Assets.Id,
                Code = x.Assets.Code,
                Name = x.Assets.Name,
                Description = x.Assets.Description,
                AssetCategoryName = x.AssetCategories.Name,
                AssetTypneName = x.AssetTypes.Name,
                AppUser = x.Users.NameSurName,
                AppUserId = x.Users.Id,

            }).ToList();


            return View(listOfAsset);
        }

        public IActionResult AssetUserList(int Id)
        {
            var result = _context.Assets.Where(x => x.AppUserId == Id).ToList();
            var user = _context.Users.Where(x=> x.Id == Id).FirstOrDefault();    

            var listOfAssetUser = result.Select(x => new AssetListViewModel()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                AssetCategoryName = x.Name,
                AssetTypneName = x.Name,
                AppUser = user.NameSurName,
                AppUserId = x.Id,

            }).ToList();


            return View(listOfAssetUser);
        }

    }
}
