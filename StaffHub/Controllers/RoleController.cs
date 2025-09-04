using Microsoft.AspNetCore.Mvc;
using StaffHub.Data;
using StaffHub.Models;

namespace StaffHub.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            ViewBag.Departments = _context.Departments.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(Role role)
        {
            if(ModelState.IsValid)
            {
                _context.Roles.Add(role);
                _context.SaveChanges();
                return RedirectToAction("Create");
            }
            ViewBag.Departments = _context.Departments.ToList();    
            return View(role);
        }
    }
}
