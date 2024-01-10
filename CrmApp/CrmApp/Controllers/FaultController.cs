using CrmApp.Models;
using CrmApp.Models.Entities;
using CrmApp.ViewModel.FaultViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CrmApp.Controllers
{
    [Authorize(Roles = "admin, varlık arıza")]

    public class FaultController : Controller
    {
        private readonly CrmAppDbContext _context;

        public FaultController(CrmAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FaultCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FaultCreate(FaultCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var result = _context.Faults.Where(x => x.Name == model.Name).FirstOrDefault();
            if (result != null)
            {
                TempData["messageError"] = "Bu kayıt daha önce yapılmış. Lütfen yeni kayıt için tekrar deneyiniz.";

                return View();
            }

            Fault fault = new Fault()
            {
                Code = model.Code,
                Name = model.Name.ToUpper(),
                Description = model.Description
            };

            await _context.AddAsync(fault);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(FaultList));
        }

        public IActionResult FaultList()
        {
            var result = _context.Faults.ToList();
            var listOfFault = result.Select(x => new FaultListViewModel()
            {
                Id= x.Id,   
                Code = x.Code,
                Name = x.Name,
                Description = x.Description
            }).OrderBy(x=> x.Name).ToList();
            return View(listOfFault);
        }
    }
}
