using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Expressions;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.DomainServices.SchoolManager.Students.Dto;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Models.Shared;

namespace RhyoliteOrbit.DomainServices.SchoolManager.Students;

public class StudentManager: Abp.Domain.Services.DomainService, IStudentManager
{
        private readonly IRepository<Student, Guid> _repositoryStudent;
        private readonly IRepository<PromotionHistory, Guid> _repositoryPromotionHistory;
        private readonly IRepository<SchClass, Guid> _repositorySchClass;
        private readonly IRepository<Religion, Guid> _repositoryReligion;
        private readonly IRepository<Country, Guid> _repositoryCountry;
        private readonly IRepository<AcademicYear, Guid> _repositoryAcademicYear;
        private readonly IRepository<PersonnelStatus, Guid> _repositoryPersonnelStatus;
        private readonly IRepository<SchoolProfile, Guid> _repositorySchoolProfile;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IHashids _hashids;

        public StudentManager(IRepository<Student, Guid> repositoryStudent, IRepository<PersonnelStatus, Guid> repositoryPersonnelStatus, IRepository<SchClass, Guid> repositorySchClass, IRepository<Religion, Guid> repositoryReligion, IRepository<Country, Guid> repositoryCountry, IRepository<PromotionHistory, Guid> repositoryPromotionHistory, IRepository<AcademicYear, Guid> repositoryAcademicYear, IRepository<SchoolProfile, Guid> repositorySchoolProfile, IUnitOfWorkManager unitOfWorkManager, IHashids hashids)
        {
            _repositoryStudent = repositoryStudent;
            _repositoryPersonnelStatus = repositoryPersonnelStatus;
            _repositorySchClass = repositorySchClass;
            _repositoryReligion = repositoryReligion;
            _repositoryCountry = repositoryCountry;
            _repositoryPromotionHistory = repositoryPromotionHistory;
            _repositoryAcademicYear = repositoryAcademicYear;
            _repositorySchoolProfile = repositorySchoolProfile;
            _unitOfWorkManager = unitOfWorkManager;
            _hashids = hashids;
        }

        public async Task<object> Create(Student entity)
        {
            //obtain default employee status...
            var classInfo = await _repositorySchClass.FirstOrDefaultAsync(entity.ClassId);
            var religionInfo = await _repositoryReligion.FirstOrDefaultAsync(entity.ReligionId);
            var countryInfo = await _repositoryCountry.FirstOrDefaultAsync(entity.NationalityId);
            var academicYearInfo = await _repositoryAcademicYear.FirstOrDefaultAsync(entity.AcademicYearId);

            var datta = await _repositoryStudent.FirstOrDefaultAsync(x => x.StudentIdentifier == entity.StudentIdentifier);
            
            if (datta == null)
            {
                 
                if (classInfo != null && !string.IsNullOrEmpty(classInfo.StreamName))
                {
                    entity.ClassName = $"{classInfo.ClassName}-{classInfo.StreamName}";
                }
                else if (classInfo != null && string.IsNullOrEmpty(classInfo.StreamName))
                {
                    entity.ClassName = classInfo.ClassName;
                }

                if (religionInfo != null)
                {
                    entity.ReligionName = religionInfo.Name;
                }

                if (countryInfo != null)
                {
                    entity.NationalityName = countryInfo.Nationality;
                }

                if (academicYearInfo != null)
                {
                    entity.AcademicYearName = academicYearInfo.Name;
                }

                entity.FirstName = entity.FirstName.Trim();

                entity.LastName = entity.LastName.Trim();

                if (!string.IsNullOrEmpty(entity.MiddleName))
                {
                    entity.MiddleName = entity.MiddleName.Trim();

                }

                return await _repositoryStudent.InsertAsync(entity);
            }

             
            return await _repositoryStudent.UpdateAsync(datta);
        }

        public async Task<object> GetAsync(Guid id)
        {
            return await _repositoryStudent.FirstOrDefaultAsync(id);
        }

