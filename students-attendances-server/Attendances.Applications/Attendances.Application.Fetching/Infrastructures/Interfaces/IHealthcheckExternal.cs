namespace Attendances.Application.Fetching.Infrastructures.Interfaces;

public interface IHealthcheckExternal
{
    Task<bool> IsHealthy();
}