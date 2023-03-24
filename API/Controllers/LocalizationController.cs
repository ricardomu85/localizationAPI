using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using API.Data;
using API.Dto.Localization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalizationController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;

        public LocalizationController(ApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("estados")]
        [ProducesResponseType(typeof(IReadOnlyList<EstadoDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetEstados()
        {
            var estados = await _context.Estados!.Include(p => p.Pais).Where(x => x.PaisId == 145).OrderBy(x => x.Name).ToListAsync();
            var estadosDTO = _mapper.Map<IReadOnlyList<EstadoDto>>(estados);

            return Ok(estadosDTO);
        }

        [HttpGet("municipios/{estadoId}")]
        [ProducesResponseType(typeof(IReadOnlyList<MunicipioDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IReadOnlyList<MunicipioDto>>> GetMunicipios(int estadoId)
        {
            var municipios = await _context.Municipios!.Include(p => p.Estado).ThenInclude(p => p!.Pais!).OrderBy(x => x.Name).Where(x => x.EstadoId == estadoId).ToListAsync();
            var municipiosDTO = _mapper.Map<IReadOnlyList<MunicipioDto>>(municipios);

            return Ok(municipiosDTO);
        }

        [HttpGet("codigopostales/{municipioId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetCodigoPostales(int municipioId)
        {
            var cps = await _context.CodigosPostales!.Include(p => p.Municipio).ThenInclude(p => p!.Estado).ThenInclude(p => p!.Pais).Where(p => p.MunicipioId == municipioId).ToListAsync();
            var dtos = _mapper.Map<IReadOnlyList<CodigoPostalDto>>(cps);
            var tmp = dtos.Select(x => new
            {
                x.Id,
                x.Nombre,
                x.PaisNombre,
                x.EstadoNombre,
                x.MunicipioNombre
            });
            return Ok(tmp);
        }

        [HttpGet("codigopostal/{codigoPostal}")]
        [ProducesResponseType(typeof(CodigoPostalDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CodigoPostalDto>> GetCodigoPostal(string codigoPostal)
        {
            var cps = await _context.CodigosPostales!.Include(p => p.Colonias).Include(p => p.Municipio).ThenInclude(p => p!.Estado).ThenInclude(p => p!.Pais).FirstOrDefaultAsync(p => p.Name == codigoPostal);
            var dtos = _mapper.Map<CodigoPostalDto>(cps);
            return Ok(dtos);
        }

        [HttpGet("colonias/{codigoPostalId}")]
        [ProducesResponseType(typeof(IReadOnlyList<ColoniaDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IReadOnlyList<ColoniaDto>>> GetColonias(int codigoPostalId)
        {
            var colonias = await _context.Colonias!.Include(p => p.CodigoPostal).ThenInclude(p => p!.Municipio).ThenInclude(p => p!.Estado).ThenInclude(p => p!.Pais).Where(p => p.CodigoPostalId == codigoPostalId).ToListAsync();
            var dtos = _mapper.Map<IReadOnlyList<ColoniaDto>>(colonias);
            return Ok(dtos);
        }

        [HttpGet("coloniasporcodigo/{codigoPostal}")]
        [ProducesResponseType(typeof(IReadOnlyList<ColoniaDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IReadOnlyList<ColoniaDto>>> GetColoniasPorClave(string codigoPostal)
        {
            var colonias = await _context.Colonias!.Include(p => p.CodigoPostal).ThenInclude(p => p!.Municipio).ThenInclude(p => p!.Estado).ThenInclude(p => p!.Pais).Where(p => p.CodigoPostal!.Name == codigoPostal).ToListAsync();
            var dtos = _mapper.Map<IReadOnlyList<ColoniaDto>>(colonias);
            return Ok(dtos);
        }

    }
}
