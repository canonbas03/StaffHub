using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffHub.Data;
using StaffHub.Models;

namespace StaffHub.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            var employees = _context.Employees.Include(e => e.Role).ThenInclude(e => e.Department).ToList();
            return View(employees);
        }

        public IActionResult Add()
        {
            ViewBag.Departments = _context.Departments.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Add(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Add(employee);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            ViewBag.Departments = _context.Departments.ToList();
            return View(employee);
        }
        [HttpGet]
        public JsonResult GetRolesByDepartment(int id)
        {
            var roles = _context.Roles.Where(r => r.RoleId == id).Select(r => new {r.RoleId,r.Name}).ToList();
            return Json(roles);
        }
    }
}
