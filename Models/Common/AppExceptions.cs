namespace SchoolManagementSystem.Models.Common;

// ── Custom Exception Hierarchy ────────────────────────────────────────────────

/// <summary>Base for all domain-level exceptions. Produces a 400-level HTTP response.</summary>
public abstract class SchoolAppException : Exception
{
    public int StatusCode { get; }
    protected SchoolAppException(string message, int statusCode = 400) : base(message)
        => StatusCode = statusCode;
}

/// <summary>Thrown when a requested resource is not found. → 404</summary>
public sealed class NotFoundException : SchoolAppException
{
    public NotFoundException(string entity, object id)
        : base($"{entity} with ID '{id}' was not found.", 404) { }

    public NotFoundException(string message) : base(message, 404) { }
}

/// <summary>Thrown when a user does not have permission. → 403</summary>
public sealed class ForbiddenException : SchoolAppException
{
    public ForbiddenException(string? message = null)
        : base(message ?? "You do not have permission to perform this action.", 403) { }
}

/// <summary>Thrown for business rule violations. → 422</summary>
public sealed class BusinessRuleException : SchoolAppException
{
    public BusinessRuleException(string message) : base(message, 422) { }
}

/// <summary>Thrown when input validation fails. → 400</summary>
public sealed class ValidationException : SchoolAppException
{
    public IReadOnlyList<string> Errors { get; }

    public ValidationException(IEnumerable<string> errors)
        : base("One or more validation errors occurred.", 400)
        => Errors = errors.ToList();

    public ValidationException(string error) : this(new[] { error }) { }
}

/// <summary>Thrown when a duplicate/conflict exists. → 409</summary>
public sealed class ConflictException : SchoolAppException
{
    public ConflictException(string message) : base(message, 409) { }
}
