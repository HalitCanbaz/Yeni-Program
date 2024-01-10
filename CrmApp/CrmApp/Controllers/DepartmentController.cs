using CrmApp.Models;
using CrmApp.Models.Entities;
using CrmApp.ViewModel.AssetTypeViewModels;
using CrmApp.ViewModel.DepartmentViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CrmApp.Controllers
{
    [Authorize(Roles = "admin")]

    public class DepartmentController : Controller
    {
        private readonly CrmAppDbContext _context;

        public DepartmentController(CrmAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DepartmentCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DepartmentCreate(DepartmentCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var result = _context.Department.Where(x => x.DepartmanName == model.Name).FirstOrDefault();
            if (result != null)
            {
                TempData["messageError"] = "Bu kayıt daha önce yapılmış. Lütfen yeni kayıt için tekrar deneyiniz.";

                return View();
            }



            Department department = new Department()
            {
                DepartmanName = model.Name.ToUpper(),
            };

            await _context.AddAsync(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DepartmentList));
        }

        public IActionResult DepartmentList()
        {
            var result = _context.Department.ToList();
            var listOfDepartment = result.Select(x => new DepartmentListViewModel()
            {
                Id=x.Id,    
                Name = x.DepartmanName

            }).OrderBy(x => x.Name).ToList();
            return View(listOfDepartment);
        }

        public IActionResult DepartmentEdit(int Id)
        {
            var result = _context.Department.Where(x => x.Id == Id).FirstOrDefault();

            var department = new DepartmentEditViewModel
            {
                Id = result.Id,
                DepartmentName = result.DepartmanName,

            };

            return View(department);
        }


        [HttpPost]
        public IActionResult DepartmentEdit(int Id, DepartmentEditViewModel model)
        {

            var result = _context.Department.Where(x => x.Id == Id).FirstOrDefault();

            if (result != null)
            {
                result.DepartmanName = model.DepartmentName.ToUpper();
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(DepartmentList));
        }



    }
}
