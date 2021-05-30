﻿using nanoFramework.OpenThread.Net.IPv6;
using nanoFramework.OpenThread.Net.Lowpan;
using nanoFramework.OpenThread.Net.Sockets;
using System.Collections;

namespace nanoFramework.OpenThread.Net
{
    internal static class NetworkInterface
    {
        private static ushort currenEphemeraltPort = 0xC000;
        private static ILowpanInterface lowpanInterface;
        private static Hashtable udpClients = new Hashtable();
        internal const byte MaxSimultaneousSockets = 8;

        internal static IPAddress IPAddress { get; set; }
        internal static Socket[] listeners = new Socket[MaxSimultaneousSockets];

        internal static void SetupInterface(ILowpanInterface Interface)
        {
            lowpanInterface = Interface;
            IPAddress = lowpanInterface.IPLinkLocal;
            lowpanInterface.OnPacketReceived += IPv6PacketHandler;
            lowpanInterface.OnIpChanged += OnIpChanged;
        }

        internal static void Send(byte[] data)
        {
            lowpanInterface.Send(data);
        }

        internal static void SendAndWait(byte[] data)
        {
            lowpanInterface.SendAndWait(data);
        }

        internal static void OnIpChanged()
        {
            IPAddress = lowpanInterface.IPLinkLocal;
        }

        internal static void IPv6PacketHandler(object sender, byte[] frame)
        {
            IPv6Packet ipv6Packet = new IPv6Packet();
            ipv6Packet.FromBytes(frame);

            if (ipv6Packet.NextHeader == IPv6Protocol.ICMPv6)
            {               
                Icmpv6.PacketHandler(ipv6Packet);               
            }
            else if (ipv6Packet.NextHeader == IPv6Protocol.Udp)
            {
                var udpDatagram = ipv6Packet.Payload as UdpDatagram;

                var udpClient = udpClients[udpDatagram.DestinationPort] as Socket;
                
                if (udpClient == null) return;
             
                udpClient.PacketHandler(ipv6Packet);
            }
        }

        internal static void CreateSocket(Socket udpClient)
        {
            var client = udpClients[udpClient.sourcePort] as Socket;
            
            if(client!=null && client.sourceIpAddress == udpClient.sourceIpAddress){

                throw new SocketException("Port and ip adress already in use.");                
            }

            udpClients.Add(udpClient.sourcePort, udpClient);
        }

        internal static void CloseSocket(Socket udpClient)
        {
            var client = udpClients[udpClient.sourcePort] as Socket;

            client.Dispose();

            udpClients.Remove(client.sourcePort);
        }

        internal static ushort GetEphemeralPort()
        {
            currenEphemeraltPort++;
            return currenEphemeraltPort;
        }
    }
}