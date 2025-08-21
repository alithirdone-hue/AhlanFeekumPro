using CommunityToolkit.Mvvm.Input;
using Volo.Abp.DependencyInjection;

namespace AhlanFeekumPro.Maui.ViewModels;

public partial class MainPageViewModel : AhlanFeekumProViewModelBase, ITransientDependency
{
    public MainPageViewModel()
    {
    }

    [RelayCommand]
    async Task SeeAllUsers()
    {
        await Shell.Current.GoToAsync("///users");
    }
}
