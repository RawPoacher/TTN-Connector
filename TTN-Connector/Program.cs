using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using uPLibrary.Networking.M2Mqtt.Internal;
using uPLibrary.Networking.M2Mqtt.Session;
using uPLibrary.Networking.M2Mqtt.Utility;


// TODO: use TLS

namespace TTN_Connector
{
    class Program
    {

        private const string AppEUI = @"70B3D57ED0000538"; // TODO: make configurable
        private const string AccessKey = @"Dj8ebO/p+agcxgKoRMmF0rKd07X7m4idj+M6HiS48EY="; // TODO: make configurable
        private const string mqttBrokerHostName = "rene-ttn-test"; // TODO: make configurable
        private const string clientId = "RSTest";
        private const string topicFilter = @"+/+/+/up";
        private static MqttClient client = null;
        private const int retryDelay = 1000;

        static void Main(string[] args)
        {
            client = new MqttClient(mqttBrokerHostName); // TODO: add error handling!
            
            // Register event handler for Connection being closed
            client.ConnectionClosed += client_ConnectionClosed;

            // Register event handler for subscription
            client.MqttMsgSubscribed += client_MqttMsgSubscribed;

            // register event handler for received messages
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            Connect();

            // subscribe to the topic ( all Applications, all Devices)
            // TODO: check if QOS_LEVEL is the right one for this application; QOS Level 1 should be OK
            // TODO: error handling
            client.Subscribe(new string[] { topicFilter }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            DoStuff();

            Disconnect();
        }

        static void DoStuff()
        {
            Console.WriteLine("Press <Enter> to quit");
            Console.ReadLine();
        }

        static async void Connect()
        {
            // Make a connection with a persistent session. Send keep-alive every 60 seconds 
            _connect();

            while (!client.IsConnected)
            {
                await DelayTask(retryDelay);
                _connect();
            }
            Console.WriteLine("Connected");
        }

        static void _connect()
        {
            Console.WriteLine("Connecting...");
            client.Connect(clientId, AppEUI, AccessKey, false, 60);
        }

        static void Disconnect()
        {
            if (client.IsConnected)
            {
                // Remove event handler for Connection being closed
                client.ConnectionClosed -= client_ConnectionClosed;
                client.Disconnect();
            }
        }

        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {

            // TODO: handle message received (instead of output to console)
            Console.WriteLine("Topic: "+ e.Topic);
            var str = System.Text.Encoding.Default.GetString(e.Message);
            Console.WriteLine(str);
        }

        static void client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            Console.WriteLine("Subscribed for id = " + e.MessageId);
        }

        static void client_ConnectionClosed(object sender, EventArgs e)
        {
            Console.WriteLine("Connection lost.");
            Connect();
        }

        static async Task DelayTask(int milisecondsDelay)
        {
            await Task.Delay(milisecondsDelay);
        }

    }
}
