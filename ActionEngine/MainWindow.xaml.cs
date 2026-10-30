using Ike.ActionEngine.Model;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static Ike.Standard.LOG;

namespace Ike.ActionEngine
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// UI绑定的日志输出数据
        /// </summary>
        private readonly ObservableCollection<LogDisplayItem> _logItems = new ObservableCollection<LogDisplayItem>();
        /// <summary>
        /// 是否展开
        /// </summary>
        private bool _isExpanded = false;
        /// <summary>
        /// 折叠高度
        /// </summary>
        private const double CollapsedHeight = 48;
        /// <summary>
        /// 展开高度
        /// </summary>
        private const double ExpandedHeight = 360;

        public MainWindow()
        {
            InitializeComponent();
            LogItemsControl.ItemsSource = _logItems;
            GlobalVariable.Log.LogEvent += Log_LogEvent;
            GlobalVariable.Log.StartRecordLog();
        }




        /// <summary>
        /// 载入完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GlobalVariable.Log.Info(string.Format("{0}{1}{0}{2}START{0}{1}", Environment.NewLine, Standard.Text.CopyString("=", 140), Standard.Text.CopyString(" ", 70)));
            Task.Run(() => ProcessCommandLineArgs());
        }



        /// <summary>
        /// 开始解析处理接收参数,并执行JSON动作
        /// </summary>
        private void ProcessCommandLineArgs()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                string[] args = Environment.GetCommandLineArgs();
                //string[] args = new string[] { "", @"F:\Desktop\update-manifest.json" };

                if (args.Length <= 1)
                {
                    DynamicTestScript dynamic = new DynamicTestScript();
                    dynamic.Execute(new UpdateContext());
                }
                else
                {
                    string arg = args[1];
                    if (arg.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        Directory.CreateDirectory(GlobalVariable.CurrentDataDir);
                        using (HttpClient httpClient = new HttpClient())
                        {
                            string jsonContent = httpClient.GetStringAsync(arg).Result;
                            File.WriteAllText(GlobalVariable.ActionJsonPath, jsonContent);
                        }
                    }
                    else if (File.Exists(arg))
                    {
                        Directory.CreateDirectory(GlobalVariable.CurrentDataDir);
                        if (arg != GlobalVariable.ActionJsonPath)
                        {
                            File.Copy(arg, GlobalVariable.ActionJsonPath, true);
                        }
                    }
                    else
                    {
                        GlobalVariable.Log.Error($"传参不合法,只接收JSON URL或者JSON file path -> {arg}");
                        Thread.Sleep(1000);
                        Environment.Exit(0);
                    }
                    // 开始执行动作
                    ManifestEngine.Run(GlobalVariable.ActionJsonPath);
                    // 确保日志输出已完成
                    while (GlobalVariable.Log.QueueNumbers > 0)
                    {
                        Thread.Sleep(100);
                    }
                    stopwatch.Stop();
                    GlobalVariable.Log.Info($"执行完成,总耗时 {Math.Round(stopwatch.ElapsedMilliseconds / 1000.0, 2)}s");
                    Directory.CreateDirectory(GlobalVariable.CurrentDataDir);
                    File.WriteAllText(GlobalVariable.ActionLogPath, GlobalVariable.Log.GetRecordLog());
                }
            }
            catch (Exception ex)
            {
                GlobalVariable.Log.Exception(ex);
                GlobalVariable.Log.Info($"执行完成,总耗时 {Math.Round(stopwatch.ElapsedMilliseconds / 1000.0, 2)}s");
                File.WriteAllText(GlobalVariable.ActionLogPath, GlobalVariable.Log.GetRecordLog());
            }
            finally
            {
                Exit(2);
            }
        }


        /// <summary>
        /// 等待指定秒后退出
        /// </summary>
        /// <param name="wait">等待秒</param>
        private void Exit(int wait = 1)
        {
            Thread.Sleep(wait * 1000);
            Environment.Exit(0);
        }

        /// <summary>
        /// LOG事件订阅
        /// </summary>
        /// <param name="log"></param>
        private void Log_LogEvent(LogStruct log)
        {
            Dispatcher.Invoke(() =>
            {
                var item = BuildDisplayItem(log);

                _logItems.Add(item);

                // 实时覆盖更新顶部单行小卡片
                LatestTimeText.Text = item.Time;
                LatestStepActionText.Text = $"[{item.Step}/{item.StepNumbers}] - [{item.ActionName}]";
                LatestTagText.Text = $"[{GetCompactTypeLabel(log.LogType)}]";
                LatestTagText.Foreground = item.LabelForeground;
                LatestTagBorder.Background = item.LabelBackground;
                LatestInfoText.Text = item.Info;
                LatestInfoText.Foreground = item.InfoForeground;
                LogScrollViewer.ScrollToEnd();
            });
        }


        private static string GetCompactTypeLabel(LogType type)
        {
            switch (type)
            {
                case LogType.Verbose: return "VER";
                case LogType.Debug: return "DEB";
                case LogType.Info: return "INF";
                case LogType.Warning: return "WRN";
                case LogType.Error: return "ERR";
                case LogType.Fatal: return "FTL";
                case LogType.Success: return "SUC";
                default: return "LOG";
            }
        }


        /// <summary>
        /// 左键点击/按住卡片主体：纯粹拖动窗口
        /// </summary>
        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /// <summary>
        /// 点击右侧箭头按钮：专门触发展开/折叠
        /// </summary>
        private void ToggleBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ToggleExpandState();

            // 阻止事件向上冒泡，避免触发 Header_MouseLeftButtonDown 中的 DragMove
            e.Handled = true;
        }

        /// <summary>
        /// 切换卡片展开/折叠
        /// </summary>
        private void ToggleExpandState()
        {
            _isExpanded = !_isExpanded;

            if (_isExpanded)
            {
                DetailPanel.Visibility = Visibility.Visible;
                Height = ExpandedHeight;
                ToggleIcon.Text = "▲";
                ToggleMenuItem.Header = "折叠"; // 状态改变时同步更新右键菜单标题
            }
            else
            {
                DetailPanel.Visibility = Visibility.Collapsed;
                Height = CollapsedHeight;
                ToggleIcon.Text = "▼";
                ToggleMenuItem.Header = "展开"; // 状态改变时同步更新右键菜单标题
            }
        }

        #region 右键菜单逻辑

        /// <summary>
        /// 右键菜单真正打开时触发（Opened 事件）
        /// </summary>
        private void MainContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            // 打开菜单时，根据当前 _isExpanded 状态精确设置 Header
            ToggleMenuItem.Header = _isExpanded ? "折叠" : "展开";
        }

        private void ToggleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ToggleExpandState();
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion


        private static SolidColorBrush Brush(string hex) => new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));

        private static LogDisplayItem BuildDisplayItem(LogStruct log)
        {
            var (label, labelFg, labelBg, infoFg) = ResolveStyle(log.LogType);

            return new LogDisplayItem
            {
                Time = log.Time.ToString("HH:mm:ss"),
                TypeLabel = label,
                Info = log.Info,
                Step = GlobalVariable.Step,
                StepNumbers = GlobalVariable.StepNumbers,
                ActionName = GlobalVariable.ActionName,
                LabelForeground = Brush(labelFg),
                LabelBackground = Brush(labelBg),
                InfoForeground = Brush(infoFg)
            };
        }

        private static (string label, string labelFg, string labelBg, string infoFg) ResolveStyle(LogType type)
        {
            switch (type)
            {
                case LogType.Verbose: return ("VERB", "#5F6368", "#F1F3F4", "#5F6368");
                case LogType.Debug: return ("INFO", "#1A73E8", "#E8F0FE", "#1A73E8");
                case LogType.Info: return ("DEBG", "#4A5568", "#EDF2F7", "#2D3748");
                case LogType.Warning: return ("WARN", "#B45309", "#FEF3C7", "#D97706");
                case LogType.Error: return ("ERRO", "#C5221F", "#FDE8E8", "#C5221F");
                case LogType.Fatal: return ("FATL", "#991B1B", "#FEE2E2", "#991B1B");
                case LogType.Success: return ("SUCC", "#137333", "#E6F4EA", "#137333");
                default: return ("LOG ", "#555555", "#F0F0F0", "#333333");
            }
        }
    }
}
