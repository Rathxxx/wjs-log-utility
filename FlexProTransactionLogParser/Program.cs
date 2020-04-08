using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FlexProTransactionLogParser
{
    internal class Program
    {

        private static string _logPath;


        private static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Please provide a path to the transaction log files");
                Console.ReadKey();
                return;
            }

            var nonUniqueOut = new List<string>();

            _logPath = args[0];
            Console.WriteLine($"Log Path: {_logPath}");

            // Remove output from any previous runs
            if (File.Exists(_logPath + "\\UniqueRecords.trn"))
            {
                File.Delete(_logPath + "\\UniqueRecords.trn");
            }
            if (File.Exists(_logPath + "\\NonUniqueRecords.trn"))
            {
                File.Delete(_logPath + "\\NonUniqueRecords.trn");
            }

            var keep = Parser.GetRecords(_logPath);
            
            Console.WriteLine($"{keep.Count} records located");

            var uniques = new Dictionary<string, IRecord>();
            var nonUniques = new Dictionary<string, List<IRecord>>();

            Parser.SortUniqueFromNonUnique(keep, 23, ref uniques, ref nonUniques); // Sort using column 24
            var uniqueOut = uniques.Select(a => a.Value).Select(b => b.Raw).ToList();

            foreach (var a in nonUniques)
            {
                nonUniqueOut.AddRange(a.Value.Select(b => b.Raw));
            }
            uniques.Clear();
            nonUniques.Clear();
            Parser.SortUniqueFromNonUnique(keep, 12, ref uniques, ref nonUniques); // Sort using column 13

            uniqueOut.AddRange(uniques.Select(a => a.Value).Select(b => b.Raw));

            foreach (var a in nonUniques)
            {
                nonUniqueOut.AddRange(a.Value.Select(b => b.Raw));
            }

            File.WriteAllLines(_logPath + "\\UniqueRecords.trn", uniqueOut);
            File.WriteAllLines(_logPath + "\\NonUniqueRecords.trn", nonUniqueOut);

            Console.WriteLine("Complete: Press any key to exit");
            Console.ReadKey();
        }
    }
}
