using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PLCApi;
using static PLCApi.PLCSymbols;
using System.Threading;
using System.IO;
using PLC_Symbol_Parser_App;

namespace PLCSymbolTestToolApp
{
    public partial class Form1 : Form , IPLCNotify
    {
        PLCSymbols m_plcSymbols;
      
        public Form1()
        {
            InitializeComponent();
            try
            {
                
                Control.CheckForIllegalCrossThreadCalls = false;
                m_plcSymbols = new PLCSymbols();
                m_plcSymbols.Connect(851);
              
            }
            catch(Exception err)
            {
                MessageBox.Show("Failed to connect to PLC:" + err.Message);
                return;
            }

            try
            {
                LoadFilters();
            }
            catch (Exception err)
            {

            }
            LoadAsyncSymbols();
            
        }
        Dictionary<string, int> m_notifyIndex = new Dictionary<string, int>();
        int m_notifyVarIndex;

        public void NotifyChanges(string varName)
        {
            if (m_notifyIndex.ContainsKey(varName) == false)
            {
                m_notifyIndex.Add(varName, m_notifyVarIndex++);
                object value;
                m_plcSymbols.ReadSymbol(varName, out value);
                listBox2.Items.Add(value.ToString());
            }
            else
            {
                int v = m_notifyIndex[varName];
                object value;
                m_plcSymbols.ReadSymbol(varName, out value);
                listBox2.Items[v] = value.ToString();
            }
         
        }

