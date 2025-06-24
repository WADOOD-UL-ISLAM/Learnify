using LearnifyD1.Data;
using LearnifyD1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace LearnifyD1.Controllers
{
    public class BatchController : Controller
    {
        public readonly ApplicationDbContext _context;

        public BatchController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }
        public async Task< IActionResult> Index()
        {
            ViewBag.Courses = await _context.Courses.ToListAsync();
            var batches = await _context.Batches.
                Include(c => c.Course)
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewBatch(Batch batch)
        {

            _context.Batches.Add(batch);
            await _context.SaveChangesAsync();
           
            ViewBag.Courses = await _context.Courses.ToListAsync();
            var batches = await _context.Batches.ToListAsync();
            return View("Index",batches);

        }

        [HttpGet]
        public async Task<IActionResult> EditBatch(int id)
        {
            
            Batch batch = await _context.Batches
                .Include(c => c.Course)
                .FirstOrDefaultAsync(b => b.BatchId == id);
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "CourseName");
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
