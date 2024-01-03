using CrmApp.Models;
using CrmApp.Models.Entities;
using CrmApp.ViewModel.AssetFaultViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CrmApp.Controllers
{
    [Authorize(Roles = "admin, varlık arıza")]

    public class AssetFaultController : Controller
    {
        private readonly CrmAppDbContext _context;

        public AssetFaultController(CrmAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int Id)
        {

            var assetFaultList = _context.AssetFaults
               .Join(_context.Assets, x => x.AssetId, y => y.Id, (x, y)
               => new { AssetFaults = x, Assets = y })
               .Join(_context.AssetFaults, x => x.AssetFaults.FaultId, y => y.Id, (x, y)
               => new { x.AssetFaults, x.Assets, Fault = y })
               .Join(_context.Users, x => x.Assets.AppUserId, y => y.Id, (x, y)
               => new { x.AssetFaults, x.Assets, x.Fault, Users = y }).Select(x => new AssetFaultListViewModel
               {
                   UserId = x.Users.Id,
                   UserName = x.Users.NameSurName,
                   AssetName = x.Assets.Name,
                   FaultNameName = x.AssetFaults.Name
               }).Where(x => x.UserId == Id);

            var list = assetFaultList.Select(x => new AssetFaultListViewModel()
            {
                UserName = x.UserName,
                AssetName = x.AssetName,
                FaultNameName = x.FaultNameName
            }).OrderBy(x => x.FaultNameName);

            return View(list.ToList());
        }

        public IActionResult AssetFaultCreate(int Id)
        {
            var varliks = _context.Assets.Where(x => x.Id == Id);

            var user = _context.Users.Join(_context.Assets, x => x.Id, y => y.AppUserId, (x, y)
                => new { Users = x, Assets = y }).Where(x => x.Assets.Id == Id).FirstOrDefault();



            ViewData["FaultsId"] = new SelectList(_context.Faults, "Id", "Name");
            ViewData["AssetId"] = new SelectList(varliks, "Id", "Name");


            var result = new AssetFaultCreateViewModel()
            {

                AppUserName = user.Users.NameSurName.ToUpper(),
            };

            return View(result);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssetFaultCreate(int Id, AssetFaultCreateViewModel model)
        {
            var assets = await _context.Assets.Where(x => x.Id == Id).FirstOrDefaultAsync();
            var asset = await _context.Assets.FindAsync(model.AssetId);
            var fault = await _context.Faults.FindAsync(model.FaultsId);




            AssetFault assetFault = new AssetFault()
            {
                AssetId = model.AssetId,
                FaultId = model.FaultsId,
                Name = asset.Name + "-" + fault.Name,
            };

            await _context.AddAsync(assetFault);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = assets.AppUserId });
        }


        public IActionResult AssetFaultList(int Id)
        {

            var assetFaultList = _context.AssetFaults
               .Join(_context.Assets, x => x.AssetId, y => y.Id, (x, y)
               => new { AssetFaults = x, Assets = y })
               .Join(_context.AssetFaults, x => x.AssetFaults.FaultId, y => y.Id, (x, y)
               => new { x.AssetFaults, x.Assets, Fault = y })
               .Join(_context.Users, x => x.Assets.AppUserId, y => y.Id, (x, y)
               => new { x.AssetFaults, x.Assets, x.Fault, Users = y }).Select(x => new AssetFaultListViewModel
               {
                   UserId = x.Users.Id,
                   UserName = x.Users.NameSurName,
                   AssetName = x.Assets.Name,
                   FaultNameName = x.AssetFaults.Name,
                   AssetId=x.Assets.Id
                   
               }).Where(x => x.AssetId == Id);

            var list = assetFaultList.Select(x => new AssetFaultListViewModel()
            {
                UserName = x.UserName,
                AssetName = x.AssetName,
                FaultNameName = x.FaultNameName
            }).OrderBy(x => x.FaultNameName);

            return View(list.ToList());
        }





    }
}
