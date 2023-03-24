namespace API.Models.Localization;

public class Pais : Localization
{
    public virtual ICollection<Estado>? Estados { get; set; }
}
