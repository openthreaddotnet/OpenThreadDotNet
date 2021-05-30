# nanoFramework.OpenThread
nanoFramework.OpenThread is a library for .Net nanoFramework designed to work with the 6lowpan Thread network. 

To develop 6LoWPAN applications, you will need a radio module running as an OpenThread Network Co-Processor (NCP). More information is available on the webpage  https://openthread.io/platforms/co-processor. The end device can be used on any module from supported platforms https://openthread.io/platforms with NCP firmware. 

The library tested on TI CC2652 based board http://www.ti.com/tool/LAUNCHXL-CC26X2R1 and nrf52840 based modules http://www.skylabmodule.com/skylab-125k-ram-industry-grade-low-energy-multiprotocol-5-0-ant-bluetooth-module/ 

With OpenThreadDotNet library is possible to scan for nearby wireless networks, join to the wireless networks and form a new wireless mesh network.

Form a new Thread wireless network and UDP server we need just 6 lines of code.

```csharp

    LoWPAN loWPAN = new LoWPAN("COM2");

    loWPAN.Open();
              
    loWPAN.Form("Networkname", 11, "00112233445566778899AABBCCDDEEFF", 1234);
   
    Socket receiver = new Socket();
            
    receiver.Bind(IPAddress.Any, 1000);
            
    IPEndPoint remoteIp = null;
           
    while (true)
    {
      if (receiver.Poll(-1, SelectMode.SelectRead))
      {
        byte[] data = receiver.Receive(ref remoteIp);        
        string message = Encoding.ASCII.GetString(data);
        Console.WriteLine("\n");
        Console.WriteLine("{0} bytes from {1} {2} {3}", message.Length, remoteIp.Address, remoteIp.Port, message);                
      }
    }		
```
