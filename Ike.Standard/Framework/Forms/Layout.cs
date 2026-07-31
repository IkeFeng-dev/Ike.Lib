#if NETFRAMEWORK
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ike.Standard.Forms
{
    /// <summary>
    /// <see cref="System.Windows.Forms"/> UI 布局操作
    /// </summary>
    public partial class Layout
    {
        #region 布局方法
        /// <summary>
        /// 设置指定方向填充最大化
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="direction">方向</param>
        public void FillMax(Control control, FillDirection direction)
        {
            Control parent = control.Parent;
            if (direction == FillDirection.Transverse)
            {
                control.Width = parent.Width;
                control.Location = new Point(0, control.Location.Y);
            }
            else if (direction == FillDirection.Bottom)
            {
                control.Width = parent.Width;
                control.Height = parent.Height - control.Location.Y;
                control.Location = new Point(0, control.Location.Y);
            }
            else if (direction == FillDirection.Top)
            {
                control.Width = parent.Width;
                control.Height = control.Location.Y + control.Height;
                control.Location = new Point(0, 0);
            }
            else if (direction == FillDirection.Left)
            {
                control.Width = control.Location.X + control.Width;
                control.Height = parent.Height;
                control.Location = new Point(0, 0);
            }
            else if (direction == FillDirection.Right)
            {
                control.Width = parent.Width - control.Location.X;
                control.Height = parent.Height;
                control.Location = new Point(control.Location.X, 0);
            }
            else if (direction == FillDirection.Vertical)
            {
                control.Height = parent.Height;
                control.Location = new Point(control.Location.X, 0);
            }
            else
            {
                throw new Exception("未找到枚举值[" + direction.ToString() + "]");
            }
        }


        /// <summary>
        /// 向指定方向指定距离移动控件
        /// </summary>
        /// <param name="setControl">控件</param>
        /// <param name="distance">移动距离</param>
        /// <param name="direction">移动方向</param>
        /// <exception cref="Exception"></exception>
        public void Move(Control setControl, int distance, Direction direction)
        {
            if (direction == Direction.Top)
            {
                setControl.Location = new Point(setControl.Location.X, setControl.Location.Y - distance);
            }
            else if (direction == Direction.Button)
            {
                setControl.Location = new Point(setControl.Location.X, setControl.Location.Y + distance);
            }
            else if (direction == Direction.Left)
            {
                setControl.Location = new Point(setControl.Location.X - distance, setControl.Location.Y);
            }
            else if (direction == Direction.Right)
            {
                setControl.Location = new Point(setControl.Location.X + distance, setControl.Location.Y);
            }
            else
            {
                throw new Exception("未找到枚举值[" + direction.ToString() + "]");
            }
        }

        /// <summary>
        /// 设置控件固定边距
        /// </summary>
        /// <param name="setControl">设置的控件</param>
        /// <param name="referenceControl">参照控件</param>
        /// <param name="margin">边距距离</param>
        /// <param name="direction">布局方向</param>
        public void SetFixedMargin(Control setControl, Control referenceControl, int margin, Direction direction)
        {
            if (direction == Direction.Top)
            {
                SetRelativePosition(setControl, referenceControl, RelativePosition.Above);
            }
            else if (direction == Direction.Button)
            {
                SetRelativePosition(setControl, referenceControl, RelativePosition.Below);
            }
            else if (direction == Direction.Left)
            {
                SetRelativePosition(setControl, referenceControl, RelativePosition.OnLeft);
            }
            else if (direction == Direction.Right)
            {
                SetRelativePosition(setControl, referenceControl, RelativePosition.OnRight);
            }
            else
            {
                throw new Exception("未找到枚举值[" + direction.ToString() + "]");
            }
            Move(setControl, margin, direction);
        }
        /// <summary>
        /// 控件排列
        /// </summary>
        /// <param name="control">需要排列的子控件</param>
        /// <param name="horizontalSort">横排第几个</param>
        /// <param name="verticalSort">竖排第几个</param>
        /// <param name="horizontalQuantity">横排控件数量</param>
        /// <param name="verticalQuantity">竖排控件数量</param>
        /// <returns></returns>
        public void SetArrange(Control control, int horizontalSort, int verticalSort, int horizontalQuantity, int verticalQuantity)
        {
            Control parent = control.Parent;
            int horizontalSverage = parent.Width / horizontalQuantity;
            int verticalSverage = parent.Height / verticalQuantity;
            int horizontalHalf = horizontalSverage / 2 - control.Width / 2;
            int verticalHalf = verticalSverage / 2 - control.Height / 2;
            if (horizontalSort < 1)
            {
                horizontalSort = 1;
            }
            if (verticalSort < 1)
            {
                verticalSort = 1;
            }
            int xCalculate = horizontalHalf + horizontalSverage * (horizontalSort - 1);
            int yCalculate = verticalHalf + verticalSverage * (verticalSort - 1);
            control.Location = new Point(xCalculate, yCalculate);
        }

        /// <summary>
        /// 根据属性设置控件相对属性
        /// </summary>
        /// <param name="control">布局控件</param>
        /// <param name="relative">相对控件</param>
        /// <param name="relativePosition">相对位置选择</param>
        public void SetRelativePosition(Control control, Control relative, RelativePosition relativePosition)
        {
            if (control == relative)
            {
                throw new Exception("布局控件和相对控件不能为同一个控件");
            }
            int currentX = control.Location.X;
            int currentY = control.Location.Y;
            int relativeX = relative.Location.X;
            int relativeY = relative.Location.Y;
            int currentWidth = control.Width;
            int currentHeight = control.Height;
            int relativeWidth = relative.Width;
            int relativeHeoght = relative.Height;
            Point point;
            if (relativePosition == RelativePosition.OnLeft)
            {
                point = new Point(relativeX - currentWidth, currentY);
                control.Location = point;
            }
            else if (relativePosition == RelativePosition.Above)
            {
                point = new Point(currentX, relativeY - currentHeight);
                control.Location = point;
            }
            else if (relativePosition == RelativePosition.OnRight)
            {
                point = new Point(relativeX + relativeWidth, currentY);
                control.Location = point;
            }
            else if (relativePosition == RelativePosition.Below)
            {
                point = new Point(currentX, relativeY + relativeHeoght);
                control.Location = point;
            }
            else if (relativePosition == RelativePosition.AlignLeft)
            {
                point = new Point(relativeX - currentWidth, relativeY);
                control.Location = point;
            }
            else if (relativePosition == RelativePosition.AlignTop)
            {
                point = new Point(relativeX, relativeY - currentHeight);
                control.Location = point;
            }
            else if (relativePosition == RelativePosition.AlignRight)
            {
                point = new Point(relativeX + relativeWidth, relativeY);
                control.Location = point;
            }
            else if (relativePosition == RelativePosition.AlignBottom)
            {
                point = new Point(relativeX, relativeY + relativeHeoght);
                control.Location = point;
            }
            else if (relativePosition == RelativePosition.Center)
            {
                Control parent = relative;
                int parentWidth = parent.Width;
                int parentHeight = parent.Height;
                currentWidth = control.Width;
                currentHeight = control.Height;
                int parentWidthHalf = parentWidth / 2;
                int parentHeightHalf = parentHeight / 2;
                int currentWidthHalf = currentWidth / 2;
                int currentHeightHalf = currentHeight / 2;
                point = new Point(parentWidthHalf - currentWidthHalf + parent.Location.X, parentHeightHalf - currentHeightHalf + parent.Location.Y);
                //如果控件为lable控件,则需要更改控件属性才能设置居中位置
                if (control is Label label)
                {
                    label.AutoSize = true;
                }
                control.Location = point;
            }
            else
            {
                throw new Exception("未找到枚举值[" + relativePosition.ToString() + "]");
            }
        }

        /// <summary>
        /// 根据传递属性,设置控件位置
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="alignment">对齐方式</param>
        public void SetAlignment(Control control, Alignment alignment)
        {
            Control parent = control.Parent;
            int parentWidth = parent.Width;
            int parentHeight = parent.Height;
            int currentWidth = control.Width;
            int currentHeight = control.Height;
            int parentWidthHalf = parentWidth / 2;
            int parentHeightHalf = parentHeight / 2;
            int currentWidthHalf = currentWidth / 2;
            int currentHeightHalf = currentHeight / 2;
            Point point;

            if (alignment == Alignment.Bottom)
            {
                point = new Point(control.Location.X, parentHeight - currentHeight);
                control.Location = point;
            }
            else if (alignment == Alignment.BottomLeft)
            {
                point = new Point(0, parentHeight - currentHeight);
                control.Location = point;
            }
            else if (alignment == Alignment.BottomRight)
            {
                point = new Point(parentWidth - currentWidth, parentHeight - currentHeight);
                control.Location = point;
            }
            else if (alignment == Alignment.Center)
            {
                point = new Point(parentWidthHalf - currentWidthHalf, parentHeightHalf - currentHeightHalf);
                control.Location = point;
            }
            else if (alignment == Alignment.CenterHorizontal)
            {
                point = new Point(parentWidthHalf - currentWidthHalf, 0);
                control.Location = point;
            }
            else if (alignment == Alignment.CenterHorizontalOnBottom)
            {
                point = new Point(parentWidthHalf - currentWidthHalf, parentHeight - currentHeight);
                control.Location = point;
            }
            else if (alignment == Alignment.CenterVertical)
            {
                point = new Point(0, parentHeightHalf - currentHeightHalf);
                control.Location = point;
            }
            else if (alignment == Alignment.CenterVerticalOnRight)
            {
                point = new Point(parentWidth - currentWidth, parentHeightHalf - currentHeightHalf);
                control.Location = point;
            }
            else if (alignment == Alignment.CurrentHorizontal)
            {
                point = new Point(parentWidthHalf - currentWidthHalf, control.Location.Y);
                control.Location = point;
            }
            else if (alignment == Alignment.CurrentVertical)
            {
                point = new Point(control.Location.X, parentHeightHalf - currentHeightHalf);
                control.Location = point;
            }
            else if (alignment == Alignment.Left)
            {
                point = new Point(0, control.Location.Y);
                control.Location = point;
            }
            else if (alignment == Alignment.Right)
            {
                point = new Point(parentWidth - currentWidth, control.Location.Y);
                control.Location = point;
            }
            else if (alignment == Alignment.Top)
            {
                point = new Point(control.Location.X, 0);
                control.Location = point;
            }
            else if (alignment == Alignment.TopLeft)
            {
                point = new Point(0, 0);
                control.Location = point;
            }
            else if (alignment == Alignment.TopRight)
            {
                point = new Point(parentWidth - currentWidth, 0);
                control.Location = point;
            }
            else
            {
                throw new Exception("未找到属性[" + alignment.ToString() + "]");
            }
        }
        /// <summary>
        /// 设置控件中心点水平与父控件
        /// </summary>
        /// <param name="controls">需要与父控件对齐的控件</param>
        public void SetControlsCenterHorizontal(params Control[] controls)
        {
            if (controls.Length > 0)
            {
                for (int i = 0; i < controls.Length; i++)
                {
                    SetAlignment(controls[i], Alignment.CurrentHorizontal);
                }
            }
        }

        /// <summary>
        /// 按照父控件比例设置控件宽高
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="widthScale">宽比例(0 到 1,0为不设置,1等于父控件)</param>
        /// <param name="heightScale">高比例(0 到 1,0为不设置,1等于父控件)</param>
        public void SetControlsScaleSize(Control control, double widthScale, double heightScale)
        {
            Control parent = control.Parent;
            if (widthScale > 0)
                control.Width = (int)(parent.Width * widthScale);
            if (heightScale > 0)
                control.Height = (int)(parent.Height * heightScale);
        }

        /// <summary>
        /// 批量排序控件,可用null占位
        /// </summary>
        /// <param name="verticalSort">当前垂直排</param>
        /// <param name="verticalQuantity">垂直总排数</param>
        /// <param name="controls">需要与父控件对齐的控件</param>
        public void SetBatchSort(int verticalSort, int verticalQuantity, params Control[] controls)
        {
            for (int i = 0; i < controls.Length; i++)
            {
                if (controls[i] != null)
                {
                    SetArrange(controls[i], (i + 1), verticalSort, controls.Length, verticalQuantity);
                }
            }
        }

        /// <summary>
        /// 批量排序控件,并向相对位置移动控件,可用null占位
        /// </summary>
        /// <param name="verticalSort">当前垂直排</param>
        /// <param name="verticalQuantity">垂直总排数</param>
        /// <param name="direction">移动方向</param>
        /// <param name="distance">移动距离</param>
        /// <param name="controls">需要与父控件对齐的控件</param>
        public void SetBatchSort(int verticalSort, int verticalQuantity, int distance, Direction direction, params Control[] controls)
        {
            for (int i = 0; i < controls.Length; i++)
            {
                if (controls[i] != null)
                {
                    SetArrange(controls[i], (i + 1), verticalSort, controls.Length, verticalQuantity);
                    Move(controls[i], distance, direction);
                }
            }
        }



        #endregion
    }
}


#endif