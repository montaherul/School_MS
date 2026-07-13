using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FeeStructureWizardService : IFeeStructureWizardService
{
    private readonly IUnitOfWork _uow;

    public FeeStructureWizardService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<FeeStructureWizardViewModel> GetWizardDataAsync(FeeStructureWizardDto? state = null)
    {
        var vm = new FeeStructureWizardViewModel
        {
            Wizard = state ?? new FeeStructureWizardDto(),
            AcademicYears = await _uow.Repository<AcademicYear>().Query().AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.StartsOn)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToListAsync(),
            SchoolClasses = await _uow.Repository<SchoolClass>().Query().AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.SortOrder)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToListAsync(),
            FeeCategories = await _uow.Repository<FeeCategory>().Query().AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToListAsync()
        };

        if (state != null && state.SchoolClassId > 0)
        {
            vm.Sections = await _uow.Repository<Section>().Query().AsNoTracking()
                .Where(x => x.SchoolClassId == state.SchoolClassId && !x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToListAsync();

            vm.StudentGroups = await _uow.Repository<StudentGroup>().Query().AsNoTracking()
                .Where(x => !x.IsDeleted && x.MinClass <= state.SchoolClassId && x.MaxClass >= state.SchoolClassId)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToListAsync();
        }
        else
        {
            vm.Sections = [];
            vm.StudentGroups = [];
        }

        return vm;
    }

    public async Task<AutoBillingResultDto> SaveWizardAsync(FeeStructureWizardDto wizard, string createdBy)
    {
        var result = new AutoBillingResultDto();
        try
        {
            var structureRepo = _uow.Repository<FeeStructure>();
            var discountRepo = _uow.Repository<FeeDiscount>();
            var fineRuleRepo = _uow.Repository<FineRule>();
            var feeHeadsCreated = 0;
            var discountsCreated = 0;
            var fineRulesCreated = 0;

            foreach (var head in wizard.FeeHeads)
            {
                var entity = new FeeStructure
                {
                    SchoolClassId = wizard.SchoolClassId,
                    FeeCategoryId = head.FeeCategoryId > 0 ? head.FeeCategoryId : null,
                    AcademicYearId = wizard.AcademicYearId > 0 ? wizard.AcademicYearId : null,
                    FeeName = head.FeeName,
                    Amount = head.Amount,
                    IsRecurring = head.IsRecurring,
                    Frequency = (FeeFrequency)head.Frequency,
                    DueDay = head.DueDay,
                    IsActive = wizard.IsActive,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                };
                await structureRepo.AddAsync(entity);
                feeHeadsCreated++;
            }

            foreach (var d in wizard.Discounts)
            {
                var entity = new FeeDiscount
                {
                    Name = d.Name,
                    DiscountType = (FeeDiscountType)d.DiscountType,
                    Value = d.Value,
                    FeeCategoryId = d.FeeCategoryId > 0 ? d.FeeCategoryId : null,
                    IsActive = wizard.IsActive,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                };
                await discountRepo.AddAsync(entity);
                discountsCreated++;
            }

            foreach (var f in wizard.FineRules)
            {
                var entity = new FineRule
                {
                    Name = f.Name,
                    GraceDays = f.GraceDays,
                    FinePerDay = f.FinePerDay,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                };
                await fineRuleRepo.AddAsync(entity);
                fineRulesCreated++;
            }

            await _uow.SaveChangesAsync();
            result.InvoicesGenerated = feeHeadsCreated;
            result.StudentsBilled = discountsCreated + fineRulesCreated;
            result.TotalAmount = wizard.FeeHeads.Sum(x => x.Amount);
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
        }

        return result;
    }
}
