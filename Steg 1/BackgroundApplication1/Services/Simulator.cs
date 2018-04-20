using BackgroundApplication1.Models;
using System;
using System.Threading.Tasks;

namespace BackgroundApplication1.Services
{
    internal class Simulator
    {
        string _location;
        float _frequencyFactor;

        Random _randomFrequence = new Random();
        Random _randomAge = new Random();
        Random _randomGender = new Random();

        public void SetLocation(string name) => _location = name;
        public void SetFrequencyFactor(float factor) => _frequencyFactor = factor;

        public Simulator(string location, float frequencyFactor)
        {
            _location = location;
            _frequencyFactor = frequencyFactor;
        }

        public void Start(Action<Signal> action)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    // Generate values
                    int age = _randomAge.Next(0, 100);
                    string gender = _randomGender.Next(0, 2) == 0 ? "female" : "male";

                    var signal = new Signal()
                    {
                        Timestamp = DateTimeOffset.UtcNow,                        
                        Direction = "in",
                        Location = _location,
                        Age = age,
                        Gender = gender
                    };

                    int delay = _randomFrequence.Next(500, Convert.ToInt32(60000 * _frequencyFactor));
                    System.Diagnostics.Trace.WriteLine($"wait {delay} ms.");
                    await Task.Delay(delay);

                    System.Diagnostics.Trace.WriteLine($"sending signal.");
                    action(signal);
                }
            });
        }
    }
}
