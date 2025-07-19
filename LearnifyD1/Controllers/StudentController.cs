using DinkToPdf;
using DinkToPdf.Contracts;
using Humanizer;
using LearnifyD1.Data;
using LearnifyD1.Models;
using LearnifyD1.Models.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace LearnifyD1.Controllers
{
    public class StudentController : Controller
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
        private readonly IWebHostEnvironment _env;
       

        public StudentController(ApplicationDbContext dbContext, IWebHostEnvironment env )
        {
            _context = dbContext;
            _env = env;
            
        }


        public async Task<IActionResult> Index(string searchString, int page = 1, int pageSize = 10)
        {
            var query = _context.Students
                .Include(s => s.studentBatches)
                .ThenInclude(sb => sb.batch)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s =>
                    s.StudentName.Contains(searchString) ||
                    s.StudentEmail.Contains(searchString));
            }

            int totalStudents = await query.CountAsync();

            var students = await query
                .OrderBy(s => s.StudentName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalStudents = totalStudents;
            ViewBag.SearchString = searchString;

            return View(students);
        }



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
        public async Task<IActionResult> AdmissionForm()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdmissionForm(Student student)
        {
            bool exists = await _context.Students.AnyAsync(s => s.CNIC == student.CNIC);
            if (exists)
            {
                ModelState.AddModelError("CNIC", "This CNIC is already registered.");
            }

            if (ModelState.IsValid)
            {
                // Image Upload
                if (student.ImageFile != null && student.ImageFile.Length > 0)
                {
                    var folderPath = Path.Combine(_env.WebRootPath, "images", "students");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    var uniqueFile = $"{Guid.NewGuid()}_{Path.GetFileName(student.ImageFile.FileName)}";
                    var fullPath = Path.Combine(folderPath, uniqueFile);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await student.ImageFile.CopyToAsync(stream);

                    student.ImagePath = $"/images/students/{uniqueFile}";
                }

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                TempData["success"] = "Admission form submitted successfully!";
                return RedirectToAction("Index"); // or RedirectToAction("AdmissionSuccess");
            }

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

            int currentMonth = DateTime.Now.Month;
            // Get all available batches
            ViewBag.Batches = await _context.Batches.Include(b => b.Course).Where( t => t.EndMonth >= currentMonth)
                .ToListAsync();
            // Get currently selected batch IDs
            ViewBag.SelectedBatches = student.studentBatches.Select(sb => sb.BatchId).ToList();

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student, List<int> selectedBatches)
        {
    
           
            var studentInDb = await _context.Students
                .Include(s => s.studentBatches)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (studentInDb == null)
                return NotFound();

            // 3️⃣ Update student batches
            var existing = await _context.studentBatches
                .Where(sb => sb.StudentId == id)
                .ToListAsync();

            _context.studentBatches.RemoveRange(existing);

            if (selectedBatches != null)
            {
                foreach (var batchId in selectedBatches)
                {
                    _context.studentBatches.Add(new StudentBatch
                    {
                        StudentId = id,
                        BatchId = batchId,
                        dateTime = DateTime.Now
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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

            int totalFee = 0;
            int paidFee = 0;

            // 👇 Store batch-wise status here
            var batchStatuses = new Dictionary<int, bool>(); // BatchId -> isPaidFully

            foreach (var sb in student.studentBatches)
            {
                var courseFee = sb.batch.Course.Fees;
                totalFee += courseFee;

                var feeRecords = await _context.FeeRecords
                    .Where(f => f.StudentId == id && f.BatchId == sb.BatchId)
                    .ToListAsync();

                int paidForBatch = feeRecords.Sum(f => f.AmountPaid);
                paidFee += paidForBatch;

                batchStatuses[sb.BatchId] = paidForBatch >= courseFee;
            }

            ViewBag.TotalFee = totalFee;
            ViewBag.PaidFee = paidFee;
            ViewBag.Remaining = totalFee - paidFee;
            ViewBag.BatchStatuses = batchStatuses;




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

            var feeRecords = await _context.FeeRecords
           .Where(f => f.StudentId == studentid && f.BatchId == batchid)
           .OrderByDescending(f => f.PaymentDate)
           .ToListAsync();

            ViewBag.FeeRecords = feeRecords;

            return View(enrollment);
        }

        [HttpPost]
        public async Task<IActionResult> AddFeePayment(int StudentId, int BatchId, DateTime PaymentDate, int AmountPaid)
        {
            var fee = new FeeRecord
            {
                StudentId = StudentId,
                BatchId = BatchId,
                PaymentDate = PaymentDate,
                AmountPaid = AmountPaid
            };

            _context.FeeRecords.Add(fee);
            await _context.SaveChangesAsync();

            return RedirectToAction("BatchDetail", new { studentId = StudentId, batchId = BatchId });
        }

        public async Task<IActionResult> ShowReceipt(int studentId, int batchId)
        {
            var student = await _context.Students.FindAsync(studentId);
            var batch = await _context.Batches.Include(b => b.Course).FirstOrDefaultAsync(b => b.BatchId == batchId);
            var feeRecords = await _context.FeeRecords
                .Where(f => f.StudentId == studentId && f.BatchId == batchId)
                .ToListAsync();

            var lastPayment = feeRecords.LastOrDefault();
            if (lastPayment == null) return NotFound();

            var model = new ReceiptViewModel
            {
                ReceiptNumber = $"RCP-{Guid.NewGuid().ToString().Substring(0, 6)}",
                StudentName = student.StudentName,
                StudentEmail = student.StudentEmail,
                PaymentDate = lastPayment.PaymentDate,
                AmountPaid = lastPayment.AmountPaid,
                AmountInWords = ConvertToWords(lastPayment.AmountPaid),
                CourseName = batch.Course.CourseName,
                PaymentMethod = "Cash",
                PaidSoFar = feeRecords.Sum(f => f.AmountPaid),
                Due = batch.Course.Fees - feeRecords.Sum(f => f.AmountPaid),
                FeeRecords = feeRecords.OrderBy(f => f.PaymentDate).ToList() // 👈 Add this
            };

            return View("FeeReceipt", model);
        }
         string ConvertToWords(int number) => number.ToWords().Transform(To.TitleCase) + " Rupees Only";
        [HttpGet]
        public async Task<IActionResult> AdmissionFormPreview(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            
            string imagePath = $"/images/students/student_{student.StudentId}.jpg";
            string physicalPath = Path.Combine(_env.WebRootPath, "images", "students", $"student_{student.StudentId}.jpg");

            if (System.IO.File.Exists(physicalPath))
            {
                student.ImagePath = imagePath;
            }

            return View(student);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdmissionFormPreview(int id, Student model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            // Update all fields except image
            student.StudentName = model.StudentName;
            student.FatherName = model.FatherName;
            student.CNIC = model.CNIC;
            student.DateofBirth = model.DateofBirth;
            student.Gender = model.Gender;
            student.age = model.age;
            student.StudentMobileNumber = model.StudentMobileNumber;
            student.StudentEmail = model.StudentEmail;
            student.GuardianMobileNumber = model.GuardianMobileNumber;
            student.Address = model.Address;

            // Handle image upload if new file was provided
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "students");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Delete old image if exists
                var oldImagePath = Path.Combine(uploadsFolder, $"student_{student.StudentId}.jpg");
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                // Save new image
                var fileName = $"student_{student.StudentId}.jpg";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                student.ImagePath = $"/images/students/{fileName}";
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Student updated successfully!";
            return RedirectToAction("AdmissionFormPreview", new { id = student.StudentId });
        }




    }

}
