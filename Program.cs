using OpenTK.Windowing.Desktop;

namespace Voxel_Project
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(

                @"
### Player Controls:
Press P to switch to the editor mode  
WASD for movement  
Number keys for switching currently held item  

## Item Controls:
# 1 (Hoe)
Left click to till and untill grass

# 2 (Shovel)
Left click on a tilled block to plant a seed  
Q and E to cycle through all seeds  
R to buy the current seed  
The color of the border around the seed icon represents how many of that seed you have

# 3 (Scythe)
Left click on a tilled block that has a fully grown plant above it to harvest it for money

# 4 (Axe)
Left click on a log block of a tree to cut it down and gain wood

# 5 (Hammer)
Left click on a block to place a fence on it  
Right click on a block to break the fence on it

### Editor Controls:

Hold down left control to use editor controls  

P: Switch between editor and player mode  
R: Switch between voxel and fence mode  
T: Place / remove tree  
Q and E: Cycle through voxel types  
Y and U: Select corners of selection volume  
F: Fill selection volume with current voxel type  
G: Remove all voxels in selected volume  
C: Copy voxels in selected volume  
V: Paste copied voxels at your current cursor location relative to the selection point made with Y  
B: Rotate selected voxels 90 degrees  
WASD / Space / Left Shift: Move the editor cursor  
Left Alt: Hold to increase the speed of moving the editor cursor 
                "
            );
            App app = new App(1200, 900, "MyApp");
            app.Run();
        }
    }
}
