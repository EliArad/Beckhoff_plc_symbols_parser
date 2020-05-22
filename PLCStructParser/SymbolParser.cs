using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PLC_Struct_Parser_App.SymbolParser;

namespace PLC_Struct_Parser_App
{
    public class SymbolParser
    {
        static List<LineStruct> listOfLineStructs = new List<LineStruct>();
        static string previousPartOfStruct = string.Empty;
        private static int prevousNodeLevel = 0;
        static string fullText = string.Empty;

        public string FullText { get => fullText; set => fullText = value; }

        public void ExtractLineInformation(List<string> lines)
        {

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
                    lastNode = string.Empty,
                    nodeLevel = 0,
                    nodeDatatype = string.Empty
                };

                lineStruct.fullLineText = line;

                if (line == "GVL_HMI.stControlCommand,ST_ControlCommand,")
                {

                }

                //Make sure there is two parts ... 0:nodes 1:dataType
                if (lineStruct.fullLineText.Split(',').Length < 2)
                {
                    return;
                }

                lineStruct.structText = lineStruct.fullLineText.Split(',')[0];
                lineStruct.dataTypeText = lineStruct.fullLineText.Split(',')[1];

                lineStruct.currentNodes = (lineStruct.structText.Split('.')).ToList();

                lineStruct.previousNodes = (previousPartOfStruct.Split('.')).ToList();

                lineStruct.currentNodes.Insert(0, "STPLC");         //Insert STPLC into each nodes' list
                lineStruct.previousNodes.Insert(0, "STPLC");         //Insert STPLC into each nodes' list

                lineStruct.lastNode = lineStruct.currentNodes.Last();

                int currentNodeMatchCount = 0, previousNodeMatchCount = 0;

