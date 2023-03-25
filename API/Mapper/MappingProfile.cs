using AutoMapper;
using API.Dto.Localization;
using API.Models.Localization;

namespace API.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Estado, EstadoDto>()
                .ForMember(p => p.PaisNombre, x => x.MapFrom(a => a.Pais!.Name))
                .ForMember(p => p.Nombre, x => x.MapFrom(a => a.Name));

            CreateMap<Municipio, MunicipioDto>()
                .ForMember(p => p.PaisNombre, x => x.MapFrom(a => a.Estado!.Pais!.Name))
                .ForMember(p => p.EstadoNombre, x => x.MapFrom(a => a.Estado!.Name))
                .ForMember(p => p.Nombre, x => x.MapFrom(a => a.Name));

            CreateMap<CodigoPostal, CodigoPostalDto>()
                .ForMember(p => p.EstadoNombre, x => x.MapFrom(a => a.Municipio!.Estado!.Name))
                .ForMember(p => p.PaisNombre, x => x.MapFrom(a => a.Municipio!.Estado!.Pais!.Name))
                .ForMember(p => p.MunicipioNombre, x => x.MapFrom(a => a.Municipio!.Name))
                .ForMember(p => p.CodigoPostal, x => x.MapFrom(a => a.Name))
                .ForMember(p => p.Nombre, x => x.MapFrom(a => a.Name));

            CreateMap<Colonia, ColoniaDto>()
                .ForMember(p => p.Nombre, x => x.MapFrom(a => a.Name))
                .ForMember(p => p.PaisNombre, x => x.MapFrom(a => a.CodigoPostal!.Municipio!.Estado!.Pais!.Name))
                .ForMember(p => p.EstadoNombre, x => x.MapFrom(a => a.CodigoPostal!.Municipio!.Estado!.Name))
                .ForMember(p => p.MunicipioNombre, x => x.MapFrom(a => a.CodigoPostal!.Municipio!.Name))
                .ForMember(p => p.CodigoPostal, x => x.MapFrom(a => a.CodigoPostal!.Clave));

        }
    }
}
