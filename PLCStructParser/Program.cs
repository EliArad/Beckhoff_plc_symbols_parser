using PLC_Symbol_Parser_App;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PLC_Struct_Parser_App
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Select the File to Parse
            string textFilePath = "";
            //textFilePath = "plc_symbols_2018.10.16.txt";
            textFilePath = "plc_symbols.txt";

            List<string> listToSearch = new List<string>();
            listToSearch.Add("stControlCommand");
            listToSearch.Add("stControl_Command");

            if (!File.Exists(textFilePath))
            {
                Console.WriteLine("File didn't found");
                Console.ReadKey();
                return;
            }

            string[] allLines = File.ReadAllLines(textFilePath);

            for (int i = 0; i < allLines.Count(); i++)
            {
                allLines[i] = allLines[i].Replace("\"", "");
            }

            //var filterLines = from line in allLines
            //                  from structToSearch in listToSearch
            //                  where line.Contains(l => l.Contains(structToSearch))
            //                  select line;

            var filterLines = allLines.Where(l => listToSearch.Any(s => l.Contains(s)));

            List<string> listOfSortedLines = filterLines.ToList();
            listOfSortedLines.Sort();

            string filePath1 = Path.Combine(Environment.CurrentDirectory, textFilePath.Replace(".txt", "_sorted.txt"));
            File.WriteAllText(filePath1, string.Join(Environment.NewLine, listOfSortedLines));

            if (listOfSortedLines.Any(s=>s.Contains("[")))
            {
                ArrayParser arrayParser = new ArrayParser();
                arrayParser.ExtractLineInformation(listOfSortedLines);
                arrayParser.FilteringInformation();
                string filePath = Path.Combine(Environment.CurrentDirectory, "PLCStructSymbols.cs");
                File.WriteAllText(filePath, arrayParser.FullText);
            }
            else
            {
                SymbolParser symbolParser = new SymbolParser();
                symbolParser.ExtractLineInformation(listOfSortedLines);
                symbolParser.FilteringInformation();
                string filePath = Path.Combine(Environment.CurrentDirectory, "PLCStructSymbols.cs");
                File.WriteAllText(filePath, symbolParser.FullText);
            }
             

            //ExtractLineInformation(listOfSortedLines);
            //FilteringInformation();
            //string filePath = Path.Combine(Environment.CurrentDirectory, "PLCTypes.cs");
            //File.WriteAllText(filePath, fullText);

            Console.ReadLine();
        }         
    }
}
