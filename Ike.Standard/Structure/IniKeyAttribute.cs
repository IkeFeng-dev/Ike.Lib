using System;
namespace Ike.Standard
{
    /// <summary>
    /// 重定向节点键名特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IniKeyAttribute : Attribute
    {
        /// <summary>
        /// 键名称
        /// </summary>
        public string KeyName { get; }
        /// <summary>
        /// 重定向节点键名特性
        /// </summary>
        /// <param name="keyName">键名</param>
        public IniKeyAttribute(string keyName)
        {
            KeyName = keyName;
        }
    }
}
