using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PLC_Symbol_Parser_App
{
    public class ArrayParser
    {
        List<string> listOfSortedLines = new List<string>();
        static List<LineStruct> listOfLineStructs = new List<LineStruct>();
        static string previousPartOfStruct = string.Empty;
        private static int prevousNodeLevel = 0;
        static string fullText = string.Empty;

        static string previousStructFull = string.Empty;
        static string previousStruct = string.Empty;
        static bool isStructReplicated = false;
        static int structIndex = 0;

        static string previousArray = string.Empty;
        static bool isArrayReplicated = false;
        static int arrayIndex = 0;

        static List<string> listOfNodes = new List<string>();
        static List<string> listOfStructsTypes = new List<string>();
        static List<string> listOfStructs = new List<string>();
        private int childNumber;

        public string FullText { get => fullText; set => fullText = value; }

        public string Start(string filePathWithName)
        {

            try
            {
                FileCleaningAndArraySorting(filePathWithName);
                ExtractLineInformation(listOfSortedLines);
                FilteringInformation();

                //Debugging: Print Sorted File
                //for (int i = 0; i < listOfSortedLines.Count; i++)
                //{
                //    fullText += listOfSortedLines[i] + Environment.NewLine;
                //}

                //Saving File
                string filePath = Path.Combine(Environment.CurrentDirectory, "PLCArrays.cs");
                File.WriteAllText(filePath, FullText);

                return "ok";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private void FileCleaningAndArraySorting(string textFilePath)
        {
            if (!File.Exists(textFilePath))
            {
                Console.WriteLine("File didn't found");
                Console.ReadKey();
                return;
            }

            string[] allLines = File.ReadAllLines(textFilePath);

            //Remove Inverted Comma's
            for (int i = 0; i < allLines.Count(); i++)
            {
                allLines[i] = allLines[i].Replace("\"", "");
            }

            //Filtering
            var filterLines = from line in allLines
                              where !line.StartsWith(".") && !line.StartsWith("Error_List") && line.Contains("[") && line.Contains("]")
                              select line;

            listOfSortedLines = filterLines.ToList();
            listOfSortedLines.Sort();
        }

        public void ExtractLineInformation(List<string> lines)
        {
            int lineCounter = 0;

            foreach (string line in lines)
            {
                //Intialize LineStruct with default values...
                LineStruct lineStruct = new LineStruct
                {
                    fullLineText = string.Empty,
                    structText = string.Empty,
                    dataTypeText = string.Empty,
                    nodeStatus = NodeStatus.None,
                    nodeType = NodeType.None,
                    currentNodes = new List<string>(),
                    previousNodes = new List<string>(),
                    currentNodesCleaned = new List<string>(),
                    previousNodesCleaned = new List<string>(),
                    lastNode = string.Empty,
                    lastNodeCleaned = string.Empty,
                    nodeLevel = 0,
                    nodeDatatype = string.Empty,
                    isStructReplicated = false,
                    lineNumber = 0,
                    previousChildNumbers = 0
                };

                
                //Debugging: To Print Line Number in Output
                lineStruct.lineNumber = lineCounter++;

                lineStruct.fullLineText = line;

                //Make sure there is two parts ... 0:nodes 1:dataType
                if (lineStruct.fullLineText.Split(',').Length < 2)
                {
                    return;
                }

                lineStruct.structText = lineStruct.fullLineText.Split(',')[0];
                lineStruct.dataTypeText = lineStruct.fullLineText.Split(',')[1];

                lineStruct.currentNodes = (lineStruct.structText.Split('.')).ToList();
                lineStruct.previousNodes = (previousPartOfStruct.Split('.')).ToList();

                lineStruct.currentNodesCleaned = (lineStruct.structText.Split('.')).ToList();
                lineStruct.previousNodesCleaned = (previousPartOfStruct.Split('.')).ToList();

                bool isArray = false;
                bool isArray2 = false;

                //Node's Clean Up
                lineStruct.lastNode = lineStruct.currentNodes.Last();
                lineStruct.lastNodeCleaned = RemoveBetween(lineStruct.currentNodes.Last(), '[', ']', out isArray);

                for (int i = 0; i < lineStruct.currentNodes.Count; i++)
                {
                    //Node's Clean Up (Simple)
                    if (i == lineStruct.currentNodes.Count - 1)
                    {
                        lineStruct.currentNodesCleaned[i] = RemoveBetween(lineStruct.currentNodes[i], '[', ']', isArray: out isArray2);
                    }

                    //Node's Clean Up (Replace index with [{0}] for string.format( ) )
                    else
                    {
                        //HR: 2018.12.18: Fix the command0 issue.
                        //lineStruct.currentNodesCleaned[i] = lineStruct.currentNodes[i].Replace("[", "[{").Replace("]", "}]").Replace("1", "0");
                        if (lineStruct.currentNodes[i].Contains("[") && lineStruct.currentNodes[i].Contains("]"))
                        {
                            lineStruct.currentNodesCleaned[i] = lineStruct.currentNodes[i].Replace("[", "[{").Replace("]", "}]").Replace("1", "0");
                        }
                        lineStruct.currentNodesCleaned[i] = lineStruct.currentNodes[i].Replace("[", "[{").Replace("]", "}]");

                    }
                }

                int currentNodeMatchCount = 0, previousNodeMatchCount = 0;

                //If parents matches:   1:Node appended   2. Peer node
                if (lineStruct.currentNodes[0] == lineStruct.previousNodes[0])
                {
                    //If new node contains the previous node ... then it will be appended
                    if (lineStruct.structText.Contains(previousPartOfStruct))
                    {
                        lineStruct.nodeStatus = NodeStatus.AppendedNode;

                        for (int i = 0; i < lineStruct.currentNodes.Count; i++)
                        {
                            currentNodeMatchCount++;

                            if (i < lineStruct.previousNodes.Count())
                            {
                                if (lineStruct.currentNodes[i] == lineStruct.previousNodes[i])
                                {
                                    previousNodeMatchCount++;
                                }
                            }
                        }
                    }

                    //If new node didn't contain the previous node && second last item are same for both ... then it will be peer node
                    else if (lineStruct.currentNodes.AsEnumerable().Reverse().Skip(1).FirstOrDefault() ==
                        lineStruct.previousNodes.AsEnumerable().Reverse().Skip(1).FirstOrDefault())
                    {
                        lineStruct.nodeStatus = NodeStatus.PeerNode;

                        for (int i = 0; i < lineStruct.currentNodes.Count; i++)
                        {
                            currentNodeMatchCount++;

                            if (i < lineStruct.previousNodes.Count())
                            {
                                if (lineStruct.currentNodes[i] == lineStruct.previousNodes[i])
                                {
                                    previousNodeMatchCount++;
                                }
                            }
                        }
                    }

                    //If new node didn't contain the previous node && second last item are not same for both ... then it will be previous node
                    else
                    {
                        lineStruct.nodeStatus = NodeStatus.PreviousNode;

                        for (int i = 0; i < lineStruct.currentNodes.Count; i++)
                        {
                            currentNodeMatchCount++;

                            if (i < lineStruct.previousNodes.Count())
                            {
                                if (lineStruct.currentNodes[i] == lineStruct.previousNodes[i])
                                {
                                    previousNodeMatchCount++;
                                }
                            }
                        }
                    }
                }

                //If both didn't have same parent, then it be new node
                else
                {
                    lineStruct.nodeStatus = NodeStatus.NewNode;

                    for (int i = 0; i < lineStruct.currentNodes.Count; i++)
                    {
                        currentNodeMatchCount++;

                        if (i < lineStruct.previousNodes.Count())
                        {
                            if (lineStruct.currentNodes[i] == lineStruct.previousNodes[i])
                            {
                                previousNodeMatchCount++;
                            }
                        }

                        //HR:2018.12.18 All structs should be added.
                        listOfStructs.Add(lineStruct.currentNodes[i]);
                    }
                }
                
                //Debugging...
                if (lineStruct.fullLineText == "GVL_HMI.stControlCommand1,ST_ControlCommand,")
                {

                }

                //Testing the NodeType
                string[] arrays = new string[] { "ARRAY" };

                string[] dataTypes = new string[] { "BOOL", "INT", "TIME", "E_ErrorPiton", "STRING", "UINT", "SINT", "LREAL", "WORD", "DWORD", "UDINT", "DINT", "BYTE", "LINT"/*, "E_CarrierStationState"*/ };

                //Skip the headings (with Text "ARRAY")
                if (lineStruct.dataTypeText.ContainsAny(arrays, StringComparison.Ordinal))
                {
                    string textBwBrackets = lineStruct.dataTypeText.Split('[', ']')[1];

                    int arrayCount = 0;

                    int.TryParse(textBwBrackets.Replace("..", ".").Split('.')[1], out arrayCount);              //TODO: Take arrayCount globally

                    string structName = lineStruct.dataTypeText.Split(new string[] { "OF" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    if ((!structName.ContainsAny(dataTypes, StringComparison.Ordinal) && !listOfStructsTypes.Contains(structName)))
                    {
                        listOfStructsTypes.Add(structName.Trim());
                    }

                    continue;
                }
                
                //Strcutures
                else if (lineStruct.dataTypeText.ContainsAny(listOfStructsTypes, StringComparison.Ordinal))
                {
                    string structNameCleaned = RemoveBetween(lineStruct.structText, '[', ']', isArray: out isArray2);

                    if (listOfStructs.Exists(x => x == structNameCleaned))
                    {
                        isStructReplicated = true;
                        structIndex++;
                    }
                    else
                    {
                        isStructReplicated = false;
                        structIndex = 0;

                        listOfStructs.Add(structNameCleaned);
                        previousStruct = lineStruct.lastNodeCleaned;
                    }

                    lineStruct.nodeType = NodeType.Structure;
                    lineStruct.nodeDatatype = "struct";

                    lineStruct.previousChildNumbers = childNumber;
                    childNumber = 0;
                }
                
                else if ((lineStruct.nodeStatus == NodeStatus.PreviousNode && !lineStruct.currentNodes.AsEnumerable().Reverse().Skip(1).FirstOrDefault().Contains("[")))
                {
                    string structNameCleaned = lineStruct.currentNodes.AsEnumerable().Reverse().Skip(1).FirstOrDefault();

                    if (listOfStructs.Exists(x => x == structNameCleaned))
                    {
                        isStructReplicated = true;
                        structIndex++;
                    }
                    else
                    {
                        isStructReplicated = false;
                        structIndex = 0;

                        listOfStructs.Add(structNameCleaned);
                        previousStruct = lineStruct.lastNodeCleaned;
                    }

                    lineStruct.nodeType = NodeType.Structure;
                    lineStruct.nodeDatatype = "struct";

                    lineStruct.nodeStatus = NodeStatus.PreviousNodeWithPreviousStruct;

                    childNumber = 0;
                    lineStruct.previousChildNumbers = childNumber;
                }

                //Properties
                else if (lineStruct.dataTypeText.ContainsAny(dataTypes, StringComparison.Ordinal))
                {
                    string structNameCleaned = lineStruct.currentNodes.AsEnumerable().Reverse().Skip(1).FirstOrDefault();

                    if (listOfStructs.Exists(x => x == structNameCleaned))
                    {

                    }
                    else
                    {
                        listOfStructs.Add(structNameCleaned);
                    }

                    if (isArray)
                    {
                        lineStruct.isArray = true;
                    }
                    else
                    {
                        lineStruct.isArray = false;
                    }

                    if (lineStruct.lastNodeCleaned == previousArray)
                    {
                        isArrayReplicated = true;
                        arrayIndex++;
                    }
                    else
                    {
                        isArrayReplicated = false;
                        arrayIndex = 0;
                        
                        previousArray = lineStruct.lastNodeCleaned;
                    }

                    lineStruct.nodeType = NodeType.DataType;

                    if (lineStruct.dataTypeText == "BOOL")
                    {
                        lineStruct.nodeDatatype = "bool";
                    }
                    else if (lineStruct.dataTypeText == "INT")
                    {
                        lineStruct.nodeDatatype = "int";
                    }
                    else if (lineStruct.dataTypeText == "TIME")
                    {
                        lineStruct.nodeDatatype = "int";
                    }
                    else if (lineStruct.dataTypeText == "E_ErrorPiton")
                    {
                        lineStruct.nodeDatatype = "int";
                    }
                    else if (lineStruct.dataTypeText.Contains("STRING"))
                    {
                        lineStruct.nodeDatatype = "string";
                    }

                    else if (lineStruct.dataTypeText == "UINT")
                    {
                        lineStruct.nodeDatatype = "uint";
                    }
                    else if (lineStruct.dataTypeText == "SINT")
                    {
                        lineStruct.nodeDatatype = "int";
                    }
                    else if (lineStruct.dataTypeText == "LREAL")
                    {
                        lineStruct.nodeDatatype = "double";
                    }
                    else if (lineStruct.dataTypeText == "WORD")
                    {
                        lineStruct.nodeDatatype = "ushort";
                    }
                    else if (lineStruct.dataTypeText == "DWORD")
                    {
                        lineStruct.nodeDatatype = "uint";
                    }
                    else if (lineStruct.dataTypeText == "UDINT")
                    {
                        lineStruct.nodeDatatype = "double";
                    }
                    else if (lineStruct.dataTypeText == "DINT")
                    {
                        lineStruct.nodeDatatype = "double";
                    }
                    else if (lineStruct.dataTypeText == "BYTE")
                    {
                        lineStruct.nodeDatatype = "byte";
                    }
                    else if (lineStruct.dataTypeText == "LINT")
                    {
                        lineStruct.nodeDatatype = "int";
                    }
                    else
                    {
                        lineStruct.nodeDatatype = "int";
                    }

                    childNumber = childNumber + 1;
                    lineStruct.previousChildNumbers = childNumber;
                }

                else
                {
                    lineStruct.nodeType = NodeType.NotConfirmed;
                    lineStruct.nodeDatatype = "int";

                    childNumber = childNumber + 1;
                    lineStruct.previousChildNumbers = childNumber;
                }

                lineStruct.nodeLevel = currentNodeMatchCount;

                lineStruct.isStructReplicated = isStructReplicated;
                lineStruct.structIndex = structIndex;

                lineStruct.isArrayReplicated = isArrayReplicated;
                lineStruct.arrayIndex = arrayIndex;

                Debug.WriteLine(
                    lineStruct.lineNumber.ToString().PadRight(5) +
                    "cm:" + currentNodeMatchCount.ToString().PadRight(3) + " pm:"+ previousNodeMatchCount.ToString().PadRight(3) +
                    "s:" + lineStruct.nodeStatus.ToString().PadRight(12) + " t:"+ lineStruct.nodeType.ToString().PadRight(12) +
                    "structR:" + lineStruct.isStructReplicated.ToString().PadRight(7) + " arrayR:" + lineStruct.isArrayReplicated.ToString().PadRight(7) + "isArray:" + lineStruct.isArray.ToString().PadRight(10) +
                    "n:" + lineStruct.lastNode.ToString().PadRight(20) + "cm:" + lineStruct.structText+ "cNum:" + lineStruct.previousChildNumbers);

                previousPartOfStruct = lineStruct.structText;

                listOfLineStructs.Add(lineStruct);

            }
        }

        public void FilteringInformation()
        {
            //Adding Namespace and Class code ...
            FullText =
                "using System;" + Environment.NewLine + Environment.NewLine +

                "namespace PLCApi" + Environment.NewLine +
                "{" + Environment.NewLine +
                "    public partial class PLCTypes : PLCSymbols" + Environment.NewLine +
                "    {" + Environment.NewLine;

            for (int i = 0; i < listOfLineStructs.Count; i++)
            {
                //Handle the NotConfirmed case
                if (listOfLineStructs[i].nodeType == NodeType.NotConfirmed)
                {
                    if (i < listOfLineStructs.Count - 1)
                    {
                        if (listOfLineStructs[i + 1].nodeStatus == NodeStatus.AppendedNode)
                        {
                            //It's not INT (It should be struct)
                            listOfLineStructs[i].nodeType = NodeType.Structure;
                            listOfLineStructs[i].nodeDatatype = "struct";
                        }
                    }

                }

                //Debugging ...
                if (listOfLineStructs[i].dataTypeText.Contains("FB_"))
                {

                }

                switch (listOfLineStructs[i].nodeStatus)
                {
                    case NodeStatus.NewNode:

                        LineStruct lineStruct = (LineStruct)listOfLineStructs[i].Clone();

                        for (int j = 0; j < listOfLineStructs[i].currentNodes.Count; j++)
                        {
                            if (j < listOfLineStructs[i].currentNodes.Count - 1)
                            {
                                listOfLineStructs[i].nodeType = NodeType.Structure;
                                listOfLineStructs[i].nodeDatatype = "struct";
                            }
                            else
                            {
                                listOfLineStructs[i].nodeType = lineStruct.nodeType;
                                listOfLineStructs[i].nodeDatatype = lineStruct.nodeDatatype;
                            }
                            WriteIntoCodeFile(listOfLineStructs[i], listOfLineStructs[i].nodeDatatype + " " + listOfLineStructs[i].currentNodesCleaned[j], j + 1);
                        }

                        break;
                    case NodeStatus.AppendedNode:
                        WriteIntoCodeFile(listOfLineStructs[i]);
                        break;
                    case NodeStatus.PeerNode:
                        WriteIntoCodeFile(listOfLineStructs[i]);
                        break;
                    case NodeStatus.PreviousNode:
                        WriteIntoCodeFile(listOfLineStructs[i]);
                        break;
                    case NodeStatus.PreviousNodeWithPreviousStruct:
                        WriteIntoCodeFile(listOfLineStructs[i], listOfLineStructs[i].nodeDatatype + " " + listOfLineStructs[i].currentNodesCleaned.AsEnumerable().Reverse().Skip(1).FirstOrDefault(), listOfLineStructs[i].nodeLevel - 1);
                        break;
                    case NodeStatus.None:
                        break;
                    default:
                        break;

                }
            }

            //Closing Brackets
            int bracketsToMoveBack = prevousNodeLevel + 1 - 0;          //Cater the last struct ...

            for (int i = bracketsToMoveBack; i > 0; i--)
            {
                var extraEmptySpacing = "";
                for (int j = 1; j < i; j++)
                {
                    extraEmptySpacing += "    ";
                }

                FullText += extraEmptySpacing + "}" + Environment.NewLine;
            }
        }

        private void WriteIntoCodeFile(LineStruct lineStruct, string nodeInformation = "", int nodeLevel = -1)
        {
            if (lineStruct.lastNodeCleaned == "stCarr")
            {

            }


            //No need to write the next index of same struct in the file again
            if (lineStruct.isStructReplicated)
            {
                return;
            }

            //No need to write the next index of same array in the file again
            if (lineStruct.isArray && lineStruct.isArrayReplicated)
            {
                return;
            }

            // Use defaults if not explicilty defined
            string nodeDatatypePlusNodeName = string.Empty;

            if (lineStruct.isArray)
            {
                nodeDatatypePlusNodeName = (nodeInformation == "") ? lineStruct.nodeDatatype + "[] " + lineStruct.lastNodeCleaned : nodeInformation;
            }
            else
            {
                nodeDatatypePlusNodeName = (nodeInformation == "") ? lineStruct.nodeDatatype + " " + lineStruct.lastNodeCleaned : nodeInformation;
            }

            // Use defaults if not explicilty defined
            lineStruct.nodeLevel = (nodeLevel == -1) ? lineStruct.nodeLevel : nodeLevel;

            //Empty Space / Node Level Calculation
            string emptySpacing = string.Empty;
            for (int i = 0; i < lineStruct.nodeLevel * 4 + 4; i++)            //VS uses 4 spaces for indentation
            {
                Console.Write(" ");                     //For Console Debugging
                emptySpacing += " ";                    //For File Writing 
            }

            Console.WriteLine(nodeDatatypePlusNodeName);

            //Closing Brackets Calculation (If previous node has no child: Just insert a single closing bracket)
            if (lineStruct.nodeLevel == 1 && lineStruct.previousChildNumbers == 0)
            {
                FullText += emptySpacing + "    " + "}" + Environment.NewLine;
            }

            //Closing Brackets Calculation
            int bracketsToMoveBack = prevousNodeLevel - lineStruct.nodeLevel;
            for (int i = bracketsToMoveBack; i > 0; i--)
            {
                var extraEmptySpacing = string.Empty;
                for (int j = 1; j < i; j++)
                {
                    extraEmptySpacing += "    ";
                }
                FullText += emptySpacing + extraEmptySpacing + "}" + Environment.NewLine;
            }

            //Populate Structures
            if (lineStruct.nodeType == NodeType.Structure)
            {
                FullText += emptySpacing + "public " + nodeDatatypePlusNodeName + Environment.NewLine;
                FullText += emptySpacing + "{" + Environment.NewLine;
                FullText += emptySpacing + "    public int index;" + Environment.NewLine;
                FullText += emptySpacing + "    public int arrayIndex;" + Environment.NewLine;
            }

            //Populate Arrays
            else if (lineStruct.isArray)
            {
                string structtext1 = lineStruct.currentNodesCleaned.Last();
                string structtext2 = String.Join(".", lineStruct.currentNodesCleaned.ToArray());
                FullText += emptySpacing + "public void "+structtext1+" (int i, bool value)" + Environment.NewLine;
                FullText += emptySpacing + "{" + Environment.NewLine;
                FullText += emptySpacing + "    string str = string.Format(\"" + structtext2 + "" + "[{1}]\", index, i);" + Environment.NewLine;
                FullText += emptySpacing + "    symbol = m_plcSymbols[str];" + Environment.NewLine;
                FullText += emptySpacing + "    Client.WriteSymbol(symbol, value.ToString());" + Environment.NewLine;
                FullText += emptySpacing + "}" + Environment.NewLine;
            }

            //Populate Properties
            else
            {

                FullText += emptySpacing + "public " + nodeDatatypePlusNodeName + Environment.NewLine;
                FullText += emptySpacing + "{" + Environment.NewLine;

                //Populating Getter

                FullText += emptySpacing + "    get" + Environment.NewLine;
                FullText += emptySpacing + "    {" + Environment.NewLine;

                string getterText = String.Join(".", lineStruct.currentNodesCleaned.ToArray());
                switch (lineStruct.nodeDatatype)
                {
                    case "string":
                        FullText += emptySpacing + "        string str = string.Format(\""+getterText+"\", index);" + Environment.NewLine;
                        FullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        FullText += emptySpacing + "        return Client.ReadSymbol(symbol).ToString();" + Environment.NewLine;
                        break;
                    case "bool":
                        FullText += emptySpacing + "        string str = string.Format(\""+getterText+"\", index);" + Environment.NewLine;
                        FullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        FullText += emptySpacing + "        return Convert.ToBoolean(Client.ReadSymbol(symbol));" + Environment.NewLine;
                        break;
                    case "int":
                        FullText += emptySpacing + "        string str = string.Format(\""+getterText+"\", index);" + Environment.NewLine;
                        FullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        FullText += emptySpacing + "        return Convert.ToInt32(Client.ReadSymbol(symbol));" + Environment.NewLine;
                        break;
                    case "uint":
                        FullText += emptySpacing + "        string str = string.Format(\""+getterText+"\", index);" + Environment.NewLine;
                        FullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        FullText += emptySpacing + "        return Convert.ToUInt32(Client.ReadSymbol(symbol));" + Environment.NewLine;
                        break;
                    case "double":
                        FullText += emptySpacing + "        string str = string.Format(\""+getterText+"\", index);" + Environment.NewLine;
                        FullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        FullText += emptySpacing + "        return Convert.ToDouble(Client.ReadSymbol(symbol));" + Environment.NewLine;
                        break;
                    case "ushort":
                        FullText += emptySpacing + "        string str = string.Format(\""+getterText+"\", index);" + Environment.NewLine;
                        FullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        FullText += emptySpacing + "        return Convert.ToUInt16(Client.ReadSymbol(symbol));" + Environment.NewLine;
                        break;
                    case "byte":
                        FullText += emptySpacing + "        string str = string.Format(\""+getterText+"}\", index);" + Environment.NewLine;
                        FullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        FullText += emptySpacing + "        return Convert.ToByte(Client.ReadSymbol(symbol));" + Environment.NewLine;
                        break;
                    default:
                        break;
                }
                FullText += emptySpacing + "    }" + Environment.NewLine;

                //Populating Setter

                FullText += emptySpacing + "    set" + Environment.NewLine;
                FullText += emptySpacing + "    {" + Environment.NewLine;

                string setterText = String.Join(".", lineStruct.currentNodesCleaned.ToArray());
                switch (lineStruct.nodeDatatype)
                {
                    case "string":
                        FullText += emptySpacing + "        string str = string.Format(\""+setterText+"\", index);" + Environment.NewLine;
                        FullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        FullText += emptySpacing + "        Client.WriteSymbol(symbol, value.ToString());" + Environment.NewLine;
                        break;
                    case "bool":
                        FullText += emptySpacing + "        string str = string.Format(\""+setterText+"\", index);" + Environment.NewLine;
                        FullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        FullText += emptySpacing + "        Client.WriteSymbol(symbol, value.ToString());" + Environment.NewLine;
                        break;
                    case "int":
                        FullText += emptySpacing + "        string str = string.Format(\""+setterText+"\", index);" + Environment.NewLine;
                        FullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        FullText += emptySpacing + "        Client.WriteSymbol(symbol, value.ToString());" + Environment.NewLine;
                        break;
                    default:
                        FullText += emptySpacing + "        string str = string.Format(\"" + setterText + "\", index);" + Environment.NewLine;
                        FullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        FullText += emptySpacing + "        Client.WriteSymbol(symbol, value.ToString());" + Environment.NewLine;
                        break;
                }
                FullText += emptySpacing + "    }" + Environment.NewLine;

                FullText += emptySpacing + "}" + Environment.NewLine;
            }

            prevousNodeLevel = lineStruct.nodeLevel;
        }

        private string RemoveBetween(string s, char begin, char end, out bool isArray)
        {
            if (s.Contains("[") && s.Contains("]"))
            {
                isArray = true;
            }
            else
            {
                isArray = false;
            }

            Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
            //return regex.Replace(s, string.Empty);
            return new Regex(" +").Replace(regex.Replace(s, string.Empty), " ");
        }

        public enum NodeStatus
        {
            NewNode,
            AppendedNode,
            PeerNode,
            PreviousNode,
            PreviousNodeWithPreviousStruct,
            None
        }

        public enum NodeType
        {
            Heading,
            Structure,
            DataType,
            NotConfirmed,
            None
        }

    }
    
    public class LineStruct : ICloneable
    {
        public string fullLineText;
        public string structText;
        public string dataTypeText;
        public PLC_Symbol_Parser_App.ArrayParser.NodeStatus nodeStatus;
        public PLC_Symbol_Parser_App.ArrayParser.NodeType nodeType;
        public List<string> currentNodes;
        public List<string> previousNodes;
        public List<string> currentNodesCleaned;
        public List<string> previousNodesCleaned;
        public string lastNode;
        public string lastNodeCleaned;
        public int nodeLevel;
        public string nodeDatatype;
        public bool isStructReplicated;
        internal int structIndex;
        internal int arrayIndex;
        internal bool isArrayReplicated;
        internal string lastNodeWithoutCleanUp;
        internal bool isArray;
        internal bool isNodeReplicated;
        internal int lineNumber;
        internal int previousChildNumbers;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public static class StringExtensions
    {
        public static bool ContainsAny(this string input, IEnumerable<string> containsKeywords, StringComparison comparisonType)
        {
            return containsKeywords.Any(keyword => input.IndexOf(keyword, comparisonType) >= 0);
        }
    }

}
