using CommunityToolkit.Mvvm.ComponentModel;

namespace Informatics.Appetite.ViewModels;

/// <summary>
/// BaseViewModel provides common properties for all ViewModels,
/// including loading state (`IsBusy`) and page title (`Title`).
/// </summary>
public partial class BaseViewModel : ObservableObject
{
    /// <summary>
    /// Indicates whether a long-running operation is in progress.
    /// Used to prevent duplicate actions and show loading indicators.
    /// </summary>
    [ObservableProperty]
    private bool isBusy;

    /// <summary>
    /// The title of the page, useful for UI bindings.
    /// </summary>
    [ObservableProperty]
    private string? title;
}