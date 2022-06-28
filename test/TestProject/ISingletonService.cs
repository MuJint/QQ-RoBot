using System;
using System.Threading.Tasks;

namespace TestProject
{
    /// <summary>
    /// Mixin 泛型单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISingletonService<T> where T : class
    {
        private static T _instance;
        static T Instance
        {
            get
            {
                if (_instance is null)
                {
                    lock (typeof(T))
                    {
                        if (_instance is null)
                            _instance = Activator.CreateInstance(typeof(T), true) as T;
                    }
                }
                return _instance;
            }
        }

        //默认实现
        public void RemoveSrv()
        {
            //
        }
    }

    //ManagerService 继承 ISingletonService
    public class ManagerService : ISingletonService<ManagerService>
    {
        public void AddMsg() { }
        public void RemoveMsg() { }
        public void ClearMsg() { }
        public override string ToString() => "";
    }

    public interface ILight
    {
        void SwitchOn();
        void SwitchOff();
        bool IsOn();
        public PowerStatus Power(PowerStatus powerStatus = PowerStatus.NoPower) => powerStatus;
    }

    public interface ITimerLight : ILight
    {
        public async ValueTask TurnOnFor(int duration)
        {
            Console.WriteLine("Using the default interface method for the ITimerLight.TurnOnFor.");
            SwitchOn();
            await Task.Delay(duration);
            SwitchOff();
            Console.WriteLine("Completed ITimerLight.TurnOnFor sequence.");
        }
    }

    public interface IBlinkingLight : ILight
    {
        public async ValueTask Blink(int duration, int repeatCount)
        {
            Console.WriteLine("Using the default interface method for IBlinkingLight.Blink.");
            for (int count = 0; count < repeatCount; count++)
            {
                SwitchOn();
                await Task.Delay(duration);
                SwitchOff();
                await Task.Delay(duration);
            }
            Console.WriteLine("Done with the default interface method for IBlinkingLight.Blink.");
        }
    }

    public class OverheadLight : ILight, ITimerLight, IBlinkingLight
    {
        private bool isOn;
        public bool IsOn() => isOn;

        public void SwitchOff() => isOn = false;

        public void SwitchOn() => isOn = true;

        public override string ToString() => $"The light is {(isOn ? "on" : "off")}.";
    }

    public class LedLight : IBlinkingLight, ITimerLight, ILight
    {
        private bool isOn;
        public bool IsOn() => isOn;

        public void SwitchOff() => isOn = false;

        public void SwitchOn() => isOn = true;

        public async Task Blink(int duration, int repeatCount)
        {
            Console.WriteLine("LED Light starting the Blink function.");
            await Task.Delay(duration * repeatCount);
            Console.WriteLine("LED Light has finished the Blink function.");
        }

        public override string ToString() => $"The light is {(isOn ? "on" : "off")}";
    }

    public class TestMethods
    {
        public static async Task Run()
        {
            //单例
            var managerSrv = ISingletonService<ManagerService>.Instance;
            managerSrv.AddMsg();
            managerSrv.RemoveMsg();
            managerSrv.ClearMsg();
            if(managerSrv is ISingletonService<ManagerService> srv)
            {
                srv.RemoveSrv();
            }

            //error
            managerSrv.ClearMsg();

            var overhead = new OverheadLight();
            await TestLightCapabilities(overhead);

            var ledLight = new LedLight();
            await ledLight.Blink(500, 3);
            await TestLightCapabilities(ledLight);

            if (overhead is ITimerLight light)
            {
                await light.TurnOnFor(50);
                light.Power();
            }
        }

        private static async Task TestLightCapabilities(ILight light)
        {
            // Perform basic tests:
            light.SwitchOn();
            Console.WriteLine($"\tAfter switching on, the light is {(light.IsOn() ? "on" : "off")}");
            light.SwitchOff();
            Console.WriteLine($"\tAfter switching off, the light is {(light.IsOn() ? "on" : "off")}");

            if (light is ITimerLight timer)
            {
                Console.WriteLine("\tTesting timer function");
                await timer.TurnOnFor(1000);
                Console.WriteLine("\tTimer function completed");
                var power1 = timer.Power(PowerStatus.ACPower);
                Console.WriteLine($"\tTimer function change power status.{power1}");
            }
            else
            {
                Console.WriteLine("\tTimer function not supported.");
            }

            if (light is IBlinkingLight blinker)
            {
                Console.WriteLine("\tTesting blinking function");
                await blinker.Blink(500, 5);
                Console.WriteLine("\tBlink function completed");
                var power2 = blinker.Power(PowerStatus.ACPower);
                Console.WriteLine($"\tBlink function change power status.{power2}");
            }
            else
            {
                Console.WriteLine("\tBlink function not supported.");
            }

            var power = light.Power(PowerStatus.NoPower);
            Console.WriteLine($"close power,{power}");
        }
    }


    public enum PowerStatus
    {
        NoPower,
        ACPower,
        FullBattery,
        MidBattery,
        LowBattery
    }
}
