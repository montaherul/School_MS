using QRCoder;

namespace SchoolManagementSystem.Helpers;

public static class IdCardQRHelper
{
    private static readonly byte[] DarkBlue = [0x1B, 0x4D, 0x8C, 0xFF];
    private static readonly byte[] White = [0xFF, 0xFF, 0xFF, 0xFF];

    public static string GenerateQrCodeBase64(string data)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.M);
        using var qrCode = new PngByteQRCode(qrData);
        var qrBytes = qrCode.GetGraphic(10, DarkBlue, White, true);
        return $"data:image/png;base64,{Convert.ToBase64String(qrBytes)}";
    }

    public static byte[] GenerateQrCodePng(string data)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.M);
        using var qrCode = new PngByteQRCode(qrData);
        return qrCode.GetGraphic(10, DarkBlue, White, true);
    }
}
