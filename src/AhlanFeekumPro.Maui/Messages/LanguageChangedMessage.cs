using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AhlanFeekumPro.Maui.Messages;

public class LanguageChangedMessage : ValueChangedMessage<string?>
{
    public LanguageChangedMessage(string? value) : base(value)
    {
    }
}
