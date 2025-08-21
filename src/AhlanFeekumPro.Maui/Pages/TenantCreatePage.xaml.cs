using AhlanFeekumPro.Maui.ViewModels;
using Volo.Abp.DependencyInjection;

namespace AhlanFeekumPro.Maui.Pages;

public partial class TenantCreatePage : ContentPage, ITransientDependency
{
    public TenantCreatePage(TenantCreateViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
    }
}
