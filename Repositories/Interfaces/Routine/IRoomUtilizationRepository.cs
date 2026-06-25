using SchoolManagementSystem.Models.DTOs.Routine;

namespace SchoolManagementSystem.Repositories.Interfaces.Routine;

public interface IRoomUtilizationRepository
{
    Task<List<RoomUtilizationDto>> GetRoomUtilizationAsync(int academicYearId);
}
