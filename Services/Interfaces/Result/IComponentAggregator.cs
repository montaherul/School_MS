using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IComponentAggregator
{
    decimal Aggregate(MarkEntry entry, List<ComponentColumnDto> components);
    decimal AggregateAll(MarkEntry entry);
}
