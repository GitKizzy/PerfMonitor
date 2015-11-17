using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Speech.Synthesis;

namespace PerfMonitor
{
    class Program
    {
        private static SpeechSynthesizer synth = new SpeechSynthesizer();

        static void Main(string[] args)
        {
            // 100% CPU messages to be selected from
            List<string> cpuMaxedOutMessages = new List<string>();
            cpuMaxedOutMessages.Add("WARNING: CPU load at 100%, watch out youngfella!");
            cpuMaxedOutMessages.Add("Warning: please don't do this to me");
            cpuMaxedOutMessages.Add("Warning: stop now or else");
            cpuMaxedOutMessages.Add("RED ALERT! RED ALERT!");
            cpuMaxedOutMessages.Add("You must construct additional pylons");

            Random rand = new Random();
            
            // synth.Speak("Welcome to Performance Monitor Version 1.0");

            #region performance counters
            // Pull CPU load in percentage
            PerformanceCounter perfCpuCount = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            perfCpuCount.NextValue();

            // Pull current available memory in MB
            PerformanceCounter perfMemCount = new PerformanceCounter("Memory", "Available MBytes");
            perfMemCount.NextValue();

            // Pull system uptime
            PerformanceCounter perfUptimeCount = new PerformanceCounter("System", "System Up Time");
            perfUptimeCount.NextValue();
            #endregion

            TimeSpan uptimeSpan = TimeSpan.FromSeconds(perfUptimeCount.NextValue());
            string sysUptimeMessage = string.Format("The current system up time is {0} days {1} hours {2} minutes {3} seconds",
                (int)uptimeSpan.TotalDays, (int)uptimeSpan.Hours, (int)uptimeSpan.Minutes, (int)uptimeSpan.Seconds);

            // Tell the user current system uptime
            Speak(sysUptimeMessage, VoiceGender.Female, 2);

            int speechSpeed = 1;

            while (true)
            {
                // get current performance values
                int currentCpuPercentage = (int)perfCpuCount.NextValue();
                int currentAvailableMemory = (int)perfMemCount.NextValue();


                // every 1 seond print the CPU load in percentage to screen
               
                Console.WriteLine("CPU Load       : {0}%", currentCpuPercentage);
                Console.WriteLine("Avalable Memory: {0}MB", currentAvailableMemory);
                

                // Only speak if CPU usage above 80%
                if(currentCpuPercentage > 80)
                {
                    if (currentCpuPercentage == 100)
                    {
                        if(speechSpeed < 5)
                        {
                            speechSpeed++;
                        }
                        string cpuLoadVocalMessage = cpuMaxedOutMessages[rand.Next(4)];
                        Speak(cpuLoadVocalMessage, VoiceGender.Female, speechSpeed);
                    }
                    else
                    {
                        
                        string cpuLoadVocalMessage = String.Format("The current CPU load is {0} percent", currentCpuPercentage);
                        Speak(cpuLoadVocalMessage, VoiceGender.Male, 2);
                    }
                }


                // Only speak if memory under 1GB
                if (currentAvailableMemory < 1024)
                {
                   
                    string memAvailVocalMessage = String.Format("You currently have {0} MB of memory available", currentAvailableMemory);
                    Speak(memAvailVocalMessage, VoiceGender.Male, 10);
                }


                Thread.Sleep(1000);

            } // end of loop       
        }
        public static void Speak(string message, VoiceGender voiceGender)
        {
            synth.SelectVoiceByHints(voiceGender);
            synth.Speak(message);
        }
        
        public static void Speak(string message, VoiceGender voiceGender, int rate)
        {
            synth.Rate = rate;
            Speak(message, voiceGender);
        }
    }
}
