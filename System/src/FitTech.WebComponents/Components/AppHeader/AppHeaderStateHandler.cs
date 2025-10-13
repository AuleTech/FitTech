using Microsoft.AspNetCore.Components;

namespace FitTech.WebComponents.Components.AppHeader;

public class AppHeaderStateHandler
{
    public List<RenderFragment> HeaderButtons { get; } = new();
    public event Action? OnChange;

    public void AddButton(string iconName, Func<Task> onClickAsync, string? tooltip = null)
    {
        HeaderButtons.Add(builder =>
        {
            builder.OpenComponent(0, typeof(AppHeaderButton));
            builder.AddAttribute(1, nameof(AppHeaderButton.IconName), iconName);
            builder.AddAttribute(2, nameof(AppHeaderButton.Tooltip), tooltip);
            builder.AddAttribute(3, nameof(AppHeaderButton.Delegate), onClickAsync);
            builder.CloseElement();
        });

        OnChange?.Invoke();
    }

    public void Clear()
    {
        HeaderButtons.Clear();
        OnChange?.Invoke();
    }
}
