using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Academic;

namespace SchoolManagementSystem.Repositories.Implementations.Academic;

public class AcademicYearRepository : BaseRepository<AcademicYear>, IAcademicYearRepository 
{ 
    public AcademicYearRepository(SchoolDbContext db) : base(db) { } 
}

public class SchoolClassRepository : BaseRepository<SchoolClass>, ISchoolClassRepository 
{ 
    public SchoolClassRepository(SchoolDbContext db) : base(db) { } 
}

public class SectionRepository : BaseRepository<Section>, ISectionRepository 
{ 
    public SectionRepository(SchoolDbContext db) : base(db) { } 
}

public class SubjectRepository : BaseRepository<Subject>, ISubjectRepository 
{ 
    public SubjectRepository(SchoolDbContext db) : base(db) { } 
}
