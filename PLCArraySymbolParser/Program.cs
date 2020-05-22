using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static PLC_Symbol_Parser_App.Program;

namespace PLC_Symbol_Parser_App
{
    public class Program
    {
        //Select the File to Parse
        static string textFilePath = "plc_symbols.txt";                //Full File
        //static string textFilePath = "plc_symbols3.txt";             //Simplified File

        static void Main(string[] args)
        {

            SymbolArrayParser symbolArrayParser = new SymbolArrayParser();

            string res = symbolArrayParser.Start(textFilePath, @"PLCSymbolArrays.cs");
            Console.WriteLine(res);
        }       
    }
}
