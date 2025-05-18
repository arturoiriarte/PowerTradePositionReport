namespace PositionReport.Application.PowerPositionRunner
{
    public interface IPowerPositionRunnerWithRetry
    {
        /// <summary>
        /// Runs the process once with retry logic.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        Task RunOnceWithRetryAsync(CancellationToken cancellationToken);
    }
}