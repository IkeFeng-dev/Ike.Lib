#if NETFRAMEWORK
namespace Ike.Standard
{
    public partial class Regedit
    {
        /// <summary>
        /// 注册表常用地址
        /// </summary>
        public class Paths
        {
            /// <summary>
            /// 控制USB存储设备驱动程序的加载行为
            /// <list type="table">
            /// <item>
            /// <term>Start</term>
            /// <description>控制USB存储设备驱动程序的加载行为 值包括：
            ///     <list type="bullet">
            ///     <item>0 (SERVICE_BOOT_START): 启动时加载</item>
            ///     <item>1 (SERVICE_SYSTEM_START): 系统初始化时加载</item>
            ///     <item>2 (SERVICE_AUTO_START): 自动加载</item>
            ///     <item>3 (SERVICE_DEMAND_START): 按需加载</item>
            ///     <item>4 (SERVICE_DISABLED): 禁用</item>
            ///     </list>
            /// </description>
            /// </item>
            /// <item>
            /// <term>Type</term>
            /// <description>指示服务类型。值包括：
            ///     <list type="bullet">
            ///     <item>1 (SERVICE_KERNEL_DRIVER): 内核驱动程序。</item>
            ///     <item>2 (SERVICE_FILE_SYSTEM_DRIVER): 文件系统驱动程序，对于USBSTOR通常不适用</item>
            ///     </list>
            /// </description>
            /// </item>
            /// <item>
            /// <term>ErrorControl</term>
            /// <description>指定如果无法加载驱动程序，系统启动过程中的错误控制级别。值包括：
            ///     <list type="bullet">
            ///     <item>0 (SERVICE_ERROR_IGNORE): 忽略错误继续启动</item>
            ///     <item>1 (SERVICE_ERROR_NORMAL): 正常错误处理，显示消息框请求用户输入</item>
            ///     <item>2 (SERVICE_ERROR_SEVERE): 严重错误，系统尝试重新启动，使用最后一次成功的配置</item>
            ///     <item>3 (SERVICE_ERROR_CRITICAL): 关键错误，系统尝试重新启动，如果失败则停止</item>
            ///     </list>
            /// </description>
            /// </item>
            /// <item>
            /// <term>ImagePath</term>
            /// <description>指定服务的可执行文件的路径。例如：“\SystemRoot\System32\drivers\USBSTOR.SYS”</description>
            /// </item>
            /// <item>
            /// <term>DisplayName</term>
            /// <description>提供服务的友好名称。例如：“USB Mass Storage Driver”表示这是USB大容量存储设备的驱动程序</description>
            /// </item>
            /// <item>
            /// <term>Group</term>
            /// <description>指定驱动程序加载的顺序组。例如：“SCSI miniport”，表示该驱动程序在SCSI小型端口驱动程序组中加载</description>
            /// </item>
            /// </list>
            /// </summary>

            public const string USBDriveLoad = @"SYSTEM\CurrentControlSet\Services\USBSTOR";
            /// <summary>
            /// 定义在系统启动时自动执行的程序列表
            /// </summary>
            public const string AutoRun = @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run";
            /// <summary>
            /// 文件右键
            /// </summary>
            public const string FileRightKey = @"HKEY_CLASSES_ROOT\*";
            /// <summary>
            /// 文件右键Shell项
            /// </summary>
            public const string FileRightKeyShell = @"HKEY_CLASSES_ROOT\*\shell";
            /// <summary>
            /// 文件夹右键
            /// </summary>
            public const string DirectoryRightKey = @"HKEY_CLASSES_ROOT\Directory";
            /// <summary>
            /// 文件夹右键Shell项
            /// </summary>
            public const string DirectoryRightKeyShell = @"HKEY_CLASSES_ROOT\Directory\shell";
            /// <summary>
            /// 空白处右键
            /// </summary>
            public const string EmptyRightKey = @"HKEY_CLASSES_ROOT\Directory\Background";
            /// <summary>
            /// 空白处右键Shell项
            /// </summary>
            public const string EmptyRightKeyShell = @"HKEY_CLASSES_ROOT\Directory\Background\shell";
            /// <summary>
            /// 使用的USB设备
            /// </summary>
            public const string USBRecord = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceClasses\{a5dcbf10-6530-11d2-901f-00c04fb951ed}";
            /// <summary>
            /// 近期打开的文件记录
            /// </summary>
            public const string RecentlyOpenedFile = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU\*";
            /// <summary>
            /// 网卡信息
            /// </summary>
            public const string NetworkCard = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}";

        }
    }
}
#endif