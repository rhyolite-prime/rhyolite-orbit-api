using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RhyoliteOrbit.Models.SchoolManager;
using RhyoliteOrbit.Models.Shared;
using RhyoliteOrbit.Redis;

namespace RhyoliteOrbit.DomainServices.SchoolManager.Parents;

public class ParentManager: Abp.Domain.Services.DomainService, IParentManager
{
    private readonly IRepository<Parent, Guid> _repositoryParent;
        private readonly IRepository<Relationship, Guid> _repositoryRelationship;
        private readonly IRepository<Student, Guid> _repositoryStudent;
        private readonly IRepository<StudentParent, Guid> _repositoryStudentParent;
        private readonly IRedisCacheManager _redisCacheManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public ParentManager(IRepository<Parent, Guid> repositoryParent, IRepository<Relationship, Guid> repositoryRelationship, IRepository<Student, Guid> repositoryStudent, IRepository<StudentParent, Guid> repositoryStudentParent, IUnitOfWorkManager unitOfWorkManager, IRedisCacheManager redisCacheManager)
        {
            _repositoryParent = repositoryParent;
            _repositoryRelationship = repositoryRelationship;
            _repositoryStudent = repositoryStudent;
            _repositoryStudentParent = repositoryStudentParent;
            _unitOfWorkManager = unitOfWorkManager;
            _redisCacheManager = redisCacheManager;
        }

        public async Task<object> Create(Parent entity)
        {
            var firstGuardianRelationship = await _repositoryRelationship.FirstOrDefaultAsync(x => x.Id == entity.FirstGuardianRelationshipId);
            var SecondGuardianRelationship = await _repositoryRelationship.FirstOrDefaultAsync(x => x.Id == entity.SecondGuardianRelationshipId);

            if (firstGuardianRelationship != null)
            {
                entity.FirstGuardianRelationshipName = firstGuardianRelationship.Name;
            }

            if (SecondGuardianRelationship != null)
            {
                entity.SecondGuardianRelationshipName = SecondGuardianRelationship.Name;
            }

            if (!string.IsNullOrEmpty(entity.FirstGuardianPhoneNo))
            {
                entity.FirstGuardianPhoneNo = Regex.Replace(entity.FirstGuardianPhoneNo, @"\s", "").Trim();
            }

            if (!string.IsNullOrEmpty(entity.SecondGuardianPhoneNo))
            {
                entity.SecondGuardianPhoneNo = Regex.Replace(entity.SecondGuardianPhoneNo, @"\s", "").Trim();

            }
            return await _repositoryParent.InsertAsync(entity);

        }

        public async Task<object> ListParentWards()
        {
            var students = await _repositoryStudent.GetAllListAsync();
            var stuParents = await _repositoryStudentParent.GetAllListAsync();
             
            var query = from u1 in students
                        join u2 in stuParents on u1.Id equals u2.StudentId
                        
                        select new
                        {
                            u1.StudentIdentifier,
                            StudentName = u1.MiddleName == null ? $"{u1.LastName}, {u1.FirstName}" : $"{u1.LastName}, {u1.FirstName} {u1.MiddleName}",
                            Id = u2.ParentId,
                            u2.StudentId,
                            u1.ClassName,
                        };

            return query.ToList();
        }
        public async Task<object> GetParentInfo(Guid id)
        {
            return await _repositoryParent.FirstOrDefaultAsync(a => a.Id == id);
        }



        public async Task<object> ListAll(Guid id)
        {
            return await _repositoryParent.GetAllListAsync(x=>x.Id == id);
        }

