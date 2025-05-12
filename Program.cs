using OpenTK.Windowing.Desktop;

namespace OpenTK_Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            App app = new App(800, 600, "MyApp");
            app.Run();
        }
    }
}
