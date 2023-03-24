namespace API.Models.Localization;

public class Estado : Localization
{
    public Pais? Pais { get; set; }
    public int PaisId { get; set; }

    public virtual ICollection<Municipio>? Municipios { get; set; }
    public virtual ICollection<CodigoPostal>? CodigosPostales { get; set; }
}
