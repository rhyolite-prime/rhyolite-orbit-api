using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.UI;
using RhyoliteOrbit.DomainServices.SchoolManager.AcademicYears.Dto;
using RhyoliteOrbit.Models.SchoolManager;

namespace RhyoliteOrbit.DomainServices.SchoolManager.AcademicYears;

public class AcademicYearManager: Abp.Domain.Services.DomainService, IAcademicYearManager
{
    private readonly IRepository<AcademicYear, Guid> _repositoryAcademicYear;

        public AcademicYearManager(IRepository<AcademicYear, Guid> repositoryAcademicYear)
        {
            _repositoryAcademicYear = repositoryAcademicYear;
        }

        public async Task<object> Create(AcademicYear entity)
        {
            var datta = await _repositoryAcademicYear.FirstOrDefaultAsync(x => x.Name == entity.Name);

            if (datta == null)
            {
                await _repositoryAcademicYear.InsertAsync(entity);

                return new
                {
                    code = 200,
                    message = "successful"
                };
            }

            throw new UserFriendlyException(400, "Duplicate records are not allowed.");
        }

        public async Task<object> CreateTerm(TermDto input)
        {
            var academicYearData = await _repositoryAcademicYear.FirstOrDefaultAsync(input.AcademicYearId);
            var termList = academicYearData.Terms;
            var termInfo = termList.FirstOrDefault(x => x.Id == input.Id);
            if (termInfo == null)
            {
                academicYearData.Terms.Add(new Term
                {
                    Id = Guid.NewGuid(),
                    Name = input.Name,
                    PrecedenceNo = input.PrecedenceNo,
                    NoOfDays = input.NoOfDays,
                    StartDate = input.StartDate,
                    EndDate = input.EndDate

                });

                await _repositoryAcademicYear.UpdateAsync(academicYearData);

                return new
                {
                    code = 200,
                    message = "successful"
                };
            }
            else
            {
                return new
                {
                    code = 400,
                    message = "Duplicate records are not allowed."
                };

            }


        }

        public async Task Update(AcademicYear entity)
        {
            await _repositoryAcademicYear.UpdateAsync(entity);
        }

        public async Task UpdateTerm(TermDto input)
        {
            var academicYearData = await _repositoryAcademicYear.FirstOrDefaultAsync(input.AcademicYearId);
            var termList = academicYearData.Terms;
            var termInfo = termList.FirstOrDefault(x => x.Id == input.Id);
            termList.Remove(termInfo);

            termInfo.Name = input.Name; 
            termInfo.StartDate = input.StartDate;
            termInfo.EndDate = input.EndDate;
            termInfo.NoOfDays = input.NoOfDays;
            termInfo.PrecedenceNo = input.PrecedenceNo;


            termList.Add(termInfo);

            academicYearData.Terms = termList;

           await _repositoryAcademicYear.UpdateAsync(academicYearData);
        }

        public async Task<object> GetAsync(Guid id)
        {
            return await _repositoryAcademicYear.GetAsync(id);
        }

        public async Task<object> ListAll()
        {
            return await _repositoryAcademicYear.GetAllListAsync();
        }
        public async Task Delete(Guid id)
        {
            await _repositoryAcademicYear.DeleteAsync(id);

        }

        
        public async Task DeleteTerm(TermDto input)
        {
            var academicYearData = await _repositoryAcademicYear.FirstOrDefaultAsync(input.AcademicYearId);
            var termList = academicYearData.Terms;
            var termInfo = termList.FirstOrDefault(x => x.Id == input.Id);
            termList.Remove(termInfo);
 
            academicYearData.Terms = termList;

            await _repositoryAcademicYear.UpdateAsync(academicYearData);

        }
}