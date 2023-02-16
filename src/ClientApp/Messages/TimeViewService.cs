namespace ChatApp.Messages;

public sealed class TimeViewService : IDisposable
{
    Task task = default!;
    PeriodicTimer? timer;
    CancellationTokenSource? cts;
    DateTime dateTime;

    public TimeViewService() 
    {
        cts = new CancellationTokenSource();
        task = Task.Run(async () => await Do(cts.Token), cts.Token);
    }

    public event EventHandler Tick = default!;

    async Task Do(CancellationToken token)
    {
        timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        
        try
        {
            while(await timer.WaitForNextTickAsync(token))
            {
                dateTime = DateTime.UtcNow;
                Tick?.Invoke(this, EventArgs.Empty);
            }
        }
        catch(TaskCanceledException)
        {

        }
        finally
        {
            timer = null;
        }
    }

    public void Dispose()
    {
        timer?.Dispose();
        timer = null;

        if(cts is not null) 
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }
    }
}