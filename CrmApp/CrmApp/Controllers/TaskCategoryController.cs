﻿using CrmApp.Models;
using CrmApp.Models.Entities;
using CrmApp.ViewModel.TaskCategoryViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CrmApp.Controllers
{
    [Authorize(Roles = "admin, görev kategori")]

    public class TaskCategoryController : Controller
    {
        private readonly CrmAppDbContext _context;

        public TaskCategoryController(CrmAppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TaskCategoryCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TaskCategoryCreate(TaskCategoryCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }

            var result = _context.TaskCategories.Where(x => x.Name == model.Name).FirstOrDefault();
            if (result != null)
            {
                TempData["messageError"] = "Bu kayıt daha önce yapılmış. Lütfen yeni kayıt için tekrar deneyiniz.";

                return View();
            }


            TaskCategory task = new TaskCategory()
            {
                Name = model.Name.ToUpper(),
            };

            await _context.AddAsync(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TaskCategoryList));


        }

        public IActionResult TaskCategoryList()
        {
            var result = _context.TaskCategories.ToList();
            var listOfTaskCategory = result.Select(x => new TaskCategoryListViewModel()
            {
                Name = x.Name

            }).OrderBy(x => x.Name).ToList();
            return View(listOfTaskCategory);
        }


    }
}
