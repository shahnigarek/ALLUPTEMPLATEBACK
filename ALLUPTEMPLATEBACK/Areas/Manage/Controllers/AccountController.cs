using ALLUPTEMPLATEBACK.Areas.Manage.ViewModels.Account;
using ALLUPTEMPLATEBACK.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALLUPTEMPLATEBACK.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]

        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);

            }

            AppUser appUser = new AppUser
            {
                Name = registerVM.Name,
                Email = registerVM.Email,
                UserName = registerVM.UserName

            };

            if (!await _userManager.Users.AnyAsync(u => u.NormalizedEmail == registerVM.UserName.Trim().ToUpperInvariant()))
            {
                ModelState.AddModelError("UserName", "Already Exists");
                return View(registerVM);
            }

            if (!await _userManager.Users.AnyAsync(u => u.NormalizedEmail == registerVM.UserName.Trim().ToUpperInvariant()))
            {
                ModelState.AddModelError("Email", "Already Exists");
                return View(registerVM);
            }



            IdentityResult identityResult = await _userManager.CreateAsync(appUser, registerVM.Password);

            if (!identityResult.Succeeded)
            {
                foreach (var item in identityResult.Errors)
                {
                    ModelState.AddModelError("", item.Description);

                }

                return View(registerVM);
            }

            await _userManager.AddToRoleAsync(appUser, "Admin");

            //return RedirectToAction("Index","Dashboard",new { area = "manage" });

            return RedirectToAction("login");

        }



        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);

            }

            AppUser appUser = await _userManager.FindByEmailAsync(loginVM.Email);

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email and Password are wrong");
                return View(loginVM);
            }


            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(appUser, loginVM.Password, true);
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", appUser.AccessFailedCount.ToString());
                return View(loginVM);
            }
            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email and Password are wrong");
                return View(loginVM);
            }

            signInResult = await _signInManager.PasswordSignInAsync(appUser, loginVM.Password, loginVM.RemindMe, true);



            return RedirectToAction("Index", "Dashboard", new { area = "manage" });


        }



        public IActionResult Index()
        {
            return View();
        }

        //public async Task<IActionResult> CreateRole()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "SuperAdmin" });
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "Member" });

        //    return Ok();

        //}

        //public async Task<IActionResult> CreateSuperAdmin()
        //{
        //    AppUser appUser = new AppUser
        //    {
        //          Email="superadmin@code.az",
        //          Name="Super",
        //          UserName="SuperAdmin"

        //    };

        //    await _userManager.CreateAsync(appUser, "Superadmin123");
        //    await _userManager.AddToRoleAsync(appUser, "SuperAdmin");

        //    return Ok();
        //}

        [HttpGet]
        [Authorize("")]
        public async Task<IActionResult> Profile()
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            ProfileVM profileVM = new ProfileVM
            {
                Name = appUser.Name,
                UserName = appUser.UserName,
                Email = appUser.Email

            };


            return View(profileVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Profile(ProfileVM profileVM)
        {
            if (!ModelState.IsValid)
            {
                return View(profileVM);

            }

            bool check = false;

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);


            if (appUser.Name.ToLowerInvariant() != profileVM.Name.Trim().ToLowerInvariant())
            {
                check = true;
                appUser.Name = profileVM.Name.Trim();
            }
            if (appUser.NormalizedEmail != profileVM.Email.Trim().ToUpperInvariant())
            {
                check = true;
                appUser.Email = profileVM.Email.Trim();
            }
            if (appUser.NormalizedUserName != profileVM.UserName.Trim().ToLowerInvariant())
            {
                check = true;
                appUser.UserName = profileVM.UserName.Trim();
            }

            if (check)
            {
                IdentityResult identityResult =await _userManager.UpdateAsync(appUser);
                if (!identityResult.Succeeded)
                {
                    foreach (var item in identityResult.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                    return View(profileVM);
                }
            }

            if (!string.IsNullOrWhiteSpace(profileVM.CurrentPassword))
            {
                if (!await _userManager.CheckPasswordAsync(appUser,profileVM.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Enter right password");
                    return View(profileVM);
                }
                if (profileVM.Password == profileVM.CurrentPassword)
                {
                    ModelState.AddModelError("Password", "New password cant be equal to currentpassword");
                    return View(profileVM);
                }

                string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                IdentityResult identityResult = await _userManager.ResetPasswordAsync(appUser, token, profileVM.Password);
                if (!identityResult.Succeeded)
                {
                    foreach (var item in identityResult.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                    return View(profileVM);
                }

            }

            return RedirectToAction("Index", "Dashboard", new { area = "manage" });

        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Dashboard", new { area = "manage" });
        }
    }
}
