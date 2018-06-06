namespace TestScreen
{
	partial class FrmGenNormal
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
			this.btnOpen = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.trackBumpiness = new System.Windows.Forms.TrackBar();
			this.lblBumpiness = new System.Windows.Forms.Label();
			this.btnSave = new System.Windows.Forms.Button();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBumpiness)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOpen
			// 
			this.btnOpen.Location = new System.Drawing.Point(784, 23);
			this.btnOpen.Name = "btnOpen";
			this.btnOpen.Size = new System.Drawing.Size(75, 23);
			this.btnOpen.TabIndex = 1;
			this.btnOpen.Text = "打开";
			this.btnOpen.UseVisualStyleBackColor = true;
			this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(13, 13);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(512, 512);
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			// 
			// trackBumpiness
			// 
			this.trackBumpiness.Location = new System.Drawing.Point(566, 95);
			this.trackBumpiness.Maximum = 100;
			this.trackBumpiness.Name = "trackBumpiness";
			this.trackBumpiness.Size = new System.Drawing.Size(210, 45);
			this.trackBumpiness.TabIndex = 5;
			this.trackBumpiness.Value = 16;
			this.trackBumpiness.Scroll += new System.EventHandler(this.trackBumpiness_Scroll);
			// 
			// lblBumpiness
			// 
			this.lblBumpiness.AutoSize = true;
			this.lblBumpiness.Location = new System.Drawing.Point(782, 95);
			this.lblBumpiness.Name = "lblBumpiness";
			this.lblBumpiness.Size = new System.Drawing.Size(77, 12);
			this.lblBumpiness.TabIndex = 4;
			this.lblBumpiness.Text = "lblBumpiness";
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(784, 168);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(75, 23);
			this.btnSave.TabIndex = 6;
			this.btnSave.Text = "保存";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.Filter = "images|*.png";
			// 
			// FrmGenNormal
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(888, 614);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.lblBumpiness);
			this.Controls.Add(this.trackBumpiness);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.btnOpen);
			this.Name = "FrmGenNormal";
			this.Text = "FrmGenNormal";
			this.Load += new System.EventHandler(this.FrmGenNormal_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBumpiness)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button btnOpen;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.TrackBar trackBumpiness;
		private System.Windows.Forms.Label lblBumpiness;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
	}
}