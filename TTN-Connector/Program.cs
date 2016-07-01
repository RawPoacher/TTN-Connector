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

namespace TTN_Connector
{
    class Program
    {
        static void Main(string[] args)
        {
            string AppEUI = @"70B3D57ED0000538"; // TODO: make configurable
            string AccessKey = @"Dj8ebO/p+agcxgKoRMmF0rKd07X7m4idj+M6HiS48EY="; // TODO: make configurable

            string username = AppEUI;   
            string password = AccessKey;   

            string mqttBrokerHostName = "rene-ttn-test"; // TODO: make configurable
                                                         // TODO: use TLS

            Console.WriteLine("starting up...");
            MqttClient client = new MqttClient(mqttBrokerHostName); // TODO: add error handling!

            // Register event handler for Connection being closed
            client.ConnectionClosed += client_ConnectionClosed;

            // register event handler for received messages
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            // Register event handler for subscription
            client.MqttMsgSubscribed += client_MqttMsgSubscribed;

            string clientId = "RSTest";
            client.Connect(clientId, username, password);   // TODO: add error handling

            // subscribe to the topic ( all Applications, all Devices
            client.Subscribe(new string[] { @"+/devices/+/up" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            Console.WriteLine("Press <Enter> to quit");
            Console.ReadLine();

            // Remove event handler for Connection being closed
            client.ConnectionClosed -= client_ConnectionClosed;
            client.Disconnect();

            Console.WriteLine("Ended!");
        }

        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var str = System.Text.Encoding.Default.GetString(e.Message);
            // handle message received 
            Console.WriteLine(str);
        }

        static void client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            Console.WriteLine("Subscribed for id = " + e.MessageId);
        }

         static void client_ConnectionClosed(object sender, EventArgs e)
        {
            Console.WriteLine("Connection lost.");
        }
    }
}