        public async Task<object> GetAll(int pageNo, int pageSize, string query, Guid classId)
        {
            var predicate = PredicateBuilder.New<Student>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.FirstName.ToLower().Contains(query.ToLower()) ||
                                   x.LastName.ToLower().Contains(query.ToLower()) ||
                                   x.MiddleName.ToLower().Contains(query.ToLower()) ||
                                   x.StudentIdentifier.ToLower().Contains(query.ToLower()) ||
                                   x.ClassName.ToLower().Contains(query.ToLower()));
            }
            
            if (classId != Guid.Empty)
            {
                predicate.And(x => x.ClassId == classId);
            }
            
            var totalCount =
                
                await _repositoryStudent.CountAsync(predicate);
            
            var data = await (await _repositoryStudent.GetAllAsync()).Where(predicate).Select(a=> new {
                a.Id,
                a.FirstName,
                a.LastName,
                a.MiddleName,
                a.ClassName,
                Status = RhyoliteOrbitConsts.GetStudentStatusString((RhyoliteOrbitConsts.StudentStatus)a.Status),
                a.AcademicYearName,
                a.DateOfBirth,
                a.CityOrLocation,
                a.Gender,
                a.HomeAddress,
                a.NationalityName,
                a.EnrollmentType,
                a.StudentIdentifier,
                a.ReligionName,
                a.ReligionId,
                a.NationalityId,
                a.ClassId,
                Pin = _hashids.Decode(a.Pin).Length > 0 ? _hashids.Decode(a.Pin)[0].ToString("D4") : null,
                a.EnrollmentDate
            }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

            var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

            var paginatedResult = new
            {
                pageNo,
                pageSize,
                totalCount,
                totalPages,
                data,
                lowerBound = pageSize * (pageNo - 1) + 1,
                upperBound = totalPages == pageNo ? totalCount : pageNo * pageSize,
            };

            return paginatedResult;
        }

        public async Task Promote(StudentPromotion studentPromotion)
        {
            var schoolProfile = await (await _repositorySchoolProfile.GetAllAsync()).FirstOrDefaultAsync();

            foreach (var studentId in studentPromotion.StudentIds)
            {
                
                var student = await _repositoryStudent.FirstOrDefaultAsync(studentId);
                await _repositoryStudent.UpdateAsync(student);

                var promotedFromClassInfo = await _repositorySchClass.FirstOrDefaultAsync(x => x.Id == studentPromotion.CurrentClassId);
                var promotedToClassInfo = await _repositorySchClass.FirstOrDefaultAsync(x => x.Id == studentPromotion.DestinationClassId);

                var history = new PromotionHistory
                {
                    DatePromoted = studentPromotion.DatePromoted,
                    PromotedFrom = studentPromotion.CurrentClassId,
                    PromotedTo = studentPromotion.DestinationClassId,
                    StudentId = student.Id,
                    AcademicYearId = schoolProfile.CurrentAcademicYearId,
                    AcademicYearName = schoolProfile.CurrentAcademicYearName,
                    StudentIdentifier = student.StudentIdentifier,
                    StudentName = string.IsNullOrEmpty(student.MiddleName) ? $"{student.FirstName} {student.LastName}" : $"{student.FirstName} {student.MiddleName} {student.LastName}"

                };


                if (promotedFromClassInfo != null && !string.IsNullOrEmpty(promotedFromClassInfo.StreamName))
                {
                    history.CurrentClassName = $"{promotedFromClassInfo.ClassName}-{promotedFromClassInfo.StreamName}";
                }
                else if (promotedFromClassInfo != null && string.IsNullOrEmpty(promotedFromClassInfo.StreamName))
                {
                    history.CurrentClassName = promotedFromClassInfo.ClassName;
                }


                if (promotedToClassInfo != null && !string.IsNullOrEmpty(promotedToClassInfo.StreamName))
                {
                    history.DestinationClassName = $"{promotedToClassInfo.ClassName}-{promotedToClassInfo.StreamName}";
                }
                else if (promotedToClassInfo != null && string.IsNullOrEmpty(promotedToClassInfo.StreamName))
                {
                    history.DestinationClassName = promotedToClassInfo.ClassName;
                }

                await _repositoryPromotionHistory.InsertAsync(history);
            }
        }


        public async Task PromoteAlumni(AlumniStudentPromotion studentPromotion, DateTime completionDate)
        {

            foreach (var student in studentPromotion.StudentIds)
            {
                var studentInfo = await _repositoryStudent.FirstOrDefaultAsync(student);
                if (studentInfo == null)
                {
                    continue;
                }
                
                studentInfo.Status = (int)RhyoliteOrbitConsts.StudentStatus.Graduated;
                studentInfo.ClassId = Guid.Empty;
                studentInfo.ClassName = "Alumni";
                studentInfo.CompletionDate = completionDate;
                studentInfo.IsAlumni = true;
                
                await _repositoryStudent.UpdateAsync(studentInfo);
                 
            }
        }

        public async Task<object> ListStudentsForPromotion(Guid classId)
        {
            return await _repositoryStudent.GetAllListAsync(a => a.ClassId == classId);
        }

        public async Task<object> ListAllByClass(Guid classId)
        {
           return await _repositoryStudent.GetAllListAsync(a => a.ClassId == classId);
        }

        public async Task<IEnumerable<object>> OpeningBalanceExcelExport(Guid classId)
        {
            var datta = await _repositoryStudent.GetAllListAsync(a => a.ClassId == classId);

            var query = from u1 in datta
                        select new
                        {
                            StudentID = u1.StudentIdentifier,
                            Amount = 0,
                            u1.Gender,
                            StudentName = u1.MiddleName == null ? $"{u1.FirstName} {u1.LastName}" : $"{u1.FirstName} {u1.MiddleName} {u1.LastName}",
                        };

            return query.OrderBy(a => a.StudentName).ToList();
        }

       
        public async Task<IEnumerable<object>> AttitudeExcelExport(Guid classId)
        {
            var datta = await _repositoryStudent.GetAllListAsync(a => a.ClassId == classId);
            var query = from u1 in datta
                        select new
                        {
                            StudentID = u1.StudentIdentifier,
                            Attitude = "",
                            StudentName = u1.MiddleName == null ? $"{u1.FirstName} {u1.LastName}" : $"{u1.FirstName} {u1.MiddleName} {u1.LastName}",
                        };
            return query.OrderBy(a => a.StudentName).ToList();

        }

        public async Task<IEnumerable<object>> ConductsExcelExport(Guid classId)
        {
            var datta = await _repositoryStudent.GetAllListAsync(a => a.ClassId == classId);
            var query = from u1 in datta
                        select new
                        {
                            StudentID = u1.StudentIdentifier,
                            Conduct = "",
                            StudentName = u1.MiddleName == null ? $"{u1.FirstName} {u1.LastName}" : $"{u1.FirstName} {u1.MiddleName} {u1.LastName}",
                        };
            return query.OrderBy(a => a.StudentName).ToList();

        }


        public async Task<IEnumerable<object>> TeacherRemarksExcelExport(Guid classId)
        {
            var datta = await _repositoryStudent.GetAllListAsync(a => a.ClassId == classId);
            var query = from u1 in datta
                        select new
                        {
                            StudentID = u1.StudentIdentifier,
                            Remarks = "",
                            StudentName = u1.MiddleName == null ? $"{u1.FirstName} {u1.LastName}" : $"{u1.FirstName} {u1.MiddleName} {u1.LastName}",
                        };
            return query.OrderBy(a => a.StudentName).ToList();

        }

        public async Task<IEnumerable<object>> ResultsUploadExcelExport(Guid classId)
        {
            var datta = await _repositoryStudent.GetAllListAsync(a => a.ClassId == classId);
            var query = from u1 in datta
                        select new
                        {
                            StudentID = u1.StudentIdentifier,
                            Marks = 0,
                            StudentName = u1.MiddleName == null ? $"{u1.FirstName} {u1.LastName}" : $"{u1.FirstName} {u1.MiddleName} {u1.LastName}",
                        };
            return query.OrderBy(a => a.StudentName).ToList();

        }

        public async Task<IEnumerable<object>> ListStudsByClass(Guid classId)
        {
            var datta = await _repositoryStudent.GetAllListAsync(a => a.ClassId == classId);
            var query = from u1 in datta
                        select new
                        {
                            u1.Id,
                            StudentID = u1.StudentIdentifier,
                            u1.LastName,
                            u1.FirstName,
                            MiddleName = u1.MiddleName ?? "",
                            StudentName = u1.MiddleName == null ? $"{u1.FirstName} {u1.LastName}" : $"{u1.FirstName} {u1.MiddleName} {u1.LastName}",

                        };
            return query.OrderBy(a => a.StudentName).ToList();

        }

        public async Task Delete(Guid id)
        {
            await _repositoryStudent.DeleteAsync(id);
        }


        public async Task<object> Update(Student entity)
        {
            var classInfo = await _repositorySchClass.FirstOrDefaultAsync(entity.ClassId);
            var religionInfo = await _repositoryReligion.FirstOrDefaultAsync(entity.ReligionId);
            var countryInfo = await _repositoryCountry.FirstOrDefaultAsync(entity.NationalityId);

            if (classInfo != null && !string.IsNullOrEmpty(classInfo.StreamName))
            {
                entity.ClassName = $"{classInfo.ClassName}-{classInfo.StreamName}";
            }
            else if (classInfo != null && string.IsNullOrEmpty(classInfo.StreamName))
            {
                entity.ClassName = classInfo.ClassName;
            }

            if (religionInfo != null)
            {
                entity.ReligionName = religionInfo.Name;
            }

            if (countryInfo != null)
            {
                entity.NationalityName = countryInfo.Nationality;

            }

            entity.FirstName = entity.FirstName.Trim();

            entity.LastName = entity.LastName.Trim();


            if (!string.IsNullOrEmpty(entity.MiddleName))
            {
                entity.MiddleName = entity.MiddleName.Trim();

            }

            return await _repositoryStudent.UpdateAsync(entity);
        }

        public async Task<Guid> GetStudentId(string studentIdentifier)
        {
            var obj = await _repositoryStudent.FirstOrDefaultAsync(a => !string.IsNullOrEmpty(studentIdentifier) && a.StudentIdentifier == studentIdentifier.Trim());
            if (obj != null)
            {
                return obj.Id;

            }

            return Guid.Empty;
        }

        //
        public async Task<IEnumerable<object>> EnqStudsByClass(Guid classId)
        {
            return await _repositoryStudent.GetAllListAsync(a => a.ClassId == classId);
        }

        public async Task<IEnumerable<object>> EnqStudsByNationality(Guid nationalityId)
        {
            return await _repositoryStudent.GetAllListAsync(a => a.NationalityId == nationalityId);
        }


        public async Task<IEnumerable<object>> EnqStudsByReligion(Guid religionId)
        {
            return await _repositoryStudent.GetAllListAsync(a => a.ReligionId == religionId);
        }
        
        //api ...
        
        
        
        public async Task<object> GetStudentInfo(string studentId, string pin, int tenantId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                string hashedPin = _hashids.Encode(int.Parse(pin));
                
                var studentInfo = await _repositoryStudent.FirstOrDefaultAsync(a => a.StudentIdentifier.Trim() == studentId.Trim() && a.Pin == hashedPin);
                
                if (studentInfo != null)
                {
                    return new 
                    {
                        studentInfo.StudentIdentifier,
                        studentInfo.Id,
                        FullName = studentInfo.MiddleName == null ? $"{studentInfo.FirstName} {studentInfo.LastName}" : $"{studentInfo.FirstName} {studentInfo.MiddleName} {studentInfo.LastName}",
                        studentInfo.ClassName,
                    };
                }

                return null;
            }
        }

        public async Task BiometricEnrollment(int tenantId, int biometricId, string studentId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var studentInfo = await _repositoryStudent.FirstOrDefaultAsync(a => a.StudentIdentifier.Trim() == studentId.Trim());

                if (studentInfo != null)
                {
                    var encryptedBiometricId = _hashids.Encode(biometricId);
                    
                    studentInfo.BiometricIdentifier = encryptedBiometricId;
                    await _repositoryStudent.UpdateAsync(studentInfo);
                }

            }

        }

        public async Task AssignPinsToAllStudents()
        {
            var students = await _repositoryStudent.GetAllListAsync();
            var random = new Random();
            foreach (var student in students)
            {
                int pin = random.Next(1000, 10000); // 4-digit pin
                string hashedPin = _hashids.Encode(pin);
                student.Pin = hashedPin;
                await _repositoryStudent.UpdateAsync(student);
            }
        }
}