        public async Task<object> ListAll(int pageNo, int pageSize, string query)
        {
            
            var predicate = PredicateBuilder.New<Parent>();

            predicate.And(x => !x.IsDeleted);
            
            if (!string.IsNullOrEmpty(query))
            {
                predicate.And(x => x.FirstGuardianName.Contains(query) || x.FirstGuardianPhoneNo.Contains(query) || x.SecondGuardianName.Contains(query) || x.SecondGuardianPhoneNo.Contains(query));

            }
            
            var totalCount =
                
                await _repositoryParent.CountAsync(predicate);
            
            var data = await (await _repositoryParent.GetAllAsync()).Where(predicate).Select(a=> new { a.Id,a.FirstGuardianName, a.FirstGuardianEmail, a.FirstGuardianPhoneNo, a.FirstGuardianRelationshipName, a.SecondGuardianName, a.SecondGuardianEmail, a.SecondGuardianPhoneNo, a.SecondGuardianRelationshipName  }).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

            var totalPages = totalCount > 0 ? Convert.ToInt32(Math.Ceiling(totalCount / (decimal)pageSize)) : 0;

            var paginatedResult = new
            {
                pageNo,
                totalCount,
                totalPages,
                data,
                lowerBound = pageSize * (pageNo - 1) + 1,
                upperBound = totalPages == pageNo ? totalCount : pageNo * pageSize,
            };

            return paginatedResult;
        }

        public async Task Delete(Guid id)
        {
            await _repositoryParent.DeleteAsync(id);
        }

        public async Task<object> ListParents()
        {
            var datta = await _repositoryParent.GetAllListAsync();
            return (from u1 in datta
                    select new
                    {
                        u1.Id,
                        ParentName = u1.FirstGuardianName + "-" + u1.FirstGuardianPhoneNo + ";" + u1.SecondGuardianName + "-" + u1.SecondGuardianPhoneNo

                    }).ToList();
        }

        public async Task<object> Update(Parent entity)
        {
            var firstGuardianRelationship = await _repositoryRelationship.FirstOrDefaultAsync(x => x.Id == entity.FirstGuardianRelationshipId);
            var SecondGuardianRelationship = await _repositoryRelationship.FirstOrDefaultAsync(x => x.Id == entity.SecondGuardianRelationshipId);

            if (firstGuardianRelationship != null)
            {
                entity.FirstGuardianRelationshipName = firstGuardianRelationship.Name;
            }

            if (SecondGuardianRelationship != null)
            {
                entity.SecondGuardianRelationshipName = SecondGuardianRelationship.Name;
            }

            if (!string.IsNullOrEmpty(entity.FirstGuardianPhoneNo))
            {
                entity.FirstGuardianPhoneNo = Regex.Replace(entity.FirstGuardianPhoneNo, @"\s", "").Trim();
            }

            if (!string.IsNullOrEmpty(entity.SecondGuardianPhoneNo))
            {
                entity.SecondGuardianPhoneNo = Regex.Replace(entity.SecondGuardianPhoneNo, @"\s", "").Trim();

            }
            return await _repositoryParent.UpdateAsync(entity);
        }

        // external api
        
        public async Task<object> VerifyParent(string phoneNumber, int tenantId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var parentInfo =
                    await _repositoryParent.FirstOrDefaultAsync(a => a.FirstGuardianPhoneNo.Trim() == phoneNumber.Trim() || a.SecondGuardianPhoneNo.Trim() == phoneNumber.Trim());

                if (parentInfo == null)
                    return new
                    {
                        IsGuardianVerified = false,
                        PhoneNo = phoneNumber,
                        code = 1
                    };

                // 2. Generate OTP
                var otp = GenerateOtp();

                await _redisCacheManager.SetValueAsync(phoneNumber, otp);
                await _redisCacheManager.SetExpireTimeAsync(phoneNumber, TimeSpan.FromMinutes(5));

                //retrieve SMS configuration and settings AND Send OTP via SMS/EMAIL

                //_smsActorProvider.Tell(new SmsModel { });

                return new
                {
                    IsGuardianVerified = true,
                    PhoneNo = phoneNumber,
                    code = 0
                };
                
            }
        }
        
        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(1000, 9999).ToString();
        }
        
}