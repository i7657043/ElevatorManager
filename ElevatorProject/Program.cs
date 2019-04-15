using ElevatorProject.Implementations;
using ElevatorProject.Interfaces;
using ElevatorProject.Services.Implementations;
using ElevatorProject.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorProject
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceProvider serviceProvider = new ServiceCollection()
                    .AddSingleton<IElevatorManager, ElevatorManager>() //Controls Elevators
                    .AddSingleton<IElevatorService, ElevatorService>() //Gets up to date data relating to Elevators for manager
                    .AddSingleton<IElevatorManagerTestSimulatorService, ElevatorManagerTestSimulatorService>() //Runs simulation requests
                    .BuildServiceProvider();

            IElevatorManager manager = serviceProvider.GetService<IElevatorManager>();
            IElevatorManagerTestSimulatorService simulator = serviceProvider.GetService<IElevatorManagerTestSimulatorService>();
            
            try
            {
                //Choose to run manual or automated. Change as neccessary. If choosing manual change in ElevatorManagerTestSimulator.RunManualSimulation() 
                bool automatedMode = true;
                string automatedModeHelp = automatedMode ? "\n\nRunning in automated mode. Keep pressing keys to generate random requests and let the Manager pick them up..." : null;

                //Start an ElevatorManager on a seperate thread
                Task task = Task.Factory.StartNew(async () =>
                {
                    int exitCode = await manager.RunElevatorManager();
                    Environment.Exit(exitCode);
                });
                                
                Console.WriteLine($"Welcome to Jordan Griffiths' Elevator Code Challenge\n\nPress Escape at any point to Quit. Press any Key to Continue {automatedModeHelp}");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    Environment.Exit(0);
                }

                if (automatedMode)
                {
                    simulator.RunAutomatedSimulation();
                }
                else
                {
                    simulator.RunManualSimulation();
                }
            }
            catch (Exception)
            {
                //Log error or raise event
                Environment.Exit(-1);
            }            
        }
    }
}
