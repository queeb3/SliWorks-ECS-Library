namespace SliLib.ECS;

using System.Diagnostics;

public class Time
{
    public float DeltaTime { get; private set; }
    public float Elapsed { get; private set; }


    private readonly float tickInterval;
    private readonly float frameRateLimit;

    public float TickInterval => tickInterval;
    public float TicksPerSecond => 1 / tickInterval;
    public float NextTick { get; private set; }
    public bool IsTickFrame() => (int)((Elapsed - NextTick) / tickInterval) >= 0;

    public float FrameRate { get; private set; }
    public float FrameRateLimit => frameRateLimit;

    private readonly Stopwatch sw = Stopwatch.StartNew();
    public float CurrentTime => (float)sw.Elapsed.TotalSeconds;

    public Time(float tickInterval = 1f, float frameLimit = 60f)
    {
        this.tickInterval = tickInterval;
        frameRateLimit = frameLimit;

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
        if (IsTickFrame())
            NextTick = Elapsed - ((Elapsed - NextTick) % tickInterval);
    }

    private void LimitFrameRate()
    {
        if (frameRateLimit == float.PositiveInfinity) return;

        float targetTime = 1 / frameRateLimit;
        float timeLeft = targetTime - (CurrentTime - Elapsed);

        if (timeLeft > 0.005f)
            Thread.Sleep((int)(timeLeft * 1000));
    }
}
