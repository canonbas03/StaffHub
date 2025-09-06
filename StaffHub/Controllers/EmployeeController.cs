using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StaffHub.Data;
using StaffHub.Models;
using StaffHub.Models.ViewModels;
using System.Threading.Tasks;

namespace StaffHub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public EmployeeController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
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
            if(string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("Password", "Password is required.");
                ViewBag.Departments = _context.Departments.ToList();
                return View(model);
            }
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Password", error.Description);
                }
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

        [HttpGet]
        public JsonResult GetRolesByDepartment(int id)
        {
            var roles = _context.Roles.Where(r => r.RoleId == id).Select(r => new { r.RoleId, r.Name }).ToList();
            return Json(roles);
        }

        public async Task<IActionResult> Delete(int id)
        {
            // Load the employee including the linked IdentityUser
            var employee = await _context.Employees
                .Include(e => e.IdentityUser)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
                return RedirectToAction("List"); // Already gone

            // Delete the IdentityUser first
            if (employee.IdentityUser != null)
            {
                var userResult = await _userManager.DeleteAsync(employee.IdentityUser);
                if (!userResult.Succeeded)
                {
                    // Handle failure (optional)
                    ModelState.AddModelError("", "Could not delete associated user.");
                    return RedirectToAction("List");
                }
            }

            // Check if Employee still exists (in case cascade delete deleted it)
            var stillExists = await _context.Employees.AnyAsync(e => e.EmployeeId == id);
            if (stillExists)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("List");
        }


        public async Task<IActionResult> Edit(int id)
        {
            var employee = _context.Employees.Include(e => e.Role).Include(e => e.IdentityUser).FirstOrDefault(e => e.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }
            var roles = await _userManager.GetRolesAsync(employee.IdentityUser!);
            var permissionRole = roles.FirstOrDefault() ?? "User";

            var vm = new EmployeeCreateVM
            {
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Salary = employee.Salary,
                RoleId = employee.RoleId,
                Email = employee.IdentityUser?.Email,
                PermissionRole = permissionRole,
                AvailableRoles = new List<string> { "Admin", "User" }
            };

            ViewBag.Departments = _context.Departments.ToList();
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments.ToList();
                return View(model);
            }
            var employee = _context.Employees.Include(e => e.IdentityUser).FirstOrDefault(e => e.EmployeeId == model.EmployeeId);
            if (employee == null) return NotFound();
            employee.FirstName = model.FirstName;
            employee.LastName = model.LastName;
            employee.Salary = model.Salary;
            employee.RoleId = model.RoleId;

            if (employee.IdentityUser!.Email != model.Email)
            {
                employee.IdentityUser!.Email = model.Email;
                employee.IdentityUser.UserName = model.Email;
                await _userManager.UpdateAsync(employee.IdentityUser);
            }

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(employee.IdentityUser);
                await _userManager.ResetPasswordAsync(employee.IdentityUser, token, model.Password);
            }

            var currentRoles = await _userManager.GetRolesAsync(employee.IdentityUser);
            if (currentRoles != null)
            {
                await _userManager.RemoveFromRolesAsync(employee.IdentityUser, currentRoles);
            }
            await _userManager.AddToRoleAsync(employee.IdentityUser, model.PermissionRole);
            await _signInManager.RefreshSignInAsync(employee.IdentityUser);
            await _context.SaveChangesAsync();
            return RedirectToAction("List");

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
