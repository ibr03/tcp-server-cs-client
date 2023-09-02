# Stock Ticker Data C# Client

This is a C# client application for a TCP server set up in Node.js. It connects to the server, streams data packets, processes responses, and handles missing sequences to generate a JSON file as output.

## Prerequisites

Before running the C# Client, ensure you have the following prerequisites installed:

.NET Core SDK

## Getting Started

Follow these steps to set up and run the Client:

1. Clone the repository to your local machine:

```
git clone https://github.com/ibr03/tcp-server-cs-client.git
```

2. Navigate to the project directory:

```
cd betacrew-exchange-client
```

3. Open the betacrewExchangeClient.csproj file in your preferred C# development environment (e.g., Visual Studio, Visual Studio Code).

4. Configure the server address and port in the Main method of the Program.cs file:

```
string serverAddress = "127.0.0.1"; // Replace with the server IP or hostname
int serverPort = 3000;
```

5. Run the server:
Navigate inside betacrew_exchange_server folder and run the following command -

```
node main.js
```

6. Build and run the application:

```
dotnet run
```

6. The client will connect to the server, stream data packets, process responses, and generate a JSON file named packetData.json in the project directory. For reference, I have included the packetData.json that was created when I ran the project on my local machine.

## Project Structure

Inside betacrew_exchange_server folder -
1. main.js: Node.js TCP server file

Inside betacrewExchangeClient folder -
1. Program.cs: Main program file containing client logic.
2. packetData.json: JSON file where received packet data is stored.
