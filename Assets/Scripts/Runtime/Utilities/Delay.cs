using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public class Delay 
{
    private CancellationTokenSource _cts;

    public UniTask WaitSeconds(float seconds)
    {
        return Wait(TimeSpan.FromSeconds(Math.Max(0f, seconds)));
    }

    public UniTask WaitMinute(float minutes)
    {
        return Wait(TimeSpan.FromMinutes(Math.Max(0f, minutes)));
    }

    public void Cancel()
    {
        _cts?.Cancel();
    }

    private async UniTask Wait(TimeSpan duration)
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();

        try
        {
            await UniTask.Delay(
                duration,
                DelayType.DeltaTime,
                PlayerLoopTiming.Update,
                _cts.Token);
        }
        catch (OperationCanceledException)
        {
        }
    }
}
