using System;
using DeviceSimulator;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Device device = new Device();

            device.MouseMoveToPosition(100, 100);
        }
    }
}
