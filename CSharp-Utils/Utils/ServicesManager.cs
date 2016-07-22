using CSharpUtils.GUI;
using CSharpUtils.Utils.StatusLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpUtils.Utils
{
    public class ServicesManager
    {
        private struct ActionData
        {
            public Action<ServicesCollection> Action { get; set; }
            public string Description { get; set; }
        }

        /// <summary>
        /// The managed services.
        /// </summary>
        private ServicesCollection services;

        /// <summary>
        /// The actions that can be invoked.
        /// </summary>
        private Dictionary<string, ActionData> actions = new Dictionary<string, ActionData>();

        /// <summary>
        /// Called before an action is performed.
        /// </summary>
        public EventHandler<ActionEventArgs> BeforeAction = (object sender, ActionEventArgs e) => { };

        /// <summary>
        /// Called after an action has been performed.
        /// </summary>
        public EventHandler<ActionEventArgs> AfterAction = (object sender, ActionEventArgs e) => { };

        public ServicesManager(ServicesCollection services)
        {
            this.services = services;

            RegisterAction("install", "Install the service", ActionInstall);
            RegisterAction("uninstall", "Uninstall the service", ActionUninstall);
            RegisterAction("reinstall", "Reinstall the service", ActionReinstall);
            RegisterAction("start", "Start the service", ActionStart);
            RegisterAction("stop", "Stop the service", ActionStop);
            RegisterAction("restart", "Restart the service", ActionRestart);
            RegisterAction("simulate", null, ActionSimulate);
        }

        /// <summary>
        /// Register an custom action.
        /// </summary>
        /// <param name="name">The name of the action</param>
        /// <param name="description">The description of the action</param>
        /// <param name="action">The action to call</param>
        public void RegisterAction(string name, string description, Action<ServicesCollection> action)
        {
            if (actions.ContainsKey(name))
            {
                throw new ArgumentException(string.Format("Action name {0} is already taken", name), "name");
            }

            actions.Add(name, new ActionData { Action = action, Description = description });
        }

        /// <summary>
        /// Process the command line arguments, and perform the requested actions.
        /// </summary>
        /// <param name="args"></param>
        public void Run(string[] args)
        {
            if (args.Length == 0)
            {
                if (Environment.UserInteractive)
                {
                    // No arguments + interactive = show usage
                    Usage();
                }
                else
                {
                    // No arguments + non-interactive = the service is being started.
                    services.Run();
                }
            }
            else
            {
                foreach (string arg in args)
                {
                    ActionData action;
                    if (actions.TryGetValue(arg, out action))
                    {
                        // Action exists, call it.
                        BeforeAction(this, new ActionEventArgs(arg));
                        action.Action.Invoke(services);
                        AfterAction(this, new ActionEventArgs(arg));
                    }
                    else
                    {
                        // Action does not exits, call the method and stop/continue depending on return value.
                        if (!ActionNotFound(arg))
                        {
                            return;
                        }
                    }
                }
            }
        }

        private void ActionInstall(ServicesCollection services)
        {
            Console.WriteLine("Installing service...");
            services.Install();
        }

        private void ActionUninstall(ServicesCollection services)
        {
            Console.WriteLine("Uninstalling service...");
            services.InvokeStop();
            services.Uninstall();
        }

        private void ActionReinstall(ServicesCollection services)
        {
            Console.WriteLine("Reinstalling service...");
            services.InvokeStop();
            services.Reinstall();
        }

        private void ActionSimulate(ServicesCollection services)
        {
            Console.WriteLine("Simulating service...");
            services.Simulate(Task.Factory.StartNew(() =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new StatusDisplay(LocalStatusLogger.GetInstance()));
            }));
        }

        private void ActionStart(ServicesCollection services)
        {
            Console.WriteLine("Starting service...");
            services.InvokeStart();
        }

        private void ActionStop(ServicesCollection services)
        {
            Console.WriteLine("Stoping service...");
            services.InvokeStop();
        }

        private void ActionRestart(ServicesCollection services)
        {
            Console.WriteLine("Restarting service...");
            services.InvokeRestart();
        }

        /// <summary>
        /// Called when an unknown action is passed as an argument.
        /// </summary>
        /// <param name="name">The name of the action</param>
        /// <returns>Whether to continue running</returns>
        protected bool ActionNotFound(string name)
        {
            Usage();
            return false;
        }

        /// <summary>
        /// Output the usage of the script.
        /// </summary>
        protected void Usage()
        {
            string exeName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().CodeBase);
            string serviceText = services.Count > 1 ? "services" : "service";

            Console.Error.WriteLine("Usage: {0} ACTION ...", exeName);
            Console.Error.WriteLine("Actions: (all actions require admin access)");
            foreach (KeyValuePair<string, ActionData> action in actions)
            {
                if (action.Value.Description != null)
                {
                    Console.Error.WriteLine("  {0,-15} {1}", action.Key + ":", action.Value.Description);
                }
            }
        }
    }

    public class ActionEventArgs : EventArgs
    {
        public string Action { get; private set; }

        public ActionEventArgs(string action)
        {
            Action = action;
        }
    }
}
