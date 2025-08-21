using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AhlanFeekumPro.Maui.Messages;
public class LoginMessage : ValueChangedMessage<bool?>
{
    public LoginMessage(bool? value = null) : base(value)
    {
    }
}
