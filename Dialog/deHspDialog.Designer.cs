namespace KttK.HspDecompiler
{
	partial class deHspDialog
	{
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param defaultName="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナで生成されたコード

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(deHspDialog));
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.txtBoxMainInfo = new System.Windows.Forms.TextBox();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.ToolStripMenuItemFile = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItemOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.ToolStripMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItemAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.dpmFileList = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "HSP出力ファイル(*.ax; *.dpm; *.exe)|*.ax;*.dpm;*.exe|全てのファイル(*.*)|*.*";
			// 
			// txtBoxMainInfo
			// 
			this.txtBoxMainInfo.Location = new System.Drawing.Point(12, 36);
			this.txtBoxMainInfo.Multiline = true;
			this.txtBoxMainInfo.Name = "txtBoxMainInfo";
			this.txtBoxMainInfo.ReadOnly = true;
			this.txtBoxMainInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtBoxMainInfo.Size = new System.Drawing.Size(301, 319);
			this.txtBoxMainInfo.TabIndex = 1;
			this.txtBoxMainInfo.Text = "ここにドロップしてください";
			// 
			// menuStrip
			// 
			this.menuStrip.AllowMerge = false;
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemFile,
            this.ToolStripMenuItemHelp});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(664, 24);
			this.menuStrip.TabIndex = 2;
			this.menuStrip.Text = "menuStrip";
			// 
			// ToolStripMenuItemFile
			// 
			this.ToolStripMenuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemOpen,
            this.toolStripSeparator1,
            this.ToolStripMenuItemExit});
			this.ToolStripMenuItemFile.Name = "ToolStripMenuItemFile";
			this.ToolStripMenuItemFile.Size = new System.Drawing.Size(66, 20);
			this.ToolStripMenuItemFile.Text = "ファイル(&F)";
			// 
			// ToolStripMenuItemOpen
			// 
			this.ToolStripMenuItemOpen.Name = "ToolStripMenuItemOpen";
			this.ToolStripMenuItemOpen.Size = new System.Drawing.Size(94, 22);
			this.ToolStripMenuItemOpen.Text = "開く";
			this.ToolStripMenuItemOpen.Click += new System.EventHandler(this.ToolStripMenuItemOpen_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(91, 6);
			// 
			// ToolStripMenuItemExit
			// 
			this.ToolStripMenuItemExit.Name = "ToolStripMenuItemExit";
			this.ToolStripMenuItemExit.Size = new System.Drawing.Size(94, 22);
			this.ToolStripMenuItemExit.Text = "終了";
			this.ToolStripMenuItemExit.Click += new System.EventHandler(this.ToolStripMenuItemExit_Click);
			// 
			// ToolStripMenuItemHelp
			// 
			this.ToolStripMenuItemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemAbout});
			this.ToolStripMenuItemHelp.Name = "ToolStripMenuItemHelp";
			this.ToolStripMenuItemHelp.Size = new System.Drawing.Size(62, 20);
			this.ToolStripMenuItemHelp.Text = "ヘルプ(&H)";
			// 
			// ToolStripMenuItemAbout
			// 
			this.ToolStripMenuItemAbout.Name = "ToolStripMenuItemAbout";
			this.ToolStripMenuItemAbout.Size = new System.Drawing.Size(155, 22);
			this.ToolStripMenuItemAbout.Text = "バージョン情報(&A)";
			this.ToolStripMenuItemAbout.Click += new System.EventHandler(this.ToolStripMenuItemAbout_Click);
			// 
			// dpmFileList
			// 
			this.dpmFileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
			this.dpmFileList.Location = new System.Drawing.Point(319, 36);
			this.dpmFileList.Name = "dpmFileList";
			this.dpmFileList.Size = new System.Drawing.Size(332, 319);
			this.dpmFileList.TabIndex = 4;
			this.dpmFileList.UseCompatibleStateImageBehavior = false;
			this.dpmFileList.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "ファイル名";
			this.columnHeader1.Width = 100;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "暗号化";
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "オフセット";
			this.columnHeader3.Width = 80;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "ファイル長";
			this.columnHeader4.Width = 80;
			// 
			// deHspDialog
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(664, 366);
			this.Controls.Add(this.dpmFileList);
			this.Controls.Add(this.txtBoxMainInfo);
			this.Controls.Add(this.menuStrip);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip;
			this.MaximizeBox = false;
			this.Name = "deHspDialog";
			this.Text = "逆コンパイラ for HSP";
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
			this.Activated += new System.EventHandler(this.deHspDialog_Activated);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
			this.Load += new System.EventHandler(this.deHspDialog_Load);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.TextBox txtBoxMainInfo;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemFile;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemOpen;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemExit;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemHelp;
		private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemAbout;
		private System.Windows.Forms.ListView dpmFileList;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
	}
}

