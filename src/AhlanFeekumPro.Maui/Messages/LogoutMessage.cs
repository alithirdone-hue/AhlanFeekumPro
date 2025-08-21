using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AhlanFeekumPro.Maui.Messages;
public class LogoutMessage : ValueChangedMessage<bool?>
{
    public LogoutMessage(bool? value = null) : base(value)
    {
    }
}