using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Services.Interfaces.Base;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;

namespace SchoolManagementSystem.Controllers.Exam;

public class ExamController : GenericCrudController<ExamEntity>
{
    public ExamController(IBaseService<ExamEntity> service) : base(service, "Exam") { }
}

