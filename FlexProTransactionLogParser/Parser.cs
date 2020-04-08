using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FlexProTransactionLogParser
{
    internal static class Parser
    {
        /// <summary>
        /// Retrieves all valid records from files found at the specified path
        /// </summary>
        /// <param name="path">Path to read files / records from</param>
        /// <returns>Collection of valid parsed records </returns>
        public static List<IRecord> GetRecords(string path)
        {
            var logFiles = Directory.GetFiles(path).ToList();
            var records = new List<IRecord>();
            foreach (string lf in logFiles)
            {
                records.AddRange(GetValidRecords(File.ReadAllLines(lf).ToList()));
            }
            return records;
        }
        
        /// <summary>
        /// Parses records and collections those that are valid
        /// </summary>
        /// <param name="records">Records to validate</param>
        /// <returns>Collection of valid parsed records</returns>
        private static IEnumerable<IRecord> GetValidRecords(IEnumerable<string> records)
        {
            var bytes = new byte[1];
            bytes[0] = Convert.ToByte("7F", 16);
            string delim1 = Encoding.GetEncoding("ISO-8859-1").GetString(bytes);
            const string delim2 = ",";

            var keep = new List<Record>();
            
            foreach (string record in records)
            {
                // Determine which delimiter to use. Default is comma
                string delim = delim2;
                if (record.Contains(delim1))
                {
                    delim = delim1;
                }

                // Parse the record and add to collection provided the record is valid (column 6 == 4)
                var a = record.Split(delim);
                if (a[5] == "4")
                {

                    keep.Add(new Record()
                    {
                        Column6 = a[5],
                        Column24 = a[23],
                        Column13 = a[12],
                        Raw = record
                    });
                }
            }
            return keep;
        }
        
        /// <summary>
        /// Loops through a collection or records and separates the unique records from non-unique records based on the provided column of interest
        /// </summary>
        /// <param name="records">Collection of records to sort into unique and non-unique collections</param>
        /// <param name="columnOfInterest">the column to use for determining uniqueness</param>
        /// <param name="uniques">collection of unique records</param>
        /// <param name="nonUniques">collection of non-unique records</param>
        public static void SortUniqueFromNonUnique(List<IRecord> records, int columnOfInterest, ref Dictionary<string, IRecord> uniques, ref Dictionary<string, List<IRecord>> nonUniques)
        {
            //Walk through records and collect duplicates and uniques
            foreach (var record in records)
            {
                string key = (columnOfInterest == 23) ? record.Column24 : record.Column13;

                if (!uniques.ContainsKey(key))
                {
                    uniques.Add(key, record);
                }
                else
                {
                    var temp = uniques[key];
                    if (!nonUniques.ContainsKey(key))
                    {
                        nonUniques.Add(key, new List<IRecord>() {
                            temp,
                            record
                        });
                    }
                    else
                    {
                        var t = nonUniques[key];
                        t.Add(record);
                        nonUniques[key] = t;
                    }

                }
            }
            Console.WriteLine($"{records.Count} records : using column {columnOfInterest} : {uniques.Count} uniques : {nonUniques.Count} non uniques");
        }
    }
}
