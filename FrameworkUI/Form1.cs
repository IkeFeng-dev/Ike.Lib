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

        Ike.Standard.IniMap<DataStruct> iniMap;

        Ike.Standard.Forms.AutoAdaptWindowsSize windowsSize;

        private void Form1_Load(object sender, EventArgs e)
        {
            windowsSize = new Ike.Standard.Forms.AutoAdaptWindowsSize(this);
            windowsSize.CreateAutoUpdater();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            iniMap = new Ike.Standard.IniMap<DataStruct>(@"F:\Desktop\temp.ini", true);
            //iniMap.Binding(textBox1,nameof(TextBox.Text),nameof(DataStruct.Name), nameof(TextChanged));
            //iniMap.Binding(checkBox1,nameof(CheckBox.Checked), nameof(DataStruct.Select), nameof(CheckBox.CheckedChanged));
            iniMap.Binding(textBox1, nameof(TextBox.Text), nameof(DataStruct.Name), nameof(TextChanged));
            iniMap.Binding(checkBox1, nameof(CheckBox.Checked), nameof(DataStruct.Select), nameof(CheckBox.CheckedChanged));
            iniMap.Binding(listBox1, nameof(ListBox.DataSource), nameof(DataStruct.Items), null);
            iniMap.Binding(textBox2, nameof(TextBox.Text), nameof(DataStruct.Port), nameof(TextChanged));



            Ike.MySQL mysql = new Ike.MySQL("10.62.201.200", "root", "123456");





            //Ike.Standard.Ini.IniConfigManager<DataStruct> iniConfig = new Ike.Standard.Ini.IniConfigManager<DataStruct>()

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
            listBox1.DataSource = datas.ToArray();
        }



        Ike.Standard.Keyboard.Hook mouseHook = new Ike.Standard.Keyboard.Hook();

        private void button4_Click(object sender, EventArgs e)
        {

            mouseHook.InstallHook();

            mouseHook.KeyEvents += (key, msg) =>
            {
                Console.WriteLine($"{key} => {msg.KeyCode} | {msg.ScanCode} | {msg.Time} | {msg.Flags} | {msg.DwExtraInfo}");
            };

        }

        private void button5_Click(object sender, EventArgs e)
        {
            mouseHook.UninstallHook();
            MessageBox.Show("卸载完成");
        }

        private void button6_Click(object sender, EventArgs e)
        {


           
        }
    }




    public class DataStruct
    { 
        public string Name { get; set; }

        public bool Select { get; set; }


        public int Port { get; set; }

        public string[] Items { get; set; }

    }

}
