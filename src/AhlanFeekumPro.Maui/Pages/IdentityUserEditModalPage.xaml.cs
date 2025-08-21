using AhlanFeekumPro.Maui.ViewModels;
using Volo.Abp.DependencyInjection;

namespace AhlanFeekumPro.Maui.Pages;

public partial class IdentityUserEditModalPage : ContentPage, ITransientDependency
{
	public IdentityUserEditModalPage(IdentityUserEditViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}