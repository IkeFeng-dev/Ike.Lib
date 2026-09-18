using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test.Framework.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // 1. 实例化映射器
            Ike.Standard.WPF.IniMap<DataStruct> iniMap = new Ike.Standard.WPF.IniMap<DataStruct>(@"F:\Desktop\temp.ini", true);

            // 2. TextBox 绑定 (正确)
            iniMap.Binding(txb1, nameof(TextBox.Text), nameof(DataStruct.Name), nameof(TextBox.TextChanged));

            // 3. CheckBox 绑定 (修正：属性名改为 IsChecked，事件改为 Click 以同时支持勾选和取消勾选)
            iniMap.Binding(ckb1, nameof(CheckBox.IsChecked), nameof(DataStruct.Select), nameof(CheckBox.Click));

            // 4. ListBox 绑定 (修正：属性名改为 ItemsSource)
            iniMap.Binding(lbx1, nameof(ListBox.ItemsSource), nameof(DataStruct.Items), null);

        }

        public class DataStruct
        {
            public string Name { get; set; }

            public bool Select { get; set; }


            public int Port { get; set; }

            public string[] Items { get; set; }

        }

    }
}
