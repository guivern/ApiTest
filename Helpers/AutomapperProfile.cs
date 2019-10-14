using System;
using System.Linq;
using ApiTest.Models;
using ApiTest.Models.Dtos;
using AutoMapper;

namespace ApiTest.Helpers
{
    /// <summary>
    /// Automapper es una libreria c# que realiza mapeos entre clases
    /// Decidí utilizar esta libraria para ahorrar lineas de codigo y facilitar los mapeos entre clases
    /// Aquí se configuran los mapeos
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