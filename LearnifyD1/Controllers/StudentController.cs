using LearnifyD1.Data;
using LearnifyD1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace LearnifyD1.Controllers
{
    public class StudentController : Controller
    {
        public readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext dbContext)
        {
            _context = dbContext;

        }
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .Include(sb => sb.studentBatches)
                .ThenInclude(b => b.batch)
                .ThenInclude(c => c.Course)
                .ToListAsync();

            return View(students);
        }

        public async Task <IActionResult> CreateEnrollment()
        {
            
   
            return View();
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Batches = await _context.Batches.Include(b => b.Course).ToListAsync();
            ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "CourseId", "CourseName");
            return View();
        }

        
       
        [HttpPost]
        public async Task<IActionResult> Create(Student student, List<int> selectedBatches)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();

                if (selectedBatches != null)
                {
                    foreach (var batchId in selectedBatches)
                    {
                        _context.studentBatches.Add(new StudentBatch
                        {
                            StudentId = student.StudentId,
                            BatchId = batchId,
                            dateTime = DateTime.Now
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Batches = await _context.Batches.Include(b => b.Course).ToListAsync();
            return View(student);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.studentBatches)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            // Get all available batches
            ViewBag.Batches = await _context.Batches.Include(b => b.Course).ToListAsync();
            // Get currently selected batch IDs
            ViewBag.SelectedBatches = student.studentBatches.Select(sb => sb.BatchId).ToList();

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student, List<int> selectedBatches)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Update student basic info
                    _context.Update(student);

                    // 2. Simplify batch update process
                    var existingBatches = await _context.studentBatches
                        .Where(sb => sb.StudentId == id)
                        .ToListAsync();

                    // Remove all existing batches first
                    _context.studentBatches.RemoveRange(existingBatches);

                    // Add newly selected batches
                    if (selectedBatches != null)
                    {
                        foreach (var batchId in selectedBatches)
                        {
                            _context.studentBatches.Add(new StudentBatch
                            {
                                StudentId = student.StudentId,
                                BatchId = batchId,
                                dateTime = DateTime.Now
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentId))
                        return NotFound();
                    throw;
                }
            }

            // Reload view data if model state is invalid
            ViewBag.Batches = await _context.Batches.Include(b => b.Course).ToListAsync();
            ViewBag.SelectedBatches = selectedBatches ?? new List<int>();
            return View(student);
        }
        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students
                .Include(s => s.studentBatches)
                .FirstOrDefaultAsync(s => s.StudentId == id);
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Student student)
        {
            if(student == null)
            {
                return NotFound();
            }

            _context.studentBatches.RemoveRange(student.studentBatches);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            var students = await _context.Students
                .Include(sb => sb.studentBatches)
                .ThenInclude(b => b.batch)
                .ThenInclude(c => c.Course)
                .ToListAsync();

            return View("Index",students);
        }


    }
}
