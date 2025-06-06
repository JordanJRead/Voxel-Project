using OpenTK.Windowing.Desktop;

namespace Voxel_Project
{
    internal class Program
    {
        static void Main(string[] args)
        {
            App app = new App(1200, 900, "MyApp");
            app.Run();
        }
    }
}
