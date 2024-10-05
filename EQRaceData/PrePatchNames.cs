using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EQRaceData
{
    class PrePatchNames
    {
        static void PrePatchName(string[] args)
        {
            // In the January 20, 2021 Patch, they deleted the internal name from the racedata.txt file
            // This should also make older stuff more accurate to both dbstr + racedata values besides 1 over other
            // This version is for older file versions
            var dbstrLines = File.ReadAllLines("dbstr_us.txt");
            var raceLines = File.ReadAllLines("racedata.txt");
            Dictionary<int, string> dbstrData = new Dictionary<int, string>(),
                raceDataFile = new Dictionary<int, string>(),
                raceDataName = new Dictionary<int, string>(),
                completeData = new Dictionary<int, string>();
            string fieldType = string.Empty,
                value = string.Empty,
                dbstrDataFinal = string.Empty,
                raceDataFileFinal = string.Empty,
                raceDataNameFinal = string.Empty;

            // Get dbstr race Data
            for (int i = 0; i < dbstrLines.Length; i++)
            {
                var dbstrFields = dbstrLines[i].Split('^');
                fieldType = dbstrFields[1];
                if (fieldType == "11")
                {
                    dbstrData.Add(Int32.Parse(dbstrFields[0]), dbstrFields[2]);
                }
            }

            // Get race data file race id, file and name
            for (int i = 0; i < raceLines.Length; i++)
            {
                var raceFields = raceLines[i].Split('^');
                int raceField = Int32.Parse(raceFields[0]);

                if (raceDataFile.TryGetValue(raceField, out value))
                {
                    raceDataFile[raceField] += $" & " + raceFields[93];
                    raceDataName[raceField] += $" & " + raceFields[94];
                }
                else
                {
                    raceDataFile.Add(raceField, raceFields[93]);
                    raceDataName.Add(raceField, raceFields[94]);
                }
            }

            // loop through the race file and name data, add blanks if dbstr has no data
            // then add to a complete list
            foreach (KeyValuePair<int, string> rData in raceDataFile)
            {
                int a = rData.Key;

                dbstrDataFinal = (dbstrData.TryGetValue(a, out value)) ? dbstrData[a] : "";
                raceDataFileFinal = (raceDataFile.TryGetValue(a, out value)) ? raceDataFile[a] : "";
                raceDataNameFinal = (raceDataName.TryGetValue(a, out value)) ? raceDataName[a] : "";

                completeData.Add(a, $"ID: {a} | Name: {dbstrDataFinal} | File: {raceDataFileFinal} | Internal Name: {raceDataNameFinal}");
            }

            // loop through the dbstr file for races without race data
            // then add to a complete list
            foreach (KeyValuePair<int, string> dData in dbstrData)
            {
                int a = dData.Key;

                dbstrDataFinal = (dbstrData.TryGetValue(a, out value)) ? dbstrData[a] : "";
                raceDataFileFinal = (raceDataFile.TryGetValue(a, out value)) ? raceDataFile[a] : "";
                raceDataNameFinal = (raceDataName.TryGetValue(a, out value)) ? raceDataName[a] : "";

                // if the key and value doesn't already exist in complete list
                if (!completeData.TryGetValue(a, out value))
                {
                    completeData.Add(a, $"ID: {a} | Name: {dbstrDataFinal} | File: {raceDataFileFinal} | Internal Name: {raceDataNameFinal}");
                }
            }

            // sort the list by ID then write to a file
            using (StreamWriter writer = new StreamWriter("racedatacomplete.txt"))
            {
                foreach (KeyValuePair<int, string> cData in completeData.OrderBy(k => k.Key))
                {
                    writer.WriteLine(cData.Value);
                    Console.WriteLine(cData.Value);
                }
            }

            // add a console count
            Console.WriteLine($"Race Data Count {raceDataFile.Count} - Dbstr Data Count {dbstrData.Count}");

            // make the cmd be open until closed
            Console.ReadLine();
        }
    }
}
