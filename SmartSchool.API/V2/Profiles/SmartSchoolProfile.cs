using AutoMapper;
using SmartSchool.Helpers;
using SmartSchool.Models;
using SmartSchool.V2.Dtos;

namespace SmartSchool.V2.Profiles
{
    public class SmartSchoolProfile : Profile
    {
        public SmartSchoolProfile()
        {
            CreateMap<Aluno, AlunoDto>()

                .ForMember(
                    destino => destino.Nome,
                    pegar => pegar.MapFrom(src => $"{src.Nome} {src.Sobrenome}")
                )
                .ForMember(
                    destino => destino.Idade,
                    pegar => pegar.MapFrom(src => src.DataNascimento.GetCurrentAge())
                );

            CreateMap<AlunoDto, Aluno>();

            CreateMap<Professor, ProfessorDto>()
                .ForMember(

                    destino => destino.Nome,
                    pegar => pegar.MapFrom(src => $"{src.Nome} {src.Sobrenome}")
                );

            CreateMap<ProfessorDto, Professor>();

            CreateMap<Aluno, AlunoRegistrarDto>().ReverseMap();

            CreateMap<Professor, ProfessorDto>().ReverseMap();
        }
    }
}
