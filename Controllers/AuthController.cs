﻿using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    public class AuthController : Controller
    {
        OrganicContext context;
        public AuthController(OrganicContext context){
            this.context = context;
        }
        public IActionResult Index()
        {
            ViewBag.Departments = context.Departments.ToList();
            return View();
        }
        public IActionResult Register()
        {
            ViewBag.Departments = context.Departments.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Register(Member obj){
            ModelState.Remove(nameof(obj.MemberId));
            if(ModelState.IsValid){
                obj.RoleId = 234102922;
                obj.MemberId = Guid.NewGuid().ToString().Replace("-", string.Empty);
                obj.Password = Helper.Hash(obj.Password);
                context.Members.Add(obj);
                int ret = context.SaveChanges();
                if(ret > 0){
                    return Redirect("/auth/login");
                }
                ModelState.AddModelError("Error", "Register Failed");
            }
                ViewBag.Departments = context.Departments.ToList();
            return View(obj);
        }
       
        public IActionResult Login()
        {
            ViewBag.Departments = context.Departments.ToList();
            return View();
        }
       
       [HttpPost]
        public async Task<IActionResult> Login(LoginModel obj)
        {
            Member? member = context.Members.Where(p => p.Email == obj.Eml && p.Password == Helper.Hash(obj.Pwd)).FirstOrDefault<Member>();
            if (member is null){
                    ModelState.AddModelError("Error", "Login Failed");
                    ViewBag.Departments = context.Departments.ToList(); // Đảm bảo load lại dữ liệu cần thiết
                    return View(obj); // Trả về view với thông báo lỗi
}
            List<Claim> claims = new List<Claim>{
                new Claim(ClaimTypes.NameIdentifier, member.MemberId),
                new Claim(ClaimTypes.Name, member.Name),
                new Claim(ClaimTypes.GivenName, member.GivenName),
                new Claim(ClaimTypes.Surname, member.Surname),
                new Claim(ClaimTypes.Email, member.Email),
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal, new AuthenticationProperties{
                IsPersistent = obj.Rem
            });
            return Redirect("/auth");
        }
        public IActionResult changePassword()
        {
            return View();
        }
        public IActionResult forgotPassword()
        {
            return View();
        }
    }
}
