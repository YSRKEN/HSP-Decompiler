using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;

namespace KttK.HspDecompiler
{
	/// <summary>
	/// AboutDialog の概要の説明です。
	/// </summary>
	internal sealed class AboutDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label LB_Title;
		private Label LB_Copyright;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		internal AboutDialog()
		{
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();
			Assembly mainAssembly = Assembly.GetEntryAssembly();
			string appCopyright = "-";
			object[] CopyrightArray =
			  mainAssembly.GetCustomAttributes(
				typeof(AssemblyCopyrightAttribute), false);
			if ((CopyrightArray != null) && (CopyrightArray.Length > 0))
			{
			  appCopyright =
				((AssemblyCopyrightAttribute)CopyrightArray[0]).Copyright;
			}
			LB_Copyright.Text = appCopyright;
			//
			// TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
			//
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows フォーム デザイナで生成されたコード 
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.LB_Title = new System.Windows.Forms.Label();
			this.LB_Copyright = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(12, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(32, 32);
			this.pictureBox1.TabIndex = 4;
			this.pictureBox1.TabStop = false;
			// 
			// LB_Title
			// 
			this.LB_Title.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.LB_Title.Location = new System.Drawing.Point(50, 12);
			this.LB_Title.Name = "LB_Title";
			this.LB_Title.Size = new System.Drawing.Size(172, 16);
			this.LB_Title.TabIndex = 5;
			this.LB_Title.Text = "フリー HSP逆コンパイラ　Ver 1.20";
			// 
			// LB_Copyright
			// 
			this.LB_Copyright.AutoSize = true;
			this.LB_Copyright.Location = new System.Drawing.Point(66, 37);
			this.LB_Copyright.Name = "LB_Copyright";
			this.LB_Copyright.Size = new System.Drawing.Size(11, 12);
			this.LB_Copyright.TabIndex = 6;
			this.LB_Copyright.Text = "-";
			// 
			// AboutDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(240, 63);
			this.Controls.Add(this.LB_Copyright);
			this.Controls.Add(this.LB_Title);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(246, 90);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(246, 90);
			this.Name = "AboutDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "バージョン情報";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.RegistDialog_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		private void buttonOK_Click(object sender, System.EventArgs e)
		{
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}


		private void RegistDialog_Load(object sender, System.EventArgs e)
		{
			this.Location = initialLocation;
		}

		private Point initialLocation;
		internal void SetInitialLocation(System.Windows.Forms.Form frm)
		{
			initialLocation = frm.Location;
			initialLocation.Offset(frm.Width/2, frm.Height/2);
			initialLocation.Offset(-this.Width/2, -this.Height/2);
		}


	}
}
