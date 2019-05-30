﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechnoSale.MvcWebUI.Entities;
using TechnoSale.MvcWebUI.Models;

namespace TechnoSale.MvcWebUI.Controllers
{
    public class AccountController:Controller
    {
        private UserManager<CustomIdentityUser> _userManager;
        private RoleManager<CustomIdentityRole> _roleManager;
        private SignInManager<CustomIdentityUser> _signInManager;

        public AccountController(UserManager<CustomIdentityUser> userManager, RoleManager<CustomIdentityRole> roleManager, SignInManager<CustomIdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                CustomIdentityUser user = new CustomIdentityUser
                {
                    UserName = registerViewModel.Email
                };

                IdentityResult result =
                    _userManager.CreateAsync(user, registerViewModel.Sifre).Result;

                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync("Admin").Result)
                    {
                        CustomIdentityRole role = new CustomIdentityRole
                        {
                            Name = "Admin"
                        };

                        IdentityResult roleResult = _roleManager.CreateAsync(role).Result;

                        if (!roleResult.Succeeded)
                        {
                            ModelState.AddModelError("","Bu rolü ekleyemedik!");
                            return View(registerViewModel);
                        }
                    }

                    _userManager.AddToRoleAsync(user, "Admin").Wait();
                    return RedirectToAction("Login", "Account");
                }
            }

            return View(registerViewModel);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _signInManager.PasswordSignInAsync(loginViewModel.Email,
                    loginViewModel.Sifre, loginViewModel.RememberMe, false).Result;

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Admin");
                }

                ModelState.AddModelError("","Giriş Başarısız!");
            }

            return View(loginViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            _signInManager.SignOutAsync().Wait();
            return RedirectToAction("Login");
        }
    }
}
