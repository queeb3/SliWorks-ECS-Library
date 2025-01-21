namespace SliLib.Core;

using System.Diagnostics;

public class Time
{
    readonly Config config;

    public float DeltaTime { get; private set; }
    public float Elapsed { get; private set; }

    public float TickInterval => config.TickIntervals;
    public float TicksPerSecond => 1 / TickInterval;
    public float NextTick { get; private set; } = 0f;
    public int TicksThisFrame { get; private set; } = 0;
    public bool IsTickFrame { get; private set; }

    public float FrameRate { get; private set; }
    public float FrameRateLimit => config.FrameRateCap;

    private readonly Stopwatch sw = Stopwatch.StartNew();
    public float CurrentTime => (float)sw.Elapsed.TotalSeconds;

    public Time(Config config)
    {
        this.config = config;

        Console.WriteLine("Time Initialized...");
    }


    public void UpdateTime() // must be ran at start of every frame
    {
        DeltaTime = CurrentTime - Elapsed;
        Elapsed += DeltaTime;

        FrameRate = MathF.Round(1 / DeltaTime, 2);

        UpdateTicks();
        LimitFrameRate();
    }

    private void UpdateTicks()
    {
        TicksThisFrame = (int)((Elapsed - NextTick) / TickInterval);
        IsTickFrame = TicksThisFrame > 0;

        if (IsTickFrame)
            NextTick = Elapsed - ((Elapsed - NextTick) % TickInterval);
    }

    private void LimitFrameRate()
    {
        if (FrameRateLimit == float.PositiveInfinity) return;

        float targetTime = 1 / FrameRateLimit;
        float timeLeft = targetTime - (CurrentTime - Elapsed);

        while (timeLeft > 0)
        {
            if (timeLeft > 0.005f)
                Thread.Sleep((int)(timeLeft * 1000));
            else
                Thread.SpinWait(1);

            timeLeft = targetTime - (CurrentTime - Elapsed);
        }
    }
}
