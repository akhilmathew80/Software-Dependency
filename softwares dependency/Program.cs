//Group 7
//Akhil Mathew(#8680949)
//Alaa Kabbani(#8704828)
//Andy Lien(#7805872)
//Rohit Chaudhry(#8680339)

using System;
using System.Collections.Generic;
using System.Linq;

namespace softwares_dependency
{
    #region CLASS 'Software'
    public class Software
    {
        public string softwareName { get; set; }
        public List<string> whomIDependOn { get; set; } = new List<string>();
        public List<string> whoDependsOnMe { get; set; } = new List<string>();
    }
    #endregion
    class Program
    {
        public static bool explicitRemove = true;
        public static List<string> installedSoftwares = new List<string>();
        public static List<Software> softwares = new List<Software>();
        static void Main()
        {
            #region TAKING USER INPUTS
            List<string> inputList = new List<string>();
            while (!inputList.Contains("END"))
            {
                inputList.Add(Console.ReadLine().ToString());
            }
            #endregion
            foreach (string input in inputList)
            {
                Console.WriteLine(input);
                #region COMMAND 'DEPEND'
                if (input.Contains("DEPEND"))
                {
                    string[] dependencyArray = input.Split(" ");
                    dependencyArray = dependencyArray.Where((item, index) => index != 0).ToArray();
                    for (int i = 0; i < dependencyArray.Length; i++)
                    {
                        bool alreadyInstalled = false;
                        foreach (var software in softwares)
                        {
                            if (software.softwareName == dependencyArray[i])
                            {
                                software.whomIDependOn.Add(i == dependencyArray.Length - 1 ? null : dependencyArray[i + 1]);
                                software.whoDependsOnMe.Add(i == 0 ? null : dependencyArray[i - 1]);
                                alreadyInstalled = true;
                            }
                        }
                        if (alreadyInstalled == false)
                        {
                            softwares.Add(new Software
                            {
                                softwareName = dependencyArray[i],
                                whomIDependOn = new List<string>() { i == dependencyArray.Length - 1 ? null : dependencyArray[i + 1] },
                                whoDependsOnMe = new List<string>() { i == 0 ?
                                                                        null : dependencyArray[i - 1] }
                            });
                        }
                    }
                }
                #endregion
                #region COMMNAD 'INSTALL'
                if (input.Contains("INSTALL"))
                {
                    string[] inputArray = input.Split(" ");
                    string softwareName = inputArray[1];
                    string outputString = string.Empty;

                    outputString = InstallSoftware(softwareName);
                    foreach (string output in outputString.Split(",").Reverse().Distinct())
                    {
                        if (output != "")
                            Console.WriteLine("\tinstalling " + output);
                        installedSoftwares.Add(output);
                    }
                }
                #endregion
                #region COMMAND 'REMOVE'
                if (input.Contains("REMOVE"))
                {
                    string[] inputArray = input.Split(" ");
                    string softwareName = inputArray[1];
                    string outputString = string.Empty;

                    outputString = RemoveSoftware(softwareName);
                    foreach (string output in outputString.Split(",").Distinct())
                    {
                        if (output != "")
                            Console.WriteLine("\tremoving " + output);
                        installedSoftwares.Remove(output);
                    }
                }
                #endregion
                #region COMMAND 'LIST'
                if (input.Contains("LIST"))
                {
                    ListSoftwares();       
                }
                #endregion
            }
        }
        public static string InstallSoftware(string softwareName)
        {
            string outputString = string.Empty;
            var software = softwares.FirstOrDefault(software => software.softwareName == softwareName);
            if (software != null)
            {
                if (CheckSoftwareInstalled(software.softwareName))
                    Console.WriteLine("\t" + software.softwareName + " is already installed");
                else
                {
                    outputString = software.softwareName + ",";
                    if (software.whomIDependOn.Any() && software.whomIDependOn.First() != null)
                    {
                        foreach (var dependent in software.whomIDependOn)
                        {
                            if (!CheckSoftwareInstalled(dependent))
                                outputString += InstallSoftware(dependent);
                        }
                    }
                }     
            }
            else
            {
                if (CheckSoftwareInstalled(softwareName))
                    Console.WriteLine("\t" + software.softwareName + " is already installed");
                else
                {
                    if(softwareName != null) {
                        outputString = softwareName + ",";
                        softwares.Add(new Software
                        {
                            softwareName = softwareName,
                            whomIDependOn = new List<string>() { null },
                            whoDependsOnMe = new List<string>() { null }
                        });
                    }
                }
            }
            return outputString;
        }
        public static string RemoveSoftware(string softwareName)
        {
            string outputString = string.Empty;
            bool allDependentsUninstalled = true;
            var software = softwares.FirstOrDefault(software => software.softwareName == softwareName);
            if (software != null)
            {
                if (CheckSoftwareInstalled(softwareName))
                {
                    foreach (var dependent in software.whoDependsOnMe)
                    {
                        if (CheckSoftwareInstalled(dependent))
                            allDependentsUninstalled = false;
                    }
                    if (allDependentsUninstalled == true)
                    {
                        outputString += software.softwareName + ",";
                        installedSoftwares.Remove(software.softwareName);
                        if (software.whomIDependOn.Any())
                        {
                            foreach (var whomIDependOn in software.whomIDependOn)
                            {
                                explicitRemove = false;
                                outputString += RemoveSoftware(whomIDependOn);
                            }
                        }
                    }
                    else
                    {
                        if (explicitRemove == true)
                            Console.WriteLine("\t" + software.softwareName + " is still needed");
                    }
                    explicitRemove = true;
                }
                else
                    Console.WriteLine("\t" + software.softwareName + " is not installed");
            }
            return outputString;
        }
        public static void ListSoftwares()
        {
            foreach (string software in installedSoftwares)
            {
                if (software != "")
                    Console.WriteLine("\t" + software);
            }
        }
        public static bool CheckSoftwareInstalled(string softwareName) => installedSoftwares.Contains(softwareName);
    }
}
