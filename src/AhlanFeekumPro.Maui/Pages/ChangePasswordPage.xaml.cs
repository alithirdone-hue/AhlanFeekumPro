using AhlanFeekumPro.Maui.ViewModels;
using Volo.Abp.DependencyInjection;

namespace AhlanFeekumPro.Maui.Pages;

public partial class ChangePasswordPage : ContentPage, ITransientDependency
{
	public ChangePasswordPage(ChangePasswordViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}