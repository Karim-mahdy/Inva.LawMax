using Inva.LawMax.LawCases.Hearings;
using Inva.LawMax.Permissions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Inva.LawMax.LawCases
{
    public class HearingAppService :
         CrudAppService<
             Hearing,
             HearingDto,
             int,
             PagedAndSortedResultRequestDto,
             CreateUpdateHearingDto>,
         IHearingAppService
    {
        public HearingAppService(IRepository<Hearing, int> repository)
            : base(repository)
        {
            GetPolicyName = LawCasesPermissions.Hearings.Default;
            GetListPolicyName = LawCasesPermissions.Hearings.Default;
            CreatePolicyName = LawCasesPermissions.Hearings.Create;
            UpdatePolicyName = LawCasesPermissions.Hearings.Edit;
            DeletePolicyName = LawCasesPermissions.Hearings.Delete;
        }

        public override async Task<PagedResultDto<HearingDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {

            var hearingEntity = await Repository
                 .GetQueryableAsync()
                 .Result
                 .Include(h => h.Case)
                 .ToListAsync();

            var totalCount = hearingEntity.Count();

            var items = hearingEntity
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToList();

                return new PagedResultDto<HearingDto>(
                totalCount,
                ObjectMapper.Map<List<Hearing>, List<HearingDto>>(items)
            );
        }
        public override async Task<HearingDto> GetAsync(int id)
        {
            var hearingEntity = await Repository
                .GetQueryableAsync()
                .Result
                .Include(h => h.Case)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hearingEntity == null)
            {
                throw new EntityNotFoundException(typeof(Hearing), id);
            }

            return ObjectMapper.Map<Hearing, HearingDto>(hearingEntity);
        }
        

    }
}
