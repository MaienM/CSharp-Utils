using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

namespace CSharpUtils.Utils
{
    public class ServicesCollection : List<ServiceBase>
    {
        /// <summary>
        /// Automatically either run or simulate the services.
        /// </summary>
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

        /// <summary>
        /// Simulate starting the services.
        /// </summary>
        private void SimulateStart()
        {
            // Call OnStart method.
            foreach (ServiceBase service in this)
            {
                Console.WriteLine("Starting service {0}", service.ServiceName);
                service.GetType().GetMethod("OnStart", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(service, new object[] { new string[0] });
            }
        }

        /// <summary>
        /// Simulate stopping the services.
        /// </summary>
        private void SimulateStop()
        { 
            // Call OnStop method.
            foreach (ServiceBase service in this)
            {
                Console.WriteLine("Stopping service {0}", service.ServiceName);
                service.GetType().GetMethod("OnStop", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(service, new object[0]);
            }
        }

        /// <summary>
        /// Simulate running the services.
        /// 
        /// This invokes the OnStart method of all services, waits for input from the user, and then invokes the OnStop method of all services.
        /// </summary>
        public void Simulate()
        {
            SimulateStart();
            Console.WriteLine("Press any key to stop program");
            Console.Read();
            SimulateStop();
        }

        /// <summary>
        /// Run the services.
        /// </summary>
        public void Run()
        {
            ServiceBase.Run(this.ToArray());
        }

        /// <summary>
        /// Invoke the start task on the _installed_ services.
        /// </summary>
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

        /// <summary>
        /// Invoke the stop task on the _installed_ services.
        /// </summary>
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

        /// <summary>
        /// Invoke the restart task on the _installed_ services.
        /// </summary>
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

        /// <summary>
        /// Install the services.
        /// </summary>
        public void Install()
        {
            DoInstall(false);
        }

        /// <summary>
        /// Uninstall the services.
        /// </summary>
        public void Uninstall()
        {
            DoInstall(true);
        }

        /// <summary>
        /// Reinstall the services.
        /// </summary>
        public void Reinstall()
        {
            Uninstall();
            Install();
        }

        private void DoInstall(bool undo)
        {
            try
            {
                using (AssemblyInstaller inst = new AssemblyInstaller(this[0].GetType().Assembly, new string[0]))
                {
                    IDictionary state = new Hashtable();
                    inst.UseNewContext = true;
                    try
                    {
                        if (undo)
                        {
                            inst.Uninstall(state);
                        }
                        else
                        {
                            inst.Install(state);
                            inst.Commit(state);
                        }
                    }
                    catch
                    {
                        try
                        {
                            inst.Rollback(state);
                        }
                        catch { }
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }
}
