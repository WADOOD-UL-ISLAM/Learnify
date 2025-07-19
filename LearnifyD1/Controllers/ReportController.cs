using LearnifyD1.Data;
using LearnifyD1.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearnifyD1.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
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

    }
}
