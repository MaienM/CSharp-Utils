using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceProcess;

namespace CSharpUtils.Utils
{
    public class ServicesCollection : List<ServiceBase>
    {
        public void Auto()
        {
            if (Environment.UserInteractive)
            {
                Simulate();
            }
            else
            {
                Run();
            }
        }

        public void SimulateStart()
        {
            // Call OnStart method.
            foreach (ServiceBase service in this)
            {
                Console.WriteLine("Starting service {0}", service.ServiceName);
                service.GetType().GetMethod("OnStart", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(service, new object[] { new string[0] });
            }
        }

        public void SimulateStop()
        { 
            // Call OnStop method.
            foreach (ServiceBase service in this)
            {
                Console.WriteLine("Stopping service {0}", service.ServiceName);
                service.GetType().GetMethod("OnStop", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(service, new object[0]);
            }
        }

        public void Simulate()
        {
            SimulateStart();
            Console.WriteLine("Press any key to stop program");
            Console.Read();
            SimulateStop();
        }

        public void Run()
        {
            ServiceBase.Run(this.ToArray());
        }

        public void InvokeStart()
        {
            foreach (ServiceBase service in this)
            {
                try
                {
                    ServiceController controller = new ServiceController(service.ServiceName);
                    controller.Start();
                    controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(5000));
                }
                catch
                {
                    Console.Error.WriteLine("Failed to start service {0}", service.ServiceName);
                }
            }
        }

        public void InvokeStop()
        {
            foreach (ServiceBase service in this)
            {
                try
                {
                    ServiceController controller = new ServiceController(service.ServiceName);
                    controller.Stop();
                    controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(5000));
                }
                catch
                {
                    Console.Error.WriteLine("Failed to stop service {0}", service.ServiceName);
                }
            }
        }

        public void InvokeRestart()
        {
            foreach (ServiceBase service in this)
            {
                try
                {
                    ServiceController controller = new ServiceController(service.ServiceName);
                    controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(5000));
                    controller.Stop();
                    controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(5000));
                    controller.Start();
                    controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(5000));
                }
                catch
                {
                    Console.Error.WriteLine("Failed to restart service {0}", service.ServiceName);
                }
            }
        }
    }
}
