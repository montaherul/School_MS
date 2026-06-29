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

            var ordId = reader.GetOrdinal("Id");
            var ordAppNo = reader.GetOrdinal("ApplicationNo");
            var ordName = reader.GetOrdinal("ApplicantName");
            var ordNameBangla = reader.GetOrdinal("ApplicantNameBangla");
            var ordDob = reader.GetOrdinal("DateOfBirth");
            var ordGender = reader.GetOrdinal("Gender");
            var ordAppClass = reader.GetOrdinal("AppliedClassId");
            var ordAppliedGroupId = reader.GetOrdinal("AppliedStudentGroupId");
            var ordAppliedGroupName = reader.GetOrdinal("AppliedStudentGroupName");
            var ordClassName = reader.GetOrdinal("ClassName");
            var ordStatus = reader.GetOrdinal("Status");
            var ordFName = reader.GetOrdinal("FatherName");
            var ordFOcc = reader.GetOrdinal("FatherOccupation");
            var ordMName = reader.GetOrdinal("MotherName");
            var ordMOcc = reader.GetOrdinal("MotherOccupation");
            var ordGName = reader.GetOrdinal("GuardianName");
            var ordGOcc = reader.GetOrdinal("GuardianOccupation");
            var ordGEmail = reader.GetOrdinal("GuardianEmail");
            var ordGMobile = reader.GetOrdinal("GuardianMobileNumber");
            var ordGRel = reader.GetOrdinal("GuardianRelationship");
            var ordGNid = reader.GetOrdinal("GuardianNationalId");
            var ordGAddr = reader.GetOrdinal("GuardianAddress");
            var ordGPhoto = reader.GetOrdinal("GuardianPhoto");
            var ordGRem = reader.GetOrdinal("GuardianRemarks");
            var ordGLink = reader.GetOrdinal("LinkedGuardianId");
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

            while (await reader.ReadAsync(ct))
            {
                items.Add(new AdmissionListResultDto
                {
                    Id = reader.GetInt32(ordId),
                    ApplicationNo = reader.IsDBNull(ordAppNo) ? string.Empty : reader.GetString(ordAppNo),
                    ApplicantName = reader.IsDBNull(ordName) ? string.Empty : reader.GetString(ordName),
                    ApplicantNameBangla = reader.IsDBNull(ordNameBangla) ? null : reader.GetString(ordNameBangla),
                    DateOfBirth = reader.IsDBNull(ordDob) ? DateTime.MinValue : reader.GetDateTime(ordDob),
                    Gender = reader.IsDBNull(ordGender) ? string.Empty : reader.GetString(ordGender),
                    AppliedClassId = reader.IsDBNull(ordAppClass) ? 0 : reader.GetInt32(ordAppClass),
                    AppliedStudentGroupId = reader.IsDBNull(ordAppliedGroupId) ? null : reader.GetInt32(ordAppliedGroupId),
                    AppliedStudentGroupName = reader.IsDBNull(ordAppliedGroupName) ? null : reader.GetString(ordAppliedGroupName),
                    ClassName = reader.IsDBNull(ordClassName) ? string.Empty : reader.GetString(ordClassName),
                    Status = reader.IsDBNull(ordStatus) ? string.Empty : reader.GetString(ordStatus),
                    FatherName = reader.IsDBNull(ordFName) ? string.Empty : reader.GetString(ordFName),
                    FatherOccupation = reader.IsDBNull(ordFOcc) ? string.Empty : reader.GetString(ordFOcc),
                    MotherName = reader.IsDBNull(ordMName) ? string.Empty : reader.GetString(ordMName),
                    MotherOccupation = reader.IsDBNull(ordMOcc) ? string.Empty : reader.GetString(ordMOcc),
                    GuardianName = reader.IsDBNull(ordGName) ? string.Empty : reader.GetString(ordGName),
                    GuardianOccupation = reader.IsDBNull(ordGOcc) ? string.Empty : reader.GetString(ordGOcc),
                    GuardianEmail = reader.IsDBNull(ordGEmail) ? null : reader.GetString(ordGEmail),
                    GuardianMobileNumber = reader.IsDBNull(ordGMobile) ? null : reader.GetString(ordGMobile),
                    GuardianRelationship = reader.IsDBNull(ordGRel) ? null : reader.GetString(ordGRel),
                    GuardianNationalId = reader.IsDBNull(ordGNid) ? null : reader.GetString(ordGNid),
                    GuardianAddress = reader.IsDBNull(ordGAddr) ? null : reader.GetString(ordGAddr),
                    GuardianPhoto = reader.IsDBNull(ordGPhoto) ? null : reader.GetString(ordGPhoto),
                    GuardianRemarks = reader.IsDBNull(ordGRem) ? null : reader.GetString(ordGRem),
                    LinkedGuardianId = reader.IsDBNull(ordGLink) ? null : (int?)reader.GetInt32(ordGLink),
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
