using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using TwinCAT.Ads;


namespace Sample06
{	
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TreeView treeViewSymbols;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbDatatype;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbIndexGroup;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbIndexOffset;
		private System.Windows.Forms.Button btnFindSymbol;
		private System.Windows.Forms.Button btnReadSymbolInfo;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbDatatypeId;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tbValue;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox tbSymbolname;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox tbSize;
		private System.Windows.Forms.Button btnWrite;		
		private System.Windows.Forms.TextBox tbName;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox cbFlat;
	
		private TcAdsSymbolInfoLoader symbolLoader;
		private TcAdsClient adsClient;
		private ITcAdsSymbol currentSymbol = null;

		public Form1()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.treeViewSymbols = new System.Windows.Forms.TreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbFlat = new System.Windows.Forms.CheckBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbDatatype = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbSize = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbIndexGroup = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbIndexOffset = new System.Windows.Forms.TextBox();
            this.btnFindSymbol = new System.Windows.Forms.Button();
            this.btnReadSymbolInfo = new System.Windows.Forms.Button();
            this.tbSymbolname = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbDatatypeId = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbValue = new System.Windows.Forms.TextBox();
            this.btnWrite = new System.Windows.Forms.Button();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewSymbols
            // 
            this.treeViewSymbols.Location = new System.Drawing.Point(8, 16);
            this.treeViewSymbols.Name = "treeViewSymbols";
            this.treeViewSymbols.Size = new System.Drawing.Size(887, 440);
            this.treeViewSymbols.TabIndex = 0;
            this.treeViewSymbols.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSymbols_AfterSelect);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbFlat);
            this.groupBox1.Controls.Add(this.btnLoad);
            this.groupBox1.Controls.Add(this.treeViewSymbols);
            this.groupBox1.Location = new System.Drawing.Point(8, 44);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(901, 491);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Symbols";
            // 
            // cbFlat
            // 
            this.cbFlat.Location = new System.Drawing.Point(120, 462);
            this.cbFlat.Name = "cbFlat";
            this.cbFlat.Size = new System.Drawing.Size(72, 24);
            this.cbFlat.TabIndex = 2;
            this.cbFlat.Text = "flat";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(8, 462);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(104, 23);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load Symbols";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(927, 140);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 23);
            this.label3.TabIndex = 17;
            this.label3.Text = "Datatype:";
            // 
            // tbDatatype
            // 
            this.tbDatatype.Location = new System.Drawing.Point(1007, 140);
            this.tbDatatype.Name = "tbDatatype";
            this.tbDatatype.ReadOnly = true;
            this.tbDatatype.Size = new System.Drawing.Size(144, 20);
            this.tbDatatype.TabIndex = 16;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(927, 108);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 23);
            this.label9.TabIndex = 15;
            this.label9.Text = "Size:";
            // 
            // tbSize
            // 
            this.tbSize.Location = new System.Drawing.Point(1007, 108);
            this.tbSize.Name = "tbSize";
            this.tbSize.ReadOnly = true;
            this.tbSize.Size = new System.Drawing.Size(144, 20);
            this.tbSize.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(927, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 23);
            this.label2.TabIndex = 13;
            this.label2.Text = "Index Group:";
            // 
            // tbIndexGroup
            // 
            this.tbIndexGroup.Location = new System.Drawing.Point(1007, 44);
            this.tbIndexGroup.Name = "tbIndexGroup";
            this.tbIndexGroup.ReadOnly = true;
            this.tbIndexGroup.Size = new System.Drawing.Size(144, 20);
            this.tbIndexGroup.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(927, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 23);
            this.label1.TabIndex = 11;
            this.label1.Text = "Index Offset:";
            // 
            // tbIndexOffset
            // 
            this.tbIndexOffset.Location = new System.Drawing.Point(1007, 76);
            this.tbIndexOffset.Name = "tbIndexOffset";
            this.tbIndexOffset.ReadOnly = true;
            this.tbIndexOffset.Size = new System.Drawing.Size(144, 20);
            this.tbIndexOffset.TabIndex = 10;
            // 
            // btnFindSymbol
            // 
            this.btnFindSymbol.Location = new System.Drawing.Point(120, 56);
            this.btnFindSymbol.Name = "btnFindSymbol";
            this.btnFindSymbol.Size = new System.Drawing.Size(104, 23);
            this.btnFindSymbol.TabIndex = 18;
            this.btnFindSymbol.Text = "Find Symbol";
            this.btnFindSymbol.Click += new System.EventHandler(this.btnFindSymbol_Click);
            // 
            // btnReadSymbolInfo
            // 
            this.btnReadSymbolInfo.Location = new System.Drawing.Point(8, 56);
            this.btnReadSymbolInfo.Name = "btnReadSymbolInfo";
            this.btnReadSymbolInfo.Size = new System.Drawing.Size(104, 23);
            this.btnReadSymbolInfo.TabIndex = 19;
            this.btnReadSymbolInfo.Text = "Read Symbol Info";
            this.btnReadSymbolInfo.Click += new System.EventHandler(this.btnReadSymbolInfo_Click);
            // 
            // tbSymbolname
            // 
            this.tbSymbolname.Location = new System.Drawing.Point(88, 24);
            this.tbSymbolname.Name = "tbSymbolname";
            this.tbSymbolname.Size = new System.Drawing.Size(368, 20);
            this.tbSymbolname.TabIndex = 20;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.btnReadSymbolInfo);
            this.groupBox2.Controls.Add(this.tbSymbolname);
            this.groupBox2.Controls.Add(this.btnFindSymbol);
            this.groupBox2.Location = new System.Drawing.Point(8, 541);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(480, 88);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Symbol Info";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 23);
            this.label4.TabIndex = 21;
            this.label4.Text = "Symbol Name:";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(927, 172);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 23);
            this.label5.TabIndex = 23;
            this.label5.Text = "Datatype Id:";
            // 
            // tbDatatypeId
            // 
            this.tbDatatypeId.Location = new System.Drawing.Point(1007, 172);
            this.tbDatatypeId.Name = "tbDatatypeId";
            this.tbDatatypeId.ReadOnly = true;
            this.tbDatatypeId.Size = new System.Drawing.Size(144, 20);
            this.tbDatatypeId.TabIndex = 22;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(927, 204);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 23);
            this.label6.TabIndex = 25;
            this.label6.Text = "Value:";
            // 
            // tbValue
            // 
            this.tbValue.Location = new System.Drawing.Point(1007, 204);
            this.tbValue.Name = "tbValue";
            this.tbValue.Size = new System.Drawing.Size(144, 20);
            this.tbValue.TabIndex = 24;
            // 
            // btnWrite
            // 
            this.btnWrite.Location = new System.Drawing.Point(927, 244);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(80, 23);
            this.btnWrite.TabIndex = 26;
            this.btnWrite.Text = "Write Value";
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(104, 9);
            this.tbName.Name = "tbName";
            this.tbName.ReadOnly = true;
            this.tbName.Size = new System.Drawing.Size(1047, 20);
            this.tbName.TabIndex = 27;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(24, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 23);
            this.label7.TabIndex = 28;
            this.label7.Text = "Name:";
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1163, 641);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnWrite);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbValue);
            this.Controls.Add(this.tbDatatypeId);
            this.Controls.Add(this.tbDatatype);
            this.Controls.Add(this.tbSize);
            this.Controls.Add(this.tbIndexGroup);
            this.Controls.Add(this.tbIndexOffset);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Sample06";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			try
			{
				adsClient = new TcAdsClient();
				adsClient.Connect(851);
				symbolLoader = adsClient.CreateSymbolInfoLoader();
			}
			catch(Exception err)
			{
				MessageBox.Show(err.Message);
			}
		}
		
		private void btnLoad_Click(object sender, System.EventArgs e)
		{						
			treeViewSymbols.Nodes.Clear();
			try
			{
				if( !cbFlat.Checked ) 
				{
					TcAdsSymbolInfo symbol = symbolLoader.GetFirstSymbol(true);
					while( symbol != null )
					{
						treeViewSymbols.Nodes.Add(CreateNewNode(symbol));
						symbol = symbol.NextSymbol;
					}	
				}
				else
				{
					foreach( TcAdsSymbolInfo symbol in symbolLoader )
					{
						TreeNode node = new TreeNode(symbol.Name);
						node.Tag = symbol;
						treeViewSymbols.Nodes.Add(node);
					}
				}
			}
			catch(Exception err)
			{
				MessageBox.Show(err.Message);
			}
		}

		private void btnReadSymbolInfo_Click(object sender, System.EventArgs e)
		{
			try
			{
				ITcAdsSymbol symbol = adsClient.ReadSymbolInfo(tbSymbolname.Text);
				if( symbol == null)
				{
					MessageBox.Show("Symbol " + tbSymbolname.Text + " not found");
					return;
				}
				SetSymbolInfo(symbol);
			}
			catch( Exception err )
			{
				MessageBox.Show(err.Message);
			}
		}		

		private void btnFindSymbol_Click(object sender, System.EventArgs e)
		{	
			try
			{			
				ITcAdsSymbol symbol = symbolLoader.FindSymbol(tbSymbolname.Text);
				if( symbol == null)
				{
					MessageBox.Show("Symbol " + tbSymbolname.Text + " not found");
					return;
				}
				SetSymbolInfo(symbol);
			}
			catch( Exception err )
			{
				MessageBox.Show(err.Message);
			}
		}	

		private void btnWrite_Click(object sender, System.EventArgs e)
		{	
			try
			{				
				if( currentSymbol != null )
					adsClient.WriteSymbol(currentSymbol, tbValue.Text);
			}
			catch(Exception err)
			{
				MessageBox.Show("Unable to write Value. " + err.Message);
			}
		}
		
		private void treeViewSymbols_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if( e.Node.Text.Length > 0 )
			{
				if( e.Node.Tag is TcAdsSymbolInfo )
				{
					SetSymbolInfo((ITcAdsSymbol)e.Node.Tag);
				}
			}
		}
		
		private TreeNode CreateNewNode(TcAdsSymbolInfo symbol)
		{
			TreeNode node = new TreeNode(symbol.Name);			
			node.Tag = symbol;
			TcAdsSymbolInfo subSymbol = symbol.FirstSubSymbol;
			while( subSymbol != null )
			{
				node.Nodes.Add(CreateNewNode(subSymbol));
				subSymbol = subSymbol.NextSymbol;
			}
			return node;
		}		

		private void SetSymbolInfo(ITcAdsSymbol symbol)
		{
			currentSymbol = symbol;
			tbName.Text = symbol.Name.ToString();
			tbIndexGroup.Text = symbol.IndexGroup.ToString();
			tbIndexOffset.Text = symbol.IndexOffset.ToString();
			tbSize.Text = symbol.Size.ToString();
			tbDatatype.Text = symbol.Type;
			tbDatatypeId.Text = symbol.Datatype.ToString();
			try
			{
				tbValue.Text = adsClient.ReadSymbol(symbol).ToString();
			}
			catch( AdsDatatypeNotSupportedException err )
			{
				tbValue.Text = err.Message;
			}
			catch(Exception err)
			{
				MessageBox.Show("Unable to read Symbol Info. " + err.Message); 
			}
		}
	}
}
