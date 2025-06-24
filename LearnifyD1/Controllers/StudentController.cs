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
        private readonly IWebHostEnvironment _env;

        public StudentController(ApplicationDbContext dbContext, IWebHostEnvironment env)
        {
            _context = dbContext;
            _env = env;
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

        //public async Task <IActionResult> CreateEnrollment()
        //{
            
   
        //    return View();
        //}

        public async Task<IActionResult> Create()
        {
            ViewBag.Batches = await _context.Batches.Include(b => b.Course).ToListAsync();
            ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "CourseId", "CourseName");
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student, List<int> selectedBatches)
        {
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Batches = await _context.Batches.Include(b => b.Course).ToListAsync();
            //    return View(student);
            //}

            // ─── 1️⃣  HANDLE IMAGE UPLOAD ──────────────────────────────────────────────
            if (student.ImageFile != null && student.ImageFile.Length > 0)
            {
                // ‑‑ Ensure target folder exists
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "students");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // ‑‑ Generate unique file name → GUID_originalName.ext
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(student.ImageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // ‑‑ Copy to disk
                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await student.ImageFile.CopyToAsync(stream);
                }

                // ‑‑ Save **relative** path for <img src=""> usage
                student.ImagePath = $"/images/students/{uniqueFileName}";
            }
            // (If no file uploaded, ImagePath stays null or whatever default you prefer)

            // ─── 2️⃣  SAVE STUDENT ─────────────────────────────────────────────────────
            _context.Add(student);
            await _context.SaveChangesAsync();

            // ─── 3️⃣  ASSOCIATE BATCHES (unchanged) ───────────────────────────────────
            if (selectedBatches?.Any() == true)
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

        
        public async Task<IActionResult> Details(int id)
        {
            var student = await _context.Students
                .Include(s => s.studentBatches)
                .ThenInclude(b => b.batch)
                .ThenInclude(c => c.Course)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        public async Task<IActionResult> BatchDetail(int studentid , int batchid)
        {
            var enrollment = await _context.studentBatches
                .Include(sb => sb.student)
                .Include(sb => sb.batch)
                .ThenInclude(b => b.Course)
                .FirstOrDefaultAsync(sb => sb.StudentId == studentid && sb.BatchId == batchid);

            if (enrollment == null) {
                return NotFound();
            }
            return View(enrollment);
        }


    }

}
