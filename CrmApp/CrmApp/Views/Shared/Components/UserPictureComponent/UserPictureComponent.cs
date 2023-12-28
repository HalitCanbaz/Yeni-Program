﻿using CrmApp.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CrmApp.Views.Shared.Components.UserPictureComponent
{
    public class UserPictureComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _UserManager;

        public UserPictureComponent(UserManager<AppUser> userManager)
        {
            _UserManager = userManager;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            var controlUser = await _UserManager.FindByNameAsync(User.Identity.Name);

            UserPictureViewModel model = new UserPictureViewModel();
            if (controlUser.Picture != null)
            {
                model.PictureUrl = controlUser.Picture;

            }
            else
            {
                model.PictureUrl = "hms.jpg";
            }

            return View(model);

        }


    }
}
