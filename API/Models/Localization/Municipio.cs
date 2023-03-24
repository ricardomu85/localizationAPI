namespace API.Models.Localization;

public class Municipio : Localization
{
    public Estado? Estado { get; set; }
    public int EstadoId { get; set; }

    public virtual ICollection<CodigoPostal>? CodigosPostales { get; set; }
}
