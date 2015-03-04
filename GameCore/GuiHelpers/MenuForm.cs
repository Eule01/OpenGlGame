#region

using System;
using System.Windows.Forms;

#endregion

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
            theGameCore.SaveMapToXml(textBoxMapName.Text);
        }

        private void buttonLoadMap_Click(object sender, EventArgs e)
        {
            theGameCore.LoadMapFromXml(textBoxMapName.Text);
        }

        private void buttonSaveMapObject_Click(object sender, EventArgs e)
        {
            theGameCore.SaveMapObject(textBoxMapObject.Text);
        }

        private void buttonLoadMapObject_Click(object sender, EventArgs e)
        {
            theGameCore.LoadMapObject(textBoxMapObject.Text);
        }

        private void buttonFindPath_Click(object sender, EventArgs e)
        {
            theGameCore.FindPath();
        }
    }
}