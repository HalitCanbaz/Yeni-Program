using CrmApp.Models;
using CrmApp.Models.Entities;
using CrmApp.ViewModel.AssetFaultViewModels;
using CrmApp.ViewModel.WorkViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CrmApp.Controllers
{
    public class WorkController : Controller
    {
        private readonly SignInManager<AppUser> _SignInManager;
        private readonly UserManager<AppUser> _UserManager;
        private readonly CrmAppDbContext _context;

        public WorkController(CrmAppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _UserManager = userManager;
            _SignInManager = signInManager;
        }

        [Authorize]
        public async Task<IActionResult> WorkCreate()
        {
            #region include
            //var categoryVarlik = await _context.Varlikcategories.Include(v => v.Categories).Include(v => v.Varlik).ToListAsync();

            #endregion
            var user = await _UserManager.FindByNameAsync(User.Identity.Name);
            var assetFaults = _context.AssetFaults.Join(_context.Assets, x => x.Id, y => y.Id, (x, y)
                => new { AssetFaults = x, Assets = y }).Join(_context.Users, x => x.Assets.AppUserId, y => y.Id, (x, y)
                => new { x.AssetFaults, x.Assets, Users = y }).Select(x => new WorkFaultViewModel
                {
                    Id = x.AssetFaults.Id,
                    Name = x.AssetFaults.Name,
                    UserId = x.Users.Id

                }).Where(x => x.UserId == user.Id);




            ViewData["AssetFaultId"] = new SelectList(assetFaults, "Id", "Name");

            //ViewData["AssetFaultId"] = new SelectList(_context.AssetFaults, "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "DepartmanName");

            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> WorkCreate(WorkCreateViewModel model)
        {

            ViewData["AssetFaultId"] = new SelectList(_context.AssetFaults, "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_context.Department, "Id", "DepartmanName");

            DateTime systemClock = DateTime.Now;
            DateTime controlClock = systemClock.AddMinutes(30);

            if (controlClock >= model.DeadLine)
            {
                TempData["timeMessage"] = "Talep ettiğiniz tarih sistem saatinden minimum 30 dk yukarıda olmak zorundadır!";
                return View();
            }

            string numberUp = "";

            DateTime thisYear = DateTime.Now;
            string years = Convert.ToString(thisYear.Year % 100);
            int yearThis = Convert.ToInt32(years);

            var currentWorks = await _context.Works.OrderByDescending(x => x.WorkOrderNumber).FirstOrDefaultAsync();
            if (currentWorks != null)
            {
                string newYear = currentWorks.WorkOrderNumber.Substring(0, 2);
                int workYear = Convert.ToInt32(newYear);
                if (yearThis > workYear)
                {
                    numberUp = years + "-" + "0001";
                }
                else
                {
                    string number = currentWorks.WorkOrderNumber.Substring(currentWorks.WorkOrderNumber.Length - 3);
                    int numberIntl = Convert.ToInt32(number) + 1;
                    string numberString = Convert.ToString(numberIntl);

                    if (numberString.Length <= 1)
                    {
                        numberUp = years + "-" + "000" + (Convert.ToString(numberString));
                    }
                    else if (numberString.Length <= 2)
                    {
                        numberUp = years + "-" + "00" + (Convert.ToString(numberString));
                    }
                    else if (numberString.Length <= 3)
                    {
                        numberUp = years + "-" + "0" + (Convert.ToString(numberString));
                    }
                }
            }
            else
            {
                numberUp = years + "-" + "0001";
            }
            var user = await _UserManager.FindByNameAsync(User.Identity.Name);
            var departmanId = _context.Department.Join(_context.Users, x => x.Id, y => y.DepartmentId, (x, y) => new
            { Departman = x, Users = y }).Where(x => x.Users.Id == model.AppUserId).FirstOrDefault();
            var tasCategoryId = _context.TaskCategories.Where(x => x.Name.Contains("varlık")).FirstOrDefault();
            var appUserId = _context.Users.Where(x => x.UserName.Contains("destek")).FirstOrDefault();

            var result = new Work()
            {
                Title = model.Title,
                Description = model.Description,
                Status = model.Status,
                Progress = model.Progress,
                WhoIsCreate = user.NameSurName,
                Create = model.Create,
                DeadLine = model.DeadLine,
                DepartmentId = model.DepartmentId,
                WorkOpenDepartmanId = user.DepartmentId,
                WorkOrderNumber = numberUp,
                AppUserId = appUserId.Id,
                AssetFaultId = model.AssetFaultId,
                TaskCategoryId = tasCategoryId.Id
            };

            await _context.AddAsync(result);
            await _context.SaveChangesAsync();
            TempData["message"] = "İş ataması başarılı. Yeni kayıt açabilirsiniz!";

            return RedirectToAction(nameof(WorkController.WorkCreate));
        }

        [Authorize(Roles = "admin, sorumlu, user, müdür")]
        public async Task<IActionResult> TaskCreate()
        {
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "NameSurName");
            ViewData["TaskCategoryId"] = new SelectList(_context.TaskCategories, "Id", "Name");

            return View();
        }

        [Authorize(Roles = "admin, sorumlu, user, müdür")]

        [HttpPost]
        public async Task<IActionResult> TaskCreate(WorkCreateViewModel model)
        {

            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "NameSurName");
            ViewData["TaskCategoryId"] = new SelectList(_context.TaskCategories, "Id", "Name");

            DateTime systemClock = DateTime.Now;
            DateTime controlClock = systemClock.AddMinutes(30);

            if (controlClock >= model.DeadLine)
            {
                TempData["timeMessage"] = "Talep ettiğiniz tarih sistem saatinden minimum 30 dk yukarıda olmak zorundadır!";
                return View();
            }

            string numberUp = "";

            DateTime thisYear = DateTime.Now;
            string years = Convert.ToString(thisYear.Year % 100);
            int yearThis = Convert.ToInt32(years);

            var currentWorks = await _context.Works.OrderByDescending(x => x.WorkOrderNumber).FirstOrDefaultAsync();
            if (currentWorks != null)
            {
                string newYear = currentWorks.WorkOrderNumber.Substring(0, 2);
                int workYear = Convert.ToInt32(newYear);
                if (yearThis > workYear)
                {
                    numberUp = years + "-" + "0001";
                }
                else
                {
                    string number = currentWorks.WorkOrderNumber.Substring(currentWorks.WorkOrderNumber.Length - 3);
                    int numberIntl = Convert.ToInt32(number) + 1;
                    string numberString = Convert.ToString(numberIntl);

                    if (numberString.Length <= 1)
                    {
                        numberUp = years + "-" + "000" + (Convert.ToString(numberString));
                    }
                    else if (numberString.Length <= 2)
                    {
                        numberUp = years + "-" + "00" + (Convert.ToString(numberString));
                    }
                    else if (numberString.Length <= 3)
                    {
                        numberUp = years + "-" + "0" + (Convert.ToString(numberString));
                    }
                }
            }
            else
            {
                numberUp = years + "-" + "0001";
            }
            var user = await _UserManager.FindByNameAsync(User.Identity.Name);
            var departmanId = _context.Department.Join(_context.Users, x => x.Id, y => y.DepartmentId, (x, y) => new
            { Departman = x, Users = y }).Where(x => x.Users.Id == model.AppUserId).FirstOrDefault();


            var assetFaultId = _context.AssetFaults.Where(x => x.Name.Contains("proje")).FirstOrDefault();
            var appUserId = _context.Users.Where(x => x.UserName.Contains("destek")).FirstOrDefault();

            var result = new Work()
            {
                Title = model.Title,
                Description = model.Description,
                Status = model.Status,
                Progress = model.Progress,
                WhoIsCreate = user.NameSurName,
                Create = model.Create,
                DeadLine = model.DeadLine,
                DepartmentId = departmanId.Departman.Id,
                WorkOpenDepartmanId = user.DepartmentId,
                WorkOrderNumber = numberUp,
                AppUserId = model.AppUserId,
                AssetFaultId = assetFaultId.Id,
                TaskCategoryId = model.TaskCategoryId
            };

            await _context.AddAsync(result);
            await _context.SaveChangesAsync();
            TempData["message"] = "İş ataması başarılı. Yeni kayıt açabilirsiniz!";

            return RedirectToAction(nameof(WorkController.WorkCreate));
        }

        [Authorize(Roles = "admin, müdür")]

        public async Task<IActionResult> WorkPendingApprovalList(int Id)
        {
            var worksList = await _context.Works.ToListAsync();

            var userControl = await _UserManager.FindByNameAsync(User.Identity.Name);


            var worksListViewModel = worksList.Where(x => x.DepartmentId == (userControl.DepartmentId) & x.Status == "beklemede").Select(x => new WorkPendingApprovalListViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                Create = x.Create,
                DeadLine = x.DeadLine,
                WhoIsCreate = x.WhoIsCreate,
                Status = x.Status,
                WorkOrderNumber = x.WorkOrderNumber

            }).OrderByDescending(x => x.Id).ToList();
            return View(worksListViewModel);
        }

        [Authorize(Roles = "admin, sorumlu, user, müdür")]

        public async Task<IActionResult> WorkStatusApprovalDetail(int Id)
        {
            var userControl = await _UserManager.FindByNameAsync(User.Identity.Name);

            var bilgiIslemKullanicilari = _context.Users
               .Where(u => u.DepartmentId == userControl.DepartmentId && u.Status == "onaylandı")
               .Select(u => new { Id = u.Id, NameSurName = u.NameSurName })
               .ToList();

            // SelectList'e eklerken kullanabilirsiniz
            ViewData["AppUserId"] = new SelectList(bilgiIslemKullanicilari, "Id", "NameSurName");


            var worksDetails = await _context.Works.Where(x => x.Id == Id).FirstOrDefaultAsync();
            var user = await _context.Users.Where(x => x.Id == worksDetails.AppUserId).FirstOrDefaultAsync();
            var assetFault = await _context.AssetFaults.Where(x => x.Id == worksDetails.AssetFaultId).FirstOrDefaultAsync();


            var details = new WorkPendingApprovalDetailViewModel()
            {
                Id = worksDetails.Id,
                Title = worksDetails.Title,
                Description = worksDetails.Description,
                WhoIsCreate = worksDetails.WhoIsCreate,
                Status = worksDetails.Status,
                Create = worksDetails.Create,
                Update = worksDetails.Update,
                DeadLine = worksDetails.DeadLine,
                AppUserId = user.Id,
                AppUser = user.UserName,
                AssetFault = assetFault.Name,
                WorkOrderNumber = worksDetails.WorkOrderNumber,
                ApprovedNote = worksDetails.ApprovedNote

            };
            return View(details);
        }

        [Authorize(Roles = "admin, sorumlu, user, müdür")]

        [HttpPost]
        public async Task<IActionResult> WorkStatusApprovalDetail(int Id, WorkPendingApprovalDetailViewModel model)
        {
            var userControl = await _UserManager.FindByNameAsync(User.Identity.Name);

            var bilgiIslemKullanicilari = _context.Users
               .Where(u => u.DepartmentId == userControl.DepartmentId && u.Status == "onaylandı")
               .Select(u => new { Id = u.Id, NameSurName = u.NameSurName })
               .ToList();

            ViewData["AppUserId"] = new SelectList(bilgiIslemKullanicilari, "Id", "NameSurName");


            var worksDetails = await _context.Works.Where(x => x.Id == Id).FirstOrDefaultAsync();
            var user = await _context.Users.Where(x => x.Id == worksDetails.AppUserId).FirstOrDefaultAsync();
            var assetFault = await _context.AssetFaults.Where(x => x.Id == worksDetails.AssetFaultId).FirstOrDefaultAsync();


            if (worksDetails != null)
            {
                worksDetails.Status = "onaylandı";
                worksDetails.AppUserId = model.AppUserId;
                worksDetails.ApprovedNote = model.ApprovedNote;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(WorkController.WorkPendingApprovalList));
        }

        [Authorize(Roles = "admin, sorumlu, user, müdür")]

        public async Task<IActionResult> WorkDetail(int Id)
        {
            var worksDetails = await _context.Works.Where(x => x.Id == Id).FirstOrDefaultAsync();
            var user = await _context.Users.Where(x => x.Id == worksDetails.AppUserId).FirstOrDefaultAsync();
            var category = await _context.AssetFaults.Where(x => x.Id == worksDetails.AssetFaultId).FirstOrDefaultAsync();


            var details = new WorkDetailViewModel()
            {
                Id = worksDetails.Id,
                Title = worksDetails.Title,
                Description = worksDetails.Description,
                WhoIsCreate = worksDetails.WhoIsCreate,
                Status = worksDetails.Status,
                Create = worksDetails.Create,
                Update = worksDetails.Update,
                DeadLine = worksDetails.DeadLine,
                Finished = worksDetails.Finished,
                AppUser = user.UserName,
                AssetFault = category.Name,
                WorkOrderNumber = worksDetails.WorkOrderNumber,
                ApprovedNote = worksDetails.ApprovedNote,
                FinishedDescription = worksDetails.FinishedDescription

            };
            return View(details);
        }

        [Authorize(Roles = "admin, sorumlu, user, müdür")]

        public async Task<IActionResult> MyWorks()
        {
            var worksList = await _context.Works.ToListAsync();

            var userControl = await _UserManager.FindByNameAsync(User.Identity.Name);

            var worksListViewModel = worksList.Where(x => x.AppUserId == userControl.Id & (x.Status == "onaylandı" || x.Status == "başlandı")).Select(x => new MyWorksViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                Create = x.Create,
                DeadLine = x.DeadLine,
                WhoIsCreate = x.WhoIsCreate,
                Status = x.Status,
                WorkOrderNumber = x.WorkOrderNumber


            }).OrderByDescending(x => x.Id).ToList();
            return View(worksListViewModel);
        }

        [Authorize(Roles = "admin, sorumlu, user, müdür")]

        [HttpPost]
        public async Task<IActionResult> WorksStatusStarted(int id)
        {
            var work = await _context.Works.FindAsync(id);

            if (work != null)
            {
                work.Status = "başlandı";

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(WorkController.MyWorks));
        }

        [Authorize(Roles = "admin, sorumlu, user, müdür")]

        public async Task<IActionResult> WorkStatusFinished()
        {
            return View();
        }
        [Authorize(Roles = "admin, sorumlu, user, müdür")]


        [HttpPost]
        public async Task<IActionResult> WorkStatusFinished(int id, WorkStatusFinishedViewModel model)
        {
            var work = await _context.Works.FindAsync(id);

            if (work != null)
            {
                work.FinishedDescription = model.FinishedDescription;
                work.Status = "bitti";

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(WorkController.MyWorks));
        }






    }
}
