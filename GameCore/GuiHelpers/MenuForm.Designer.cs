namespace GameCore.GuiHelpers
{
    partial class MenuForm
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
            this.buttonSaveMap = new System.Windows.Forms.Button();
            this.buttonLoadMap = new System.Windows.Forms.Button();
            this.textBoxMapName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonSaveMap
            // 
            this.buttonSaveMap.Location = new System.Drawing.Point(160, 12);
            this.buttonSaveMap.Name = "buttonSaveMap";
            this.buttonSaveMap.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveMap.TabIndex = 0;
            this.buttonSaveMap.Text = "Save Map";
            this.buttonSaveMap.UseVisualStyleBackColor = true;
            this.buttonSaveMap.Click += new System.EventHandler(this.buttonSaveMap_Click);
            // 
            // buttonLoadMap
            // 
            this.buttonLoadMap.Location = new System.Drawing.Point(241, 13);
            this.buttonLoadMap.Name = "buttonLoadMap";
            this.buttonLoadMap.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadMap.TabIndex = 1;
            this.buttonLoadMap.Text = "Load Map";
            this.buttonLoadMap.UseVisualStyleBackColor = true;
            this.buttonLoadMap.Click += new System.EventHandler(this.buttonLoadMap_Click);
            // 
            // textBoxMapName
            // 
            this.textBoxMapName.Location = new System.Drawing.Point(13, 13);
            this.textBoxMapName.Name = "textBoxMapName";
            this.textBoxMapName.Size = new System.Drawing.Size(141, 20);
            this.textBoxMapName.TabIndex = 2;
            // 
            // MenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 272);
            this.Controls.Add(this.textBoxMapName);
            this.Controls.Add(this.buttonLoadMap);
            this.Controls.Add(this.buttonSaveMap);
            this.Name = "MenuForm";
            this.Text = "MenuForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSaveMap;
        private System.Windows.Forms.Button buttonLoadMap;
        private System.Windows.Forms.TextBox textBoxMapName;
    }
}