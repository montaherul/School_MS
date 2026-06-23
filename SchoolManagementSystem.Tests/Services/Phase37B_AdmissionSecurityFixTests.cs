using Microsoft.AspNetCore.Http;
using Xunit;
using Moq;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Implementations.Admissions;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Admission;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.Repositories.Interfaces.Auth;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Repositories.Interfaces.Students;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using SchoolManagementSystem.Services.Interfaces.Guardian;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.Entities.Academic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Tests.Services;

public class Phase37B_AdmissionSecurityFixTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new(MockBehavior.Loose);
    private readonly Mock<IAdmissionRepository> _admissionRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IStudentService> _studentServiceMock = new(MockBehavior.Loose);
    private readonly Mock<IEmailService> _emailServiceMock = new(MockBehavior.Loose);
    private readonly Mock<IUserRepository> _userRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IRoleRepository> _roleRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IUserRoleRepository> _userRoleRepoMock = new(MockBehavior.Loose);
    private readonly Mock<ISchoolClassRepository> _classRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IStudentRepository> _studentRepoMock = new(MockBehavior.Loose);
    private readonly Mock<ISectionRepository> _sectionRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IGuardianService> _guardianServiceMock = new(MockBehavior.Loose);
    private readonly Mock<ILogger<AdmissionService>> _loggerMock = new(MockBehavior.Loose);
    private readonly Mock<ISchoolSettingRepository> _settingRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IHttpContextAccessor> _httpMock = new(MockBehavior.Loose);

    private AdmissionService CreateService()
    {
        return new AdmissionService(
            _uowMock.Object, _admissionRepoMock.Object,
            _studentServiceMock.Object, _emailServiceMock.Object,
            _userRepoMock.Object, _roleRepoMock.Object,
            _userRoleRepoMock.Object, _classRepoMock.Object,
            _studentRepoMock.Object, _sectionRepoMock.Object,
            _guardianServiceMock.Object, _settingRepoMock.Object, _loggerMock.Object,
            _httpMock.Object);
    }

    // ─── P6: Status Transition Guards ────────────────────────────

    [Fact(DisplayName = "1. DeleteAsync throws when application is Converted")]
    public async Task DeleteAsync_Throws_WhenConverted()
    {
        _admissionRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<AdmissionApplication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdmissionApplication { Id = 1, Status = AdmissionStatus.Converted });
        var svc = CreateService();
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.DeleteAsync(1, "user"));
        Assert.Contains("cannot delete", ex.Message, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "2. DeleteAsync succeeds when application is Pending")]
    public async Task DeleteAsync_Succeeds_WhenPending()
    {
        _admissionRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AdmissionApplication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdmissionApplication { Id = 1, Status = AdmissionStatus.Pending });
        _uowMock.Setup(u => u.Repository<AdmissionDocument>().Query())
            .Returns(new List<AdmissionDocument>().AsAsyncQueryable());
        var svc = CreateService();
        await svc.DeleteAsync(1, "user");
    }

    [Fact(DisplayName = "3. RejectAsync throws when application is Converted")]
    public async Task RejectAsync_Throws_WhenConverted()
    {
        _admissionRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<AdmissionApplication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdmissionApplication { Id = 1, Status = AdmissionStatus.Converted });
        var svc = CreateService();
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.RejectAsync(1, "user"));
        Assert.Contains("cannot reject", ex.Message, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "4. RejectAsync throws when already Rejected")]
    public async Task RejectAsync_Throws_WhenAlreadyRejected()
    {
        _admissionRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<AdmissionApplication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdmissionApplication { Id = 1, Status = AdmissionStatus.Rejected });
        var svc = CreateService();
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.RejectAsync(1, "user"));
        Assert.Contains("already been rejected", ex.Message, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "5. RejectAsync succeeds when Pending")]
    public async Task RejectAsync_Succeeds_WhenPending()
    {
        _admissionRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<AdmissionApplication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdmissionApplication { Id = 1, Status = AdmissionStatus.Pending });
        var svc = CreateService();
        await svc.RejectAsync(1, "user");
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact(DisplayName = "6. UpdateAsync throws when Converted")]
    public async Task UpdateAsync_Throws_WhenConverted()
    {
        _admissionRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<AdmissionApplication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdmissionApplication { Id = 1, Status = AdmissionStatus.Converted });
        var svc = CreateService();
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.UpdateAsync(1, new AdmissionCreateDto(), "user", default));
        Assert.Contains("cannot update", ex.Message, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "7. UpdateAsync throws when Rejected")]
    public async Task UpdateAsync_Throws_WhenRejected()
    {
        _admissionRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<AdmissionApplication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdmissionApplication { Id = 1, Status = AdmissionStatus.Rejected });
        var svc = CreateService();
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.UpdateAsync(1, new AdmissionCreateDto(), "user", default));
        Assert.Contains("cannot update", ex.Message, System.StringComparison.OrdinalIgnoreCase);
    }

    // ─── P5: Review Audit Trail ──────────────────────────────────

    [Fact(DisplayName = "8. ReviewedByUserId set on RejectAsync when userId is int")]
    public async Task RejectAsync_SetsReviewedByUserId()
    {
        var app = new AdmissionApplication { Id = 1, Status = AdmissionStatus.Pending };
        _admissionRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AdmissionApplication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(app);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var svc = CreateService();
        await svc.RejectAsync(1, "42");
        Assert.Equal(42, app.ReviewedByUserId);
    }

    // ─── P4: Fee Enforcement ─────────────────────────────────────

    private void SetupApproveConvertBase(bool feePaid, AdmissionStatus status)
    {
        _admissionRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AdmissionApplication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdmissionApplication { Id = 1, Status = status, AdmissionFeePaid = feePaid });
        _admissionRepoMock.Setup(r => r.Query())
            .Returns(new List<AdmissionApplication> { new() { Id = 1, Status = status, AdmissionFeePaid = feePaid, ApplicationNo = "APP-001" } }.AsAsyncQueryable());
        _roleRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Role { Id = 1, Name = "Student" });
    }

    [Fact(DisplayName = "9. ApproveAndConvertAsync throws when fee unpaid")]
    public async Task ApproveAndConvertAsync_Throws_WhenFeeUnpaid()
    {
        SetupApproveConvertBase(false, AdmissionStatus.Pending);
        var svc = CreateService();
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.ApproveAndConvertAsync(1, 1, "user", default));
        Assert.Contains("fee", ex.Message, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "10. ApproveAndConvertAsync throws when Rejected status")]
    public async Task ApproveAndConvertAsync_Throws_WhenRejected()
    {
        SetupApproveConvertBase(true, AdmissionStatus.Rejected);
        var svc = CreateService();
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.ApproveAndConvertAsync(1, 1, "user", default));
        Assert.Contains("cannot convert", ex.Message, System.StringComparison.OrdinalIgnoreCase);
    }

    // ─── P1: IDOR — GetByIdAsync filters IsDeleted ──────────────

    [Fact(DisplayName = "11. DeleteAsync throws when application not found")]
    public async Task DeleteAsync_Throws_WhenNotFound()
    {
        _admissionRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AdmissionApplication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AdmissionApplication?)null);
        var svc = CreateService();
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.DeleteAsync(999, "user"));
        Assert.Contains("not found", ex.Message, System.StringComparison.OrdinalIgnoreCase);
    }

    // ─── P1: GetByIdAsync filters IsDeleted ─────────────────────

    [Fact(DisplayName = "12. GetByIdAsync returns null for deleted record")]
    public async Task GetByIdAsync_ReturnsNull_WhenDeleted()
    {
        _admissionRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<AdmissionApplication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AdmissionApplication?)null);
        var svc = CreateService();
        var result = await svc.GetByIdAsync(999);
        Assert.Null(result);
    }
}
