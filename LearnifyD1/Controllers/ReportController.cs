using LearnifyD1.Data;
using LearnifyD1.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace LearnifyD1.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            
            _context = context;
        }

        public async Task <IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> UnpaidFees()
        {
            var unpaidList = await _context.studentBatches
                .Include(sb => sb.student)
                .Include(sb => sb.batch)
                    .ThenInclude(b => b.Course)
                .GroupJoin(_context.FeeRecords,
                    sb => new { sb.StudentId, sb.BatchId },
                    fr => new { fr.StudentId, fr.BatchId },
                    (sb, feeRecords) => new { sb, feeRecords })
                .SelectMany(
                    s => s.feeRecords.DefaultIfEmpty(),
                    (s, fee) => new
                    {
                        s.sb.student.StudentName,
                        s.sb.student.StudentMobileNumber,
                        s.sb.student.GuardianMobileNumber,
                        BatchName = s.sb.batch.BatchName,
                        TotalFee = s.sb.batch.Course.Fees,
                        PaidAmount = fee != null ? fee.AmountPaid : 0,
                        RemainingAmount = s.sb.batch.Course.Fees - (fee != null ? fee.AmountPaid : 0)
                    })
                .Where(x => x.RemainingAmount > 0)
                .ToListAsync();

            var viewModelList = unpaidList.Select(x => new UnpaidFeeViewModel
            {
                StudentName = x.StudentName,
                StudentMobile = x.StudentMobileNumber,
                GuardianMobile = x.GuardianMobileNumber,
                BatchName = x.BatchName,
                TotalFee = x.TotalFee,
                PaidAmount = x.PaidAmount
            }).ToList();

            return View(viewModelList);
        }

        public async Task<IActionResult> StudentAdmissionReport(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.studentBatches
                .Include(sb => sb.student)
                .Include(sb => sb.batch)
                    .ThenInclude(b => b.Course)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(sb => sb.dateTime.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(sb => sb.dateTime.Date <= toDate.Value.Date);

            var result = await query.Select(sb => new
            {
                StudentName = sb.student.StudentName,
                Email = sb.student.StudentEmail,
                Mobile = sb.student.StudentMobileNumber,
                AdmissionDate = sb.dateTime,
                BatchName = sb.batch.BatchName,
                CourseName = sb.batch.Course.CourseName
            }).ToListAsync();

            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            return View(result);
        }

        public async Task<IActionResult> FeeReport(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.FeeRecords
                .Include(fr => fr.Student)
                .Include(fr => fr.Batch)
                    .ThenInclude(b => b.Course)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(fr => fr.PaymentDate.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(fr => fr.PaymentDate.Date <= toDate.Value.Date);

            var result = await query
                .Select(fr => new
                {
                    StudentName = fr.Student.StudentName,
                    BatchName = fr.Batch.BatchName,
                    CourseName = fr.Batch.Course.CourseName,
                    AmountPaid = fr.AmountPaid,
                    PaymentDate = fr.PaymentDate
                }).ToListAsync();

            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            return View(result);
        }

        public async Task<IActionResult> BatchWiseReport()
        {
            var batchData = await _context.Batches
                .Include(b => b.Course)
                .Include(b => b.Instructor)
                .Include(b => b.studentbatches)
                .ToListAsync();

            var feeData = await _context.FeeRecords
                .GroupBy(fr => fr.BatchId)
                .Select(g => new
                {
                    BatchId = g.Key,
                    TotalFees = g.Sum(fr => fr.AmountPaid)
                }).ToListAsync();

            var result = batchData.Select(b => new
            {
                BatchName = b.BatchName,
                CourseName = b.Course.CourseName,
                TotalStudents = b.studentbatches.Count,
                TotalFeesCollected = feeData.FirstOrDefault(f => f.BatchId == b.BatchId)?.TotalFees ?? 0,
                InstructorName = b.Instructor != null ? b.Instructor.Name : "N/A"
            }).ToList();

            return View(result);
        }

        public async Task<IActionResult> InstructorWiseReport()
        {
            var instructors = await _context.Employees
                .Include(e => e.Role)
                .Where(e => e.Role.RoleName.ToLower() == "instructor")
                .ToListAsync();

            var batches = await _context.Batches
                .Include(b => b.studentbatches)
                .ToListAsync();

            var result = instructors.Select(i => new
            {
                InstructorName = i.Name,
                Role = i.Role.RoleName,
                AssignedBatches = batches
                    .Where(b => b.InstructorId == i.EmployeeId)
                    .Select(b => new
                    {
                        BatchName = b.BatchName,
                        TotalStudents = b.studentbatches.Count
                    }).ToList()
            }).ToList();

            return View(result);
        }

        public async Task<IActionResult> MonthlyCollection(int? year)
        {
            int selectedYear = year ?? DateTime.Now.Year;

            // Get all fee records for the selected year
            var feeData = await _context.FeeRecords
                .Where(fr => fr.PaymentDate.Year == selectedYear)
                .GroupBy(fr => fr.PaymentDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalAmount = g.Sum(fr => fr.AmountPaid)
                })
                .ToListAsync();

            // Build full year (all 12 months)
            var fullYearData = Enumerable.Range(1, 12)
                .Select(month => new
                {
                    Month = month,
                    TotalAmount = feeData.FirstOrDefault(fd => fd.Month == month)?.TotalAmount ?? 0
                })
                .ToList();

            ViewBag.SelectedYear = selectedYear;
            ViewBag.Years = await _context.FeeRecords
                .Select(fr => fr.PaymentDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            return View(fullYearData);
        }

        public async Task<IActionResult> CourseWiseStudentCount()
        {
            int currentMonth = DateTime.Now.Month;

            var result = await _context.Courses
                .Include(c => c.Batches)
                    .ThenInclude(b => b.studentbatches)
                .Select(c => new
                {
                    CourseName = c.CourseName,
                    ActiveBatches = c.Batches.Where(b => b.EndMonth >= currentMonth).ToList(),
                })
                .Select(x => new
                {
                    CourseName = x.CourseName,
                    TotalBatches = x.ActiveBatches.Count,
                    TotalStudents = x.ActiveBatches
                        .SelectMany(b => b.studentbatches)
                        .Select(sb => sb.StudentId)
                        .Distinct()
                        .Count()
                })
                .ToListAsync();

            return View(result);
        }

        public async Task<IActionResult> AllStudents()
        {
            var students = await _context.Students
                .Include(s => s.studentBatches)
                    .ThenInclude(sb => sb.batch)
                .Select(s => new
                {
                    s.StudentName,
                    s.FatherName,
                    s.CNIC,
                    s.DateofBirth,
                    s.age,
                    s.Gender,
                    s.StudentEmail,
                    s.StudentMobileNumber,
                    s.GuardianMobileNumber,
                    s.Address,
                    AdmissionDate = s.studentBatches
                        .OrderBy(sb => sb.dateTime)
                        .Select(sb => sb.dateTime)
                        .FirstOrDefault(),
                    Batches = s.studentBatches
                        .Select(sb => sb.batch.BatchName)
                        .Distinct()
                })
                .ToListAsync();

            return View(students);
        }


    }
}
