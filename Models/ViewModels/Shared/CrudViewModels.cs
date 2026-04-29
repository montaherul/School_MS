namespace SchoolManagementSystem.Models.ViewModels.Shared;

public class CrudListViewModel
{
    public string ModuleName { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
    public IReadOnlyList<string> Columns { get; set; } = [];
    public IReadOnlyList<Dictionary<string, string?>> Rows { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);
    public string? Search { get; set; }
}

public class CrudFormViewModel
{
    public string ModuleName { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
    public int Id { get; set; }
    public bool IsEditMode => Id > 0;
    public IReadOnlyList<CrudFieldViewModel> Fields { get; set; } = [];
}

public class CrudDetailsViewModel
{
    public string ModuleName { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
    public int Id { get; set; }
    public IReadOnlyList<CrudFieldViewModel> Fields { get; set; } = [];
}

public class CrudFieldViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string InputType { get; set; } = "text";
    public bool IsRequired { get; set; }
    public IReadOnlyList<string> Options { get; set; } = [];
}
