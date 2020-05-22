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
    public class SymbolArrayParser
    {
        #region Fields
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
        static List<string> listOfAllStructs = new List<string>();
        private int childNumber; 
        #endregion

        public string Start(string filePathWithName, string outputFileName)
        {
            //try
            //{
            FileCleaningAndArraySorting(filePathWithName);
            ExtractLineInformation(listOfSortedLines);
            FilteringInformation();

            //Saving File
            string filePath = Path.Combine(Environment.CurrentDirectory, outputFileName);
            File.WriteAllText(filePath, fullText);

            return "ok";
            //}
            //catch (Exception ex)
            //{
            //    return ex.ToString();
            //}
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
                              where line.Contains("GVL")
                              where !Detect2DArray(line)
                              select line;

            listOfSortedLines = filterLines.ToList();
            listOfSortedLines.Sort();

            string filePath1 = Path.Combine(Environment.CurrentDirectory, textFilePath.Replace(".txt", "_sorted.txt"));
            File.WriteAllText(filePath1, string.Join(Environment.NewLine, listOfSortedLines));
        }

        private void ExtractLineInformation(List<string> lines)
        {
            int lineCounter = 0;

            foreach (string line in lines)
            {
                
                #region Intialize LineStruct with default values
                LineStruct lineStruct = new LineStruct
                {
                    fullLineText = string.Empty,
                    structText = string.Empty,
                    dataTypeText = string.Empty,
                    nodeStatus = NodeStatus.None,
                    nodeType = NodeType.None,
                    currentNodes = new List<string>(),
                    previousNodes = new List<string>(),
                    currentNodesFormatted = new List<string>(),
                    previousNodesFormatted = new List<string>(),
                    currentNodesWithoutIndexes = new List<string>(),
                    previousNodesWithoutIndexes = new List<string>(),
                    lastNode = string.Empty,
                    lastNodeFormatted = string.Empty,
                    nodeLevel = 0,
                    nodeDatatype = string.Empty,
                    isStructReplicated = false,
                    lineNumber = 0,
                    previousChildNumbers = 0,
                    matchedItemsCount = 0,
                    isArray = false
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

                lineStruct.currentNodesFormatted = (lineStruct.structText.Split('.')).ToList();
                lineStruct.previousNodesFormatted = (previousPartOfStruct.Split('.')).ToList();

                lineStruct.currentNodesWithoutIndexes = (lineStruct.structText.Split('.')).ToList();
                lineStruct.previousNodesWithoutIndexes = (previousPartOfStruct.Split('.')).ToList();

                bool isArray = false;
                bool isArray2 = false;

                #endregion

                #region Node's Formatting & CleanUp

                //Node's Formatting: Making [i] to [{i}]
                lineStruct.lastNode = lineStruct.currentNodes.Last();
                lineStruct.lastNodeFormatted = RemoveBetween(lineStruct.currentNodes.Last(), '[', ']', out isArray);

                for (int i = 0; i < lineStruct.currentNodes.Count; i++)
                {
                    //Node's Formatting: Making [i] to [{i}] only to last element
                    if (i == lineStruct.currentNodes.Count - 1)
                    {
                        lineStruct.currentNodesFormatted[i] = RemoveBetween(lineStruct.currentNodes[i], '[', ']', isArray: out isArray2);
                    }
                    //Node's Clean Up: Replace index with [{0}] for string.format( )
                    else
                    {
                        //2018.12.18: Bug Fixed: command0 issue.
                        //lineStruct.currentNodesFormatted[i] = lineStruct.currentNodes[i].Replace("[", "[{").Replace("]", "}]").Replace("1", "0");

                        if (lineStruct.currentNodes[i].Contains("[") && lineStruct.currentNodes[i].Contains("]"))
                        {
                            lineStruct.currentNodesFormatted[i] = lineStruct.currentNodes[i].Replace("[", "[{").Replace("]", "}]").Replace("1", "0");
                        }

                        lineStruct.currentNodesFormatted[i] = lineStruct.currentNodes[i].Replace("[", "[{").Replace("]", "}]");
                    }
                }
                
                for (int i = 0; i < lineStruct.previousNodes.Count; i++)
                {
                    lineStruct.previousNodesWithoutIndexes[i] = RemoveBetween(lineStruct.previousNodes[i], '[', ']', isArray: out isArray2).Replace("[", "").Replace("]", "");
                }

                for (int i = 0; i < lineStruct.currentNodes.Count; i++)
                {
                    lineStruct.currentNodesWithoutIndexes[i] = RemoveBetween(lineStruct.currentNodes[i], '[', ']', isArray: out isArray2).Replace("[", "").Replace("]", "");
                }

                #endregion

                #region Compare Current Nodes with Previous Nodes

                int currentNodeMatchCount = 0, previousNodeMatchCount = 0;
                int currentNodeWithoutIndexMatchCount = 0, previousNodeWithoutIndexMatchCount = 0;

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

                for (int i = 0; i < lineStruct.currentNodesWithoutIndexes.Count; i++)
                {
                    currentNodeWithoutIndexMatchCount++;

                    if (i < lineStruct.previousNodes.Count())
                    {
                        if (lineStruct.currentNodesWithoutIndexes[i] == lineStruct.previousNodesWithoutIndexes[i])
                        {
                            previousNodeWithoutIndexMatchCount++;
                        }
                    }
                }

                lineStruct.currentNodeMatchCount = currentNodeMatchCount;
                lineStruct.previousNodeMatchCount = previousNodeMatchCount;

                lineStruct.currentNodeWithoutIndexMatchCount = currentNodeWithoutIndexMatchCount;
                lineStruct.previousNodeWithoutIndexMatchCount = previousNodeWithoutIndexMatchCount;

                #endregion

                if (lineStruct.fullLineText == "GVL_Automation.fbLoadConv.fbLifterIn.pis_SideHolder.b_CloseLoop,ARRAY [0..2] OF BOOL,")
                {

                }
                if (lineStruct.fullLineText == "GVL_Automation.stCarr,ARRAY [0..6] OF E_CarrierStationState,")
                {

                }

                if (lineStruct.lineNumber == 1175)
                {

                }

                //2018.12.25: Verify the CurrentNodes against AllStructs
                var currentNodesWithIndexes = lineStruct.currentNodesWithoutIndexes;
                string currentNodesStrWithoutIndexes = string.Join(".", currentNodesWithIndexes);
                if (listOfAllStructs.Exists(x => x.Contains(currentNodesStrWithoutIndexes)))
                {
                    lineStruct.isAllStructReplicated = true;
                }
                else
                {
                    listOfAllStructs.Add(currentNodesStrWithoutIndexes);
                }

                #region NodeStatus

                //If parents matches:   1:Node appended   2. Peer node
                if (lineStruct.currentNodes[0] == lineStruct.previousNodes[0])
                {

                    //2018.12.24 Not every contains(previousNode) is appended node
                    var nodes = lineStruct.currentNodesWithoutIndexes;
                    var lastNode = lineStruct.currentNodesWithoutIndexes[nodes.Count - 1];
                    nodes.Remove(lastNode);
                    string nodesStrWithoutIndexes = string.Join(".", nodes);

                    if (listOfStructs.Exists(x => x.Contains(nodesStrWithoutIndexes)))
                    {
                        lineStruct.nodeStatus = NodeStatus.PeerNodeWithSameParents;
                        lineStruct.isAllStructReplicated = true;
                    }

                    //If new node contains the previous node ... then it will be appended
                    else if (lineStruct.structText.Contains(previousPartOfStruct))
                    {
                        lineStruct.nodeStatus = NodeStatus.AppendedNodeWithSameParents;
                    }

                    //2018.12.20: Node match count updated [May be Fix required]
                    //If new node didn't contain the previous node && second last item are same for both ... then it will be peer node
                    else if (lineStruct.currentNodes.AsEnumerable().Reverse().Skip(1).FirstOrDefault() == lineStruct.previousNodes.AsEnumerable().Reverse().Skip(1).FirstOrDefault() &&
                        currentNodeMatchCount - 1 == previousNodeMatchCount)
                    {
                        lineStruct.nodeStatus = NodeStatus.PeerNodeWithSameParents;
                    }

                    //2018.12.15  PreviousNodeWithDifferentParents Condtion added.
                    //If new node didn't contain the previous node && second last item are not same for both ... then it will be previous node
                    else if (lineStruct.previousNodeMatchCount > 1 && lineStruct.lineNumber != 1)
                    {
                        //2018.12.18 Further condition added
                        //If all parents elements are same except the last one, then they have same parents
                        if (lineStruct.previousNodes.Intersect(lineStruct.currentNodes).Count() == lineStruct.currentNodes.Count() - 1)
                        {
                            lineStruct.nodeStatus = NodeStatus.PreviousNodeWithSameParents;
                        }
                        else
                        {
                            lineStruct.nodeStatus = NodeStatus.PreviousNodeWithDifferentParents;
                            lineStruct.nodeLevel = previousNodeMatchCount;
                        }
                    }

                    //2018.12.28: Only Top Parent Matched case
                    else if (lineStruct.previousNodeMatchCount == 1)
                    {
                        lineStruct.nodeStatus = NodeStatus.PreviousNodeWithDifferentParents;
                        lineStruct.nodeLevel = previousNodeMatchCount;
                    }

                    //[May be Fix required]
                    else
                    {
                        lineStruct.nodeStatus = NodeStatus.NotFound;
                    }
                }

                //If both didn't have same parent, then it be new node
                else
                {
                    lineStruct.nodeStatus = NodeStatus.NewParent;
                } 

                #endregion

                //Testing the NodeType
                string[] arrays = new string[] { "ARRAY" };

                string[] dataTypes = new string[] { "BOOL", "INT", "TIME", "E_ErrorPiton", "STRING", "UINT", "SINT", "LREAL", "WORD", "DWORD", "UDINT", "DINT", "BYTE", "LINT"};

                //Skip the headings (with Text "ARRAY")
                if (lineStruct.dataTypeText.ContainsAny(arrays, StringComparison.Ordinal))
                {
                    string textBwBrackets = lineStruct.dataTypeText.Split('[', ']')[1];

                    int arrayCount = 0;

                    int.TryParse(textBwBrackets.Replace("..", ".").Split('.')[1], out arrayCount);              //TODO: Take arrayCount globally

                    //HR: 2018.12.13 Fixed the Index Exception
                    if (lineStruct.dataTypeText.Split(new string[] { "OF" }, StringSplitOptions.RemoveEmptyEntries).Count() >= 2)
                    {
                        string structName = lineStruct.dataTypeText.Split(new string[] { "OF" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        if ((!structName.ContainsAny(dataTypes, StringComparison.Ordinal) && !listOfStructsTypes.Contains(structName)))
                        {
                            listOfStructsTypes.Add(structName.Trim());
                        }

                        if (lineStruct.nodeStatus == NodeStatus.PreviousNodeWithDifferentParents)
                        {
                            for (int i = previousNodeMatchCount; i < lineStruct.currentNodes.Count; i++)
                            {
                                listOfStructsTypes.Add(lineStruct.currentNodes[i].Trim());
                            }
                        }
                        
                    }
                    continue;
                }

                //Strcutures
                else if (lineStruct.dataTypeText.ContainsAny(listOfStructsTypes, StringComparison.Ordinal))
                {
                    //HR: 2018.12.24: Should take the clean struct without indexes.
                    //string structNameCleaned = RemoveBetween(lineStruct.structText, '[', ']', isArray: out isArray2);
                    string structNameCleaned = RemoveBetween(lineStruct.structText, '[', ']', isArray: out isArray2).Replace("[", "").Replace("]", "");

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

                        //2018.12.25: Also add it to the listOfAllStructs
                        var nodes = lineStruct.currentNodesWithoutIndexes;
                        string nodesStrWithoutIndexes = string.Join(".", nodes);
                        listOfAllStructs.Add(nodesStrWithoutIndexes);

                        previousStruct = lineStruct.lastNodeFormatted;
                    }

                    //2018.12.16 & 2018.12.21
                    var joinedAllStruct = string.Join(",", lineStruct.currentNodes);
                    string joinedAllStructCleaned = RemoveBetween(joinedAllStruct, '[', ']', isArray: out isArray2);
                    if (listOfAllStructs.Exists(x => x == joinedAllStructCleaned))
                    {
                        lineStruct.isAllStructReplicated = true;
                    }
                    else
                    {
                        lineStruct.isAllStructReplicated = false;
                        listOfAllStructs.Add(joinedAllStructCleaned);

                        //2018.12.20 Commented
                        if (lineStruct.lineNumber != 1)
                        {
                            lineStruct.nodeStatus = NodeStatus.PreviousNodeWithDifferentParents;
                        }
                    }

                    lineStruct.nodeType = NodeType.Structure;
                    lineStruct.nodeDatatype = "struct";

                    lineStruct.previousChildNumbers = childNumber;
                    childNumber = 0;
                }

                //Properties
                else if (lineStruct.dataTypeText.ContainsAny(dataTypes, StringComparison.Ordinal))
                {
                    //string structNameCleaned = lineStruct.currentNodes.AsEnumerable().Reverse().Skip(1).FirstOrDefault();
                    //2018.12.25 Properites should also be cleaned
                    //string structNameCleaned = lineStruct.currentNodes.AsEnumerable().Reverse().FirstOrDefault();
                    string node = lineStruct.currentNodes.AsEnumerable().Reverse().FirstOrDefault();
                    string structNameCleaned = RemoveBetween(node, '[', ']', isArray: out isArray2).Replace("[", "").Replace("]", "");

                    if (listOfStructs.Exists(x => x == structNameCleaned))
                    {
                        
                    }
                    else
                    {
                        //2018.12.25: 
                        isStructReplicated = false;
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

                    if (lineStruct.lastNodeFormatted == previousArray)
                    {
                        isArrayReplicated = true;
                        arrayIndex++;
                    }
                    else
                    {
                        isArrayReplicated = false;
                        arrayIndex = 0;

                        previousArray = lineStruct.lastNodeFormatted;
                    }

                    #region NodeType
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
                    #endregion

                    childNumber = childNumber + 1;
                    lineStruct.previousChildNumbers = childNumber;
                }

                else if ((lineStruct.nodeStatus == NodeStatus.PreviousNodeWithSameParents && !lineStruct.currentNodes.AsEnumerable().Reverse().Skip(1).FirstOrDefault().Contains("[")))
                {
                    //2018.12.25 Properites should also be cleaned
                    //string structNameCleaned = lineStruct.currentNodes.AsEnumerable().Reverse().Skip(1).FirstOrDefault();
                    string node = lineStruct.currentNodes.AsEnumerable().Reverse().Skip(1).FirstOrDefault();
                    string structNameCleaned = RemoveBetween(node, '[', ']', isArray: out isArray2).Replace("[", "").Replace("]", "");
                    
                    if (listOfStructs.Exists(x => x == structNameCleaned))
                    {
                        isStructReplicated = true;
                        structIndex++;

                        //2018.12.25
                        var dump = lineStruct.currentNodesWithoutIndexes;
                        var last = lineStruct.currentNodesWithoutIndexes[dump.Count - 1];
                        dump.Remove(last);
                        //COMMENTED
                        //var joinedAllStruct = string.Join(",", lineStruct.currentNodes);
                        var joinedAllStruct = string.Join(".", dump);
                        if (listOfAllStructs.Exists(x => x == joinedAllStruct))
                        {
                            lineStruct.isAllStructReplicated = true;
                        }
                        else
                        {
                            lineStruct.isAllStructReplicated = false;
                            listOfAllStructs.Add(joinedAllStruct);

                            //2018.12.20 Commented
                            if (lineStruct.lineNumber != 1)
                            {
                                lineStruct.nodeStatus = NodeStatus.PreviousNodeWithDifferentParents;
                            }
                        }

                    }
                    else
                    {
                        isStructReplicated = false;
                        structIndex = 0;

                        listOfStructs.Add(structNameCleaned);

                        //2018.12.25: Also add it to the listOfAllStructs
                        var nodes = lineStruct.currentNodesWithoutIndexes;
                        string nodesStrWithoutIndexes = string.Join(".", nodes);
                        listOfAllStructs.Add(nodesStrWithoutIndexes);

                        previousStruct = lineStruct.lastNodeFormatted;
                    }

                    lineStruct.nodeType = NodeType.Structure;
                    lineStruct.nodeDatatype = "struct";

                    //2018.12.20: Commented
                    //lineStruct.nodeStatus = NodeStatus.PreviousNodeWithDifferentParents;

                    childNumber = 0;
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

                #region Debugging Information on Output
                //2018.12.13 & 2018.12.24 Padding Increased
                Debug.WriteLine(
                    lineStruct.lineNumber.ToString().PadRight(8) +
                    "cm:" + currentNodeMatchCount.ToString().PadRight(3) + " pm:" + previousNodeMatchCount.ToString().PadRight(3) + " nL:" + lineStruct.nodeLevel.ToString().PadRight(3) +
                    "s:" + lineStruct.nodeStatus.ToString().PadRight(35) + " t:" + lineStruct.nodeType.ToString().PadRight(14) +
                    "structR:" + lineStruct.isStructReplicated.ToString().PadRight(7) + " structR:" + lineStruct.isAllStructReplicated.ToString().PadRight(7) + "isArray:" + lineStruct.isArray.ToString().PadRight(10) +
                    "n:" + lineStruct.lastNode.ToString().PadRight(35) + "cm:" + lineStruct.structText.PadRight(50) /*+ "cNum:" + lineStruct.previousChildNumbers*/); 
                #endregion

                previousPartOfStruct = lineStruct.structText;

                listOfLineStructs.Add(lineStruct);

            }
        }

        private void FilteringInformation()
        {
            #region Adding Namespace and Class code ...
            fullText =
                    "using System;" + Environment.NewLine + Environment.NewLine +

                    "namespace PLCApi" + Environment.NewLine +
                    "{" + Environment.NewLine +
                    "    public partial class PLCTypes : PLCSymbols" + Environment.NewLine +
                    "    {" + Environment.NewLine; 
            #endregion

            for (int i = 0; i < listOfLineStructs.Count; i++)
            {
                #region Handle the NotConfirmed case
                if (listOfLineStructs[i].nodeType == NodeType.NotConfirmed)
                {
                    if (i < listOfLineStructs.Count - 1)
                    {
                        if (listOfLineStructs[i + 1].nodeStatus == NodeStatus.AppendedNodeWithSameParents)
                        {
                            //It's not INT (It should be struct)
                            listOfLineStructs[i].nodeType = NodeType.Structure;
                            listOfLineStructs[i].nodeDatatype = "struct";
                        }
                    }

                } 
                #endregion

                switch (listOfLineStructs[i].nodeStatus)
                {
                    case NodeStatus.NewParent:

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
                            WriteIntoCodeFile(listOfLineStructs[i], listOfLineStructs[i].nodeDatatype + " " + listOfLineStructs[i].currentNodesFormatted[j], j + 1);
                        }

                        break;
                    case NodeStatus.AppendedNodeWithSameParents:
                        WriteIntoCodeFile(listOfLineStructs[i]);
                        break;
                    case NodeStatus.PeerNodeWithSameParents:
                        WriteIntoCodeFile(listOfLineStructs[i]);
                        break;
                    case NodeStatus.PreviousNodeWithSameParents:
                        WriteIntoCodeFile(listOfLineStructs[i]);
                        //WriteIntoCodeFile(listOfLineStructs[i], listOfLineStructs[i].nodeDatatype + " " + listOfLineStructs[i].currentNodesFormatted.AsEnumerable().Reverse().Skip(1).FirstOrDefault(), listOfLineStructs[i].nodeLevel/* - 1*/);
                        break;
                    case NodeStatus.PreviousNodeWithDifferentParents:
                        //WriteIntoCodeFile(listOfLineStructs[i], listOfLineStructs[i].nodeDatatype + " " + listOfLineStructs[i].currentNodesFormatted.AsEnumerable().Reverse().Skip(1).FirstOrDefault(), listOfLineStructs[i].nodeLevel - 1);
                        LineStruct lineStruct2 = (LineStruct)listOfLineStructs[i].Clone();

                        for (int j = listOfLineStructs[i].previousNodeMatchCount; j < listOfLineStructs[i].currentNodes.Count; j++)
                        {
                            if (j < listOfLineStructs[i].currentNodes.Count - 1)
                            {
                                listOfLineStructs[i].nodeType = NodeType.Structure;
                                listOfLineStructs[i].nodeDatatype = "struct";
                            }
                            else
                            {
                                listOfLineStructs[i].nodeType = lineStruct2.nodeType;
                                listOfLineStructs[i].nodeDatatype = lineStruct2.nodeDatatype;
                            }
                            WriteIntoCodeFile(listOfLineStructs[i], listOfLineStructs[i].nodeDatatype + " " + listOfLineStructs[i].currentNodesFormatted[j], j + 1);
                        }

                        break;
                    case NodeStatus.None:
                        break;
                    default:
                        break;

                }
            }

            #region Closing Brackets
            int bracketsToMoveBack = prevousNodeLevel + 1 - 0;          //Cater the last struct ...

            for (int i = bracketsToMoveBack; i > 0; i--)
            {
                var extraEmptySpacing = "";
                for (int j = 1; j < i; j++)
                {
                    extraEmptySpacing += "    ";
                }

                fullText += extraEmptySpacing + "}" + Environment.NewLine;
            } 
            #endregion
        }

        private void WriteIntoCodeFile(LineStruct lineStruct, string nodeInformation = "", int nodeLevel = -1)
        {
            #region Debugging
            if (lineStruct.fullLineText == "GVL_Automation.fbLoadConv.fbLifterIn.pis_SideHolder.b_CloseLoop,ARRAY [0..2] OF BOOL,")
            {

            }
            if (lineStruct.lastNodeFormatted == "stCarr")
            {

            }
            if (lineStruct.lineNumber == 164)
            {

            }
            #endregion


            //No need to write the next index of same struct in the file again
            if (lineStruct.isStructReplicated && lineStruct.isAllStructReplicated)
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
                nodeDatatypePlusNodeName = (nodeInformation == "") ? lineStruct.nodeDatatype + "[] " + lineStruct.lastNodeFormatted : nodeInformation;
            }
            else
            {
                nodeDatatypePlusNodeName = (nodeInformation == "") ? lineStruct.nodeDatatype + " " + lineStruct.lastNodeFormatted : nodeInformation;
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
            //HR: 2018.12.14 First Line will not have any previous nodes ...
            if (lineStruct.nodeLevel == 1 && lineStruct.previousChildNumbers == 0 && lineStruct.lineNumber != 1)
            {
                fullText += emptySpacing + "    " + "}" + Environment.NewLine;
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
                fullText += emptySpacing + extraEmptySpacing + "}" + Environment.NewLine;
            }

            //Populate Structures
            if (lineStruct.nodeType == NodeType.Structure)
            {
                fullText += emptySpacing + "public " + nodeDatatypePlusNodeName + Environment.NewLine;
                fullText += emptySpacing + "{" + Environment.NewLine;
                fullText += emptySpacing + "    public int index;" + Environment.NewLine;
                fullText += emptySpacing + "    public int arrayIndex;" + Environment.NewLine;
            }

            //Populate Arrays
            else if (lineStruct.isArray)
            {
                string structtext1 = lineStruct.currentNodesFormatted.Last();
                string structtext2 = String.Join(".", lineStruct.currentNodesFormatted.ToArray());
                fullText += emptySpacing + "public void " + structtext1 + " (int i, bool value)" + Environment.NewLine;
                fullText += emptySpacing + "{" + Environment.NewLine;
                fullText += emptySpacing + "    string str = string.Format(\"" + structtext2 + "" + "[{1}]\", index, i);" + Environment.NewLine;
                fullText += emptySpacing + "    symbol = m_plcSymbols[str];" + Environment.NewLine;
                fullText += emptySpacing + "    Client.WriteSymbol(symbol, value.ToString());" + Environment.NewLine;
                fullText += emptySpacing + "}" + Environment.NewLine;
            }

            //Populate Properties
            else
            {

                fullText += emptySpacing + "public " + nodeDatatypePlusNodeName + Environment.NewLine;
                fullText += emptySpacing + "{" + Environment.NewLine;

                //Populating Getter

                fullText += emptySpacing + "    get" + Environment.NewLine;
                fullText += emptySpacing + "    {" + Environment.NewLine;

                //2018.12.28: Fix the index issue.
                //string getterText = String.Join(".", lineStruct.currentNodesFormatted.ToArray()).Replace("1", "0");
                string getterText = String.Join(".", lineStruct.currentNodesFormatted.ToArray()).Replace("1", "0");
                switch (lineStruct.nodeDatatype)
                {
                    case "string":
                        fullText += emptySpacing + "        string str = string.Format(\"" + getterText + "\", index);" + Environment.NewLine;
                        fullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        fullText += emptySpacing + "        return Client.ReadSymbol(symbol).ToString();" + Environment.NewLine;
                        break;
                    case "bool":
                        fullText += emptySpacing + "        string str = string.Format(\"" + getterText + "\", index);" + Environment.NewLine;
                        fullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        fullText += emptySpacing + "        return Convert.ToBoolean(Client.ReadSymbol(symbol));" + Environment.NewLine;
                        break;
                    case "int":
                        fullText += emptySpacing + "        string str = string.Format(\"" + getterText + "\", index);" + Environment.NewLine;
                        fullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        fullText += emptySpacing + "        return Convert.ToInt32(Client.ReadSymbol(symbol));" + Environment.NewLine;
                        break;
                    case "uint":
                        fullText += emptySpacing + "        string str = string.Format(\"" + getterText + "\", index);" + Environment.NewLine;
                        fullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        fullText += emptySpacing + "        return Convert.ToUInt32(Client.ReadSymbol(symbol));" + Environment.NewLine;
                        break;
                    case "double":
                        fullText += emptySpacing + "        string str = string.Format(\"" + getterText + "\", index);" + Environment.NewLine;
                        fullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        fullText += emptySpacing + "        return Convert.ToDouble(Client.ReadSymbol(symbol));" + Environment.NewLine;
                        break;
                    case "ushort":
                        fullText += emptySpacing + "        string str = string.Format(\"" + getterText + "\", index);" + Environment.NewLine;
                        fullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        fullText += emptySpacing + "        return Convert.ToUInt16(Client.ReadSymbol(symbol));" + Environment.NewLine;
                        break;
                    case "byte":
                        fullText += emptySpacing + "        string str = string.Format(\"" + getterText + "}\", index);" + Environment.NewLine;
                        fullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        fullText += emptySpacing + "        return Convert.ToByte(Client.ReadSymbol(symbol));" + Environment.NewLine;
                        break;
                    default:
                        break;
                }
                fullText += emptySpacing + "    }" + Environment.NewLine;

                //Populating Setter

                fullText += emptySpacing + "    set" + Environment.NewLine;
                fullText += emptySpacing + "    {" + Environment.NewLine;

                //2018.12.28: Fix the index issue.
                //string setterText = String.Join(".", lineStruct.currentNodesFormatted.ToArray());
                string setterText = String.Join(".", lineStruct.currentNodesFormatted.ToArray()).Replace("1", "0");
                switch (lineStruct.nodeDatatype)
                {
                    case "string":
                        fullText += emptySpacing + "        string str = string.Format(\"" + setterText + "\", index);" + Environment.NewLine;
                        fullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        fullText += emptySpacing + "        Client.WriteSymbol(symbol, value.ToString());" + Environment.NewLine;
                        break;
                    case "bool":
                        fullText += emptySpacing + "        string str = string.Format(\"" + setterText + "\", index);" + Environment.NewLine;
                        fullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        fullText += emptySpacing + "        Client.WriteSymbol(symbol, value.ToString());" + Environment.NewLine;
                        break;
                    case "int":
                        fullText += emptySpacing + "        string str = string.Format(\"" + setterText + "\", index);" + Environment.NewLine;
                        fullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        fullText += emptySpacing + "        Client.WriteSymbol(symbol, value.ToString());" + Environment.NewLine;
                        break;
                    default:
                        fullText += emptySpacing + "        string str = string.Format(\"" + setterText + "\", index);" + Environment.NewLine;
                        fullText += emptySpacing + "        symbol = m_plcSymbols[str];" + Environment.NewLine;
                        fullText += emptySpacing + "        Client.WriteSymbol(symbol, value.ToString());" + Environment.NewLine;
                        break;
                }
                fullText += emptySpacing + "    }" + Environment.NewLine;

                fullText += emptySpacing + "}" + Environment.NewLine;
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
            return new Regex(" +").Replace(regex.Replace(s, string.Empty), " ");
        }

        private bool Detect2DArray(string line)
        {
            if (!line.Contains("[") && !line.Contains("]"))
            {
                return false;
            }

            string textBetweenBrackets = line.Split('[', ']')[1];
            if (textBetweenBrackets.Contains(","))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region Enums
        public enum NodeStatus
        {
            NewParent,
            AppendedNodeWithSameParents,
            PeerNodeWithSameParents,
            PreviousNodeWithSameParents,
            PreviousNodeWithDifferentParents,
            None,
            NotFound
        }

        public enum NodeType
        {
            Heading,
            Structure,
            DataType,
            NotConfirmed,
            None
        } 
        #endregion

    }

    #region LineStruct & Extension Classes

    public class LineStruct : ICloneable
    {
        public string fullLineText;
        public string structText;
        public string dataTypeText;
        public PLC_Symbol_Parser_App.SymbolArrayParser.NodeStatus nodeStatus;
        public PLC_Symbol_Parser_App.SymbolArrayParser.NodeType nodeType;
        public List<string> currentNodes;
        public List<string> previousNodes;
        public List<string> currentNodesFormatted;
        public List<string> previousNodesFormatted;
        public string lastNode;
        public string lastNodeFormatted;
        public int nodeLevel;
        public string nodeDatatype;
        public bool isStructReplicated;
        public int structIndex;
        public int arrayIndex;
        public bool isArrayReplicated;
        public string lastNodeWithoutCleanUp;
        public bool isArray;
        public bool isNodeReplicated;
        public int lineNumber;
        public int previousChildNumbers;
        public int matchedItemsCount;
        public int currentNodeMatchCount;
        public int previousNodeMatchCount;
        public bool isAllStructReplicated;
        internal List<string> currentNodesWithoutIndexes;
        internal List<string> previousNodesWithoutIndexes;
        internal int currentNodeWithoutIndexMatchCount;
        internal int previousNodeWithoutIndexMatchCount;

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
    #endregion

}
