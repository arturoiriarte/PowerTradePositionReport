namespace PositionReport.Application.PowerPositionScheduler
{
    public interface IPowerPositionScheduler
    {
        /// <summary>
        /// Runs the scheduler to periodically fetch and process power position data.
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task RunAsync(CancellationToken cancellationToken);
    }
}