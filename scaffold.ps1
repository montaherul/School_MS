param(
    [string]$EntityName,
    [string]$PluralName,
    [string]$ModuleName,
    [string]$PropsString # comma separated "Name:string:30,SortOrder:int:0"
)

$basePath = "c:\Users\islam\OneDrive\Documents\New project\MvcSqlServerApp"
$namespace = "SchoolManagementSystem"

# Parse properties
$props = @()
foreach ($p in $PropsString.Split(',')) {
    $parts = $p.Split(':')
    $props += @{ Name = $parts[0]; Type = $parts[1]; MaxLength = if ($parts.Length -gt 2) { $parts[2] } else { "0" } }
}

# 1. DTOs
$dtoContent = "using System.ComponentModel.DataAnnotations;`n`nnamespace $namespace.Models.DTOs.$ModuleName;`n`n"
$dtoContent += "public class $($EntityName)ListItemDto`n{`n    public int Id { get; set; }`n"
foreach ($p in $props) {
    if ($p.Type -eq 'string') { $dtoContent += "    public string $($p.Name) { get; set; } = string.Empty;`n" }
    else { $dtoContent += "    public $($p.Type) $($p.Name) { get; set; }`n" }
}
$dtoContent += "}`n`n"

$dtoContent += "public class $($EntityName)UpsertDto`n{`n    public int Id { get; set; }`n"
foreach ($p in $props) {
    $dtoContent += "    [Required]`n"
    if ($p.Type -eq 'string' -and $p.MaxLength -ne "0") { $dtoContent += "    [StringLength($($p.MaxLength))]`n" }
    if ($p.Type -eq 'string') { $dtoContent += "    public string $($p.Name) { get; set; } = string.Empty;`n" }
    else { $dtoContent += "    public $($p.Type) $($p.Name) { get; set; }`n" }
}
$dtoContent += "}`n"
Set-Content -Path "$basePath\Models\DTOs\$ModuleName\$($EntityName)Dtos.cs" -Value $dtoContent

# 2. ViewModel
$vmContent = "using $namespace.Models.DTOs.$ModuleName;`n`nnamespace $namespace.Models.ViewModels.$ModuleName;`n`n"
$vmContent += "public class $($EntityName)ViewModel : $($EntityName)UpsertDto`n{`n    public bool IsEditMode => Id > 0;`n}`n"
Set-Content -Path "$basePath\Models\ViewModels\$ModuleName\$($EntityName)ViewModel.cs" -Value $vmContent

# 3. Service Interface
$iSvcContent = "using $namespace.Models.DTOs.$ModuleName;`nusing $namespace.Models.DTOs.Common;`n`nnamespace $namespace.Services.Interfaces.$ModuleName;`n`n"
$iSvcContent += "public interface I$($EntityName)Service`n{`n"
$iSvcContent += "    Task<PagedResult<$($EntityName)ListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);`n"
$iSvcContent += "    Task<$($EntityName)UpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);`n"
$iSvcContent += "    Task<int> CreateAsync($($EntityName)UpsertDto dto, string createdBy, CancellationToken cancellationToken = default);`n"
$iSvcContent += "    Task UpdateAsync($($EntityName)UpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);`n"
$iSvcContent += "    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);`n"
$iSvcContent += "}`n"
Set-Content -Path "$basePath\Services\Interfaces\$ModuleName\I$($EntityName)Service.cs" -Value $iSvcContent

# 4. Service Implementation
$svcContent = "using Microsoft.EntityFrameworkCore;`nusing $namespace.Data;`nusing $namespace.Models.DTOs.$ModuleName;`nusing $namespace.Models.DTOs.Common;`nusing $namespace.Models.Entities.$ModuleName;`nusing $namespace.Services.Interfaces.$ModuleName;`n`n"
$svcContent += "namespace $namespace.Services.Implementations.$ModuleName;`n`n"
$svcContent += "public class $($EntityName)Service : I$($EntityName)Service`n{`n"
$svcContent += "    private readonly SchoolDbContext _db;`n`n"
$svcContent += "    public $($EntityName)Service(SchoolDbContext db) { _db = db; }`n`n"

