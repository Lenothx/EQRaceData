using System;
using System.IO;
using System.Linq;

namespace EQRaceData
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbstrLines = File.ReadAllLines("dbstr_us.txt");
            var raceLines = File.ReadAllLines("racedata.txt");
            var fieldID = new string[80000];
            var fieldName = new string[80000];
            var raceFile = new string[80000];
            var raceIName = new string[80000];
            string fieldType = string.Empty;

            for (int i = 0; i < dbstrLines.Length; i++)
            {
                var dbstrFields = dbstrLines[i].Split('^');
                fieldType = dbstrFields[1];
                if (fieldType == "11")
                {
                    fieldID[i] = dbstrFields[0];
                    fieldName[i] = dbstrFields[2];
                }
                else
                {
                }

                if (fieldName[i] != null)
                {
                    Console.WriteLine($"{fieldID[i]} - {fieldName[i]}");
                }
            }

            for (int i = 0; i < raceLines.Length; i++)
            {
                var raceFields = raceLines[i].Split('^');
                int raceFileCheck = Int32.Parse(raceFields[0]);

                if (string.IsNullOrEmpty(raceFile[raceFileCheck]))
                {
                    raceFile[Int32.Parse(raceFields[0])] = raceFields[93];
                    raceIName[Int32.Parse(raceFields[0])] = raceFields[94];
                }
                else
                {
                    raceFile[Int32.Parse(raceFields[0])] += " & " + raceFields[93];
                    raceIName[Int32.Parse(raceFields[0])] += " & " + raceFields[94];
                }
            }

            fieldID = fieldID.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            fieldName = fieldName.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            using (StreamWriter writer = new StreamWriter("racedatacomplete.txt"))
            {
                for (int i = 0; i < fieldID.Length; i++)
                {
                    writer.WriteLine("ID: " + fieldID[i] + " | Name: " + fieldName[i] + " | File: " + raceFile[i] + " | Internal Name: " + raceIName[i]);
                    Console.WriteLine("ID: " + fieldID[i] + " | Name: " + fieldName[i] + " | File: " + raceFile[i] + " | Internal Name: " + raceIName[i]);
                }
            }

            Console.ReadLine();
        }
    }
}
