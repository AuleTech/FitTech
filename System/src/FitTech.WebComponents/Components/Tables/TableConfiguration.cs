namespace FitTech.WebComponents.Components.Tables;

public class FitTechTableConfiguration<TItem>
{
    public required string[] Headers { get; set; }
    public required TItem[] Items { get; set; }
    public required Func<TItem, string[]> RowSelector { get; set; }
    public Func<TItem, string>? LinkSelector { get; set; }
    public bool HideSearchBar { get; set; }
    public bool HasLink => LinkSelector is not null;
}
