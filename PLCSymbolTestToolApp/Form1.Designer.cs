namespace PLCSymbolTestToolApp
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnWrite = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tbValue = new System.Windows.Forms.TextBox();
            this.tbDatatypeId = new System.Windows.Forms.TextBox();
            this.tbDatatype = new System.Windows.Forms.TextBox();
            this.tbSize = new System.Windows.Forms.TextBox();
            this.tbIndexGroup = new System.Windows.Forms.TextBox();
            this.tbIndexOffset = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.txtSearchSymbol = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbSymbolname = new System.Windows.Forms.TextBox();
            this.chkIgnoreCase = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.label10 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSymbolFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnGeneratePLCTypes = new System.Windows.Forms.Button();
            this.txtPLCTypesFileName = new System.Windows.Forms.TextBox();
            this.listFilterNames = new System.Windows.Forms.ListBox();
            this.btnAddFilter = new System.Windows.Forms.Button();
            this.txtFilterName = new System.Windows.Forms.TextBox();
            this.btnRemoveFilter = new System.Windows.Forms.Button();
            this.txtArraySymbolsOutput = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(1054, 46);
            this.tbName.Name = "tbName";
            this.tbName.ReadOnly = true;
            this.tbName.Size = new System.Drawing.Size(384, 20);
            this.tbName.TabIndex = 42;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(974, 46);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 23);
            this.label7.TabIndex = 43;
            this.label7.Text = "Name:";
            // 
            // btnWrite
            // 
            this.btnWrite.ForeColor = System.Drawing.Color.Blue;
            this.btnWrite.Location = new System.Drawing.Point(1219, 234);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(83, 27);
            this.btnWrite.TabIndex = 41;
            this.btnWrite.Text = "Write Value";
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(974, 238);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 23);
            this.label6.TabIndex = 40;
            this.label6.Text = "Value:";
            // 
            // tbValue
            // 
            this.tbValue.Location = new System.Drawing.Point(1054, 238);
            this.tbValue.Name = "tbValue";
            this.tbValue.Size = new System.Drawing.Size(144, 20);
            this.tbValue.TabIndex = 39;
            // 
            // tbDatatypeId
            // 
            this.tbDatatypeId.Location = new System.Drawing.Point(1054, 206);
            this.tbDatatypeId.Name = "tbDatatypeId";
            this.tbDatatypeId.ReadOnly = true;
            this.tbDatatypeId.Size = new System.Drawing.Size(384, 20);
            this.tbDatatypeId.TabIndex = 37;
            // 
            // tbDatatype
            // 
            this.tbDatatype.Location = new System.Drawing.Point(1054, 174);
            this.tbDatatype.Name = "tbDatatype";
            this.tbDatatype.ReadOnly = true;
            this.tbDatatype.Size = new System.Drawing.Size(384, 20);
            this.tbDatatype.TabIndex = 35;
            // 
            // tbSize
            // 
            this.tbSize.Location = new System.Drawing.Point(1054, 142);
            this.tbSize.Name = "tbSize";
            this.tbSize.ReadOnly = true;
            this.tbSize.Size = new System.Drawing.Size(384, 20);
            this.tbSize.TabIndex = 33;
            // 
            // tbIndexGroup
            // 
            this.tbIndexGroup.Location = new System.Drawing.Point(1054, 78);
            this.tbIndexGroup.Name = "tbIndexGroup";
            this.tbIndexGroup.ReadOnly = true;
            this.tbIndexGroup.Size = new System.Drawing.Size(384, 20);
            this.tbIndexGroup.TabIndex = 31;
            // 
            // tbIndexOffset
            // 
            this.tbIndexOffset.Location = new System.Drawing.Point(1054, 110);
            this.tbIndexOffset.Name = "tbIndexOffset";
            this.tbIndexOffset.ReadOnly = true;
            this.tbIndexOffset.Size = new System.Drawing.Size(384, 20);
            this.tbIndexOffset.TabIndex = 29;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(974, 206);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 23);
            this.label5.TabIndex = 38;
            this.label5.Text = "Datatype Id:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(974, 174);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 23);
            this.label3.TabIndex = 36;
            this.label3.Text = "Datatype:";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(974, 142);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 23);
            this.label9.TabIndex = 34;
            this.label9.Text = "Size:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(974, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 23);
            this.label2.TabIndex = 32;
            this.label2.Text = "Index Group:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(974, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 23);
            this.label1.TabIndex = 30;
            this.label1.Text = "Index Offset:";
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new System.Drawing.Point(15, 85);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(717, 324);
            this.listBox1.TabIndex = 44;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.listBox1.DoubleClick += new System.EventHandler(this.listBox1_DoubleClick);
            // 
            // txtSearchSymbol
            // 
            this.txtSearchSymbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearchSymbol.Location = new System.Drawing.Point(15, 52);
            this.txtSearchSymbol.Name = "txtSearchSymbol";
            this.txtSearchSymbol.Size = new System.Drawing.Size(373, 26);
            this.txtSearchSymbol.TabIndex = 45;
            this.txtSearchSymbol.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(15, 30);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(44, 13);
            this.lblSearch.TabIndex = 46;
            this.lblSearch.Text = "Search:";
            // 
            // btnLoad
            // 
            this.btnLoad.ForeColor = System.Drawing.Color.Blue;
            this.btnLoad.Location = new System.Drawing.Point(738, 389);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(104, 26);
            this.btnLoad.TabIndex = 47;
            this.btnLoad.Text = "ReLoad Symbols";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.tbSymbolname);
            this.groupBox2.Location = new System.Drawing.Point(18, 415);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(480, 88);
            this.groupBox2.TabIndex = 48;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Symbol Info";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(0, 45);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 23);
            this.label8.TabIndex = 21;
            this.label8.Text = "Symbol Name:";
            // 
            // tbSymbolname
            // 
            this.tbSymbolname.BackColor = System.Drawing.Color.White;
            this.tbSymbolname.Location = new System.Drawing.Point(80, 45);
            this.tbSymbolname.Name = "tbSymbolname";
            this.tbSymbolname.ReadOnly = true;
            this.tbSymbolname.Size = new System.Drawing.Size(368, 20);
            this.tbSymbolname.TabIndex = 20;
            // 
            // chkIgnoreCase
            // 
            this.chkIgnoreCase.AutoSize = true;
            this.chkIgnoreCase.Checked = true;
            this.chkIgnoreCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIgnoreCase.Location = new System.Drawing.Point(418, 62);
            this.chkIgnoreCase.Name = "chkIgnoreCase";
            this.chkIgnoreCase.Size = new System.Drawing.Size(82, 17);
            this.chkIgnoreCase.TabIndex = 49;
            this.chkIgnoreCase.Text = "Ignore case";
            this.chkIgnoreCase.UseVisualStyleBackColor = true;
            this.chkIgnoreCase.CheckedChanged += new System.EventHandler(this.chkIgnoreCase_CheckedChanged);
            // 
            // button1
            // 
            this.button1.ForeColor = System.Drawing.Color.Blue;
            this.button1.Location = new System.Drawing.Point(977, 283);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(83, 27);
            this.button1.TabIndex = 50;
            this.button1.Text = "Read Value";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(1077, 289);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(97, 17);
            this.checkBox1.TabIndex = 51;
            this.checkBox1.Text = "Continues read";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(1077, 351);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(292, 147);
            this.listBox2.TabIndex = 52;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(1077, 332);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(98, 13);
            this.label10.TabIndex = 53;
            this.label10.Text = "Device notification:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1450, 24);
            this.menuStrip1.TabIndex = 54;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionToolStripMenuItem
            // 
            this.optionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSymbolFileToolStripMenuItem});
            this.optionToolStripMenuItem.Name = "optionToolStripMenuItem";
            this.optionToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.optionToolStripMenuItem.Text = "Option";
            // 
            // openSymbolFileToolStripMenuItem
            // 
            this.openSymbolFileToolStripMenuItem.Name = "openSymbolFileToolStripMenuItem";
            this.openSymbolFileToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.openSymbolFileToolStripMenuItem.Text = "Open symbol file";
            this.openSymbolFileToolStripMenuItem.Click += new System.EventHandler(this.openSymbolFileToolStripMenuItem_Click);
            // 
            // btnGeneratePLCTypes
            // 
            this.btnGeneratePLCTypes.Location = new System.Drawing.Point(131, 3);
            this.btnGeneratePLCTypes.Name = "btnGeneratePLCTypes";
            this.btnGeneratePLCTypes.Size = new System.Drawing.Size(206, 23);
            this.btnGeneratePLCTypes.TabIndex = 55;
            this.btnGeneratePLCTypes.Text = "Generate PLC Typs";
            this.btnGeneratePLCTypes.UseVisualStyleBackColor = true;
            this.btnGeneratePLCTypes.Click += new System.EventHandler(this.btnGeneratePLCTypes_Click);
            // 
            // txtPLCTypesFileName
            // 
            this.txtPLCTypesFileName.Location = new System.Drawing.Point(343, 5);
            this.txtPLCTypesFileName.Name = "txtPLCTypesFileName";
            this.txtPLCTypesFileName.Size = new System.Drawing.Size(290, 20);
            this.txtPLCTypesFileName.TabIndex = 56;
            this.txtPLCTypesFileName.Text = "..\\..\\..\\PLCController\\PLCTSymbolsStructs.cs";
            // 
            // listFilterNames
            // 
            this.listFilterNames.FormattingEnabled = true;
            this.listFilterNames.Location = new System.Drawing.Point(739, 91);
            this.listFilterNames.Name = "listFilterNames";
            this.listFilterNames.Size = new System.Drawing.Size(181, 121);
            this.listFilterNames.TabIndex = 57;
            // 
            // btnAddFilter
            // 
            this.btnAddFilter.Location = new System.Drawing.Point(738, 247);
            this.btnAddFilter.Name = "btnAddFilter";
            this.btnAddFilter.Size = new System.Drawing.Size(75, 23);
            this.btnAddFilter.TabIndex = 58;
            this.btnAddFilter.Text = "Add filter";
            this.btnAddFilter.UseVisualStyleBackColor = true;
            this.btnAddFilter.Click += new System.EventHandler(this.btnAddFilter_Click);
            // 
            // txtFilterName
            // 
            this.txtFilterName.Location = new System.Drawing.Point(739, 219);
            this.txtFilterName.Name = "txtFilterName";
            this.txtFilterName.Size = new System.Drawing.Size(181, 20);
            this.txtFilterName.TabIndex = 59;
            // 
            // btnRemoveFilter
            // 
            this.btnRemoveFilter.Location = new System.Drawing.Point(819, 247);
            this.btnRemoveFilter.Name = "btnRemoveFilter";
            this.btnRemoveFilter.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveFilter.TabIndex = 60;
            this.btnRemoveFilter.Text = "Remove filter";
            this.btnRemoveFilter.UseVisualStyleBackColor = true;
            this.btnRemoveFilter.Click += new System.EventHandler(this.btnRemoveFilter_Click);
            // 
            // txtArraySymbolsOutput
            // 
            this.txtArraySymbolsOutput.Location = new System.Drawing.Point(661, 4);
            this.txtArraySymbolsOutput.Name = "txtArraySymbolsOutput";
            this.txtArraySymbolsOutput.Size = new System.Drawing.Size(290, 20);
            this.txtArraySymbolsOutput.TabIndex = 61;
            this.txtArraySymbolsOutput.Text = "..\\..\\..\\PLCController\\PLCSymbolArrays.cs";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1450, 514);
            this.Controls.Add(this.txtArraySymbolsOutput);
            this.Controls.Add(this.btnRemoveFilter);
            this.Controls.Add(this.txtFilterName);
            this.Controls.Add(this.btnAddFilter);
            this.Controls.Add(this.listFilterNames);
            this.Controls.Add(this.txtPLCTypesFileName);
            this.Controls.Add(this.btnGeneratePLCTypes);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chkIgnoreCase);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.lblSearch);
            this.Controls.Add(this.txtSearchSymbol);
            this.Controls.Add(this.listBox1);
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
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PLC Symbol test tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbValue;
        private System.Windows.Forms.TextBox tbDatatypeId;
        private System.Windows.Forms.TextBox tbDatatype;
        private System.Windows.Forms.TextBox tbSize;
        private System.Windows.Forms.TextBox tbIndexGroup;
        private System.Windows.Forms.TextBox tbIndexOffset;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox txtSearchSymbol;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbSymbolname;
        private System.Windows.Forms.CheckBox chkIgnoreCase;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSymbolFileToolStripMenuItem;
        private System.Windows.Forms.Button btnGeneratePLCTypes;
        private System.Windows.Forms.TextBox txtPLCTypesFileName;
        private System.Windows.Forms.ListBox listFilterNames;
        private System.Windows.Forms.Button btnAddFilter;
        private System.Windows.Forms.TextBox txtFilterName;
        private System.Windows.Forms.Button btnRemoveFilter;
        private System.Windows.Forms.TextBox txtArraySymbolsOutput;
    }
}

