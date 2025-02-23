using SliLib.Core;

namespace SliLib.OpenGL;

public class Window
{
    public static void Run(string? title = null, int height = 800, int width = 600)
    {
        WindowGL.CreateWindow(title, height, width);
        WindowGL.Run();
    }
}
