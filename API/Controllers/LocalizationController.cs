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
  public class LocalizacionController : ControllerBase
  {
    private readonly ApplicationDBContext _context;
    private readonly IMapper _mapper;

    public LocalizacionController(ApplicationDBContext context, IMapper mapper)
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

    [HttpGet("estados/{nombre}")]
    [ProducesResponseType(typeof(IReadOnlyList<EstadoDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetEstados(string nombre)
    {
      var estados = await _context.Estados!.Include(p => p.Pais).Where(x => x.Name == nombre).OrderBy(x => x.Name).ToListAsync();
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


    [HttpGet("estados-options")]
    [ProducesResponseType(typeof(IReadOnlyList<LocalizationOptionsDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetEstadosOptions()
    {
      var estados = await _context
            .Estados!
            .Where(x => x.PaisId == 145)
            .OrderBy(x => x.Name)
            .Select(x => new LocalizationOptionsDto
            {
              Value = x.Name!,
              Label = x.Name!
            })
            .ToListAsync();


      return Ok(estados);
    }

    [HttpGet("municipios-options/{estado}")]
    [ProducesResponseType(typeof(IReadOnlyList<LocalizationOptionsDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<LocalizationOptionsDto>>> GetMunicipiosOptions(string estado)
    {
      if (string.IsNullOrEmpty(estado)) return BadRequest("Estado es requerido");

      var municipios = await _context
            .Municipios!            
            .Where(x => x.Name!.ToLower().Contains(estado.ToLower()))
            .OrderBy(x => x.Name)
            .Select(x => new LocalizationOptionsDto
            {
              Value = x.Name!,
              Label = x.Name!
            })
            .ToListAsync();

      return Ok(municipios);
    }


    [HttpGet("codigopostales-options/{municipio}")]
    [ProducesResponseType(typeof(IReadOnlyList<LocalizationOptionsDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetCodigoPostalesOptions(string municipio)
    {
      if (string.IsNullOrEmpty(municipio)) return BadRequest("Municipio es requerido");
      var cps = await _context
            .CodigosPostales!
            .Where(p => p.Name!.ToLower().Contains(municipio.ToLower()))
            .OrderBy(x => x.Name)
            .Select(p => new LocalizationOptionsDto
            {
              Value = p.Name!,
              Label = p.Name!
            })
            .ToListAsync();

      return Ok(cps);
    }

    [HttpGet("colonias-options/{codigoPostal}")]
    [ProducesResponseType(typeof(IReadOnlyList<LocalizationOptionsDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<LocalizationOptionsDto>>> GetColoniasOptions(string codigoPostal)
    {
      if (string.IsNullOrEmpty(codigoPostal)) return BadRequest("Codigo postal es requerido");
      var colonias = await _context
              .Colonias!
              .Where(p => p.Name!.ToLower().Contains(codigoPostal.ToLower()))
              .OrderBy(x => x.Name)
              .Select(p => new LocalizationOptionsDto
              {
                Value = p.Name!,
                Label = p.Name!
              })
              .ToListAsync();

      return Ok(colonias);
    }

    [HttpGet("codigopostalesporestado/{estadoId}")]
    [ProducesResponseType(typeof(IReadOnlyList<CodigoPostalDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetCodigoPostalesPorEstado(int estadoId)
    {
      var cps = await _context.CodigosPostales!.Include(p => p.Municipio).ThenInclude(p => p!.Estado).ThenInclude(p => p!.Pais).Where(p => p.Municipio!.EstadoId == estadoId).ToListAsync();
      var dtos = _mapper.Map<IReadOnlyList<CodigoPostalDto>>(cps);

      return Ok(dtos);
    }

    [HttpGet("codigopostales/{municipioId}")]
    [ProducesResponseType(typeof(IReadOnlyList<CodigoPostalDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetCodigoPostales(int municipioId)
    {
      var cps = await _context.CodigosPostales!.Include(p => p.Municipio).ThenInclude(p => p!.Estado).ThenInclude(p => p!.Pais).Where(p => p.MunicipioId == municipioId).ToListAsync();
      var dtos = _mapper.Map<IReadOnlyList<CodigoPostalDto>>(cps);

      return Ok(dtos);
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

    [HttpGet("informacionporcoloniaid/{coloniaId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetInformacionPorColoniaId(int coloniaId)
    {
      // Validate the coloniaId parameter
      if (coloniaId <= 0)
      {
        ModelState.AddModelError(nameof(coloniaId), "The colony ID must be greater than zero.");
        return BadRequest(ModelState);
      }

      var result = await (from estado in _context.Estados
                          join municipio in _context.Municipios! on estado.Id equals municipio.EstadoId
                          join codigoPostal in _context.CodigosPostales! on municipio.Id equals codigoPostal.MunicipioId
                          join colonia in _context.Colonias! on codigoPostal.Id equals colonia.CodigoPostalId
                          where colonia.Id == coloniaId
                          select new
                          {
                            PaisId = estado.PaisId,
                            EstadoId = estado.Id,
                            EstadoNombre = estado.Name,
                            MunicipioId = municipio.Id,
                            MunicipioNombre = municipio.Name,
                            CodigoPostalId = codigoPostal.Id,
                            CodigoPostalNombre = codigoPostal.Name,
                            ColoniaId = colonia.Id,
                            ColoniaNombre = colonia.Name
                          })
                     .Distinct()
                     .OrderBy(x => x.EstadoNombre) // ordenar por el nombre del estado
                     .FirstOrDefaultAsync();
      if (result is null)
      {
        return NotFound();
      }

      var estados = await _context.Estados!.Where(p => p.PaisId == result.PaisId).Select(e => new { id = e.Id, nombre = e.Name }).ToListAsync();
      var municipios = await _context.Municipios!.Where(p => p.EstadoId == result.EstadoId).Select(m => new { id = m.Id, nombre = m.Name }).ToListAsync();
      var codigoPostales = await _context.CodigosPostales!.Where(p => p.MunicipioId == result.MunicipioId).Select(c => new { id = c.Id, nombre = c.Name }).ToListAsync();
      var colonias = await _context.Colonias!.Where(p => p.CodigoPostalId == result.CodigoPostalId).Select(c => new { id = c.Id, nombre = c.Name }).ToListAsync();


      return Ok(new { estados, municipios, codigoPostales, colonias });
    }

  }
}
