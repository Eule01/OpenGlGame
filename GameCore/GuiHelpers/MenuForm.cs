using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCore.GuiHelpers
{
    public partial class MenuForm : Form
    {
        private GameCore theGameCore;

        public MenuForm()
        {
            InitializeComponent();
        }

        public MenuForm(GameCore gameCore)
        {
            InitializeComponent();
            theGameCore = gameCore;
        }

        private void buttonSaveMap_Click(object sender, EventArgs e)
        {
            theGameCore.SaveMap(textBoxMapName.Text);
        }

        private void buttonLoadMap_Click(object sender, EventArgs e)
        {
            theGameCore.LoadMap(textBoxMapName.Text);
        }
    }
}
