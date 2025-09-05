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
            if(employee == null)
            {
                return NotFound();
            }
            ViewBag.Departments = _context.Departments.ToList();
            return View(employee);
        }
        [HttpPost]
        public IActionResult Edit(Employee employee)
        {
            if(ModelState.IsValid)
            {
                //var emp = _context.Employees.FirstOrDefault(e => e.EmployeeId == employee.EmployeeId);
                //emp.FirstName = employee.FirstName;
                //emp.LastName = employee.LastName;
                //emp.Salary = employee.Salary;
                //emp.RoleId = employee.RoleId;
                _context.Update(employee);
                _context.SaveChanges();
                return RedirectToAction("List");
            }
            ViewBag.Departments = _context.Departments.ToList();
            return View(employee);
        }
    }
}
