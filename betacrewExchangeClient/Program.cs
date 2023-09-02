using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace betacrewExchangeClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Application started.");

            string serverAddress = "127.0.0.1"; // Replace with the server IP or hostname
            int serverPort = 3000;

            try
            {
                using (TcpClient client = new TcpClient(serverAddress, serverPort))
                using (NetworkStream stream = client.GetStream())
                {
                    Console.WriteLine("Connected to the server.");

                    // Create a list to store received packets
                    List<Packet> receivedPackets = new List<Packet>();

                    // Send a request to stream all packets (0x01 and 0x02)
                    byte[] requestPayload = new byte[] { 0x01, 0x02 };
                    Console.WriteLine($"Sending request: {BitConverter.ToString(requestPayload)}");
                    stream.Write(requestPayload, 0, requestPayload.Length);

                    // Process server responses and generate JSON
                    await ProcessServerResponses(stream, receivedPackets);

                    // Find the highest and lowest sequence number received
                    int highestReceivedSequence = receivedPackets.Max(p => p.PacketSequence);
                    int lowestReceivedSequence = receivedPackets.Min(p => p.PacketSequence);
                    // Request missing sequences (start from sequence 1)
                    for (int sequenceToRequest = lowestReceivedSequence + 1; sequenceToRequest < highestReceivedSequence; sequenceToRequest++)
                    {
                        if (!receivedPackets.Any(p => p.PacketSequence == sequenceToRequest))
                        {
                            // Sequence is missing, request it
                            byte[] requestMissingSequence = new byte[] { 0x02, (byte)sequenceToRequest };
                            Console.WriteLine($"Requesting missing sequence: {sequenceToRequest}");
                            stream.Write(requestMissingSequence, 0, requestMissingSequence.Length);

                            // Process the response containing the missing sequence
                            await ProcessServerResponses(stream, receivedPackets);
                        }
                    }

                    // Serialize the received packets to JSON
                    string json = JsonConvert.SerializeObject(receivedPackets, Formatting.Indented);

                    // Save the JSON to a file
                    File.WriteAllText("packetData.json", json);

                    Console.WriteLine("Disconnected from the server.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        // Define a class to represent a packet
        public class Packet
        {
            public string Symbol { get; set; }
            public string BuySellIndicator { get; set; }
            public int Quantity { get; set; }
            public int Price { get; set; }
            public int PacketSequence { get; set; }
        }

        static async Task ProcessServerResponses(NetworkStream stream, List<Packet> receivedPackets)
        {
            Console.WriteLine("Processing server responses...");

            byte[] buffer = new byte[17]; // Packet size based on the response payload format

            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead <= 0)
                    break; // End of stream

                if (bytesRead == buffer.Length)
                {
                    int sequence = BitConverter.ToInt32(buffer, 13);

                    // Extract data from the packet
                    string symbol = Encoding.ASCII.GetString(buffer, 0, 4);
                    string buySellIndicator = Encoding.ASCII.GetString(buffer, 4, 1);
                    int quantity = BitConverter.ToInt32(buffer, 5);
                    int price = BitConverter.ToInt32(buffer, 9);

                    Console.WriteLine($"Symbol: {symbol}, Buy/Sell: {buySellIndicator}, Quantity: {quantity}, Price: {price}, Sequence: {sequence}");

                    // Create a JSON object for the packet
                    Packet packet = new Packet
                    {
                        Symbol = symbol,
                        BuySellIndicator = buySellIndicator,
                        Quantity = quantity,
                        Price = price,
                        PacketSequence = sequence
                    };

                    receivedPackets.Add(packet);
                }
            }
        }  
    }
}
