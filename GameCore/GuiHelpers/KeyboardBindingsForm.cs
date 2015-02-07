using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeToast;
using GameCore.UserInterface;

namespace GameCore.GuiHelpers
{
    public partial class KeyboardBindingsForm : Form
    {
        public KeyboardBindingsForm()
        {
            InitializeComponent();
        }

        public KeyboardBindingsForm(KeyBindings theKeyBindings)
        {
            InitializeComponent();
          
            SetKeyBindings(theKeyBindings);
        }

        public void SetKeyBindingsAsync(KeyBindings aKeyBindings)
        {
            Async.UI(delegate { SetKeyBindings(aKeyBindings); }, this, false);
        }

        private void SetKeyBindings(KeyBindings aKeyBindings)
        {
            foreach (KeyBinding aKeyBinding in aKeyBindings.TheKeyBindings)
            {
                Panel tempPanel = GetKeyBindingPanel(aKeyBinding);

                this.flowLayoutPanel1.Controls.Add(tempPanel);
            }

        }

        private Panel GetKeyBindingPanel(KeyBinding aKeyBinding)
        {
            Panel tempPanel = new Panel();
            tempPanel.Width = this.flowLayoutPanel1.ClientSize.Width-5;
            tempPanel.Height = 30;
            tempPanel.BorderStyle  = BorderStyle.FixedSingle;
            Label tempLabel = new Label();
            tempLabel.Text = aKeyBinding.Description;
            tempPanel.Controls.Add(tempLabel);
            tempLabel.Width = 150;
            tempLabel.Height = 30;
            GroupBox tempGroupBox = new GroupBox();
            tempGroupBox.Width = 100;
            tempGroupBox.Height = 25;
            tempGroupBox.Text = aKeyBinding.KeyName;
            tempGroupBox.Location = new Point(tempPanel.ClientSize.Width - tempGroupBox.Width-5,0);
            tempPanel.Controls.Add(tempGroupBox);
            return tempPanel;
        }
    }
}
