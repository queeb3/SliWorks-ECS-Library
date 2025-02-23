namespace SliLib.Core;

using System.Drawing;
using Silk.NET;
using Silk.NET.Core;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;

public static class WindowGL
{
    private static WindowOptions options = WindowOptions.Default with { Size = new Vector2D<int>(800, 600), Title = "SliWindow" };
    private static IWindow window;
    private static GL gl;
    private static uint vao;

    public static void CreateWindow(string? title = null, int height = 800, int width = 600)
    {
        if (title != null) options.Title = title;
        options.Size = new Vector2D<int>(height, width);

        window = Window.Create(options);
    }

    public static void Run()
    {
        window.Load += OnLoad;
        window.Update += OnUpdate;
        window.Render += OnRender;

        window.Run();
    }

    private static void OnLoad()
    {
        gl = window.CreateOpenGL();
        gl.ClearColor(Color.CornflowerBlue);

        vao = gl.GenVertexArray();
        gl.BindVertexArray(vao);

        IInputContext input = window.CreateInput();

        for (int i = 0; i < input.Keyboards.Count; i++)
            input.Keyboards[i].KeyDown += KeyDown;
    }

    private static void OnUpdate(double deltaTime) { }

    private static void OnRender(double deltaTime)
    {
        gl.Clear(ClearBufferMask.ColorBufferBit);
    }

    private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.Escape) window.Close();
    }
}

public static class WindowVK
{
    static WindowOptions options = WindowOptions.DefaultVulkan with { Size = new Vector2D<int>(800, 600), Title = "SliWindow" };
    private static IWindow window;

    public static void CreateWindow(string? title = null, int height = 800, int width = 600)
    {
        if (title != null) options.Title = title;
        options.Size = new Vector2D<int>(height, width);

        window = Window.Create(options);
    }
}
