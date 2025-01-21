namespace SliLib.Core;

public class Config
{
    public float FrameRateCap { get; set; }
    public float TickIntervals { get; set; }
    public int EntityLimit { get; set; }

    public Config()
    {
        FrameRateCap = 60;
        TickIntervals = .033f;
        EntityLimit = 5000;

        Console.WriteLine("Config Set...");
    }
}
