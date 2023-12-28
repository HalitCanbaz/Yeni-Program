using CrmApp.Models;
using CrmApp.Models.Entities;
using CrmApp.Views.Shared.Components.WorkComponent;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrmApp.Views.Shared.Components.AylikOranComponent
{
    public class AylikOranComponent : ViewComponent
    {
        private readonly CrmAppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AylikOranComponent(CrmAppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);



            DateTime now = DateTime.Now;
            DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);


            var resultFinished = await _context.Works.Where(x => x.AppUserId == user.Id).CountAsync(x => x.Status == "bitti" && x.Finished >= startOfMonth && x.Finished <= endOfMonth);

            var resultWaiting = await _context.Works.Where(x => x.AppUserId == user.Id).CountAsync(x => x.Status == "beklemede" && x.Create >= startOfMonth && x.Create <= endOfMonth);

            var resultApproved = await _context.Works.Where(x => x.AppUserId == user.Id).CountAsync(x => x.Status == "onaylandı" && x.Create >= startOfMonth && x.Create <= endOfMonth);

            AylikOranViewModel model = new AylikOranViewModel();

            model.TotalFinished = resultFinished;
            model.TotalWaiting = resultWaiting;
            model.TotalApproved = resultApproved;

            return View(model);
        }
    }
}
