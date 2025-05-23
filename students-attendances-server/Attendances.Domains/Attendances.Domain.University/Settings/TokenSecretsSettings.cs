namespace Attendances.Domain.University.Settings;

public class TokenSecretsSettings
{
    private TokenOptions _secrets = new();
    public required TokenOptions Secrets
    {
        get => _secrets;
        set
        {
            _secrets = value;
            SecretsUpdated?.Invoke(this, _secrets);
        }
    }
    public event EventHandler<TokenOptions>? SecretsUpdated;
    public void OnSecretsUpdated(Action<TokenOptions> action)
    {
        SecretsUpdated += (@object, tokenOptions) => action(tokenOptions);
    }
}