$svcContent += "    public async Task<PagedResult<$($EntityName)ListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)`n    {`n"
$svcContent += "        page = Math.Max(page, 1); pageSize = Math.Clamp(pageSize, 5, 100); var term = search?.Trim();`n"
$svcContent += "        var query = _db.$PluralName.Where(x => !x.IsDeleted);"
if ($props[0].Type -eq 'string') {
    $svcContent += " query = query.Where(x => term == null || x.$($props[0].Name).Contains(term));`n"
} else {
    $svcContent += "`n"
}
$svcContent += "        var total = await query.CountAsync(cancellationToken);`n"
$svcContent += "        var items = await query.OrderByDescending(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).Select(x => new $($EntityName)ListItemDto {`n"
$svcContent += "            Id = x.Id,"
foreach ($p in $props) { $svcContent += "$($p.Name) = x.$($p.Name)," }
$svcContent += "        }).ToListAsync(cancellationToken);`n"
$svcContent += "        return new PagedResult<$($EntityName)ListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };`n    }`n`n"

$svcContent += "    public async Task<$($EntityName)UpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)`n    {`n"
$svcContent += "        var entity = await _db.$($PluralName).FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);`n"
$svcContent += "        if (entity is null) return null;`n"
$svcContent += "        return new $($EntityName)UpsertDto { Id = entity.Id,"
foreach ($p in $props) { $svcContent += "$($p.Name) = entity.$($p.Name)," }
$svcContent += "        };`n    }`n`n"

$svcContent += "    public async Task<int> CreateAsync($($EntityName)UpsertDto dto, string createdBy, CancellationToken cancellationToken = default)`n    {`n"
$svcContent += "        var entity = new $($EntityName) { CreatedBy = createdBy,"
foreach ($p in $props) { $svcContent += "$($p.Name) = dto.$($p.Name)," }
$svcContent += "        };`n"
$svcContent += "        _db.$($PluralName).Add(entity); await _db.SaveChangesAsync(cancellationToken); return entity.Id;`n    }`n`n"

$svcContent += "    public async Task UpdateAsync($($EntityName)UpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)`n    {`n"
$svcContent += "        var entity = await _db.$($PluralName).FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException(`"$EntityName not found.`");`n"
foreach ($p in $props) { $svcContent += "        entity.$($p.Name) = dto.$($p.Name);`n" }
$svcContent += "        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(cancellationToken);`n    }`n`n"

$svcContent += "    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)`n    {`n"
$svcContent += "        var entity = await _db.$($PluralName).FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException(`"$EntityName not found.`");`n"
$svcContent += "        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(cancellationToken);`n    }`n}`n"
Set-Content -Path "$basePath\Services\Implementations\$ModuleName\$($EntityName)Service.cs" -Value $svcContent

