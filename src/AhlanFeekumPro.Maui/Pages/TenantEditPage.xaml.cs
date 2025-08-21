using AhlanFeekumPro.Maui.ViewModels;
using Volo.Abp.DependencyInjection;

namespace AhlanFeekumPro.Maui.Pages;

public partial class TenantEditPage : ContentPage, ITransientDependency
{
	public TenantEditPage(TenantEditViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}
