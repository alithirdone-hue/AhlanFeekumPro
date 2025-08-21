using AhlanFeekumPro.Maui.ViewModels;
using Volo.Abp.DependencyInjection;

namespace AhlanFeekumPro.Maui.Pages;

public partial class TenantsPage : ContentPage, ITransientDependency
{
	public TenantsPage(TenantsPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
