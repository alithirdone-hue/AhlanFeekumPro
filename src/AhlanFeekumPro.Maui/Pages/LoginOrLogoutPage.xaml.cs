using AhlanFeekumPro.Maui.ViewModels;
using Volo.Abp.DependencyInjection;

namespace AhlanFeekumPro.Maui.Pages;

public partial class LoginOrLogoutPage : ContentPage, ITransientDependency
{
	public LoginOrLogoutPage(LoginOrLogoutViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}