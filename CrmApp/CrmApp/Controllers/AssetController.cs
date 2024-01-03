using CrmApp.Models;
using CrmApp.Models.Entities;
using CrmApp.ViewModel.AssetViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace CrmApp.Controllers
{

    public class AssetController : Controller
    {
        private readonly CrmAppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public AssetController(CrmAppDbContext context, UserManager<AppUser> userManager = null)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "admin, varlık")]
        public IActionResult AssetCreate()
        {
            ViewData["AppUserId"] = new SelectList(_context.Users.OrderBy(x => x.NameSurName), "Id", "NameSurName");
            ViewData["AssetCategoryId"] = new SelectList(_context.AssetCategories.OrderBy(x => x.Name), "Id", "Name");
            ViewData["AssetTypeId"] = new SelectList(_context.AssetTypes.OrderBy(x => x.Name), "Id", "Name");
            return View();
        }
        [Authorize(Roles = "admin, varlık")]
        [HttpPost]
        public async Task<IActionResult> AssetCreate(AssetCreateViewModel model)
        {
            ViewData["AppUserId"] = new SelectList(_context.Users.OrderBy(x => x.NameSurName), "Id", "NameSurName");
            ViewData["AssetCategoryId"] = new SelectList(_context.AssetCategories.OrderBy(x => x.Name), "Id", "Name");
            ViewData["AssetTypeId"] = new SelectList(_context.AssetTypes.OrderBy(x => x.Name), "Id", "Name");

            if (!ModelState.IsValid)
            {
                return View();
            }

            Asset asset = new Asset()
            {
                Code = model.Code,
                Name = model.Name.ToUpper(),
                SerialNo = model.SerialNo,                
                Description = model.Description,
                AssetCategoryId = model.AssetCategoryId,
                AssetTypeId = model.AssetTypeId,
                AppUserId = model.AppUserId
            };

            await _context.AddAsync(asset);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AssetList));
        }

        [Authorize(Roles = "admin, varlık")]
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
                SerialNo = x.Assets.SerialNo,
                Description = x.Assets.Description,
                AssetCategoryName = x.AssetCategories.Name,
                AssetTypneName = x.AssetTypes.Name,
                AppUser = x.Users.NameSurName,
                AppUserId = x.Users.Id,

            }).OrderBy(x=> x.Name).ToList();


            return View(listOfAsset);
        }
        [Authorize(Roles = "admin, varlık")]
        public IActionResult AssetUserList(int Id)
        {
            var result = _context.Assets.Where(x => x.AppUserId == Id).ToList();
            var user = _context.Users.Where(x => x.Id == Id).FirstOrDefault();

            var listOfAssetUser = result.Select(x => new AssetListViewModel()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                SerialNo = x.SerialNo,
                Description = x.Description,
                AssetCategoryName = x.Name,
                AssetTypneName = x.Name,
                AppUser = user.NameSurName,
                AppUserId = x.Id,

            }).OrderBy(x => x.Name).ToList();


            return View(listOfAssetUser);
        }

        [Authorize]
        public async Task<IActionResult> AssetMyList()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var result = _context.Assets.Where(x => x.AppUserId == currentUser.Id).ToList();
            var user = _context.Users.Where(x => x.Id == currentUser.Id).FirstOrDefault();

            var listOfAssetUser = result.Select(x => new AssetListViewModel()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                SerialNo= x.SerialNo,
                Description = x.Description,
                AssetCategoryName = x.Name,
                AssetTypneName = x.Name,
                AppUser = user.NameSurName,
                AppUserId = x.Id,

            }).OrderBy(x => x.Name).ToList();

            return View(listOfAssetUser);
        }




    }
}
