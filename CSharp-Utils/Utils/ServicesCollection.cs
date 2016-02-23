using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;

using TimeoutException = System.ServiceProcess.TimeoutException;

namespace CSharpUtils.Utils
{
    /// <summary>
    /// A collection of services, with some methods to facilitate handling multiple services.
    /// 
    /// The order of the services matters: when starting it's in regular order, when stopping reverse order.
    /// </summary>
    public class ServicesCollection : List<ServiceBase>
    {
        private IEnumerable<ServiceBase> StartOrder => this;
        private IEnumerable<ServiceBase> StopOrder => this.AsEnumerable().Reverse();

        private const int TIMEOUT = 1000;

        /// <summary>
        /// Automatically either run or simulate the services.
        /// </summary>
        public void Auto()
        {
            if (Environment.UserInteractive)
            {
                this.Simulate();
            }
            else
            {
                this.Run();
            }
        }

        /// <summary>
        /// Simulate starting the services.
        /// </summary>
        private void SimulateStart()
        {
            // Call OnStart method.
            foreach (ServiceBase service in this.StartOrder)
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
            foreach (ServiceBase service in this.StopOrder)
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
            this.SimulateStart();
            Console.WriteLine("Press any key to stop program");
            Console.Read();
            this.SimulateStop();
        }

        /// <summary>
        /// Run the services.
        /// </summary>
        public void Run()
        {
            ServiceBase.Run(this.ToArray());
        }

        /// <summary>
        /// Get the status of all _installed_ services.
        /// </summary>
        /// <returns></returns>
        public Dictionary<ServiceBase, ServiceControllerStatus> GetStatuses()
        {
            Dictionary<ServiceBase, ServiceControllerStatus> statuses = new Dictionary<ServiceBase, ServiceControllerStatus>();
            foreach (ServiceBase service in this)
            {
                try
                {
                    ServiceController controller = new ServiceController(service.ServiceName);
                    statuses.Add(service, controller.Status);
                }
                catch
                {
                    Console.Error.WriteLine("Failed to poll service {0}", service.ServiceName);
                }
            }
            return statuses;
        }

        /// <summary>
        /// Like ServiceController.WaitForStatus, but for all services in the collection.
        /// </summary>
        /// <param name="status">See ServiceController.WaitForStatus</param>
        /// <param name="timeout">See ServiceController.WaitForStatus</param>
        public void WaitForStatus(ServiceControllerStatus status, TimeSpan timeout)
        {
            // Call WaitForStatus for all services.
            List<Task> tasks = this.Select(service => new Task(delegate
            {
                try
                {
                    ServiceController controller = new ServiceController(service.ServiceName);
                    controller.WaitForStatus(status, timeout);
                }
                catch (Exception e)
                {
                    throw new ServiceException(service, e);
                }
            })).ToList();
            tasks.ForEach(t => t.Start());

            // Wait for all tasks to finish.
            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException e)
            {
                ServiceBase[] failed = e.InnerExceptions.Select(ie => (ie as ServiceException)?.Service).ToArray();
                throw new MultiException(failed, $"Service(s) not { status.ToString().ToLower() }: { string.Join(", ", failed.Select(s => s.ServiceName)) }", typeof(TimeoutException));
            }
        }

        /// <summary>
        /// Invoke the start task on the _installed_ services.
        /// </summary>
        public void InvokeStart()
        {
            foreach (ServiceBase service in this.StartOrder)
            {
                try
                {
                    ServiceController controller = new ServiceController(service.ServiceName);
                    if (controller.Status == ServiceControllerStatus.Running)
                    {
                        continue;
                    }
                    controller.Start();
                    controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(TIMEOUT));
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
            foreach (ServiceBase service in this.StopOrder)
            {
                try
                {
                    ServiceController controller = new ServiceController(service.ServiceName);
                    if (controller.Status == ServiceControllerStatus.Stopped)
                    {
                        continue;
                    }
                    controller.Stop();
                    controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(TIMEOUT));
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
            // Check whether all services are running.
            try
            {
                this.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(TIMEOUT));
            }
            catch (MultiException e)
            {
                Console.Error.WriteLine(e.Message);
                return;
            }

            // Stop all services.
            this.InvokeStop();

            // Check whether all services are stopped.
            try
            {
                this.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(TIMEOUT));
            }
            catch (MultiException e)
            {
                Console.Error.WriteLine(e.Message);
                return;
            }

            // Start all services.
            this.InvokeStart();
        }

        /// <summary>
        /// Install the services.
        /// </summary>
        public void Install()
        {
            this.DoInstall(false);
        }

        /// <summary>
        /// Uninstall the services.
        /// </summary>
        public void Uninstall()
        {
            this.DoInstall(true);
        }

        /// <summary>
        /// Reinstall the services.
        /// </summary>
        public void Reinstall()
        {
            this.Uninstall();
            this.Install();
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
                        catch
                        {
                            // ignored
                        }
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        private class ServiceException : Exception
        {
            /// <summary>
            /// The service that the exception applies to.
            /// </summary>
            public ServiceBase Service { get; private set; }

            public new Exception InnerException { get; private set; }

            public ServiceException(ServiceBase service, Exception innerException)
            {
                this.Service = service;
                this.InnerException = innerException;
            }
        }

        private class MultiException : Exception
        {
            /// <summary>
            /// The inner type of the exception.
            /// </summary>
            public Type InnerType { get; private set; }

            /// <summary>
            /// The services that the exception applies to.
            /// </summary>
            public IEnumerable<ServiceBase> Services { get; private set; }

            public MultiException(IEnumerable<ServiceBase> services, string message, Type innerType)
                : base(message)
            {
                this.Services = services;
                this.InnerType = innerType;
            }
        }
    }
}
