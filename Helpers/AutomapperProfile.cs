using System;
using System.Linq;
using ApiTest.Models;
using ApiTest.Models.Dtos;
using AutoMapper;

namespace ApiTest.Helpers
{
    /// <summary>
    /// Automapper es una libreria c# que realiza mapeo entre clases
    /// Utilice esta libraria para facilitar los mapeos entre los tipos dto y entidad
    /// En esta clase se configuran los mapeos
    /// </summary>
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            // CreateMap<src, dest>()
            CreateMap<PaisDto, Pais>();
            CreateMap<Pais, PaisDetailDto>()
                .ForMember(dest => dest.Capital, opt => opt
                .MapFrom(src => src.Ciudades.FirstOrDefault(c => c.EsCapital).Nombre));
            CreateMap<Pais, PaisListDto>()
                .ForMember(dest => dest.Capital, opt => opt
                .MapFrom(src => src.Ciudades.FirstOrDefault(c => c.EsCapital).Nombre));
            CreateMap<CiudadDto, Ciudad>();
            CreateMap<Ciudad, CiudadDetailDto>();
            CreateMap<Ciudad, CiudadListDto>();
        }
    }
}