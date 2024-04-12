using Ezmi;
using System;
using System.Diagnostics;

namespace ExampleProject
{
    public class Program
    {
        private static void Main(string[] args)
        {
            string executableName;
            IntPtr baseAddress;
            int offsetNum;

            Console.Write("Enter executable name (without extension): ");
            executableName = Console.ReadLine();
            Console.Write("Enter base address: ");
            baseAddress = new IntPtr(Convert.ToInt32(Console.ReadLine(), 16));
            Console.Write("Enter number of offsets: ");
            offsetNum = int.Parse(Console.ReadLine());

            int[] offsets = new int[offsetNum];
            for (int i = 0; i < offsetNum; i++)
            {
                Console.Write($"Enter offset {i + 1}: ");
                offsets[i] = Convert.ToInt32(Console.ReadLine(), 16);
            }

            // Create an instance of the Ez32 class
            Ez32 ez32 = new Ez32();

            Console.WriteLine("====================================================================");
            Console.WriteLine($"Reading pointer from {executableName} at base address {baseAddress.ToString("X")}");
            Console.WriteLine("====================================================================");

            // Get the process with the specified name
            Process[] processes = Process.GetProcessesByName(executableName);

            // Store the pointer address
            IntPtr pointerAddress;

            // Using ReadMemoryPointer to read the value and retrieve final pointer
            byte[] bytes = ez32.ReadMemoryPointer(processes[0].Handle, baseAddress, offsets, out pointerAddress);
            if (bytes == null)
            {
                Console.WriteLine("ReadMemoryPointer failed");
                return;
            }

            // Convert the byte array to an integer value
            // this is the value of the pointer
            int pointerValue = BitConverter.ToInt32(bytes, 0);

            Console.WriteLine($"Pointer Address: {pointerAddress.ToString("X")}\nValue: {pointerValue}");

            Console.Write("Inject new Value: ");
            int newValue = int.Parse(Console.ReadLine());

            // Inject new value to the pointer address
            if (ez32.WriteMemory(processes[0].Handle, pointerAddress, newValue))
            {
                Console.WriteLine("====================================================================");
                Console.WriteLine($"Writing memory to {executableName} at {pointerAddress.ToString("X")}");
                Console.WriteLine("====================================================================");
                Console.WriteLine("Injection Status: Success");
            }
            else
            {
                Console.WriteLine("====================================================================");
                Console.WriteLine($"Writing memory to {executableName} at {pointerAddress.ToString("X")}");
                Console.WriteLine("====================================================================");
                Console.WriteLine("Injection Status: Failed");
            }

            Console.ReadLine();
        }
    }
}