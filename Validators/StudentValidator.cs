using SchoolManagementSystem.Models.DTOs.Student;

namespace SchoolManagementSystem.Validators;

public static class StudentValidator
{
    public static IReadOnlyList<string> Validate(StudentUpsertDto dto)
    {
        var errors = new List<string>();
        if (dto.DateOfBirth > DateTime.Today)
        {
            errors.Add("Date of birth cannot be in the future.");
        }

        if (dto.FatherOrGuardianMobileNo.Length < 8)
        {
            errors.Add("Father/Guardian phone number is too short.");
        }

        return errors;
    }
}
