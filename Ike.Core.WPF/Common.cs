using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace Ike.Core.WPF
{
    /// <summary>
    /// 通用方法类
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// 获取指定类型的所有子控件
        /// </summary>
        /// <typeparam name="T">目标控件类型（如 Rectangle）</typeparam>
        /// <param name="parent">父控件（任意 DependencyObject）</param>
        /// <param name="includeNested">是否递归查找嵌套子控件</param>
        public static T[] GetAllChildControls<T>(this DependencyObject parent, bool includeNested = true) where T : DependencyObject
        {
            var result = new List<T>();
            CollectControls(parent, result, includeNested);
            return [.. result];
        }

        /// <summary>
        /// 获取指定类型的所有子控件
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="parent">父控件</param>
        /// <param name="result">查找结果</param>
        /// <param name="includeNested">是否递归查找嵌套子控件</param>
        private static void CollectControls<T>(DependencyObject parent, List<T> result, bool includeNested) where T : DependencyObject
        {
            // 遍历直接子控件
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T matchedControl)
                {
                    result.Add(matchedControl);
                }
                // 递归查找（根据参数控制）
                if (includeNested)
                {
                    CollectControls(child, result, includeNested);
                }
            }
        }
    }
}
