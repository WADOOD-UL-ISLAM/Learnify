using LearnifyD1.Data;
using LearnifyD1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace LearnifyD1.Controllers
{
    public class BatchController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }
            base.OnActionExecuting(context);
        }

        public readonly ApplicationDbContext _context;

        public BatchController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }
        public async Task< IActionResult> Index()
        {
            int currentMonth = DateTime.Now.Month;
            ViewBag.Courses = await _context.Courses.ToListAsync();
            var batches = await _context.Batches.Include(b => b.Course).Where(t => t.EndMonth >= currentMonth)
               .ToListAsync();
            return View(batches);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeCourse(int CourseId)
        {
            List<Batch> batchesInCourse;
            ViewBag.Courses = await _context.Courses.ToListAsync();
            ViewBag.SelectedCourse = CourseId;

            if (CourseId != 0)
            {
                 batchesInCourse = await _context.Batches
                 .Where(b => b.CourseId == CourseId)
                 .ToListAsync();

            }
            else
            {
                batchesInCourse = await _context.Batches.ToListAsync ();
            }

            return View("Index", batchesInCourse);

        }

        [HttpGet]
        public async Task<IActionResult> CreateNewBatch()
        {

            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");
            List<TimeOnly> timeSlots = new();

            for (int hour = 16; hour <= 22; hour++)
            {
                timeSlots.Add(new TimeOnly(hour, 0));   // e.g. 16:00
                if (hour < 22)
                    timeSlots.Add(new TimeOnly(hour, 30)); // e.g. 16:30 (but not after 22)
            }



            ViewBag.TimeSlots = timeSlots;

            var instructors = _context.Employees
              .Include(e => e.Role)
              .Where(r => r.Role.RoleName == "Instructor")
              .ToList();

            ViewBag.Instructors = new SelectList(instructors, "EmployeeId", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewBatch(Batch batch)
        {

            _context.Batches.Add(batch);
            await _context.SaveChangesAsync();
           
            ViewBag.Courses = await _context.Courses.ToListAsync();
            var batches = await _context.Batches.ToListAsync();
            List<TimeOnly> timeSlots = new();

            for (int hour = 16; hour <= 22; hour++)
            {
                timeSlots.Add(new TimeOnly(hour, 0));   // e.g. 16:00
                if (hour < 22)
                    timeSlots.Add(new TimeOnly(hour, 30)); // e.g. 16:30 (but not after 22)
            }

            ViewBag.TimeSlots = timeSlots;

            var instructors = _context.Employees
                .Include(e => e.Role)
                .Where(r => r.Role.RoleName == "Instructor")
                .ToList();

            ViewBag.Instructors = new SelectList(instructors, "EmployeeId", "Name");

            return View("Index",batches);

        }

        [HttpGet]
        public async Task<IActionResult> EditBatch(int id)
        {
            
            Batch batch = await _context.Batches
                .Include(c => c.Course)
                .FirstOrDefaultAsync(b => b.BatchId == id);
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");
            List<TimeOnly> timeSlots = new();

            for (int hour = 16; hour <= 22; hour++)
            {
                timeSlots.Add(new TimeOnly(hour, 0));   // e.g. 16:00
                if (hour < 22)
                    timeSlots.Add(new TimeOnly(hour, 30)); // e.g. 16:30 (but not after 22)
            }

            ViewBag.TimeSlots = timeSlots;

            var instructors = _context.Employees
            .Include(e => e.Role)
            .Where(r => r.Role.RoleName == "Instructor")
            .ToList();

            ViewBag.Instructors = new SelectList(instructors, "EmployeeId", "Name");
            if (batch == null)
            {
                return NotFound();
            }
            return View(batch);
        }

        [HttpPost]
        public async Task<IActionResult> EditBatch(int id, Batch batch)
        {
            if (id != batch.BatchId)
            {
                return NotFound();
            }
            _context.Batches.Update(batch);
            await _context.SaveChangesAsync();
            ViewBag.Courses = await _context.Courses.ToListAsync();
            var batches = await _context.Batches.ToListAsync();
            var instructors = _context.Employees
            .Include(e => e.Role)
            .Where(r => r.Role.RoleName == "Instructor")
            .ToList();

            ViewBag.Instructors = new SelectList(instructors, "EmployeeId", "Name");
            return View("Index", batches);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var batch = await _context.Batches.FindAsync(id);
            if (batch == null)
            {
                return NotFound();
            }
            return View(batch);
        }

        [HttpPost]

        public async Task<IActionResult> DeleteBatch(Batch batch)
        {
            if(batch == null)
            {
                return NotFound();
            }

            _context.Batches.Remove(batch);
            await _context.SaveChangesAsync();
            ViewBag.Courses = await _context.Courses.ToListAsync();
            var batches = await _context.Batches.ToListAsync();
            return View("Index", batches);


        }




    }
}