        bool FilterSymbols(List<string> list, string symbol)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (symbol.Contains(list[i]) == true)
                {
                    return true;
                }
            }
            return false;
        }
        void LoadSymbols(List<string> list)
        {
            lock (this)
            {
                m_plcSymbols.LoadSymbols3();
                m_plcSymbols.DumpSymbols();
                listBox1.Items.Clear();


                Dictionary<string, PLC_SYMBOLS> m_symbols2 = m_plcSymbols.GetAllSymbols2();
                foreach (KeyValuePair<string, PLC_SYMBOLS> p in m_symbols2)
                {
                    if (list.Count > 0 && FilterSymbols(list, p.Key) == false)
                        continue;
                    listBox1.Items.Add(p.Key);

                }
            }
        }
        private void btnWrite_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select symbol from the list");
                    return;
                }
                m_plcSymbols.WriteSymbol(listBox1.Items[listBox1.SelectedIndex].ToString(), tbValue.Text);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        void LoadAsyncSymbols()
        {
            try
            {
                Task task = new Task(() => {
                    txtSearchSymbol.Enabled = false;
                    lblSearch.Text = "Loading";
                    btnGeneratePLCTypes.Enabled = false;
                    var list = listFilterNames.Items.Cast<String>().ToList();
                    LoadSymbols(list);
                    btnGeneratePLCTypes.Enabled = true;
                    lblSearch.Text = "Search";
                    txtSearchSymbol.Enabled = true;
                });
                task.Start();
            }
            catch (Exception err)
            {
                MessageBox.Show("Failed to load symbols" + err.Message);
            }
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadAsyncSymbols();
        }

        void EmptySearchResults()
        {
            tbName.Text = string.Empty;
            tbIndexOffset.Text = string.Empty;
            tbIndexGroup.Text = string.Empty;
            tbDatatypeId.Text = string.Empty;
            tbSize.Text = string.Empty;
            tbSymbolname.Text = string.Empty;

        }
        void SetSearchResults(PLC_SYMBOLS symbol)
        {
            tbName.Text = symbol.Name;
            tbIndexOffset.Text = symbol.IndexOffset.ToString();
            tbIndexGroup.Text = symbol.IndexGroup.ToString();
            tbDatatypeId.Text = symbol.Datatype.ToString();
            tbDatatype.Text = symbol.Type.ToString();
            tbSize.Text = symbol.Size.ToString();
            tbSymbolname.Text = symbol.ShortName;
        }
        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SearchVar();
        }

        void SearchVar()
        {
            try
            {
                var list = listFilterNames.Items.Cast<String>().ToList();
                if (txtSearchSymbol.Text != string.Empty)
                {
                    listBox1.Items.Clear();
                    Dictionary<string, PLC_SYMBOLS> m_symbols2 = m_plcSymbols.GetAllSymbols2();
                    if (chkIgnoreCase.Checked == false)
                    {
                        foreach (KeyValuePair<string, PLC_SYMBOLS> p in m_symbols2)
                        {
                            if (list.Count > 0 && FilterSymbols(list, p.Key) == false)
                                continue;

                            if (p.Key.Contains(txtSearchSymbol.Text))
                            {
                                listBox1.Items.Add(p.Key);
                            }
                        }
                    }
                    else
                    {

                        foreach (KeyValuePair<string, PLC_SYMBOLS> p in m_symbols2)
                        {
                            if (FilterSymbols(list, p.Key) == false)
                                continue;

                            if (p.Key.Contains(txtSearchSymbol.Text, StringComparison.OrdinalIgnoreCase))
                            {
                                listBox1.Items.Add(p.Key);
                            }
                        }
                    }
                }
                else
                {
                    listBox1.Items.Clear();

                    Dictionary<string, PLC_SYMBOLS> m_symbols2 = m_plcSymbols.GetAllSymbols2();
                    foreach (KeyValuePair<string, PLC_SYMBOLS> p in m_symbols2)
                    {
                        listBox1.Items.Add(p.Key);
                    }

                }
            }
            catch (Exception)
            {
                txtSearchSymbol.ForeColor = Color.Red;
            }
        }

        private void btnFindSymbol_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedIndex >= 0)
                {
                    EmptySearchResults();
                    PLC_SYMBOLS symbol;
                    object Value;
                    m_plcSymbols.GetSymbol(listBox1.Items[listBox1.SelectedIndex].ToString(), out symbol, out Value);
                    SetSearchResults(symbol);
                    txtSearchSymbol.ForeColor = Color.Green;
                    tbValue.Text = Value.ToString();
                }
            }
            catch (Exception err)
            {

            }
        }

        private void chkIgnoreCase_CheckedChanged(object sender, EventArgs e)
        {
            SearchVar();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select symbol from the list");
                return;
            }
            try
            {
                object value;
                m_plcSymbols.ReadSymbol(listBox1.Items[listBox1.SelectedIndex].ToString(), out value);
                tbValue.Text = value.ToString();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        void ContinuesReading()
        {
            while (checkBox1.Checked == true)
            {
                if (listBox1.SelectedIndex == -1)
                {
                    checkBox1.Checked = false;
                    return;
                }
                try
                {
                    object value;
                    m_plcSymbols.ReadSymbol(listBox1.Items[listBox1.SelectedIndex].ToString(), out value);
                    tbValue.Text = value.ToString();
                }
                catch (Exception err)
                {
                    checkBox1.Checked = false;
                    return;
                }
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                var t = new Thread(ContinuesReading);
                t.Start();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            checkBox1.Checked = false;
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                checkBox1.Checked = false;
                return;
            }
            if (m_notifyIndex.ContainsKey(listBox1.Items[listBox1.SelectedIndex].ToString()) == false)
            {
                m_plcSymbols.AddNotification(this, listBox1.Items[listBox1.SelectedIndex].ToString(), "eee");
            }
        }

        private void openSymbolFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("plc_symbols.json");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnGeneratePLCTypes_Click(object sender, EventArgs e)
        {
            try
            {
                string textFilePath = "plc_symbols.txt";
                if (File.Exists(textFilePath) == true)
                {
                    File.Delete(textFilePath);
                }

                this.Cursor = Cursors.WaitCursor;
                btnGeneratePLCTypes.ForeColor = Color.Red;
                m_plcSymbols.CreateSymbolTypesFile3("STPLC");
                PLCSymbolParser.Start(txtPLCTypesFileName.Text);

                SymbolArrayParser symbolArrayParser = new SymbolArrayParser();
              
                string res = symbolArrayParser.Start(textFilePath, txtArraySymbolsOutput.Text);

                btnGeneratePLCTypes.ForeColor = Color.Green;
                this.Cursor = Cursors.Default;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void btnAddFilter_Click(object sender, EventArgs e)
        {
            if (txtFilterName.Text != string.Empty)
            {
                listFilterNames.Items.Add(txtFilterName.Text);
                SaveFilters();
                LoadAsyncSymbols();
            }
        }

        void SaveFilters()
        {
            if (listFilterNames.Items.Count == 0)
            {
                if (File.Exists("plc_filters.txt") == true)
                    File.Delete("plc_filters.txt");
                return;
            }
            for (int i = 0; i < listFilterNames.Items.Count; i++)
            {
                using (StreamWriter sw = new StreamWriter("plc_filters.txt"))
                {
                    sw.WriteLine(listFilterNames.Items[i].ToString());
                }
            }
        }

        void LoadFilters()
        {
            if (File.Exists("plc_filters.txt") == true)
            {
                listFilterNames.Items.Clear();
                using (StreamReader sw = new StreamReader("plc_filters.txt"))
                {
                    string line = sw.ReadLine();
                    listFilterNames.Items.Add(line);
                }
            }
            
        }
        private void btnRemoveFilter_Click(object sender, EventArgs e)
        {
            if (listFilterNames.SelectedIndex == -1)
                return;

            listFilterNames.Items.RemoveAt(listFilterNames.SelectedIndex);
            SaveFilters();
            LoadAsyncSymbols();
        }
    }
}
