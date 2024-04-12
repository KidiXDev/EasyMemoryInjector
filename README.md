

# Easy Memory Injector

A simple tools to read or write process memory value

# How it works?

This tools facilitates memory manipulation in a target process by leveraging functions from the kernel32.dll library. It includes methods for reading and writing memory at specified addresses and offsets, as well as providing functionality to access memory values and update them within the target process. The class encapsulates low-level memory operations, allowing for efficient memory management and data manipulation within the context of a Windows process.

This tool can also be used to get the value from an address that points to a certain pointer, you just need to provide the base address and offset, and it will automatically get the value at the pointer address.
