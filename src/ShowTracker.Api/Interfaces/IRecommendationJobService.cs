namespace ShowTracker.Api.Interfaces;

public interface IRecommendationJobService
{
    Task TriggerManually(CancellationToken cancellationToken);
}
