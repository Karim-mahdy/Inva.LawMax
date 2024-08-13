using Inva.LawMax.LawCases.Cases;
using Inva.LawMax.Permissions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using Microsoft.EntityFrameworkCore;

namespace Inva.LawMax.LawCases
{

    public class CaseAppService :
        CrudAppService<
            Case,
            CaseDto,
            int,
            PagedAndSortedResultRequestDto,
            CreateUpdateCaseDto>,
        ICaseAppService
    {
        private readonly IRepository<CaseLawyer, int> caseLawyerRepository;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public CaseAppService(IRepository<Case, int> repository,
            IRepository<CaseLawyer, int> caseLawyerRepository,
            IUnitOfWorkManager unitOfWorkManager)
            : base(repository)
        {
            GetPolicyName = LawCasesPermissions.Cases.Default;
            GetListPolicyName = LawCasesPermissions.Cases.Default;
            CreatePolicyName = LawCasesPermissions.Cases.Create;
            UpdatePolicyName = LawCasesPermissions.Cases.Edit;
            DeletePolicyName = LawCasesPermissions.Cases.Delete;
            this.caseLawyerRepository = caseLawyerRepository;
            this.unitOfWorkManager = unitOfWorkManager;
        }

        public override async Task<CaseDto> CreateAsync(CreateUpdateCaseDto input)
        {


            var caseEntity = ObjectMapper.Map<CreateUpdateCaseDto, Case>(input);
            await Repository.InsertAsync(caseEntity);
            await unitOfWorkManager.Current.SaveChangesAsync();
            // Assign lawyers to the case
            foreach (var lawyerId in input.LawyerIds)
            {
                var caseLawyer = new CaseLawyer
                {
                    CaseId = caseEntity.Id,
                    LawyerId = lawyerId
                };
                await caseLawyerRepository.InsertAsync(caseLawyer);
            }

            return ObjectMapper.Map<Case, CaseDto>(caseEntity);

        }

        public override async Task<CaseDto> UpdateAsync(int id, CreateUpdateCaseDto input)
        {
            var caseEntity = await Repository.GetAsync(id);

            // Update the case details
            ObjectMapper.Map(input, caseEntity);

            // Remove existing lawyers
            await caseLawyerRepository.DeleteAsync(cl => cl.CaseId == id);

            // Assign new lawyers to the case
            foreach (var lawyerId in input.LawyerIds)
            {
                var caseLawyer = new CaseLawyer
                {
                    CaseId = caseEntity.Id,
                    LawyerId = lawyerId
                };
                await caseLawyerRepository.InsertAsync(caseLawyer);
            }

            return ObjectMapper.Map<Case, CaseDto>(caseEntity);
        }

        public override async Task<CaseDto> GetAsync(int id)
        {

            // Retrieve the case with related entities 
            var caseEntity = await Repository
                .GetQueryableAsync().Result
                .Include(c => c.Hearings)
                .Include(c => c.CaseLawyers)
                .ThenInclude(l => l.Lawyer)
                .FirstOrDefaultAsync(c => c.Id == id);

            // If the case is not found, throw an exception or handle it accordingly
            if (caseEntity == null)
            {
                throw new EntityNotFoundException(typeof(Case), id);
            }

            var caseDto = ObjectMapper.Map<Case, CaseDto>(caseEntity);

            return caseDto;
        }
        public override async Task<PagedResultDto<CaseDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            // Retrieve the list of cases with related entities, applying pagination and sorting
            var caseEntity = await Repository
                 .GetQueryableAsync().Result
                 .Include(c => c.Hearings)
                 .Include(c => c.CaseLawyers)
                 .ThenInclude(l => l.Lawyer).ToListAsync();
            var totalCount = caseEntity.Count();

            var items = caseEntity
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToList();


            // Map the entities to DTOs and return a paginated result
            return new PagedResultDto<CaseDto>(
                totalCount,
                ObjectMapper.Map<List<Case>, List<CaseDto>>(items)
            );
        }

        public Task<List<CaseListDto>> GetListCasesListAsync()
        {
            return Repository
                .GetQueryableAsync().Result
                .Select(c => new CaseListDto
                {
                    Id = c.Id,
                    Number = c.Number,
                }).ToListAsync();
        }
    }


}
