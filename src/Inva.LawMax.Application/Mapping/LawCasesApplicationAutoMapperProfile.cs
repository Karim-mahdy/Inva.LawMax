using AutoMapper;
using Inva.LawMax.LawCases.Cases;
using Inva.LawMax.LawCases.Hearings;
using Inva.LawMax.LawCases.Lawyers;
using Inva.LawMax.LawCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inva.LawMax.Mapping
{
    public class LawCasesApplicationAutoMapperProfile : Profile
    {
        public LawCasesApplicationAutoMapperProfile()
        {
            CreateMap<Lawyer, LawyerDto>();
            CreateMap<CreateUpdateLawyerDto, Lawyer>();

            // Mapping for Case and CaseDto
            CreateMap<Case, CaseDto>()
                .ForMember(dest => dest.Lawyers, opt => opt.MapFrom(src => src.CaseLawyers.Select(cl => cl.Lawyer)))
                .ForMember(dest => dest.Hearings, opt => opt.MapFrom(src => src.Hearings))
                .ReverseMap()
                .ForMember(dest => dest.CaseLawyers, opt => opt.MapFrom(src => src.Lawyers.Select(l => new CaseLawyer { LawyerId = l.Id })));

            CreateMap<CreateUpdateCaseDto, Case>();

            CreateMap<LawyerDto, CaseLawyer>()
                .ForMember(dest => dest.LawyerId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Lawyer, opt => opt.Ignore()); // Lawyer entity should already exist


            CreateMap<Hearing, HearingDto>()
            .ForMember(dest => dest.CaseNumber, opt => opt.MapFrom(src => src.Case.Number))
            .ForMember(dest => dest.CaseId, opt => opt.MapFrom(src => src.Case.Id));
            CreateMap<CreateUpdateHearingDto, Hearing>();

        }
    }
}
