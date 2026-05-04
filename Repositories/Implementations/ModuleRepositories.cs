using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Models.Entities.Teachers;

namespace SchoolManagementSystem.Repositories.Implementations;

public class StudentRepository : GenericRepository<Student>, IStudentRepository { public StudentRepository(SchoolDbContext db) : base(db) { } }
public class AdmissionRepository : GenericRepository<AdmissionApplication>, IAdmissionRepository { public AdmissionRepository(SchoolDbContext db) : base(db) { } }
public class AcademicYearRepository : GenericRepository<AcademicYear>, IAcademicYearRepository { public AcademicYearRepository(SchoolDbContext db) : base(db) { } }
public class SchoolClassRepository : GenericRepository<SchoolClass>, ISchoolClassRepository { public SchoolClassRepository(SchoolDbContext db) : base(db) { } }
public class SectionRepository : GenericRepository<Section>, ISectionRepository { public SectionRepository(SchoolDbContext db) : base(db) { } }
public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository { public SubjectRepository(SchoolDbContext db) : base(db) { } }
public class AttendanceRecordRepository : GenericRepository<AttendanceRecord>, IAttendanceRecordRepository { public AttendanceRecordRepository(SchoolDbContext db) : base(db) { } }
public class FeeStructureRepository : GenericRepository<FeeStructure>, IFeeStructureRepository { public FeeStructureRepository(SchoolDbContext db) : base(db) { } }