# 5. Controller
$ctrlContent = "using Microsoft.AspNetCore.Authorization;`nusing Microsoft.AspNetCore.Mvc;`nusing $namespace.Models.DTOs.$ModuleName;`nusing $namespace.Models.ViewModels.$ModuleName;`nusing $namespace.Services.Interfaces.$ModuleName;`nusing System.Security.Claims;`n`n"
$ctrlContent += "namespace $namespace.Controllers.$ModuleName;`n`n"
$ctrlContent += "[Authorize]`npublic class $($EntityName)Controller : Controller`n{`n"
$ctrlContent += "    private readonly I$($EntityName)Service _service;`n"
$ctrlContent += "    public $($EntityName)Controller(I$($EntityName)Service service) { _service = service; }`n`n"
$ctrlContent += "    public IActionResult Index() { return View(); }`n`n"
$ctrlContent += "    [HttpGet]`n    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null)`n    {`n"
$ctrlContent += "        var result = await _service.GetPagedAsync(page, size, search);`n"
$ctrlContent += "        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });`n    }`n`n"
$ctrlContent += "    [HttpGet]`n    public async Task<IActionResult> CreateEdit(int? id)`n    {`n"
$ctrlContent += "        if (id.HasValue && id > 0)`n        {`n"
$ctrlContent += "            var dto = await _service.GetForEditAsync(id.Value);`n"
$ctrlContent += "            if (dto == null) return NotFound();`n"
$ctrlContent += "            var vm = new $($EntityName)ViewModel { Id = dto.Id,"
foreach ($p in $props) { $ctrlContent += "$($p.Name) = dto.$($p.Name)," }
$ctrlContent += "            };`n            return View(vm);`n        }`n"
$ctrlContent += "        return View(new $($EntityName)ViewModel());`n    }`n`n"
$ctrlContent += "    [HttpPost]`n    [ValidateAntiForgeryToken]`n    public async Task<IActionResult> CreateEdit($($EntityName)ViewModel vm)`n    {`n"
$ctrlContent += "        if (!ModelState.IsValid) return View(vm);`n"
$ctrlContent += "        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? `"System`";`n"
$ctrlContent += "        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData[`"SuccessMessage`"] = `"$EntityName updated successfully.`"; }`n"
$ctrlContent += "        else { await _service.CreateAsync(vm, userId); TempData[`"SuccessMessage`"] = `"$EntityName created successfully.`"; }`n"
$ctrlContent += "        return RedirectToAction(nameof(Index));`n    }`n`n"
$ctrlContent += "    [HttpGet]`n    public async Task<IActionResult> Delete(int id)`n    {`n"
$ctrlContent += "        try { var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? `"System`"; await _service.DeleteAsync(id, userId); return Json(new { success = true, message = `"$EntityName deleted successfully.`" }); }`n"
$ctrlContent += "        catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }`n    }`n}`n"
Set-Content -Path "$basePath\Controllers\$ModuleName\$($EntityName)Controller.cs" -Value $ctrlContent

# 6. Views
if (!(Test-Path "$basePath\Views\$EntityName")) { New-Item -ItemType Directory -Path "$basePath\Views\$EntityName" | Out-Null }

$indexContent = "@{ ViewData[`"Title`"] = `"$PluralName`"; }`n`n"
$indexContent += "@if (TempData[`"SuccessMessage`"] != null) { <div class=`"alert alert-success alert-dismissible fade show`" role=`"alert`"><i class=`"bi bi-check-circle-fill me-2`"></i>@TempData[`"SuccessMessage`"]<button type=`"button`" class=`"btn-close`" data-bs-dismiss=`"alert`"></button></div> }`n`n"
$indexContent += "<div class=`"d-flex justify-content-between align-items-center mb-3`"><h2 class=`"mb-0`"><i class=`"bi bi-grid me-2`"></i>$PluralName</h2><a href=`"/$EntityName/CreateEdit`" class=`"btn btn-primary`"><i class=`"bi bi-plus-circle me-1`"></i>Add $EntityName</a></div>`n`n"
$indexContent += "<div class=`"card shadow-sm`"><div class=`"card-body p-0`"><div id=`"data-table`"></div></div></div>`n`n"
$indexContent += "<!-- Delete Modal -->`n<div class=`"modal fade`" id=`"deleteModal`" tabindex=`"-1`"><div class=`"modal-dialog modal-dialog-centered`"><div class=`"modal-content`"><div class=`"modal-header border-0 pb-0`"><h5 class=`"modal-title text-danger`"><i class=`"bi bi-exclamation-triangle-fill me-2`"></i>Confirm Delete</h5><button type=`"button`" class=`"btn-close`" data-bs-dismiss=`"modal`"></button></div><div class=`"modal-body`">Are you sure you want to delete <strong id=`"deleteName`"></strong>?</div><div class=`"modal-footer border-0 pt-0`"><button type=`"button`" class=`"btn btn-secondary`" data-bs-dismiss=`"modal`">Cancel</button><button type=`"button`" class=`"btn btn-danger`" id=`"confirmDeleteBtn`"><i class=`"bi bi-trash me-1`"></i>Delete</button></div></div></div></div>`n`n"
$indexContent += "@section Scripts {`n<link href=`"https://unpkg.com/tabulator-tables@6.2.5/dist/css/tabulator_bootstrap5.min.css`" rel=`"stylesheet`">`n<script src=`"https://unpkg.com/tabulator-tables@6.2.5/dist/js/tabulator.min.js`"></script>`n"
$indexContent += "<script>`nvar pendingDeleteId = null; var deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));`n"
$indexContent += "var columns = [`n{ title: `"id`", field: `"id`", visible: false },`n"
foreach ($p in $props) {
    $indexContent += "{ title: `"$($p.Name)`", field: `"$($p.Name.Substring(0,1).ToLower() + $p.Name.Substring(1))`", minWidth: 150 },`n"
}
$indexContent += "{ title: `"Actions`", field: `"id`", hozAlign: `"center`", headerSort: false, width: 120, formatter: function() { return `<button class=`"btn btn-warning btn-action edit-btn me-1`"><i class=`"bi bi-pencil-fill`"></i></button><button class=`"btn btn-danger btn-action delete-btn`"><i class=`"bi bi-trash-fill`"></i></button>`; }, cellClick: function(e, cell) { var data = cell.getRow().getData(); if (e.target.closest(`.edit-btn`)) window.location.href = `/$EntityName/CreateEdit/` + data.id; if (e.target.closest(`.delete-btn`)) { pendingDeleteId = data.id; document.getElementById('deleteName').textContent = 'Item ' + data.id; deleteModal.show(); } } }`n];`n"
$indexContent += "var table = new Tabulator(`"#data-table`", { height: `"60vh`", layout: `"fitColumns`", pagination: true, paginationMode: `"remote`", filterMode: `"remote`", paginationSize: 10, ajaxURL: '@Url.Action(`"GetList`", `"$EntityName`")', ajaxResponse: function(url, params, response) { return { data: response.data, last_page: response.last_page }; }, columns: columns, placeholder: `"<div class='p-4 text-center text-muted'>No records found</div>`" });`n"
$indexContent += "document.getElementById('confirmDeleteBtn').addEventListener('click', function() { if (!pendingDeleteId) return; fetch(`/$EntityName/Delete/` + pendingDeleteId).then(res => res.json()).then(data => { deleteModal.hide(); if (data.success) { table.replaceData(); } }); });`n</script>`n}"
Set-Content -Path "$basePath\Views\$EntityName\Index.cshtml" -Value $indexContent

