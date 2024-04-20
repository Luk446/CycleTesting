# CycleTesting
> App designed to work in tandem with mechatronics code

C# App using .NET framework
- monitor serial port from arduino and export data into structured csv for further analysis


# Pre-reqs

Before running either program it is important that the arduino is attached to COM port 3. This is relatively easy and is done with the following steps

- press WIN + R
- in the new window paste (to open device manager)
- "devmgmt.msc"
- In view tab at the top select show hidden devices
- Navigate to ports
- If COM3 is occupied by another device
- remove or change the com port number
- RightClick -> Properties -> Port settings -> Advanced -> COM port number
