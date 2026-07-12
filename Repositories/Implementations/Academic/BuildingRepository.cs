using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Academic;

namespace SchoolManagementSystem.Repositories.Implementations.Academic;

public class BuildingRepository : BaseRepository<Building>, IBuildingRepository
{
    public BuildingRepository(SchoolDbContext db) : base(db) { }
}
