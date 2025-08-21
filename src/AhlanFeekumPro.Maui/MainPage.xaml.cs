using AhlanFeekumPro.Maui.ViewModels;
using Volo.Abp.DependencyInjection;

namespace AhlanFeekumPro.Maui;

public partial class MainPage : ContentPage, ITransientDependency
{
    public MainPage(
		MainPageViewModel vm)
	{
        BindingContext = vm;
        InitializeComponent();
    }
}