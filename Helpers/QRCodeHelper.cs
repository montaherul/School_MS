using QRCoder;
using System;
using System.Drawing;
using System.IO;
using System.Web;
using SchoolManagementSystem.Models.DTOs.Employee;

namespace SchoolManagementSystem.Helpers
{
    public static class QRCodeHelper
    {
        public static string GenerateEmployeeQrCode(EmployeeDetailsDto dto, string verificationBaseUrl)
        {
            var payload = new
            {
                EmployeeId = dto.Id,
                EmployeeCode = dto.EmployeeCode,
                FullName = dto.FullName,
                Designation = dto.Designation,
                Department = dto.Department,
                VerificationUrl = $"{verificationBaseUrl}/Employee/Verify/{dto.Id}"
            };
            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(json, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrData);
            using var bitmap = qrCode.GetGraphic(20, "#000000", "#FFFFFF", true);
            using var ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            var base64 = Convert.ToBase64String(ms.ToArray());
            return $"data:image/png;base64,{base64}";
        }
    }
}
