#if NETFRAMEWORK

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ike.Standard.Forms
{
    /// <summary>
    /// 控件动画
    /// </summary>
    public partial class AnimationOperation
    {

        /// <summary>
        /// 控件执行内容记录,防止动画未执行完毕重复执行
        /// </summary>
        public static List<string> controlState = new List<string>();


        /// <summary>
        /// 缩放控件
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="speed">速度(越小越快)</param>
        /// <param name="count">控件动画完成所需要的次数</param>
        /// <param name="changeSize">每次动画执行的距离</param>
        /// <param name="select">选择控件缩放类型</param>
        /// <param name="edge">选择控件边</param>
        public static void Zoom(Control control, int speed, int count, int changeSize, ZoomSelect select, ZoomEdge edge)
        {
                string judge = control.Name + select + edge;
                //判断是否正在执行当前控件动画
                if (judge != null && !controlState.Contains(judge))
                {
                    //控件动画开启,添加当前控件内容
                    controlState.Add(judge);
                    Task thread = new Task(() =>
                    {
                        for (int i = 0; i < count; i++)
                        {
                            //放大
                            if (select == ZoomSelect.Enlarge)
                            {
                                if (edge == ZoomEdge.WidthAndHeight)
                                {
                                    control.Width += changeSize;
                                    control.Height += changeSize;
                                }
                                else if (edge == ZoomEdge.Width)
                                {
                                    control.Width += changeSize;
                                }
                                else if (edge == ZoomEdge.Height)
                                {
                                    control.Height += changeSize;
                                }
                            }
                            //缩小
                            else if (select == ZoomSelect.Narrow)
                            {
                                if (control.Width >= 0 && control.Height >= 0)
                                {
                                    if (edge == ZoomEdge.WidthAndHeight)
                                    {
                                        control.Width -= changeSize;
                                        control.Height -= changeSize;
                                    }
                                    else if (edge == ZoomEdge.Width)
                                    {
                                        control.Width -= changeSize;
                                    }
                                    else if (edge == ZoomEdge.Height)
                                    {
                                        control.Height -= changeSize;
                                    }
                                }
                            }
                            //放大后缩小
                            else if (select == ZoomSelect.LargeToSmall)
                            {
                                int location = changeSize / 2;
                                if (location == 0)
                                {
                                    location = 1;
                                }
                                //缩放宽高
                                if (edge == ZoomEdge.WidthAndHeight)
                                {
                                    for (int j = 0; j < count; j++)
                                    {
                                        control.Width += changeSize;
                                        control.Height += changeSize;
                                        if (j % 2 == 0)
                                        {
                                            control.Location = new System.Drawing.Point(control.Location.X - location, control.Location.Y - location);
                                        }
                                        Thread.Sleep(speed);
                                    }
                                    for (int j = 0; j < count; j++)
                                    {
                                        control.Width -= changeSize;
                                        control.Height -= changeSize;
                                        if (j % 2 == 0)
                                        {
                                            control.Location = new System.Drawing.Point(control.Location.X + location, control.Location.Y + location);
                                        }
                                        Thread.Sleep(speed);
                                    }
                                }
                                //缩放宽
                                else if (edge == ZoomEdge.Width)
                                {
                                    for (int j = 0; j < count; j++)
                                    {
                                        control.Width += changeSize;
                                        if (j % 2 == 0)
                                        {
                                            control.Location = new System.Drawing.Point(control.Location.X - location, control.Location.Y);
                                        }
                                        Thread.Sleep(speed);
                                    }
                                    for (int j = 0; j < count; j++)
                                    {
                                        control.Width -= changeSize;
                                        if (j % 2 == 0)
                                        {
                                            control.Location = new System.Drawing.Point(control.Location.X + location, control.Location.Y);
                                        }
                                        Thread.Sleep(speed);
                                    }
                                }
                                //缩放高
                                else if (edge == ZoomEdge.Height)
                                {
                                    for (int j = 0; j < count; j++)
                                    {
                                        control.Height += changeSize;
                                        if (j % 2 == 0)
                                        {
                                            control.Location = new System.Drawing.Point(control.Location.X, control.Location.Y - location);
                                        }
                                        Thread.Sleep(speed);
                                    }
                                    for (int j = 0; j < count; j++)
                                    {
                                        control.Height -= changeSize;
                                        if (j % 2 == 0)
                                        {
                                            control.Location = new System.Drawing.Point(control.Location.X, control.Location.Y + location);
                                        }
                                        Thread.Sleep(speed);
                                    }
                                }
                                i = count;
                            }
                            //缩小后放大
                            else if (select == ZoomSelect.SmallToLarge)
                            {
                                int runCount = 0;
                                int location = changeSize / 2;
                                if (location == 0)
                                {
                                    location = 1;
                                }
                                //缩放宽高
                                if (edge == ZoomEdge.WidthAndHeight)
                                {
                                    for (int j = 0; j < count; j++)
                                    {
                                        if (control.Width > 0 && control.Height > 0)
                                        {
                                            runCount++;
                                            control.Width -= changeSize;
                                            control.Height -= changeSize;
                                            if (j % 2 == 0)
                                            {
                                                control.Location = new System.Drawing.Point(control.Location.X + location, control.Location.Y + location);
                                            }
                                            Thread.Sleep(speed);
                                        }
                                    }
                                    for (int j = 0; j < runCount; j++)
                                    {
                                        control.Width += changeSize;
                                        control.Height += changeSize;
                                        if (j % 2 == 0)
                                        {
                                            control.Location = new System.Drawing.Point(control.Location.X - location, control.Location.Y - location);
                                        }
                                        Thread.Sleep(speed);
                                    }
                                }
                                //缩放宽
                                else if (edge == ZoomEdge.Width)
                                {
                                    for (int j = 0; j < count; j++)
                                    {
                                        if (control.Width > 0 && control.Height > 0)
                                        {
                                            runCount++;
                                            control.Width -= changeSize;
                                            if (j % 2 == 0)
                                            {
                                                control.Location = new System.Drawing.Point(control.Location.X + location, control.Location.Y);
                                            }
                                            Thread.Sleep(speed);
                                        }
                                    }
                                    for (int j = 0; j < runCount; j++)
                                    {
                                        control.Width += changeSize;
                                        if (j % 2 == 0)
                                        {
                                            control.Location = new System.Drawing.Point(control.Location.X - location, control.Location.Y);
                                        }
                                        Thread.Sleep(speed);
                                    }
                                }
                                //缩放高
                                if (edge == ZoomEdge.Height)
                                {
                                    for (int j = 0; j < count; j++)
                                    {
                                        if (control.Width > 0 && control.Height > 0)
                                        {
                                            runCount++;
                                            control.Height -= changeSize;
                                            if (j % 2 == 0)
                                            {
                                                control.Location = new System.Drawing.Point(control.Location.X, control.Location.Y + location);
                                            }
                                            Thread.Sleep(speed);
                                        }
                                    }
                                    for (int j = 0; j < runCount; j++)
                                    {
                                        control.Height += changeSize;
                                        if (j % 2 == 0)
                                        {
                                            control.Location = new System.Drawing.Point(control.Location.X, control.Location.Y - location);
                                        }
                                        Thread.Sleep(speed);
                                    }
                                }
                                i = count;
                            }
                            Thread.Sleep(speed);
                        }
                        //任务执行完成后移除控件判断内容
                        controlState.Remove(judge);
                    });
                    thread.Start();
                    Task task = new Task(() =>
                    {
                        thread.Wait();
                        if (judge != null && controlState.Contains(judge))
                        {
                            controlState.Remove(judge);
                        }
                    });
                    task.Start();
                }
        }

        /// <summary>
        /// 控件不可用时清空控件数据
        /// </summary>
        public static void ClearData()
        {
            controlState.Clear();
        }

       
        /// <summary>
        /// 点击控件动画
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="select">样式</param>
        public static void ClickControl(Control control, ClickSelect select)
        {
                if (select == ClickSelect.Slow)
                {
                    Zoom(control, 25, 6, 1, ZoomSelect.SmallToLarge, ZoomEdge.WidthAndHeight);
                }
                else if (select == ClickSelect.Fast)
                {
                    Zoom(control, 1, 5, 1, ZoomSelect.SmallToLarge, ZoomEdge.WidthAndHeight);
                }
                else if (select == ClickSelect.Emphatic)
                {
                    Zoom(control, 5, 10, 1, ZoomSelect.SmallToLarge, ZoomEdge.WidthAndHeight);
                }
                else if (select == ClickSelect.Effortlessly)
                {
                    Zoom(control, 1, 2, 1, ZoomSelect.SmallToLarge, ZoomEdge.WidthAndHeight);
                }
                else if (select == ClickSelect.Rapidly)
                {
                    Zoom(control, 2, 4, 3, ZoomSelect.SmallToLarge, ZoomEdge.WidthAndHeight);
                }
        }
    }
}


#endif