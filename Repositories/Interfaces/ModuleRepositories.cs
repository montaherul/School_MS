using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Student;

namespace SchoolManagementSystem.Repositories.Interfaces;

public interface IStudentRepository : IGenericRepository<Student> { }
public interface IAdmissionRepository : IGenericRepository<AdmissionApplication> { }
public interface IAcademicYearRepository : IGenericRepository<AcademicYear> { }
public interface ISchoolClassRepository : IGenericRepository<SchoolClass> { }
public interface ISectionRepository : IGenericRepository<Section> { }
public interface ISubjectRepository : IGenericRepository<Subject> { }
public interface IAttendanceRecordRepository : IGenericRepository<AttendanceRecord> { }
public interface IFeeStructureRepository : IGenericRepository<FeeStructure> { }
