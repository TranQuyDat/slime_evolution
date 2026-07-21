using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Delay 
{
    private CancellationTokenSource _cts ;
    public async void WaitSeconds(float seconds)
    {
        _cts?.Cancel();
        _cts = new();
        try
        {
            await Task.Delay((int)(seconds * 1000), _cts.Token);
        }
        catch (TaskCanceledException)
        {
        }
    }
    public async void WaitMinute(float minutes)
    {
        _cts?.Cancel();
        _cts = new();
        try
        {
            await Task.Delay((int)(minutes * 60 * 1000), _cts.Token);
        }
        catch (TaskCanceledException)
        {
        }
    }

}
