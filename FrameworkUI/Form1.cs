using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrameworkUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Ike.Framework.IniMap<DataStruct> iniMap;

        Ike.Framework.Forms.AutoAdaptWindowsSize windowsSize;

        private void Form1_Load(object sender, EventArgs e)
        {
            windowsSize = new Ike.Framework.Forms.AutoAdaptWindowsSize(this);
            windowsSize.CreateAutoUpdater();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            iniMap = new Ike.Framework.IniMap<DataStruct>(@"F:\Desktop\temp.ini", true);
            //iniMap.Binding(textBox1,nameof(TextBox.Text),nameof(DataStruct.Name), nameof(TextChanged));
            //iniMap.Binding(checkBox1,nameof(CheckBox.Checked), nameof(DataStruct.Select), nameof(CheckBox.CheckedChanged));
             iniMap.Binding(textBox1,nameof(TextBox.Text),nameof(DataStruct.Name), null);
            iniMap.Binding(checkBox1,nameof(CheckBox.Checked), nameof(DataStruct.Select), null);
            iniMap.Binding(listBox1, nameof(ListBox.DataSource), nameof(DataStruct.Items), null);


            

            //Dictionary<string,Type> keyValues = new Dictionary<string, Type>()
            //{
            //    {"model", } 



            //};
            //var r = mysql.BatchInsertion("parameter_change_data",);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            iniMap.SaveToFile();
            //MessageBox.Show($"Name:{iniMap.Data.Name}\r\nSelect:{iniMap.Data.Select}");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var datas = ((string[])listBox1.DataSource).ToList();
            datas.Add("ABCDE");
            listBox1.DataSource= datas.ToArray();
        }
    }




    public class DataStruct
    { 
        public string Name { get; set; }

        public bool Select { get; set; }

        public string[] Items { get; set; }

    }

}
