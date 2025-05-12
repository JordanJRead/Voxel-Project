using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTK_Test
{
    internal class Scene
    {
        public Scene(string filePath)
        {
            Array fileLines = File.ReadLines(filePath).ToArray();

            foreach (string line in fileLines)
            {
                Console.WriteLine(line);
            }
        }
    }
}
