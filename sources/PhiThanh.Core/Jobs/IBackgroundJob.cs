namespace PhiThanh.Core.Jobs
{
    public interface IBackgroundJob
    {
        public Task RegisterAsync();
        public Task RegisterAsync(string jobId, string cronExp);
        public Task RemoveAsync();
        public Task RemoveAsync(string jobId);
        public void Execute();
    }
}
