using System;
namespace Ike.Standard
{

    /// <summary>
    /// 定义节名特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IniSectionAttribute : Attribute
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string SectionName { get; }
        /// <summary>
        /// 定义节名特性
        /// </summary>
        /// <param name="sectionName">节点名称</param>
        public IniSectionAttribute(string sectionName)
        {
            SectionName = sectionName;
        }
    }

}
