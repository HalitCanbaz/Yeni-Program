using CrmApp.Models;
using CrmApp.Models.Entities;
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

            Department department = new Department()
            {
                DepartmanName = model.Name.ToUpper(),
            };

            await _context.AddAsync(department);
            await _context.SaveChangesAsync();
            return View();
        }

        public IActionResult DepartmentList()
        {
            var result = _context.Department.ToList();
            var listOfDepartment = result.Select(x => new DepartmentListViewModel()
            {
                Name = x.DepartmanName

            }).ToList();
            return View(listOfDepartment);
        }
    }
}
