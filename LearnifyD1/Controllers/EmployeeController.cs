using LearnifyD1.Data;
using LearnifyD1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LearnifyD1.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var  employees = await _context.Employees
                .Include( r => r.Role).ToListAsync();
            return View(employees);
        }
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Add(employee); // RoleId is enough
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // Repopulate roles in case of error
            ViewBag.Roles = new SelectList(_context.Roles, "RoleId", "RoleName", employee.RoleId);
            return View(employee);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            ViewBag.Roles = new SelectList(_context.Roles, "RoleId", "RoleName", employee.RoleId);
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Roles = new SelectList(_context.Roles, "RoleId", "RoleName", employee.RoleId);
            return View(employee);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null) return NotFound();

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }



    }
}