$createContent = "@model $namespace.Models.ViewModels.$ModuleName.$($EntityName)ViewModel`n@{ ViewData[`"Title`"] = Model.IsEditMode ? `"Edit $EntityName`" : `"Add $EntityName`"; }`n`n"
$createContent += "<div class=`"row justify-content-center`"><div class=`"col-lg-6`"><div class=`"d-flex align-items-center mb-4`"><a href=`"/$EntityName/Index`" class=`"btn btn-outline-secondary btn-sm me-3`"><i class=`"bi bi-arrow-left`"></i> Back</a><h2 class=`"mb-0`">@(Model.IsEditMode ? `"Edit $EntityName`" : `"Add $EntityName`")</h2></div><div class=`"card shadow-sm`"><div class=`"card-body p-4`">`n"
$createContent += "<form asp-action=`"CreateEdit`" method=`"post`"><input type=`"hidden`" asp-for=`"Id`" />`n"
foreach ($p in $props) {
    $createContent += "<div class=`"mb-3`"><label asp-for=`"$($p.Name)`" class=`"form-label`">$($p.Name) <span class=`"text-danger`">*</span></label><input asp-for=`"$($p.Name)`" class=`"form-control`" /><span asp-validation-for=`"$($p.Name)`" class=`"text-danger small`"></span></div>`n"
}
$createContent += "<div class=`"d-flex gap-2`"><button type=`"submit`" class=`"btn btn-primary px-4`"><i class=`"bi bi-save me-1`"></i> Save</button><a href=`"/$EntityName/Index`" class=`"btn btn-outline-secondary px-4`">Cancel</a></div></form></div></div></div></div>`n"
$createContent += "@section Scripts { @{await Html.RenderPartialAsync(`"_ValidationScriptsPartial`");} }"
Set-Content -Path "$basePath\Views\$EntityName\CreateEdit.cshtml" -Value $createContent

Write-Host "Scaffolded $EntityName successfully!"
