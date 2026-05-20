using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Repositories.Interfaces.Admission;

namespace SchoolManagementSystem.Repositories.Implementations.Admission;

public class AdmissionRepository : BaseRepository<AdmissionApplication>, IAdmissionRepository
{
    public AdmissionRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<AdmissionListResultDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, int classId, int? status, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetAdmissionList";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@PageNumber", pageNumber));
        command.Parameters.Add(new SqlParameter("@PageSize", pageSize));
        command.Parameters.Add(new SqlParameter("@SearchTerm", (object?)searchTerm ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@ClassId", classId));
        command.Parameters.Add(new SqlParameter("@Status", status ?? (object)DBNull.Value));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            var items = new List<AdmissionListResultDto>();

<<<<<<< HEAD
=======
            var ordId = reader.GetOrdinal("Id");
            var ordAppNo = reader.GetOrdinal("ApplicationNo");
            var ordName = reader.GetOrdinal("ApplicantName");
            var ordDob = reader.GetOrdinal("DateOfBirth");
            var ordGender = reader.GetOrdinal("Gender");
            var ordAppClass = reader.GetOrdinal("AppliedClassId");
            var ordClassName = reader.GetOrdinal("ClassName");
            var ordStatus = reader.GetOrdinal("Status");
            var ordFName = reader.GetOrdinal("FatherName");
            var ordFOcc = reader.GetOrdinal("FatherOccupation");
            var ordMName = reader.GetOrdinal("MotherName");
            var ordMOcc = reader.GetOrdinal("MotherOccupation");
            var ordGName = reader.GetOrdinal("GuardianName");
            var ordGOcc = reader.GetOrdinal("GuardianOccupation");
            var ordFGMobile = reader.GetOrdinal("FatherOrGuardianMobileNo");
            var ordAppMobile = reader.GetOrdinal("ApplicantMobileNumber");
            var ordAltNum = reader.GetOrdinal("AlternativeNumber");
            var ordEmail = reader.GetOrdinal("ApplicantEmail");
            var ordNat = reader.GetOrdinal("Nationality");
            var ordRel = reader.GetOrdinal("Religion");
            var ordBlood = reader.GetOrdinal("BloodGroup");
            var ordBcNo = reader.GetOrdinal("BirthCertificateNo");
            var ordBcPath = reader.GetOrdinal("BirthCertificatePath");
            var ordPayPath = reader.GetOrdinal("PaymentSlipPath");
            var ordPayMeth = reader.GetOrdinal("PaymentMethod");
            var ordTrans = reader.GetOrdinal("TransactionDetails");
            var ordPrVill = reader.GetOrdinal("PresentVillage");
            var ordPrPO = reader.GetOrdinal("PresentPostOffice");
            var ordPrThana = reader.GetOrdinal("PresentThana");
            var ordPrDist = reader.GetOrdinal("PresentDistrict");
            var ordPeVill = reader.GetOrdinal("PermanentVillage");
            var ordPePO = reader.GetOrdinal("PermanentPostOffice");
            var ordPeThana = reader.GetOrdinal("PermanentThana");
            var ordPeDist = reader.GetOrdinal("PermanentDistrict");
            var ordPic = reader.GetOrdinal("ProfilePicturePath");
            var ordBy = reader.GetOrdinal("CreatedBy");
            var ordAt = reader.GetOrdinal("CreatedAt");
            var ordTotal = reader.GetOrdinal("TotalRecords");

>>>>>>> d8b24e6 (attendece and website curtomize)
            while (await reader.ReadAsync(ct))
            {
                items.Add(new AdmissionListResultDto
                {
<<<<<<< HEAD
                    Id = reader.GetInt32(0),
                    ApplicationNo = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    ApplicantName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    DateOfBirth = reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3),
                    Gender = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    AppliedClassId = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                    ClassName = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    Status = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    FatherName = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                    FatherOccupation = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                    MotherName = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                    MotherOccupation = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                    GuardianName = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                    GuardianOccupation = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                    FatherOrGuardianMobileNo = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                    ApplicantMobileNumber = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                    AlternativeNumber = reader.IsDBNull(16) ? string.Empty : reader.GetString(16),
                    ApplicantEmail = reader.IsDBNull(17) ? string.Empty : reader.GetString(17),
                    Nationality = reader.IsDBNull(18) ? string.Empty : reader.GetString(18),
                    Religion = reader.IsDBNull(19) ? string.Empty : reader.GetString(19),
                    BloodGroup = reader.IsDBNull(20) ? string.Empty : reader.GetString(20),
                    BirthCertificateNo = reader.IsDBNull(21) ? string.Empty : reader.GetString(21),
                    BirthCertificatePath = reader.IsDBNull(22) ? string.Empty : reader.GetString(22),
                    PaymentSlipPath = reader.IsDBNull(23) ? string.Empty : reader.GetString(23),
                    PaymentMethod = reader.IsDBNull(24) ? string.Empty : reader.GetString(24),
                    TransactionDetails = reader.IsDBNull(25) ? string.Empty : reader.GetString(25),
                    PresentVillage = reader.IsDBNull(26) ? string.Empty : reader.GetString(26),
                    PresentPostOffice = reader.IsDBNull(27) ? string.Empty : reader.GetString(27),
                    PresentThana = reader.IsDBNull(28) ? string.Empty : reader.GetString(28),
                    PresentDistrict = reader.IsDBNull(29) ? string.Empty : reader.GetString(29),
                    PermanentVillage = reader.IsDBNull(30) ? string.Empty : reader.GetString(30),
                    PermanentPostOffice = reader.IsDBNull(31) ? string.Empty : reader.GetString(31),
                    PermanentThana = reader.IsDBNull(32) ? string.Empty : reader.GetString(32),
                    PermanentDistrict = reader.IsDBNull(33) ? string.Empty : reader.GetString(33),
                    ProfilePicturePath = reader.IsDBNull(34) ? string.Empty : reader.GetString(34),
                    CreatedBy = reader.IsDBNull(35) ? string.Empty : reader.GetString(35),
                    CreatedAt = reader.IsDBNull(36) ? DateTime.MinValue : reader.GetDateTime(36),
                    TotalRecords = reader.IsDBNull(37) ? 0 : reader.GetInt32(37)
=======
                    Id = reader.GetInt32(ordId),
                    ApplicationNo = reader.IsDBNull(ordAppNo) ? string.Empty : reader.GetString(ordAppNo),
                    ApplicantName = reader.IsDBNull(ordName) ? string.Empty : reader.GetString(ordName),
                    DateOfBirth = reader.IsDBNull(ordDob) ? DateTime.MinValue : reader.GetDateTime(ordDob),
                    Gender = reader.IsDBNull(ordGender) ? string.Empty : reader.GetString(ordGender),
                    AppliedClassId = reader.IsDBNull(ordAppClass) ? 0 : reader.GetInt32(ordAppClass),
                    ClassName = reader.IsDBNull(ordClassName) ? string.Empty : reader.GetString(ordClassName),
                    Status = reader.IsDBNull(ordStatus) ? string.Empty : reader.GetString(ordStatus),
                    FatherName = reader.IsDBNull(ordFName) ? string.Empty : reader.GetString(ordFName),
                    FatherOccupation = reader.IsDBNull(ordFOcc) ? string.Empty : reader.GetString(ordFOcc),
                    MotherName = reader.IsDBNull(ordMName) ? string.Empty : reader.GetString(ordMName),
                    MotherOccupation = reader.IsDBNull(ordMOcc) ? string.Empty : reader.GetString(ordMOcc),
                    GuardianName = reader.IsDBNull(ordGName) ? string.Empty : reader.GetString(ordGName),
                    GuardianOccupation = reader.IsDBNull(ordGOcc) ? string.Empty : reader.GetString(ordGOcc),
                    FatherOrGuardianMobileNo = reader.IsDBNull(ordFGMobile) ? string.Empty : reader.GetString(ordFGMobile),
                    ApplicantMobileNumber = reader.IsDBNull(ordAppMobile) ? string.Empty : reader.GetString(ordAppMobile),
                    AlternativeNumber = reader.IsDBNull(ordAltNum) ? string.Empty : reader.GetString(ordAltNum),
                    ApplicantEmail = reader.IsDBNull(ordEmail) ? string.Empty : reader.GetString(ordEmail),
                    Nationality = reader.IsDBNull(ordNat) ? string.Empty : reader.GetString(ordNat),
                    Religion = reader.IsDBNull(ordRel) ? string.Empty : reader.GetString(ordRel),
                    BloodGroup = reader.IsDBNull(ordBlood) ? string.Empty : reader.GetString(ordBlood),
                    BirthCertificateNo = reader.IsDBNull(ordBcNo) ? string.Empty : reader.GetString(ordBcNo),
                    BirthCertificatePath = reader.IsDBNull(ordBcPath) ? string.Empty : reader.GetString(ordBcPath),
                    PaymentSlipPath = reader.IsDBNull(ordPayPath) ? string.Empty : reader.GetString(ordPayPath),
                    PaymentMethod = reader.IsDBNull(ordPayMeth) ? string.Empty : reader.GetString(ordPayMeth),
                    TransactionDetails = reader.IsDBNull(ordTrans) ? string.Empty : reader.GetString(ordTrans),
                    PresentVillage = reader.IsDBNull(ordPrVill) ? string.Empty : reader.GetString(ordPrVill),
                    PresentPostOffice = reader.IsDBNull(ordPrPO) ? string.Empty : reader.GetString(ordPrPO),
                    PresentThana = reader.IsDBNull(ordPrThana) ? string.Empty : reader.GetString(ordPrThana),
                    PresentDistrict = reader.IsDBNull(ordPrDist) ? string.Empty : reader.GetString(ordPrDist),
                    PermanentVillage = reader.IsDBNull(ordPeVill) ? string.Empty : reader.GetString(ordPeVill),
                    PermanentPostOffice = reader.IsDBNull(ordPePO) ? string.Empty : reader.GetString(ordPePO),
                    PermanentThana = reader.IsDBNull(ordPeThana) ? string.Empty : reader.GetString(ordPeThana),
                    PermanentDistrict = reader.IsDBNull(ordPeDist) ? string.Empty : reader.GetString(ordPeDist),
                    ProfilePicturePath = reader.IsDBNull(ordPic) ? string.Empty : reader.GetString(ordPic),
                    CreatedBy = reader.IsDBNull(ordBy) ? string.Empty : reader.GetString(ordBy),
                    CreatedAt = reader.IsDBNull(ordAt) ? DateTime.MinValue : reader.GetDateTime(ordAt),
                    TotalRecords = reader.IsDBNull(ordTotal) ? 0 : reader.GetInt32(ordTotal)
>>>>>>> d8b24e6 (attendece and website curtomize)
                });
            }
            return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }
}