                //If parents matches:   1:Node appended   2. Peer node
                if (lineStruct.currentNodes[1] == lineStruct.previousNodes[1])      //Ignore STPLC ... take index 1 as parent
                {
                    //If new node contains the previous node ... then it will be appended
                    if (lineStruct.structText.Contains(previousPartOfStruct))
                    {

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

                        //Algorithm Update: 2018.10.12: Append Node can contain more than 1 element. 
                        lineStruct.nodeStatus = NodeStatus.AppendedNode;

                        if (lineStruct.currentNodes.Count > lineStruct.previousNodes.Count + 1)
                        {
                            lineStruct.nodeStatus = NodeStatus.AppendedNode2;
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

                        //Algorithm Update: 2018.10.12: Previous Node can contain more than 1 element. 
                        if (currentNodeMatchCount > previousNodeMatchCount + 1)
                        {
                            lineStruct.nodeStatus = NodeStatus.PreviousNode2;
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
                    }
                }

                lineStruct.nodeLevel = currentNodeMatchCount;
                //lineStruct.nodeLevel++;                             //To cater STPLC node;

                //Testing the NodeType
                string[] structures = new string[] { "ST_", "DUT_", "FB_" };
                string[] dataTypes = new string[] { "BOOL", "LREAL", "WORD", "DWORD", "STRING", "INT", "UINT", "BYTE" };

                string[] structuresSkips = new string[] { "^" };
                string[] dataTypeSkips = new string[] { "POINTER TO" };

                if (lineStruct.structText.ContainsAny(structuresSkips, StringComparison.Ordinal))
                {
                    //Skip this item
                    continue;
                }
                if (lineStruct.dataTypeText.ContainsAny(dataTypeSkips, StringComparison.Ordinal))
                {
                    //Skip this item
                    continue;
                }
                else if (lineStruct.dataTypeText.ContainsAny(dataTypes, StringComparison.Ordinal))
                {
                    lineStruct.nodeType = NodeType.DataType;

                    if (lineStruct.dataTypeText.Contains("STRING"))
                    {
                        lineStruct.nodeDatatype = "string";
                    }
                    else if (lineStruct.dataTypeText == "BOOL")
                    {
                        lineStruct.nodeDatatype = "bool";
                    }
                    else if (lineStruct.dataTypeText == "INT")
                    {
                        lineStruct.nodeDatatype = "int";
                    }
                    else if (lineStruct.dataTypeText == "UINT")
                    {
                        lineStruct.nodeDatatype = "uint";
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
                    else if (lineStruct.dataTypeText == "TIME")
                    {
                        lineStruct.nodeDatatype = "int";
                    }
                    else if (lineStruct.dataTypeText == "BYTE")
                    {
                        lineStruct.nodeDatatype = "byte";

                    }

                }
                else if (lineStruct.dataTypeText.ContainsAny(structures, StringComparison.Ordinal))
                {
                    lineStruct.nodeType = NodeType.Structure;
                    lineStruct.nodeDatatype = "struct";

                    //Algorithm Update: 2018.10.13: Remove the struct that contains the array
                    if (listOfLineStructs.Count > 0)                    //Ignore the First Element
                    {
                        //Detection: Both (Last and Current) have same struct type. Both have same node level
                        if (listOfLineStructs.Last().nodeType == NodeType.Structure && listOfLineStructs.Last().nodeLevel == lineStruct.nodeLevel)
                        {
                            listOfLineStructs.RemoveAt(listOfLineStructs.Count - 1);
                            lineStruct.nodeStatus = NodeStatus.NewNode;
                        }
                    }
                }
                else
                {
                    lineStruct.nodeType = NodeType.NotConfirmed;
                    lineStruct.nodeDatatype = "int";
                }



                Debug.WriteLine(
                    "cm:" + currentNodeMatchCount.ToString().PadRight(5) + "pm:" + previousNodeMatchCount.ToString().PadRight(5) +
                    "s:" + lineStruct.nodeStatus.ToString().PadRight(15) + "t:" + lineStruct.nodeType.ToString().PadRight(15) +
                    "n:" + lineStruct.lastNode.ToString().PadRight(20) + "nl:" + lineStruct.nodeLevel.ToString().PadRight(5) + "cm:" + lineStruct.structText);

                previousPartOfStruct = lineStruct.structText;

                listOfLineStructs.Add(lineStruct);

            }
        }

        public void FilteringInformation()
        {
            //Adding Namespace and Class code ...
            FullText =
                "using System;" + Environment.NewLine +
                "namespace PLCApi" + Environment.NewLine +
                "{" + Environment.NewLine +
                "    public partial class PLCTypes : PLCSymbols" + Environment.NewLine +
                "    {" + Environment.NewLine +
                "        public struct STPLC" + Environment.NewLine +
                "        {" + Environment.NewLine;

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

                    //Debug.WriteLine("===================================================");
                    //Debug.WriteLine(
                    //$"cm:{i.ToString().PadRight(5)} pm:{i.ToString().PadRight(5)} " +
                    //$"s:{listOfLineStructs[i].nodeStatus.ToString().PadRight(15)} t:{listOfLineStructs[i].nodeType.ToString().PadRight(15)} " +
                    //$"n:{listOfLineStructs[i].lastNode.ToString().PadRight(20)} nl:{listOfLineStructs[i].nodeLevel.ToString().PadRight(5)} cm:{listOfLineStructs[i].structText} ");

                }

                //Debugging ...
                if (listOfLineStructs[i].dataTypeText.Contains("FB_"))
                {

                }

                if (listOfLineStructs[i].structText.Contains("GVL_AC_Motors.fbUnload"))
                {

                }

                LineStruct lineStruct;

                switch (listOfLineStructs[i].nodeStatus)
                {
                    case NodeStatus.NewNode:

                        lineStruct = (LineStruct)listOfLineStructs[i].Clone();

                        for (int j = 1; j < listOfLineStructs[i].currentNodes.Count; j++)
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
                            WriteIntoCodeFile(listOfLineStructs[i], listOfLineStructs[i].nodeDatatype + " " + listOfLineStructs[i].currentNodes[j], j + 1);
                        }

                        break;
                    case NodeStatus.AppendedNode:
                        WriteIntoCodeFile(listOfLineStructs[i]);
                        break;
                    case NodeStatus.AppendedNode2:
                        //Algorithm Update: 2018.10.12: Append Node can contain more than 1 element. 
                        lineStruct = (LineStruct)listOfLineStructs[i].Clone();
                        lineStruct.currentNodes.RemoveAt(lineStruct.currentNodes.Count - 1);
                        lineStruct.nodeType = NodeType.Structure;
                        WriteIntoCodeFile(lineStruct, "struct " + lineStruct.currentNodes.Last(), lineStruct.nodeLevel - 1);
                        WriteIntoCodeFile(listOfLineStructs[i]);
                        break;

                    case NodeStatus.PeerNode:
                        WriteIntoCodeFile(listOfLineStructs[i]);
                        break;
                    case NodeStatus.PreviousNode:
                        WriteIntoCodeFile(listOfLineStructs[i]);
                        break;

                    case NodeStatus.PreviousNode2:
                        //Algorithm Update: 2018.10.12: Previous Node can contain more than 1 element. 
                        lineStruct = (LineStruct)listOfLineStructs[i].Clone();
                        lineStruct.currentNodes.RemoveAt(lineStruct.currentNodes.Count - 1);
                        lineStruct.nodeType = NodeType.Structure;
                        WriteIntoCodeFile(lineStruct, "struct " + lineStruct.currentNodes.Last(), lineStruct.nodeLevel - 1);
                        WriteIntoCodeFile(listOfLineStructs[i]);
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

        public void WriteIntoCodeFile(LineStruct lineStruct, string nodeInformation = "", int nodeLevel = -1)
        {
            // Use defaults if not explicilty defined
            string nodeDatatypePlusNodeName = (nodeInformation == "") ? lineStruct.nodeDatatype + " " + lineStruct.lastNode : nodeInformation;

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

            //Populate Structs and Properties
            if (lineStruct.nodeType == NodeType.Structure)
            {
                FullText += emptySpacing + "public " + nodeDatatypePlusNodeName + Environment.NewLine;
                FullText += emptySpacing + "{" + Environment.NewLine;
            }
            else
            {
                //Populating Getter

                if (lineStruct.nodeType == NodeType.NotConfirmed)
                {
                    FullText += emptySpacing + @"//Datatype yet to be confirmed" + Environment.NewLine;
                }

                FullText += emptySpacing + "public static " + nodeDatatypePlusNodeName + Environment.NewLine;
                FullText += emptySpacing + "{" + Environment.NewLine;
                FullText += emptySpacing + "    get" + Environment.NewLine;
                FullText += emptySpacing + "    {" + Environment.NewLine;

                string propertyText = "";
                switch (lineStruct.nodeDatatype)
                {
                    case "string":
                        propertyText = String.Join(".", lineStruct.currentNodes.Skip(1).ToArray());
                        FullText += emptySpacing + "        symbol = m_plcSymbols[\"" + propertyText + "\"];" + Environment.NewLine;
                        FullText += emptySpacing + "        return Client.ReadSymbol(symbol).ToString();" + Environment.NewLine;
                        break;
                    case "bool":
                        propertyText = String.Join(".", lineStruct.currentNodes.Skip(1).ToArray());
                        FullText += emptySpacing + "        symbol = m_plcSymbols[\"" + propertyText + "\"];" + Environment.NewLine;
                        FullText += emptySpacing + "        return Convert.ToBoolean(Client.ReadSymbol(symbol));" + Environment.NewLine;
                        break;
                    case "int":
                        propertyText = String.Join(".", lineStruct.currentNodes.Skip(1).ToArray());
                        FullText += emptySpacing + "        symbol = m_plcSymbols[\"" + propertyText + "\"];" + Environment.NewLine;
                        FullText += emptySpacing + "        return Convert.ToInt32(Client.ReadSymbol(symbol));" + Environment.NewLine;
                        break;
                    case "uint":
                        propertyText = String.Join(".", lineStruct.currentNodes.Skip(1).ToArray());
                        FullText += emptySpacing + "        symbol = m_plcSymbols[\"" + propertyText + "\"];" + Environment.NewLine;
                        FullText += emptySpacing + "        return Convert.ToUInt32(Client.ReadSymbol(symbol));" + Environment.NewLine;
                        break;
                    case "double":
                        propertyText = String.Join(".", lineStruct.currentNodes.Skip(1).ToArray());
                        FullText += emptySpacing + "        symbol = m_plcSymbols[\"" + propertyText + "\"];" + Environment.NewLine;
                        FullText += emptySpacing + "        return Convert.ToDouble(Client.ReadSymbol(symbol));" + Environment.NewLine;
                        break;
                    case "ushort":
                        propertyText = String.Join(".", lineStruct.currentNodes.Skip(1).ToArray());
                        FullText += emptySpacing + "        symbol = m_plcSymbols[\"" + propertyText + "\"];" + Environment.NewLine;
                        FullText += emptySpacing + "        return Convert.ToUInt16(Client.ReadSymbol(symbol));" + Environment.NewLine;
                        break;
                    case "byte":
                        propertyText = String.Join(".", lineStruct.currentNodes.Skip(1).ToArray());
                        FullText += emptySpacing + "        symbol = m_plcSymbols[\"" + propertyText + "\"];" + Environment.NewLine;
                        FullText += emptySpacing + "        return Convert.ToByte(Client.ReadSymbol(symbol));" + Environment.NewLine;
                        break;
                    default:
                        break;
                }
                FullText += emptySpacing + "    }" + Environment.NewLine;

                //Populating Setter
                FullText += emptySpacing + "    set" + Environment.NewLine;
                FullText += emptySpacing + "    {" + Environment.NewLine;
                switch (lineStruct.nodeDatatype)
                {
                    case "string":
                        propertyText = String.Join(".", lineStruct.currentNodes.Skip(1).ToArray());
                        FullText += emptySpacing + "        symbol = m_plcSymbols[\"" + propertyText + "\"];" + Environment.NewLine;
                        FullText += emptySpacing + "        Client.WriteSymbol(symbol, value.ToString());" + Environment.NewLine;
                        break;
                    case "bool":
                        propertyText = String.Join(".", lineStruct.currentNodes.Skip(1).ToArray());
                        FullText += emptySpacing + "        symbol = m_plcSymbols[\"" + propertyText + "\"];" + Environment.NewLine;
                        FullText += emptySpacing + "        Client.WriteSymbol(symbol, value.ToString());" + Environment.NewLine;
                        break;
                    case "int":
                        propertyText = String.Join(".", lineStruct.currentNodes.Skip(1).ToArray());
                        FullText += emptySpacing + "        symbol = m_plcSymbols[\"" + propertyText + "\"];" + Environment.NewLine;
                        FullText += emptySpacing + "        Client.WriteSymbol(symbol, value.ToString());" + Environment.NewLine;
                        break;
                    default:
                        propertyText = String.Join(".", lineStruct.currentNodes.Skip(1).ToArray());
                        FullText += emptySpacing + "        symbol = m_plcSymbols[\"" + propertyText + "\"];" + Environment.NewLine;
                        FullText += emptySpacing + "        Client.WriteSymbol(symbol, value.ToString());" + Environment.NewLine;
                        break;
                }
                FullText += emptySpacing + "    }" + Environment.NewLine;
                FullText += emptySpacing + "}" + Environment.NewLine;
            }

            prevousNodeLevel = lineStruct.nodeLevel;
        }

        #region Enums

        public enum NodeStatus
        {
            NewNode,
            AppendedNode,
            AppendedNode2,
            PeerNode,
            PreviousNode,
            None,
            PreviousNode2
        }

        public enum NodeType
        {
            Structure,
            DataType,
            NotConfirmed,
            None
        }

        #endregion
    }

    #region Classes

    public class LineStruct : ICloneable
    {
        public string fullLineText;
        public string structText;
        public string dataTypeText;
        public NodeStatus nodeStatus;
        public NodeType nodeType;
        public List<string> currentNodes;
        public List<string> previousNodes;
        public string lastNode;
        public int nodeLevel;
        public string nodeDatatype;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }


    #endregion

    #region Extended Classes

    public static class StringExtensions
    {
        public static bool ContainsAny(this string input, IEnumerable<string> containsKeywords, StringComparison comparisonType)
        {
            return containsKeywords.Any(keyword => input.IndexOf(keyword, comparisonType) >= 0);
        }
    }

    #endregion
}
