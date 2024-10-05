using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EQRaceData
{
    internal static class Program
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
                                    var name = dbstrData.TryGetValue(key, out var value1) ? value1 : string.Empty;
                                    var file = raceDataFile.TryGetValue(key, out var value) ? value : string.Empty;
                                    return $"ID: {key} | Name: {name} | File: {file}";
                                });
        }

        private static void WriteDataToFile(string fileName, Dictionary<int, string> data)
        {
            File.WriteAllLines(fileName, data.OrderBy(k => k.Key).Select(entry => entry.Value));

            // Display the output to console using LINQ
            data.OrderBy(k => k.Key)
                .Select(entry => entry.Value)
                .ToList()
                .ForEach(Console.WriteLine);

            Console.WriteLine($"Race Data Count: {data.Count}");
            Console.ReadLine();
        }

        private static void WriteDataToFile(string filename, IEnumerable<string> data)
        {
            using StreamWriter writer = new(filename);
            foreach (var line in data)
            {
                writer.WriteLine(line);
            }
        }

        private static List<string> ExtractNamesFromFile(string racesfile)
        {
            return File.ReadLines(racesfile)
                       .Where(line => line.Contains("| Name: "))
                       .Select(line =>
                       {
                           var startIndex = line.IndexOf("| Name: ") + "| Name: ".Length;
                           var endIndex = line.IndexOf("| File: ", startIndex);
                           var name = line[startIndex..endIndex].Trim();

                           // Replace blank names with "UNKNOWN"
                           return string.IsNullOrWhiteSpace(name) ? "UNKNOWN" : name;
                       })
                       .ToList();
        }
    }
}