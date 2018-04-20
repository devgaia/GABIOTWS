using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Microsoft.Azure.Devices.Client;
using Windows.Devices.Gpio;
using Windows.System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace BackgroundApplication1
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;
        DeviceClient _deviceClient;
        
        Services.Simulator _simulator;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Informs the system that the background task might continue to
            // perform work after the IBackgroundTask.Run method returns.
            _deferral = taskInstance.GetDeferral();

            // Construct DeviceClient from connection string.
            // TODO: 1. Skriv din kod här...
            _deviceClient = DeviceClient.CreateFromConnectionString("HostName=GabIotws-Iot.azure-devices.net;DeviceId=rpi3_1;SharedAccessKey=DcXAELt9KnTpA9TjymX+KX0Uxq2AsDvmZlY6l9b6KoQ=", TransportType.Mqtt);

            // Get device twin.
            // TODO: 2. Skriv din kod här...
            Twin deviceTwin = await _deviceClient.GetTwinAsync();

            // Get values from device twin.                                    
            string location = deviceTwin.Properties.Desired["location"];
            float freqFactor = 1.0f; // (float)deviceTwin.Properties.Desired["frequencyFactor"];


            // Construct sensor simulator
            _simulator = new Services.Simulator(location, freqFactor);

            _simulator.Start((signal) =>
            {
                // This code executes when signal is received from simulated device
                
                // Serialize object (to json)
                var json = JsonConvert.SerializeObject(signal);

                // Create message 
                var message = new Message(Encoding.ASCII.GetBytes(json));

                // Send message to Azure IoT Hub
                // TODO: 4. Skriv din kod här...                
                _deviceClient.SendEventAsync(message);
            });

            // Registers a new delegate for the 'setLocation' method.
            // TODO: 5. Skriv din kod här...          
            await _deviceClient.SetMethodHandlerAsync("setLocation", SetLocationHandler, null);

            // Registers a new delegate for the 'setFrequency' method.
            // TODO: 6. Skriv din kod här...            
            await _deviceClient.SetMethodHandlerAsync("setFrequency", SetFrequencHandler, null);
        }

        private Task<MethodResponse> SetLocationHandler(MethodRequest methodRequest, object userContext)
        {
            string payload = methodRequest.DataAsJson;
            var location = JsonConvert.DeserializeObject<Models.SetLocationPayload>(payload);

            // Update simulator with new position
            _simulator.SetLocation(location.Name);

            return Task.FromResult(new MethodResponse(200));
        }

        private Task<MethodResponse> SetFrequencHandler(MethodRequest methodRequest, object userContext)
        {
            string payload = methodRequest.DataAsJson;
            var frequency = JsonConvert.DeserializeObject<Models.SetFrequencyPayload>(payload);

            // Update simulator with new factor that affect frequency.
            _simulator.SetFrequencyFactor(frequency.Factor);

            return Task.FromResult(new MethodResponse(200));
        }
    }
}
