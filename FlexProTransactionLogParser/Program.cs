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

            var bytes = new byte[1];
            bytes[0] = Convert.ToByte("7F", 16);
            string delim = Encoding.GetEncoding("ISO-8859-1").GetString(bytes);
            //Console.WriteLine(delim);
            logPath = args[0];
            Console.WriteLine($"Log Path: {logPath}");

            //Get list of all files
            List<string> logFiles = Directory.GetFiles(logPath).ToList();
            List<List<string>> allRecords = new List<List<string>>();
            foreach(var lf in logFiles)
            {
                List<string> records = new List<string>();
                records = File.ReadAllLines(lf).ToList();
                Console.WriteLine($"Found: {lf} with {records.Count}");
                allRecords.Add(records);
            }
            List<string[]> keep = new List<string[]>();
            foreach(var rec in allRecords)
            {
                foreach(var r in rec)
                {
                    var a = r.Split(delim, StringSplitOptions.None);
                    if(a[5] == "4")
                    {
                        keep.Add(a);
                        Console.WriteLine($"{a[23]} - {a[12]} - {a[5]}");
                    }
                    
                }
            }
            Console.WriteLine($"{keep.Count} records located");

            Dictionary<string, string[]> uniques24 = new Dictionary<string, string[]>();
            Dictionary<string, List<string[]>> nonUniques24 = new Dictionary<string, List<string[]>>();

            //Walk through records and collect duplicates and uniques
            foreach(var k in keep)
            {
                if (!uniques24.ContainsKey(k[23]))
                {
                    uniques24.Add(k[23], k);
                } else 
                {
                    var temp = uniques24[k[23]];
                    if (!nonUniques24.ContainsKey(k[23]))
                    {
                        List<string[]> l = new List<string[]>();
                        l.Add(temp);
                        l.Add(k);
                        nonUniques24.Add(k[23], l);
                    }
                    else
                    {
                        var t = nonUniques24[k[23]];
                        t.Add(k);
                        nonUniques24[k[23]] = t;
                    }
                    
                }
            }
            Console.WriteLine($"Uniques 24: {uniques24.Count}");
            foreach(var a in uniques24)
            {
                var b = a.Value;
                var x = String.Join(delim, b);
                uniqueOut.Add(x);
                Console.WriteLine(x);
            }
            Console.WriteLine();
            Console.WriteLine($"Non Uniques 24: {nonUniques24.Count}");
            foreach(var a in nonUniques24)
            {
                foreach(var b in a.Value)
                {
                    var x = String.Join(delim, b);
                    nonUniqueOut.Add(x);
                    Console.WriteLine(x);
                }
                Console.WriteLine();
                Console.WriteLine();
            }

            Dictionary<string, string[]> uniques13 = new Dictionary<string, string[]>();
            Dictionary<string, List<string[]>> nonUniques13 = new Dictionary<string, List<string[]>>();

            //Walk through records and collect duplicates and uniques
            foreach (var k in keep)
            {
                if (!uniques13.ContainsKey(k[12]))
                {
                    uniques13.Add(k[12], k);
                }
                else
                {
                    var temp = uniques13[k[12]];
                    if (!nonUniques13.ContainsKey(k[12]))
                    {
                        List<string[]> l = new List<string[]>();
                        l.Add(temp);
                        l.Add(k);
                        nonUniques13.Add(k[12], l);
                    }
                    else
                    {
                        var t = nonUniques13[k[12]];
                        t.Add(k);
                        nonUniques13[k[12]] = t;
                    }

                }
            }
            Console.WriteLine($"Uniques 13: {uniques13.Count}");
            foreach (var a in uniques13)
            {
                var b = a.Value;
                var x = String.Join(delim, b);
                uniqueOut.Add(x);
                Console.WriteLine(x);
            }
            Console.WriteLine();
            Console.WriteLine($"Non Uniques 13: {nonUniques13.Count}");
            foreach (var a in nonUniques13)
            {
                foreach (var b in a.Value)
                {
                    var x = String.Join(delim, b);
                    nonUniqueOut.Add(x);
                    Console.WriteLine(x);
                }
                Console.WriteLine();
                Console.WriteLine();
            }

            if(File.Exists(logPath + "\\UniqueRecords.trn"))
            {
                File.Delete(logPath + "\\UniqueRecords.trn");
            }
            if(File.Exists(logPath + "\\NonUniqueRecords.trn"))
            {
                File.Delete(logPath + "\\NonUniqueRecords.trn");
            }

            File.WriteAllLines(logPath + "\\UniqueRecords.trn", uniqueOut);
            File.WriteAllLines(logPath + "\\NonUniqueRecords.trn", nonUniqueOut);

            Console.WriteLine("Complete: Press any key to close this window");
            Console.ReadKey();
        }
    }
}
