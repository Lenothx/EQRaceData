using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EQRaceData
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Load data from files
            var dbstrData = LoadDbstrData("dbstr_us.txt");
            var raceDataFile = LoadRaceData("racedata.txt");

            // Combine the two dictionaries
            var completeData = CombineRaceData(dbstrData, raceDataFile);
            // Write to output file and console
            WriteDataToFile("racedatacomplete.txt", completeData);
            // make a MySEQ version of same.
            var names = ExtractNamesFromFile("racedatacomplete.txt");
            WriteDataToFile("races.txt", names);
        }

        private static Dictionary<int, string> LoadDbstrData(string fileName)
        {
            return File.ReadLines(fileName)
                       .Select(line => line.Split('^'))
                       .Where(fields => fields[1] == "11") // Only keep race data
                       .ToDictionary(fields => int.Parse(fields[0]), fields => fields[2]);
        }

        private static Dictionary<int, string> LoadRaceData(string fileName)
        {
            return File.ReadLines(fileName)
                       .Select(line => line.Split('^'))
                       .GroupBy(fields => int.Parse(fields[0]))
                       .ToDictionary(group => group.Key, group => string.Join(" & ", group.Select(fields => fields[50])));
        }

        private static Dictionary<int, string> CombineRaceData(Dictionary<int, string> dbstrData, Dictionary<int, string> raceDataFile)
        {
            return dbstrData.Keys.Union(raceDataFile.Keys)
                                .Distinct()
                                .ToDictionary(key => key, key =>
                                {
                                    var name = dbstrData.ContainsKey(key) ? dbstrData[key] : string.Empty;
                                    var file = raceDataFile.ContainsKey(key) ? raceDataFile[key] : string.Empty;
                                    return $"ID: {key} | Name: {name} | File: {file}";
                                });
        }

        private static void WriteDataToFile(string fileName, Dictionary<int, string> data)
        {
            using var writer = new StreamWriter(fileName);
            foreach (var entry in data.OrderBy(k => k.Key))
            {
                writer.WriteLine(entry.Value);
                Console.WriteLine(entry.Value);
            }

            Console.WriteLine($"Race Data Count: {data.Count}");
            Console.ReadLine();
        }

        private static void WriteDataToFile(string filename, IEnumerable<string> data)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                foreach (var line in data)
                {
                    writer.WriteLine(line);
                }
            }
        }

        private static List<string> ExtractNamesFromFile(string filePath)
        {
            var names = new List<string>();

            // Read each line from the file
            foreach (var line in File.ReadLines(filePath))
            {
                // Check if the line contains the "| Name: " pattern
                if (line.Contains("| Name: "))
                {
                    // Extract the name portion between "| Name: " and "| File: "
                    var startIndex = line.IndexOf("| Name: ") + "| Name: ".Length;
                    var endIndex = line.IndexOf("| File: ", startIndex);

                    // Use the startIndex and endIndex to get the name substring
                    var name = line.Substring(startIndex, endIndex - startIndex).Trim();
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        name = "UNKNOWN";
                    }
                    // Add the extracted name to the list
                    names.Add(name);
                }
            }

            return names;
        }
    }
}