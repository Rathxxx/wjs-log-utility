using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FlexProTransactionLogParser
{
    class Program
    {

        private static string logPath;
        

        static void Main(string[] args)
        {
            ///args[0] = Path to Files to Parse
            
            if(args.Length == 0)
            {
                Console.WriteLine("Please provide a path to the transaction log files");
                Console.ReadKey();
                return;
            }

            List<string> uniqueOut = new List<string>();
            List<string> nonUniqueOut = new List<string>();

            logPath = args[0];
            Console.WriteLine($"Log Path: {logPath}");

            // Remove output from any previous runs
            if (File.Exists(logPath + "\\UniqueRecords.trn"))
            {
                File.Delete(logPath + "\\UniqueRecords.trn");
            }
            if (File.Exists(logPath + "\\NonUniqueRecords.trn"))
            {
                File.Delete(logPath + "\\NonUniqueRecords.trn");
            }

            var keep = Parser.GetRecords(logPath);
            
            Console.WriteLine($"{keep.Count} records located");

            Dictionary<string, Record> uniques = new Dictionary<string, Record>();
            Dictionary<string, List<Record>> nonUniques = new Dictionary<string, List<Record>>();

            Parser.SortUniqueFromNonUnique(keep, 23, ref uniques, ref nonUniques); // Sort using column 24
            foreach (var a in uniques)
            {
                var b = a.Value;
                uniqueOut.Add(b.Raw);
            }

            foreach (var a in nonUniques)
            {
                foreach (var b in a.Value)
                {
                    nonUniqueOut.Add(b.Raw);
                }
            }
            uniques.Clear();
            nonUniques.Clear();
            Parser.SortUniqueFromNonUnique(keep, 12, ref uniques, ref nonUniques); // Sort using column 13

            foreach (var a in uniques)
            {
                var b = a.Value;
                uniqueOut.Add(b.Raw);
            }

            foreach (var a in nonUniques)
            {
                foreach (var b in a.Value)
                {
                    nonUniqueOut.Add(b.Raw);
                }
            }

            File.WriteAllLines(logPath + "\\UniqueRecords.trn", uniqueOut);
            File.WriteAllLines(logPath + "\\NonUniqueRecords.trn", nonUniqueOut);

            Console.WriteLine("Complete: Press any key to exit");
            Console.ReadKey();
        }
    }
}
