using CrmApp.Models;
using CrmApp.Models.Entities;
using CrmApp.Services;
using CrmApp.ViewModel.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace CrmApp.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _UserManager;
        private readonly SignInManager<AppUser> _SignInManager;
        private readonly RoleManager<AppRole> _RoleManager;
        private readonly IEmailServices _EmailServices;
        private readonly IFileProvider _FileProvider;
        private readonly CrmAppDbContext _context;

        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailServices emailServices, IFileProvider fileProvider, RoleManager<AppRole> roleManager, CrmAppDbContext context)
        {
            _UserManager = userManager;
            _SignInManager = signInManager;
            _EmailServices = emailServices;
            _FileProvider = fileProvider;
            _RoleManager = roleManager;
            _context = context;
        }

        public IActionResult SignUp()
        {
            Department departman = new Department();

            ViewData["DepartmanId"] = new SelectList(_context.Department, "Id", "DepartmanName", departman.Id);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            Department departman = new Department();

            ViewData["DepartmanId"] = new SelectList(_context.Department, "Id", "DepartmanName", departman.Id);

            if (!ModelState.IsValid)
            {
                return View();

            }

            var mail = _context.Users.Where(x => x.Email == model.Email).FirstOrDefault();
            if (mail != null)
            {
                TempData["messageError"] = "Bu mail ile daha önce hesap açılmış. Bir mail ile sadece bir hesap oluşturulabilir.";
                return View();
            }


            var result = await _UserManager.CreateAsync(new()
            {
                UserName = model.UserName,
                NameSurName = model.NameSurname.ToUpper(),
                Email = model.Email,
                PhoneNumber = model.Phone,
                DepartmentId = model.DepartmanId,
                RegisterDate = model.RegisterDate,
                Status = model.Status
            }, model.Password);



            if (result.Succeeded)
            {
                TempData["message"] = "Kaydınız alınmıştır. Yönetici onayı sonrasında sisteme giriş yapabilirsiniz.";
                return RedirectToAction(nameof(UserController.SignUp));
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }

        [Authorize(Roles = "admin")]

        public async Task<IActionResult> UserApprovedList(int id)
        {

            /*var currentUse= await _UserManager.FindByNameAsync(User.Identity.Name);*/
            var userList = await _UserManager.Users.Join(_context.Department, x => x.DepartmentId, y => y.Id, (x, y)
                => new { Users = x, Departman = y }).Where(x => x.Users.Status == "beklemede").ToListAsync();

            var userListViewModel = userList.Select(x => new UserApprovedListViewModel()
            {
                Id = x.Users.Id,
                UserName = x.Users.UserName,
                NameSurname = x.Users.NameSurName,
                Email = x.Users.Email,
                Phone = x.Users.PhoneNumber,
                Departman = x.Departman.DepartmanName,
                Status = x.Users.Status

            }).ToList();
            //.Where(x => x.UserName == currentUse.UserName)

            return View(userListViewModel);

        }

        [Authorize(Roles = "admin")]

        public async Task<IActionResult> UserApproved()
        {
            return View();
        }


        [Authorize(Roles = "admin")]

        [HttpPost]
        public async Task<IActionResult> UserApproved(int id, UserApprovedListViewModel model)
        {
            var result = await _UserManager.Users.Where(x => x.Id == id).SingleOrDefaultAsync();

            if (result != null)
            {
                result.Status = "onaylandı";
                await _context.SaveChangesAsync();
            }

            await _EmailServices.SendApprovedStatusdEmail($"Sayın {result.NameSurName} Kullanıcınız onaylanmıştır. Sisteme giriş yapabilirsiniz.", result.Email);

            return RedirectToAction(nameof(UserController.UserApprovedList));

        }

        [Authorize(Roles = "admin")]

        public async Task<IActionResult> UserReject()
        {

            return View();

        }
        [Authorize(Roles = "admin")]

        [HttpPost]
        public async Task<IActionResult> UserReject(int id, UserApprovedListViewModel model)
        {
            var user = await _context.Users.FindAsync(id);

            if (user != null)
            {

                await _EmailServices.SendApprovedStatusdEmail($"Sayın {user.NameSurName} Kullanıcınız onaylanmamıştır. Lütfen sistem yöneticinizle iletişime geçiniz", user.Email);

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

            }


            return RedirectToAction(nameof(UserController.UserApprovedList));

        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var controlUser = await _UserManager.FindByNameAsync(model.UserName);

            if (controlUser == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre yanlış");
                return View();
            }

            if (controlUser.Status == "beklemede")
            {
                ModelState.AddModelError(string.Empty, "Yönetici onayı bekleniyor.  Lütfen yöneticiniz ile iletişime geçiniz!");
                return View();

            }

            var result = await _SignInManager.PasswordSignInAsync(controlUser, model.Password, model.RememberMe, true);

            TempData["UserName"] = controlUser.UserName;
            TempData["UserPicture"] = controlUser.Picture;
            TempData["UserMail"] = controlUser.Email;

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");

            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Çok sayıda başarısız giriş denemeniz oldu.     Kullanıcınız 10 dakika sonra açılacaktır.");
                return View();
            }

            ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre yanlış");

            return View();
        }
        [Authorize]

        public async Task<IActionResult> Logout()
        {
            await _SignInManager.SignOutAsync();
            return RedirectToAction(nameof(UserController.SignIn));
        }
        [Authorize]

        public async Task<IActionResult> UserDetails()
        {
            var currentUser = await _UserManager.FindByNameAsync(User.Identity.Name);
            var currentDepartman = await _context.Department.Where(x => x.Id == currentUser.DepartmentId).FirstOrDefaultAsync();
            var userEditViewModel = new UserDetailsViewModel
            {
                UserName = currentUser.UserName,
                NameSurname = currentUser.NameSurName,
                Departman = currentDepartman.DepartmanName,
                Email = currentUser.Email,
                Phone = currentUser.PhoneNumber,
                PictureUrl = currentUser.Picture
            };
            return View(userEditViewModel);
        }

        [Authorize(Roles = "admin")]

        public async Task<IActionResult> UserList()
        {

            /*var currentUse= await _UserManager.FindByNameAsync(User.Identity.Name);*/
            var userList = await _UserManager.Users.ToListAsync();

            var userListViewModel = userList.Select(x => new UserListViewModel()
            {
                Id = x.Id,
                UserName = x.UserName,
                NameSurname = x.NameSurName,
                Email = x.Email,
                Phone = x.PhoneNumber,
                PictureUrl = x.Picture


            }).ToList();
            //.Where(x => x.UserName == currentUse.UserName)

            return View(userListViewModel);
        }


        [Authorize]

        public async Task<IActionResult> UserEdit()
        {
            Department departman = new Department();

            ViewData["DepartmanId"] = new SelectList(_context.Department, "Id", "DepartmanName", departman.Id);

            var currentUser = await _UserManager.FindByNameAsync(User.Identity.Name);
            var currentDepartman = await _context.Department.Where(x => x.Id == currentUser.DepartmentId).SingleOrDefaultAsync();
            ViewData["Departman"] = currentDepartman.DepartmanName;

            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName!,
                NameSurname = currentUser.NameSurName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                Description = currentUser.Description!
            };

            return View(userEditViewModel);
        }
        [Authorize]

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel model)
        {
            Department departman = new Department();

            ViewData["DepartmanId"] = new SelectList(_context.Department, "Id", "DepartmanName", departman.Id);


            if (!ModelState.IsValid)
            {
                return View();
            }

            var currentUser = await _UserManager.FindByNameAsync(User.Identity.Name);

            var currentDepartman = await _context.Department.Where(x => x.Id == currentUser.DepartmentId).SingleOrDefaultAsync();
            ViewData["Departman"] = currentDepartman.DepartmanName;
            ViewData["Id"] = currentDepartman.Id;


            currentUser.UserName = model.UserName;
            currentUser.NameSurName = model.NameSurname.ToUpper();
            currentUser.Email = model.Email;
            currentUser.PhoneNumber = model.Phone;
            currentUser.Description = model.Description;
            currentUser.DepartmentId = model.DepartmanId;

            if (model.Picture != null && model.Picture.Length > 0)
            {
                var wwwrootFolder = _FileProvider.GetDirectoryContents("wwwroot");
                var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(model.Picture.FileName)}";

                var newPicturePath = Path.Combine(wwwrootFolder.First(x => x.Name == "userpicture").PhysicalPath, randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create);
                await model.Picture.CopyToAsync(stream);
                currentUser.Picture = randomFileName;

            }
            var updateToUserResult = await _UserManager.UpdateAsync(currentUser);

            if (!updateToUserResult.Succeeded)
            {
                foreach (var error in updateToUserResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    return View();
                }
            }

            TempData["message"] = "Kayıt Başarılı";
            return RedirectToAction(nameof(UserController.UserDetails));

        }



        [Authorize]

        public IActionResult ChangePassword()
        {
            return View();
        }
        [Authorize]

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var currentUser = await _UserManager.FindByNameAsync(User.Identity.Name);
            var checkOldPassword = await _UserManager.CheckPasswordAsync(currentUser, model.PasswordOld);

            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Yanlış eski şifre");
                return View();
            }

            var resultChangePassword = await _UserManager.ChangePasswordAsync(currentUser, model.PasswordOld, model.PasswordConfirm);

            if (!resultChangePassword.Succeeded)
            {
                foreach (IdentityError error in resultChangePassword.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }



            await _SignInManager.SignOutAsync();
            await _SignInManager.PasswordSignInAsync(currentUser, model.PasswordConfirm, true, false);


            TempData["message"] = "Şifreniz başarıyla değiştirilmiştir.";


            return RedirectToAction(nameof(UserController.ChangePassword));
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            var hasUser = await _UserManager.FindByEmailAsync(model.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Bu Email adresine ait kullanıcı bulunamamıştır.");
                return View();
            }

            string forgetPasswordToken = await _UserManager.GeneratePasswordResetTokenAsync(hasUser);

            var passwordLink = Url.Action("ResetPassword", "User", new { userId = hasUser.Id, Token = forgetPasswordToken },
                HttpContext.Request.Scheme);
            await _EmailServices.SendResetPasswordEmail(passwordLink, hasUser.Email);

            TempData["message"] = "Şifre yenileme linki eposta adresinize gönderilmiştir.";
            return RedirectToAction(nameof(UserController.ForgetPassword));
        }


        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var userId = TempData["userId"].ToString();
            var token = TempData["token"].ToString();
            if (userId == null || token == null)
            {
                throw new Exception("Bir hata meydana geldi. Lütfen tekrar deneyiniz.");
            }

            var hasUser = await _UserManager.FindByIdAsync(userId);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamamıştır.");
                return View();
            }

            IdentityResult result = await _UserManager.ResetPasswordAsync(hasUser, token, model.Password);
            if (result.Succeeded)
            {
                TempData["message"] = "Şifreniz başarıyla yenilenmiştir.";
                return RedirectToAction(nameof(UserController.ResetPassword));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }
    }
}
