using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Ike.Framework
{
    /// <summary>
    /// 自动窗体大小适配器，用于在窗体大小改变时自动调整控件布局
    /// </summary>
    public class AutoAdaptWindowsSize
    {
        /// <summary>
        /// 窗体原始宽度
        /// </summary>
        private double _formOriginalWidth;

        /// <summary>
        /// 窗体原始高度
        /// </summary>
        private double _formOriginalHeight;

        /// <summary>
        /// 存储控件原始信息的字典
        /// Key: 控件名称
        /// Value: 控件中心X,中心Y,宽度,高度,字体大小(逗号分隔)
        /// </summary>
        private readonly Dictionary<string, string> _controlsInfo = new Dictionary<string, string>();

        /// <summary>
        /// 要适配的目标窗体
        /// </summary>
        private readonly Form _targetForm;

        /// <summary>
        /// 用于容纳所有控件的容器面板
        /// </summary>
        private readonly Panel _containerPanel = new Panel();

        /// <summary>
        /// 最小允许的字体大小
        /// </summary>
        private const float MinFontSize = 6f;

        /// <summary>
        /// 最大允许的字体大小
        /// </summary>
        private const float MaxFontSize = 72f;

        /// <summary>
        /// 水平方向缩放比例
        /// </summary>
        public double ScaleX { get; private set; }

        /// <summary>
        /// 垂直方向缩放比例
        /// </summary>
        public double ScaleY { get; private set; }

        /// <summary>
        /// 初始化自动适配器实例
        /// </summary>
        /// <param name="form">要适配的目标窗体</param>
        /// <exception cref="ArgumentNullException">当传入的窗体为null时抛出</exception>
        public AutoAdaptWindowsSize(Form form)
        {
            _targetForm = form ?? throw new ArgumentNullException(nameof(form));

            InitializeContainerPanel();
            MoveControlsToPanel();
            InitControlsInfo(_containerPanel);
        }

        /// <summary>
        /// 初始化容器面板的属性
        /// </summary>
        private void InitializeContainerPanel()
        {
            _containerPanel.BorderStyle = BorderStyle.None;
            _containerPanel.Dock = DockStyle.Fill;
            _containerPanel.BackColor = Color.Transparent;
            _targetForm.Controls.Add(_containerPanel);
        }

        /// <summary>
        /// 将窗体上的所有控件移动到容器面板中
        /// </summary>
        private void MoveControlsToPanel()
        {
            while (_targetForm.Controls.Count > 0 &&
                  _targetForm.Controls[0] != _containerPanel)
            {
                var control = _targetForm.Controls[0];
                _targetForm.Controls.RemoveAt(0);
                _containerPanel.Controls.Add(control);
            }
        }

        /// <summary>
        /// 初始化控件信息，记录控件的原始位置和大小
        /// </summary>
        /// <param name="ctrlContainer">要记录信息的容器控件</param>
        private void InitControlsInfo(Control ctrlContainer)
        {
            // 如果是顶级容器，记录窗体原始尺寸
            if (ctrlContainer.Parent == _targetForm)
            {
                _formOriginalWidth = ctrlContainer.Width;
                _formOriginalHeight = ctrlContainer.Height;
            }

            // 遍历容器中的所有控件
            foreach (Control item in ctrlContainer.Controls)
            {
                // 跳过没有名称的控件
                if (string.IsNullOrWhiteSpace(item.Name)) continue;

                // 记录控件中心坐标、宽高和字体大小
                _controlsInfo[item.Name] = string.Join(",",
                    item.Left + item.Width / 2,  // 中心X坐标
                    item.Top + item.Height / 2,  // 中心Y坐标
                    item.Width,                  // 宽度
                    item.Height,                 // 高度
                    item.Font.Size);             // 字体大小

                // 如果不是用户控件且有子控件，递归处理
                if (!(item is UserControl) && item.Controls.Count > 0)
                {
                    InitControlsInfo(item);
                }
            }
        }

        /// <summary>
        /// 窗体大小改变时调用此方法重新计算控件布局
        /// </summary>
        public void FormSizeChanged()
        {
            // 如果没有控件信息，直接返回
            if (_controlsInfo.Count == 0) return;
            // 计算缩放比例
            CalculateScalingFactors(_containerPanel);
            // 调整所有控件
            AdjustControls(_containerPanel);
        }

        /// <summary>
        /// 计算水平和垂直方向的缩放比例
        /// </summary>
        /// <param name="ctrlContainer">容器控件</param>
        private void CalculateScalingFactors(Control ctrlContainer)
        {
            ScaleX = ctrlContainer.Width / _formOriginalWidth;
            ScaleY = ctrlContainer.Height / _formOriginalHeight;
        }

        /// <summary>
        /// 根据缩放比例调整控件大小和位置
        /// </summary>
        /// <param name="ctrlContainer">容器控件</param>
        private void AdjustControls(Control ctrlContainer)
        {
            // 使用较小的缩放比例来调整字体，保持纵横比
            float minScale = (float)Math.Min(ScaleX, ScaleY);

            // 遍历容器中的所有控件
            foreach (Control item in ctrlContainer.Controls)
            {
                // 跳过没有名称的控件
                if (string.IsNullOrWhiteSpace(item.Name)) continue;

                // 如果不是用户控件且有子控件，递归处理
                if (!(item is UserControl) && item.Controls.Count > 0)
                {
                    AdjustControls(item);
                }

                // 获取控件原始信息
                if (!_controlsInfo.TryGetValue(item.Name, out var controlInfo)) continue;

                // 解析原始信息
                var parts = controlInfo.Split(',');
                if (parts.Length < 5) continue;

                double centerX = double.Parse(parts[0]);    // 原始中心X
                double centerY = double.Parse(parts[1]);    // 原始中心Y
                double originalWidth = double.Parse(parts[2]);  // 原始宽度
                double originalHeight = double.Parse(parts[3]); // 原始高度
                float originalFontSize = float.Parse(parts[4]); // 原始字体大小

                // 计算新尺寸
                double newWidth = originalWidth * ScaleX;
                double newHeight = originalHeight * ScaleY;

                // 设置新位置（保持中心点不变）
                item.Left = (int)(centerX * ScaleX - newWidth / 2);
                item.Top = (int)(centerY * ScaleY - newHeight / 2);

                // 设置新尺寸
                item.Width = (int)newWidth;
                item.Height = (int)newHeight;

                // 计算并限制新字体大小
                float newFontSize = originalFontSize * minScale;
                newFontSize = Math.Max(MinFontSize, Math.Min(MaxFontSize, newFontSize));

                // 如果字体大小有变化，更新字体
                if (Math.Abs(item.Font.Size - newFontSize) > float.Epsilon)
                {
                    item.Font = new Font(item.Font.Name, newFontSize, item.Font.Style);
                }
            }
        }
    }
}
