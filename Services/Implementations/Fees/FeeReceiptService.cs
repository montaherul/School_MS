using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Helpers.Pdf;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FeeReceiptService : IFeeReceiptService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IWebHostEnvironment _env;

    public FeeReceiptService(IUnitOfWork unitOfWork, IPdfGenerator pdfGenerator, IWebHostEnvironment env)
    {
        _unitOfWork = unitOfWork;
        _pdfGenerator = pdfGenerator;
        _env = env;
    }

    public async Task<FeeReceiptDto?> GetReceiptDataAsync(int paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await _unitOfWork.Repository<Payment>()
            .FirstOrDefaultAsync(x => x.Id == paymentId && !x.IsDeleted, cancellationToken);
        if (payment is null) return null;

        var invoice = await _unitOfWork.Repository<FeeInvoice>()
            .FirstOrDefaultAsync(x => x.Id == payment.FeeInvoiceId && !x.IsDeleted, cancellationToken);
        if (invoice is null) return null;

        var student = await _unitOfWork.Repository<Student>().GetByIdAsync(invoice.StudentId, cancellationToken);
        var schoolSetting = await _unitOfWork.Repository<SchoolSetting>().Query().AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        var logoBase64 = string.Empty;
        if (!string.IsNullOrEmpty(schoolSetting?.LogoPath))
        {
            var logoFullPath = Path.Combine(_env.WebRootPath, schoolSetting.LogoPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(logoFullPath))
            {
                var ext = Path.GetExtension(logoFullPath).ToLowerInvariant();
                var mime = ext switch
                {
                    ".png" => "image/png",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".gif" => "image/gif",
                    ".svg" => "image/svg+xml",
                    _ => "image/png"
                };
                var bytes = await File.ReadAllBytesAsync(logoFullPath, cancellationToken);
                logoBase64 = $"data:{mime};base64,{Convert.ToBase64String(bytes)}";
            }
        }

        return new FeeReceiptDto
        {
            ReceiptNo = $"RCP-{payment.Id:D6}",
            InvoiceNo = invoice.InvoiceNo,
            StudentId = invoice.StudentId,
            StudentName = student?.FullName ?? "N/A",
            StudentIdNo = student?.StudentNo ?? "N/A",
            ClassName = "",
            SectionName = "",
            Amount = payment.Amount,
            LateFee = payment.LateFee,
            DiscountAmount = payment.DiscountAmount,
            NetAmount = payment.Amount + payment.LateFee - payment.DiscountAmount,
            PaymentMethod = payment.Method.ToString(),
            ReferenceNo = payment.ReferenceNo ?? "",
            PaidAt = payment.PaidAt,
            SchoolName = schoolSetting?.SchoolName ?? "School Management System",
            SchoolAddress = schoolSetting?.Address ?? "",
            SchoolPhone = schoolSetting?.Phone ?? "",
            SchoolEmail = schoolSetting?.Email ?? "",
            SchoolWebsite = schoolSetting?.Website ?? "",
            SchoolMotto = schoolSetting?.SchoolMotto ?? "",
            SchoolLogoBase64 = logoBase64,
            QrVerificationCode = GenerateVerificationCode(payment.Id, payment.PaidAt)
        };
    }

    public async Task<byte[]> GenerateReceiptPdfAsync(int paymentId, CancellationToken cancellationToken = default)
    {
        var data = await GetReceiptDataAsync(paymentId, cancellationToken);
        if (data is null) return [];

        var html = BuildReceiptHtml(data);
        return _pdfGenerator.GenerateFromHtml(html);
    }

    public string GenerateVerificationCode(int paymentId, DateTime paidAt)
    {
        var input = $"{paymentId}|{paidAt:yyyyMMddHHmm}|SCHOOL-SECRET-KEY";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hash)[..12];
    }

    private static string BuildReceiptHtml(FeeReceiptDto data)
    {
        var logoHtml = !string.IsNullOrEmpty(data.SchoolLogoBase64)
            ? $"<img src='{data.SchoolLogoBase64}' alt='School Logo' style='max-height:70px;max-width:200px;margin-bottom:8px;'/>"
            : "";
        var mottoHtml = !string.IsNullOrEmpty(data.SchoolMotto)
            ? $"<p style='margin:2px 0;color:#888;font-style:italic;font-size:12px;'>{data.SchoolMotto}</p>"
            : "";

        return $@"<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8'/>
<style>
  body {{ font-family: 'Segoe UI', Arial, sans-serif; margin: 0; padding: 20px; color: #333; }}
  .receipt {{ max-width: 800px; margin: 0 auto; border: 1px solid #ddd; padding: 30px; }}
  .header {{ text-align: center; border-bottom: 2px solid #1a56db; padding-bottom: 15px; margin-bottom: 20px; }}
  .header h1 {{ color: #1a56db; margin: 0; font-size: 24px; }}
  .header p {{ margin: 3px 0; color: #666; font-size: 12px; }}
  .title {{ text-align: center; font-size: 18px; font-weight: bold; margin: 15px 0; }}
  .details {{ width: 100%; border-collapse: collapse; margin: 15px 0; }}
  .details td {{ padding: 6px 10px; border-bottom: 1px solid #eee; font-size: 13px; }}
  .details td:first-child {{ font-weight: 600; width: 140px; color: #555; }}
  .amount-table {{ width: 100%; border-collapse: collapse; margin: 15px 0; }}
  .amount-table th {{ background: #f8f9fa; padding: 8px 10px; text-align: left; border-bottom: 2px solid #ddd; font-size: 13px; }}
  .amount-table td {{ padding: 8px 10px; border-bottom: 1px solid #eee; font-size: 13px; }}
  .amount-table .total {{ font-weight: bold; font-size: 15px; }}
  .footer {{ text-align: center; margin-top: 25px; padding-top: 15px; border-top: 1px solid #ddd; font-size: 11px; color: #999; }}
  .qr {{ text-align: center; margin: 15px 0; }}
  .qr-code {{ font-family: monospace; font-size: 10px; color: #666; }}
</style>
</head>
<body>
<div class='receipt'>
  <div class='header'>
    {logoHtml}
    <h1>{data.SchoolName}</h1>
    {mottoHtml}
    <p>{data.SchoolAddress}</p>
    <p>Phone: {data.SchoolPhone} | Email: {data.SchoolEmail} | Web: {data.SchoolWebsite}</p>
  </div>
  <div class='title'>PAYMENT RECEIPT</div>
  <table class='details'>
    <tr><td>Receipt No</td><td>{data.ReceiptNo}</td></tr>
    <tr><td>Invoice No</td><td>{data.InvoiceNo}</td></tr>
    <tr><td>Student Name</td><td>{data.StudentName}</td></tr>
    <tr><td>Student ID</td><td>{data.StudentIdNo}</td></tr>
    <tr><td>Payment Date</td><td>{data.PaidAt:dd-MMM-yyyy HH:mm}</td></tr>
    <tr><td>Payment Method</td><td>{data.PaymentMethod}</td></tr>
    <tr><td>Reference No</td><td>{data.ReferenceNo}</td></tr>
  </table>
  <table class='amount-table'>
    <tr><th>Description</th><th style='text-align:right'>Amount</th></tr>
    <tr><td>Fee Amount</td><td style='text-align:right'>{data.Amount:N2}</td></tr>
    <tr><td>Late Fee</td><td style='text-align:right'>{data.LateFee:N2}</td></tr>
    <tr><td>Discount</td><td style='text-align:right'>({data.DiscountAmount:N2})</td></tr>
    <tr class='total'><td>Net Amount Paid</td><td style='text-align:right'>{data.NetAmount:N2}</td></tr>
  </table>
  <div class='qr'>
    <div class='qr-code'>Verification: {data.QrVerificationCode}</div>
  </div>
  <div class='footer'>
    <p>{data.SchoolName} &mdash; {data.SchoolAddress}</p>
    <p>Phone: {data.SchoolPhone} | Email: {data.SchoolEmail} | {data.SchoolWebsite}</p>
    <p>This is a computer-generated receipt. No signature required.</p>
    <p>Verification code: {data.QrVerificationCode}</p>
  </div>
</div>
</body>
</html>";
    }
}
