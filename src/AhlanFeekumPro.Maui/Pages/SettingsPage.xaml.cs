using AhlanFeekumPro.Maui.ViewModels;
using Volo.Abp.DependencyInjection;

namespace AhlanFeekumPro.Maui.Pages;

public partial class SettingsPage : ContentPage, ITransientDependency
{
	public SettingsPage(SettingsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}