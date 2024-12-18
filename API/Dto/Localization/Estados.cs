namespace API.Dto.Localization
{
    public abstract class LocalizationDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
    }
    public class EstadoDto : LocalizationDto
    {
        public string? PaisNombre { get; set; }
    }

    public class MunicipioDto : LocalizationDto
    {
        public string? PaisNombre { get; set; }
        public string? EstadoNombre { get; set; }
    }

    public class CodigoPostalDto : LocalizationDto
    {
        public string? PaisNombre { get; set; }
        public string? EstadoNombre { get; set; }
        public string? MunicipioNombre { get; set; }
        public string? CodigoPostal { get; set; }
        public IEnumerable<ColoniaDto>? Colonias { get; set; }
    }

    public class ColoniaDto : LocalizationDto
    {
        public int CodigoPostalId { get; set; }
        public string? CodigoPostal { get; set; }
        public string? PaisNombre { get; set; }
        public string? EstadoNombre { get; set; }
        public string? MunicipioNombre { get; set; }
    }

  public record class LocalizationOptionsDto
  {
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
  }
}
