using System;
using System.Runtime.InteropServices;

namespace Ezmi
{
    public class Ez32
    {
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        /// <summary>
        /// Reads memory at the specified address and offsets in the target process.
        /// </summary>
        /// <param name="processHandle">Handle to the target process.</param>
        /// <param name="baseAddress">Base address in the target process.</param>
        /// <param name="offsets">Offsets to reach the final memory address.</param>
        /// <param name="pointerAddress">The address of the pointer in the target process.</param>
        /// <returns>The bytes read from the memory location.</returns>
        public byte[] ReadMemoryPointer(IntPtr processHandle, IntPtr baseAddress, int[] offsets, out IntPtr pointerAddress)
        {
            IntPtr currentAddress = baseAddress;
            IntPtr targetAddress = IntPtr.Zero;
            byte[] valueBuffer = new byte[sizeof(Int32)];

            // Read base address value
            if (!ReadProcessMemory(processHandle, currentAddress, valueBuffer, (uint)valueBuffer.Length, out _))
            {
                pointerAddress = IntPtr.Zero;
                return null;
            }

            // Convert the bytes in the buffer and update the current address
            currentAddress = new IntPtr(BitConverter.ToInt32(valueBuffer, 0));

            foreach (int offset in offsets)
            {
                // Calculate the target address using the current address and offset
                targetAddress = IntPtr.Add(currentAddress, offset);

                int bytesRead;
                if (!ReadProcessMemory(processHandle, targetAddress, valueBuffer, (uint)valueBuffer.Length, out bytesRead) || bytesRead != sizeof(Int32))
                {
                    Console.WriteLine("Failed to read memory.");
                    pointerAddress = IntPtr.Zero;
                    return null;
                }

                long value = BitConverter.ToInt32(valueBuffer, 0);
                currentAddress = new IntPtr(value);
            }

            pointerAddress = targetAddress;
            return valueBuffer;
        }

        /// <summary>
        /// Reads the memory of a process at the specified address.
        /// </summary>
        /// <param name="processHandle">Handle to the process from which to read memory</param>
        /// <param name="targetAddress">Address in the process's memory to read from</param>
        /// <returns>The bytes read from the target address in the process's memory</returns>
        public byte[] ReadMemory(IntPtr processHandle, IntPtr targetAddress)
        {
            byte[] buffer = new byte[sizeof(long)];
            if (ReadProcessMemory(processHandle, targetAddress, buffer, (uint)buffer.Length, out _))
            {
                return buffer;
            }

            return null;
        }

        /// <summary>
        /// Writes a new value to the memory of a process at the specified address.
        /// </summary>
        /// <param name="processHandle">Handle to the process where memory will be written.</param>
        /// <param name="targetAddress">Address in the process where the value will be written.</param>
        /// <param name="value">The long value to be written to memory.</param>
        /// <returns>True if the value was successfully written to memory, false otherwise.</returns>
        public bool WriteMemory(IntPtr processHandle, IntPtr targetAddress, long value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            if (WriteProcessMemory(processHandle, targetAddress, valueBytes, (uint)valueBytes.Length, out _))
            {
                return true;
            }

            return false;
        }
    }
}