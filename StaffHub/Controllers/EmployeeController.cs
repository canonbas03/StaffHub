using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StaffHub.Data;
using StaffHub.Models;
using StaffHub.Models.ViewModels;

namespace StaffHub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public EmployeeController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List(string searchString)
        {
            var employees = _context.Employees.Include(e => e.Role).ThenInclude(e => e.Department).AsQueryable();
            if (!searchString.IsNullOrEmpty())
            {
                searchString = searchString.ToLower();
                employees = employees.Where(e => e.FirstName.ToLower().Contains(searchString) || e.LastName.ToLower().Contains(searchString));
            }
            return View(employees);
        }

        public IActionResult Add()
        {
            ViewBag.Departments = _context.Departments.ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(EmployeeCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments.ToList();
                return View(model);
            }
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                ViewBag.Departments = _context.Departments.ToList();
                return View(model);
            }
            await _userManager.AddToRoleAsync(user, model.PermissionRole);
            var employee = new Employee
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Salary = model.Salary,
                RoleId = model.RoleId,
                IdentityUserId = user.Id
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction("List");
        }
            //[HttpPost]
            //public IActionResult Add(Employee employee)
            //{
            //    if (ModelState.IsValid)
            //    {
            //        _context.Employees.Add(employee);
            //        _context.SaveChanges();
            //        return RedirectToAction("List");
            //    }
            //    ViewBag.Departments = _context.Departments.ToList();
            //    return View(employee);
            //}
            [HttpGet]
            public JsonResult GetRolesByDepartment(int id)
            {
                var roles = _context.Roles.Where(r => r.RoleId == id).Select(r => new { r.RoleId, r.Name }).ToList();
                return Json(roles);
            }

        public IActionResult Delete(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.EmployeeId == id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }

        public IActionResult Edit(int id)
        {
            var employee = _context.Employees.Include(e => e.Role).FirstOrDefault(e => e.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewBag.Departments = _context.Departments.ToList();
            return View(employee);
        }
        [HttpPost]
        public IActionResult Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Update(employee);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            ViewBag.Departments = _context.Departments.ToList();
            return View(employee);
        }

        public IActionResult Details(int id)
        {
            var employee = _context.Employees.Include(e => e.Role).ThenInclude(r => r.Department).FirstOrDefault(e => e.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }
    }
}
