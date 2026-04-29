using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.ViewModels.Shared;

namespace SchoolManagementSystem.Controllers.Common;

[Authorize]
public abstract class GenericCrudController<TEntity> : Controller where TEntity : BaseEntity, new()
{
    private readonly SchoolDbContext _db;
    private readonly string _moduleName;

    protected GenericCrudController(SchoolDbContext db, string moduleName)
    {
        _db = db;
        _moduleName = moduleName;
    }

    protected string ControllerName => GetType().Name.Replace("Controller", string.Empty);

    public virtual async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var query = _db.Set<TEntity>().AsNoTracking().Where(x => !x.IsDeleted);
        var searchableProperties = EditableProperties().Where(p => p.PropertyType == typeof(string)).ToList();
        if (!string.IsNullOrWhiteSpace(search) && searchableProperties.Count > 0)
        {
            var items = await query.ToListAsync(cancellationToken);
            query = items
                .Where(x => searchableProperties.Any(p => (p.GetValue(x) as string)?.Contains(search, StringComparison.OrdinalIgnoreCase) == true))
                .AsQueryable();
        }

        var total = query.Count();
        var rows = query
            .OrderByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList()
            .Select(ToRow)
            .ToList();

        var model = new CrudListViewModel
        {
            ModuleName = _moduleName,
            ControllerName = ControllerName,
            Columns = EditableProperties().Take(8).Select(p => p.Name).ToList(),
            Rows = rows,
            Page = page,
            PageSize = pageSize,
            TotalItems = total,
            Search = search
        };
        return View(model);
    }

    public virtual IActionResult Create()
    {
        return RedirectToAction(nameof(CreateEdit));
    }

    public virtual async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        return await CreateEdit(id, cancellationToken);
    }

    public virtual async Task<IActionResult> CreateEdit(int? id = null, CancellationToken cancellationToken = default)
    {
        var entity = id is > 0
            ? await _db.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            : new TEntity();
        if (entity is null)
        {
            return NotFound();
        }

        return View(ToForm(entity));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Save(IFormCollection form, CancellationToken cancellationToken = default)
    {
        var id = int.TryParse(form["Id"], out var parsedId) ? parsedId : 0;
        var entity = id > 0
            ? await _db.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            : new TEntity();
        if (entity is null)
        {
            return NotFound();
        }

        foreach (var property in EditableProperties())
        {
            if (!form.TryGetValue(property.Name, out var raw))
            {
                continue;
            }

            property.SetValue(entity, ConvertValue(raw.ToString(), property.PropertyType));
        }

        if (id == 0)
        {
            entity.CreatedBy = User.Identity?.Name ?? "system";
            _db.Set<TEntity>().Add(entity);
        }
        else
        {
            entity.UpdatedBy = User.Identity?.Name ?? "system";
            entity.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);
        TempData["SuccessMessage"] = $"{_moduleName} saved successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual Task<IActionResult> CreateEdit(IFormCollection form, CancellationToken cancellationToken = default)
    {
        return Save(form, cancellationToken);
    }

    public virtual async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        return entity is null ? NotFound() : View(ToDetails(entity));
    }

    public virtual async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        return entity is null ? NotFound() : View(ToDetails(entity));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.IsDeleted = true;
        entity.UpdatedBy = User.Identity?.Name ?? "system";
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        TempData["SuccessMessage"] = $"{_moduleName} deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    private CrudFormViewModel ToForm(TEntity entity)
    {
        return new CrudFormViewModel
        {
            ModuleName = _moduleName,
            ControllerName = ControllerName,
            Id = entity.Id,
            Fields = EditableProperties().Select(p => ToField(p, entity)).ToList()
        };
    }

    private CrudDetailsViewModel ToDetails(TEntity entity)
    {
        return new CrudDetailsViewModel
        {
            ModuleName = _moduleName,
            ControllerName = ControllerName,
            Id = entity.Id,
            Fields = EditableProperties().Select(p => ToField(p, entity)).ToList()
        };
    }

    private Dictionary<string, string?> ToRow(TEntity entity)
    {
        var row = new Dictionary<string, string?> { ["Id"] = entity.Id.ToString(CultureInfo.InvariantCulture) };
        foreach (var property in EditableProperties().Take(8))
        {
            row[property.Name] = FormatValue(property.GetValue(entity));
        }

        return row;
    }

    private CrudFieldViewModel ToField(PropertyInfo property, TEntity entity)
    {
        var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        return new CrudFieldViewModel
        {
            Name = property.Name,
            Label = property.GetCustomAttribute<DisplayAttribute>()?.Name ?? SplitWords(property.Name),
            Value = FormatValue(property.GetValue(entity)),
            InputType = InputType(propertyType),
            IsRequired = property.GetCustomAttribute<RequiredAttribute>() is not null || (!IsNullable(property) && propertyType != typeof(bool)),
            Options = propertyType.IsEnum ? Enum.GetNames(propertyType) : []
        };
    }

    private static IReadOnlyList<PropertyInfo> EditableProperties()
    {
        var excluded = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            nameof(BaseEntity.Id), nameof(BaseEntity.CreatedAt), nameof(BaseEntity.CreatedBy),
            nameof(BaseEntity.UpdatedAt), nameof(BaseEntity.UpdatedBy), nameof(BaseEntity.IsDeleted)
        };

        return typeof(TEntity).GetProperties()
            .Where(p => p.CanRead && p.CanWrite && !excluded.Contains(p.Name))
            .Where(p => IsSupportedType(Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType))
            .ToList();
    }

    private static bool IsSupportedType(Type type)
    {
        return type == typeof(string) || type == typeof(int) || type == typeof(decimal) || type == typeof(bool) ||
               type == typeof(DateTime) || type == typeof(DateOnly) || type == typeof(TimeOnly) || type.IsEnum;
    }

    private static object? ConvertValue(string value, Type targetType)
    {
        var nullableType = Nullable.GetUnderlyingType(targetType);
        var type = nullableType ?? targetType;
        if (string.IsNullOrWhiteSpace(value) && nullableType is not null)
        {
            return null;
        }

        if (type == typeof(string)) return value?.Trim() ?? string.Empty;
        if (type == typeof(int)) return int.TryParse(value, out var intValue) ? intValue : 0;
        if (type == typeof(decimal)) return decimal.TryParse(value, out var decimalValue) ? decimalValue : 0;
        if (type == typeof(bool)) return value is "true" or "on" or "1";
        if (type == typeof(DateTime)) return DateTime.TryParse(value, out var dateTimeValue) ? dateTimeValue : DateTime.UtcNow;
        if (type == typeof(DateOnly)) return DateOnly.TryParse(value, out var dateOnlyValue) ? dateOnlyValue : DateOnly.FromDateTime(DateTime.UtcNow);
        if (type == typeof(TimeOnly)) return TimeOnly.TryParse(value, out var timeOnlyValue) ? timeOnlyValue : TimeOnly.MinValue;
        if (type.IsEnum) return Enum.TryParse(type, value, true, out var enumValue) ? enumValue : Activator.CreateInstance(type);
        return null;
    }

    private static string? FormatValue(object? value)
    {
        return value switch
        {
            null => null,
            DateTime dateTime => dateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            DateOnly dateOnly => dateOnly.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            TimeOnly timeOnly => timeOnly.ToString("HH:mm", CultureInfo.InvariantCulture),
            bool boolean => boolean ? "true" : "false",
            _ => Convert.ToString(value, CultureInfo.InvariantCulture)
        };
    }

    private static string InputType(Type type)
    {
        if (type == typeof(DateTime) || type == typeof(DateOnly)) return "date";
        if (type == typeof(TimeOnly)) return "time";
        if (type == typeof(int) || type == typeof(decimal)) return "number";
        if (type == typeof(bool)) return "checkbox";
        return "text";
    }

    private static bool IsNullable(PropertyInfo property)
    {
        return Nullable.GetUnderlyingType(property.PropertyType) is not null || !property.PropertyType.IsValueType;
    }

    private static string SplitWords(string value)
    {
        return string.Concat(value.Select((ch, i) => i > 0 && char.IsUpper(ch) ? " " + ch : ch.ToString()));
    }
}
