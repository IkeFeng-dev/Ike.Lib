using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ike
{
    /// <summary>
    /// MySQL数据库操作类
    /// </summary>
    public class MySQL : IDisposable
    {
        /// <summary>
        /// DataAdapter锁
        /// </summary>
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private readonly string connectionString;
        /// <summary>
        /// 心跳包标志
        /// </summary>
        private bool StartHeartBeatFlag { get; set; } = false;
        /// <summary>
        /// 数据库的连接对象
        /// </summary>
        public MySqlConnection MySQLConnection { get; private set; }

        /// <summary>
        /// 表示 MySQL 数据库的字符集
        /// </summary>
        public enum CharSet
        {
            /// <summary>
            /// utf8 字符集
            /// </summary>
            utf8,
            /// <summary>
            /// utf8mb4 字符集,支持更多的 Unicode 字符
            /// </summary>
            utf8mb4,
            /// <summary>
            /// latin1 字符集
            /// </summary>
            latin1,
            /// <summary>
            /// ascii 字符集
            /// </summary>
            ascii
        }

        /// <summary>
        /// 表示 MySQL 数据库的 SSL 模式
        /// </summary>
        public enum SslMode
        {
            /// <summary>
            /// 不使用 SSL
            /// </summary>
            None,
            /// <summary>
            /// 如果可用则使用 SSL
            /// </summary>
            Preferred,
            /// <summary>
            /// 必须使用 SSL
            /// </summary>
            Required,
            /// <summary>
            /// SSL 加密但不验证证书
            /// </summary>
            VerifyCA,
            /// <summary>
            /// SSL 加密并验证证书
            /// </summary>
            VerifyFull
        }

        /// <summary>
        /// 通过连接字符串构造,调用<see cref="Instantiation"/>方法实例化数据库对象
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        public MySQL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// 构造MySQL连接字符串,需调用<see cref="ChangeDatabase(string)"/>选定数据库,调用<see cref="Instantiation"/>或<see cref="Open"/>方法实例化数据库对象
        /// </summary>
        /// <param name="server">数据库服务器地址</param>
        /// <param name="user">数据库用户名</param>
        /// <param name="password">数据库密码</param>
        /// <param name="port">数据库端口号</param>
        /// <param name="charset">字符集</param>
        /// <param name="allowUserVariables">是否允许用户变量</param>
        /// <param name="useCompression">是否使用压缩</param>
        /// <param name="connectionTimeout">连接超时时间</param>
        /// <param name="defaultCommandTimeout">命令执行超时时间</param>
        /// <param name="sslMode">SSL 模式</param>
        public MySQL(string server, string user, string password, uint port = 3306, CharSet charset = CharSet.utf8, bool allowUserVariables = false, bool useCompression = false, uint connectionTimeout = 15, uint defaultCommandTimeout = 30, SslMode sslMode = SslMode.None)
        {
            connectionString = $"Server={server};User={user};Password={password};Port={port};Charset={charset};AllowUserVariables={allowUserVariables};UseCompression={useCompression};Connection Timeout={connectionTimeout};Default Command Timeout={defaultCommandTimeout};SslMode={sslMode};";
        }

        /// <summary>
        /// 构造MySQL连接字符串,调用<see cref="Instantiation"/>或<see cref="Open"/>方法实例化数据库对象
        /// </summary>
        /// <param name="server">数据库服务器地址</param>
        /// <param name="database">数据库名称</param>
        /// <param name="user">数据库用户名</param>
        /// <param name="password">数据库密码</param>
        /// <param name="port">数据库端口号</param>
        /// <param name="charset">字符集</param>
        /// <param name="allowUserVariables">是否允许用户变量</param>
        /// <param name="useCompression">是否使用压缩</param>
        /// <param name="connectionTimeout">连接超时时间</param>
        /// <param name="defaultCommandTimeout">命令执行超时时间</param>
        /// <param name="sslMode">SSL 模式</param>
        public MySQL(string server, string database, string user, string password, uint port = 3306, CharSet charset = CharSet.utf8, bool allowUserVariables = false, bool useCompression = false, uint connectionTimeout = 15, uint defaultCommandTimeout = 30, SslMode sslMode = SslMode.None)
        {
            connectionString = $"Server={server};Database={database};User={user};Password={password};Port={port};Charset={charset};AllowUserVariables={allowUserVariables};UseCompression={useCompression};Connection Timeout={connectionTimeout};Default Command Timeout={defaultCommandTimeout};SslMode={sslMode};";
        }

        /// <summary>
        /// 实例化数据库连接对象,数据库使用完成后调用<see cref="Dispose(bool)"/>方法释放资源
        /// </summary>
        public void Instantiation()
        {
            if (MySQLConnection == null)
            {
                MySQLConnection = new MySqlConnection(connectionString);
            }
            else
            {
                throw new InvalidOperationException("数据库连接已存在");
            }
        }

        /// <summary>
        /// 停止心跳包保持连接
        /// </summary>
        public void StopHeartBeat()
        {
            StartHeartBeatFlag = false;
        }

        /// <summary>
        /// 启动心跳包保持连接
        /// </summary>
        /// <param name="minuteInterval">间隔时间(分钟)</param>
        public void StartHeartBeat(int minuteInterval)
        {
            if (StartHeartBeatFlag)
            {
                return;
            }
            if (minuteInterval <= 0)
            {
                throw new ArgumentException("参数值不能小于1", nameof(minuteInterval));
            }
            Task.Run(async () =>
            {
                while (StartHeartBeatFlag)
                {
                    try
                    {
                        await Task.Delay(minuteInterval * 60 * 1000);
                        ExecuteScalar("SELECT 'OK';");
                    }
                    catch { }
                }
            });
        }

        /// <summary>
        /// 释放数据库资源的实现
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (MySQLConnection != null)
                {
                    MySQLConnection.Dispose();
                    MySQLConnection = null;
                }
            }
        }

        /// <summary>
        /// 释放数据库资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 添加参数到命令
        /// </summary>
        /// <param name="command">MySqlCommand 对象</param>
        /// <param name="parameters">参数字典</param>
        private void AddParameters(MySqlCommand command, Dictionary<string, object> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }
        }

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        public void Open()
        {
            if (MySQLConnection == null)
            {
                Instantiation();
            }
            if (MySQLConnection.State == ConnectionState.Closed)
            {
                MySQLConnection.Open();
            }
        }

        /// <summary>
        /// 异步打开数据库连接
        /// </summary>
        public async Task OpenAsync()
        {
            if (MySQLConnection == null)
            {
                Instantiation();
            }
            if (MySQLConnection.State == ConnectionState.Closed)
            {
                await MySQLConnection.OpenAsync();
            }
        }

        /// <summary>
        /// 断开数据库连接
        /// </summary>
        public void Close()
        {
            if (MySQLConnection.State == ConnectionState.Open)
            {
                MySQLConnection.Close();
            }
        }

        /// <summary>
        /// 异步断开数据库连接
        /// </summary>
        public async Task CloseAsync()
        {
            if (MySQLConnection.State == ConnectionState.Open)
            {
                await MySQLConnection.CloseAsync();
            }
        }

        /// <summary>
        /// 切换到指定的数据库
        /// </summary>
        /// <param name="databaseName">目标数据库名称</param>
        public void ChangeDatabase(string databaseName)
        {
            if (MySQLConnection.State == ConnectionState.Open)
            {
                MySQLConnection.ChangeDatabase(databaseName);
            }
            else
            {
                throw new InvalidOperationException("数据库连接未打开");
            }
        }

        /// <summary>
        /// 异步切换到指定的数据库
        /// </summary>
        /// <param name="databaseName">目标数据库名称</param>
        /// <returns>任务</returns>
        public async Task ChangeDatabaseAsync(string databaseName)
        {
            if (MySQLConnection.State == ConnectionState.Open)
            {
                await Task.Run(() => MySQLConnection.ChangeDatabase(databaseName));
            }
            else
            {
                throw new InvalidOperationException("数据库连接未打开");
            }
        }

        /// <summary>
        /// 执行非查询的SQL命令
        /// </summary>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">参数字典, 可为null</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            using (var command = new MySqlCommand(query, MySQLConnection))
            {
                AddParameters(command, parameters);
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 异步执行非查询的SQL命令
        /// </summary>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">参数字典, 可为null</param>
        /// <returns>受影响的行数</returns>
        public async Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object> parameters = null)
        {
            using (var command = new MySqlCommand(query, MySQLConnection))
            {
                AddParameters(command, parameters);
                return await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// 执行标量SQL命令
        /// </summary>
        /// <param name="query">SQL 查询语句</param>
        /// <param name="parameters">参数字典, 可为null</param>
        /// <returns>查询结果的第一行第一列的值</returns>
        public object ExecuteScalar(string query, Dictionary<string, object> parameters = null)
        {
            using (var command = new MySqlCommand(query, MySQLConnection))
            {
                AddParameters(command, parameters);
                return command.ExecuteScalar();
            }
        }

        /// <summary>
        /// 异步执行标量SQL命令
        /// </summary>
        /// <param name="query">SQL 查询语句</param>
        /// <param name="parameters">参数字典, 可为null</param>
        /// <returns>查询结果的第一行第一列的值</returns>
        public async Task<object> ExecuteScalarAsync(string query, Dictionary<string, object> parameters = null)
        {
            using (var command = new MySqlCommand(query, MySQLConnection))
            {
                AddParameters(command, parameters);
                return await command.ExecuteScalarAsync();
            }
        }

        /// <summary>
        /// 执行查询SQL命令并返回<see cref="DataTable"/>数据表
        /// </summary>
        /// <param name="query">SQL 查询语句</param>
        /// <param name="parameters">参数字典, 可为null</param>
        /// <returns>查询结果的<see cref="DataTable"/></returns>
        public DataTable ExecuteQuery(string query, Dictionary<string, object> parameters = null)
        {
            // 使用 SemaphoreSlim 来同步数据库访问
            _semaphore.Wait();  
            try
            {
                using (var command = new MySqlCommand(query, MySQLConnection))
                {
                    AddParameters(command, parameters);
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            finally
            {
                // 释放锁
                _semaphore.Release();  
            }
        }


        /// <summary>
        /// 异步执行查询SQL命令并返回<see cref="DataTable"/>数据表
        /// </summary>
        /// <param name="query">SQL 查询语句</param>
        /// <param name="parameters">参数字典, 可为null</param>
        /// <returns>查询结果的<see cref="DataTable"/></returns>
        public async Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object> parameters = null)
        {
            // 异步等待锁
            await _semaphore.WaitAsync();  
            try
            {
                using (var command = new MySqlCommand(query, MySQLConnection))
                {
                    AddParameters(command, parameters);
                    // 异步填充数据表
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        // 使调用者返回到调用线程，以避免阻塞
                        await Task.Yield();
                        // 仍然是同步方法
                        adapter.Fill(dataTable);  
                        return dataTable;
                    }
                }
            }
            finally
            {
                // 释放锁
                _semaphore.Release();  
            }
        }

        /// <summary>
        /// 检查指定的数据库是否存在
        /// </summary>
        /// <param name="dataBaseName">数据库名</param>
        /// <returns>是否存在</returns>
        public bool DataBaseExists(string dataBaseName)
        {
            string query = $"SHOW DATABASES WHERE `Database` = @database;";
            var parameters = new Dictionary<string, object> { { "@database", dataBaseName} };
            var result = ExecuteScalar(query, parameters);
            if (result == null)
            {
                return false;
            }
            return result.ToString().Equals(dataBaseName,StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 异步检查指定的数据库是否存在
        /// </summary>
        /// <param name="dataBaseName">数据库名</param>
        /// <returns>是否存在</returns>
        public async Task<bool> DataBaseExistsAsync(string dataBaseName)
        {
            string query = $"SHOW DATABASES WHERE `Database` = @database;";
            var parameters = new Dictionary<string, object> { { "@database", dataBaseName } };
            var result = await ExecuteScalarAsync(query, parameters);
            if (result == null)
            {
                return false;
            }
            return result.ToString().Equals(dataBaseName, StringComparison.OrdinalIgnoreCase);
        }

         /// <summary>
        /// 检查指定的表是否存在
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>是否存在</returns>
        public bool TableExists(string tableName)
        {
            string query = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = @database AND table_name = @tableName";
            var parameters = new Dictionary<string, object> { { "@database", MySQLConnection.Database }, { "@tableName", tableName } };
            var result = ExecuteScalar(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// 异步检查指定的表是否存在
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>是否存在</returns>
        public async Task<bool> TableExistsAsync(string tableName)
        {
            string query = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = @database AND table_name = @tableName";
            var parameters = new Dictionary<string, object> { { "@database", MySQLConnection.Database }, { "@tableName", tableName } };
            var result = await ExecuteScalarAsync(query, parameters);
            return Convert.ToInt32(result) > 0;
        }


        /// <summary>
        /// 获取指定表的所有列名和类型
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>列名和类型的字典</returns>
        public Dictionary<string, string> GetTableColumns(string tableName)
        {
            string query = "SELECT column_name, data_type FROM information_schema.columns WHERE table_schema = @database AND table_name = @tableName";
            var parameters = new Dictionary<string, object> { { "@database", MySQLConnection.Database }, { "@tableName", tableName } };
            var dataTable = ExecuteQuery(query, parameters);
            var columns = new Dictionary<string, string>();
            foreach (DataRow row in dataTable.Rows)
            {
                columns.Add(row["column_name"].ToString(), row["data_type"].ToString());
            }
            return columns;
        }

        /// <summary>
        /// 异步获取指定表的所有列名和类型
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>列名和类型的字典</returns>
        public async Task<Dictionary<string, string>> GetTableColumnsAsync(string tableName)
        {
            string query = "SELECT column_name, data_type FROM information_schema.columns WHERE table_schema = @database AND table_name = @tableName";
            var parameters = new Dictionary<string, object> { { "@database", MySQLConnection.Database }, { "@tableName", tableName } };
            var dataTable = await ExecuteQueryAsync(query, parameters);
            var columns = new Dictionary<string, string>();
            foreach (DataRow row in dataTable.Rows)
            {
                columns.Add(row["column_name"].ToString(), row["data_type"].ToString());
            }
            return columns;
        }

        /// <summary>
        /// 获取数据库服务器的版本信息
        /// </summary>
        /// <returns>数据库服务器版本信息</returns>
        public string GetServerVersion()
        {
            return MySQLConnection.ServerVersion;
        }

        /// <summary>
        /// 异步获取数据库服务器的版本信息
        /// </summary>
        /// <returns>数据库服务器版本信息</returns>
        public async Task<string> GetServerVersionAsync()
        {
            return await Task.FromResult(MySQLConnection.ServerVersion);
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>受影响的行数</returns>
        public int ExecuteStoredProcedure(string procedureName, Dictionary<string, object> parameters)
        {
            using (var command = new MySqlCommand(procedureName, MySQLConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                AddParameters(command, parameters);
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 异步执行存储过程
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>受影响的行数</returns>
        public async Task<int> ExecuteStoredProcedureAsync(string procedureName, Dictionary<string, object> parameters)
        {
            using (var command = new MySqlCommand(procedureName, MySQLConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                AddParameters(command, parameters);
                return await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// 获取数据库大小
        /// </summary>
        /// <returns>数据库大小(byte)</returns>
        public double GetDatabaseSize()
        {
            string query = "SELECT SUM(data_length + index_length) AS 'DBSize' FROM information_schema.tables WHERE table_schema = @database";
            var parameters = new Dictionary<string, object> { { "@database", MySQLConnection.Database } };
            var result = ExecuteScalar(query, parameters);
            return Convert.ToDouble(result);
        }

        /// <summary>
        /// 异步获取数据库大小
        /// </summary>
        /// <returns>数据库大小(byte)</returns>
        public async Task<double> GetDatabaseSizeAsync()
        {
            string query = "SELECT SUM(data_length + index_length) AS 'DBSize' FROM information_schema.tables WHERE table_schema = @database";
            var parameters = new Dictionary<string, object> { { "@database", MySQLConnection.Database } };
            var result = await ExecuteScalarAsync(query, parameters);
            return Convert.ToDouble(result);
        }

        /// <summary>
        /// 获取指定表的大小(byte)
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>表大小(byte)</returns>
        public double GetTableSize(string tableName)
        {
            string query = "SELECT (data_length + index_length)  AS 'TableSize' FROM information_schema.tables WHERE table_schema = @database AND table_name = @tableName";
            var parameters = new Dictionary<string, object> { { "@database", MySQLConnection.Database }, { "@tableName", tableName } };
            var result = ExecuteScalar(query, parameters);
            return Convert.ToDouble(result);
        }

        /// <summary>
        /// 异步获取指定表的大小(byte)
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>表大小(byte)</returns>
        public async Task<double> GetTableSizeAsync(string tableName)
        {
            string query = "SELECT (data_length + index_length) AS 'TableSize' FROM information_schema.tables WHERE table_schema = @database AND table_name = @tableName";
            var parameters = new Dictionary<string, object> { { "@database", MySQLConnection.Database }, { "@tableName", tableName } };
            var result = await ExecuteScalarAsync(query, parameters);
            return Convert.ToDouble(result);
        }

        /// <summary>
        /// 获取指定表的行数
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>行数</returns>
        public int GetRowCount(string tableName)
        {
            string query = $"SELECT COUNT(*) FROM {tableName}";
            var result = ExecuteScalar(query, null);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 异步获取指定表的行数
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>行数</returns>
        public async Task<int> GetRowCountAsync(string tableName)
        {
            string query = $"SELECT COUNT(*) FROM {tableName}";
            var result = await ExecuteScalarAsync(query, null);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 检查指定表中是否存在某列
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnName">列名</param>
        /// <returns>是否存在</returns>
        public bool ColumnExists(string tableName, string columnName)
        {
            string query = "SELECT COUNT(*) FROM information_schema.columns WHERE table_schema = @database AND table_name = @tableName AND column_name = @columnName";
            var parameters = new Dictionary<string, object> { { "@database", MySQLConnection.Database }, { "@tableName", tableName }, { "@columnName", columnName } };
            var result = ExecuteScalar(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// 异步检查指定表中是否存在某列
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnName">列名</param>
        /// <returns>是否存在</returns>
        public async Task<bool> ColumnExistsAsync(string tableName, string columnName)
        {
            string query = "SELECT COUNT(*) FROM information_schema.columns WHERE table_schema = @database AND table_name = @tableName AND column_name = @columnName";
            var parameters = new Dictionary<string, object> { { "@database", MySQLConnection.Database }, { "@tableName", tableName }, { "@columnName", columnName } };
            var result = await ExecuteScalarAsync(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// 获取指定表的所有索引
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>索引名列表</returns>
        public List<string> GetTableIndexes(string tableName)
        {
            string query = "SELECT index_name FROM information_schema.statistics WHERE table_schema = @database AND table_name = @tableName";
            var parameters = new Dictionary<string, object> { { "@database", MySQLConnection.Database }, { "@tableName", tableName } };
            var dataTable = ExecuteQuery(query, parameters);
            var indexes = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                indexes.Add(row["index_name"].ToString());
            }
            return indexes;
        }

        /// <summary>
        /// 异步获取指定表的所有索引
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>索引名列表</returns>
        public async Task<List<string>> GetTableIndexesAsync(string tableName)
        {
            string query = "SELECT index_name FROM information_schema.statistics WHERE table_schema = @database AND table_name = @tableName";
            var parameters = new Dictionary<string, object> { { "@database", MySQLConnection.Database }, { "@tableName", tableName } };
            var dataTable = await ExecuteQueryAsync(query, parameters);
            var indexes = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                indexes.Add(row["index_name"].ToString());
            }
            return indexes;
        }

        /// <summary>
        /// 获取当前数据库中的所有表名称
        /// </summary>
        /// <returns>表名称列表</returns>
        public List<string> GetAllTableNames()
        {
            string query = "SELECT table_name FROM information_schema.tables WHERE table_schema = @database";
            var parameters = new Dictionary<string, object> { { "@database", MySQLConnection.Database } };
            var dataTable = ExecuteQuery(query, parameters);
            var tableNames = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                tableNames.Add(row["table_name"].ToString());
            }
            return tableNames;
        }

        /// <summary>
        /// 异步获取当前数据库中的所有表名称
        /// </summary>
        /// <returns>表名称列表</returns>
        public async Task<List<string>> GetAllTableNamesAsync()
        {
            string query = "SELECT table_name FROM information_schema.tables WHERE table_schema = @database";
            var parameters = new Dictionary<string, object> { { "@database", MySQLConnection.Database } };
            var dataTable = await ExecuteQueryAsync(query, parameters);
            var tableNames = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                tableNames.Add(row["table_name"].ToString());
            }
            return tableNames;
        }

        /// <summary>
        /// 获取当前数据库的字符集
        /// </summary>
        /// <returns>字符集名称</returns>
        public string GetDatabaseCharset()
        {
            string query = "SELECT @@character_set_database";
            var result = ExecuteScalar(query, null);
            return result.ToString();
        }

        /// <summary>
        /// 异步获取当前数据库的字符集
        /// </summary>
        /// <returns>字符集名称</returns>
        public async Task<string> GetDatabaseCharsetAsync()
        {
            string query = "SELECT @@character_set_database";
            var result = await ExecuteScalarAsync(query, null);
            return result.ToString();
        }

        /// <summary>
        /// 获取当前用户的权限
        /// </summary>
        /// <returns>用户权限列表</returns>
        public List<string> GetCurrentUserPrivileges()
        {
            string query = "SHOW GRANTS FOR CURRENT_USER";
            var dataTable = ExecuteQuery(query, null);
            var privileges = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    privileges.Add(item.ToString());
                }
            }
            return privileges;
        }

        /// <summary>
        /// 异步获取当前用户的权限
        /// </summary>
        /// <returns>用户权限列表</returns>
        public async Task<List<string>> GetCurrentUserPrivilegesAsync()
        {
            string query = "SHOW GRANTS FOR CURRENT_USER";
            var dataTable = await ExecuteQueryAsync(query, null);
            var privileges = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    privileges.Add(item.ToString());
                }
            }
            return privileges;
        }

        /// <summary>
        /// 获取服务器上所有数据库的列表
        /// </summary>
        /// <returns>数据库名称列表</returns>
        public List<string> GetDatabaseList()
        {
            string query = "SHOW DATABASES";
            var dataTable = ExecuteQuery(query, null);
            var databases = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                databases.Add(row[0].ToString());
            }
            return databases;
        }

        /// <summary>
        /// 异步获取服务器上所有数据库的列表
        /// </summary>
        /// <returns>数据库名称列表</returns>
        public async Task<List<string>> GetDatabaseListAsync()
        {
            string query = "SHOW DATABASES";
            var dataTable = await ExecuteQueryAsync(query, null);
            var databases = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                databases.Add(row[0].ToString());
            }
            return databases;
        }

        /// <summary>
        /// 获取指定表的外键约束信息
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>外键约束信息列表</returns>
        public List<(string ConstraintName, string ColumnName, string ReferencedTableName, string ReferencedColumnName)> GetTableForeignKeys(string tableName)
        {
            string query = @"
                            SELECT
                                constraint_name,
                                column_name,
                                referenced_table_name,
                                referenced_column_name
                            FROM
                                information_schema.key_column_usage
                            WHERE
                                table_schema = @database
                                AND table_name = @tableName
                                AND referenced_table_name IS NOT NULL";
            var parameters = new Dictionary<string, object>
            {
                { "@database", MySQLConnection.Database },
                { "@tableName", tableName }
            };
            var dataTable = ExecuteQuery(query, parameters);
            var foreignKeys = new List<(string ConstraintName, string ColumnName, string ReferencedTableName, string ReferencedColumnName)>();
            foreach (DataRow row in dataTable.Rows)
            {
                foreignKeys.Add((
                    row["constraint_name"].ToString(),
                    row["column_name"].ToString(),
                    row["referenced_table_name"].ToString(),
                    row["referenced_column_name"].ToString()
                ));
            }
            return foreignKeys;
        }

        /// <summary>
        /// 异步获取指定表的外键约束信息
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>外键约束信息列表</returns>
        public async Task<List<(string ConstraintName, string ColumnName, string ReferencedTableName, string ReferencedColumnName)>> GetTableForeignKeysAsync(string tableName)
        {
            string query = @"
                            SELECT
                                constraint_name,
                                column_name,
                                referenced_table_name,
                                referenced_column_name
                            FROM
                                information_schema.key_column_usage
                            WHERE
                                table_schema = @database
                                AND table_name = @tableName
                                AND referenced_table_name IS NOT NULL";
            var parameters = new Dictionary<string, object>
            {
                { "@database", MySQLConnection.Database },
                { "@tableName", tableName }
            };
            var dataTable = await ExecuteQueryAsync(query, parameters);
            var foreignKeys = new List<(string ConstraintName, string ColumnName, string ReferencedTableName, string ReferencedColumnName)>();
            foreach (DataRow row in dataTable.Rows)
            {
                foreignKeys.Add((
                    row["constraint_name"].ToString(),
                    row["column_name"].ToString(),
                    row["referenced_table_name"].ToString(),
                    row["referenced_column_name"].ToString()
                ));
            }
            return foreignKeys;
        }

        /// <summary>
        /// 获取指定表的创建语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>创建语句</returns>
        public string GetTableCreateStatement(string tableName)
        {
            string query = "SHOW CREATE TABLE " + tableName;
            var dataTable = ExecuteQuery(query, null);
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0]["Create Table"].ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 异步获取指定表的创建语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>创建语句</returns>
        public async Task<string> GetTableCreateStatementAsync(string tableName)
        {
            string query = "SHOW CREATE TABLE " + tableName;
            var dataTable = await ExecuteQueryAsync(query, null);
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0]["Create Table"].ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取指定表的存储引擎
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>存储引擎</returns>
        public string GetTableEngine(string tableName)
        {
            string query = "SELECT ENGINE FROM information_schema.tables WHERE table_schema = @database AND table_name = @tableName";
            var parameters = new Dictionary<string, object> { { "@database", MySQLConnection.Database }, { "@tableName", tableName } };
            var result = ExecuteScalar(query, parameters);
            return result.ToString();
        }

        /// <summary>
        /// 异步获取指定表的存储引擎
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>存储引擎</returns>
        public async Task<string> GetTableEngineAsync(string tableName)
        {
            string query = "SELECT ENGINE FROM information_schema.tables WHERE table_schema = @database AND table_name = @tableName";
            var parameters = new Dictionary<string, object> { { "@database", MySQLConnection.Database }, { "@tableName", tableName } };
            var result = await ExecuteScalarAsync(query, parameters);
            return result.ToString();
        }


        /// <summary>
        /// 获取映射对象值
        /// </summary>
        /// <param name="colMap">映射对象</param>
        /// <param name="item">值数据</param>
        /// <returns></returns>
        private object GetValueFromMap(object colMap, object item)
        {
            if (colMap is PropertyInfo propertyInfo)
            {
                return propertyInfo.GetValue(item);
            }
            else if (colMap is FieldInfo fieldInfo)
            {
                return fieldInfo.GetValue(item);
            }
            else
            {
                return colMap;
            }
        }




        /// <summary>
        /// 类结构集合数据批量插入到数据库
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="columnTypeMap">
        /// 数据库字段与数据类型的映射字典，键值对含义如下：<br/><br/>
        /// 
        /// 【键 (<see cref="string"/>)】：数据库实际字段名<br/>
        /// 示例："name", "age", "email", "created_time"<br/><br/>
        /// 
        /// 【值 (<see cref="object"/>)】：支持三种类型：<br/><br/>
        /// 1. <see cref="PropertyInfo"/>：实体类属性反射对象，从数据对象中获取属性值<br/>
        ///    示例：<see cref="Type.GetProperty(string)"/><br/>
        ///     
        /// 2. <see cref="FieldInfo"/>：实体类字段反射对象，从数据对象中获取字段值<br/>
        ///    示例：<see cref="Type.GetField(string)"/><br/>
        ///     
        /// 3. 常量值 (<see cref="string"/>/<see cref="object"/>)：固定值，所有记录插入相同的值<br/>
        ///    示例："<see  langword="默认值"/> "、<see cref="DateTime"/>、<see langword="123"/>、<see langword="true"/>、<see langword="null"/>等等数据库对应的类型<br/>
        ///    用途：为所有插入记录设置相同的固定值<br/><br/>
        ///     
        /// 【使用示例】：<br/><br/>
        /// 
        /// 1. 映射实体属性到数据库字段：<br/>
        /// <code>
        /// <see langword=" var columnMap = new Dictionary&lt;string, object&gt;"/><br/>
        /// <see langword=" {"/><br/>
        /// <see langword="     { &quot;name&quot;, typeof(Person).GetProperty(&quot;Name&quot;) },"/><br/>
        /// <see langword="     { &quot;age&quot;, typeof(Person).GetProperty(&quot;Age&quot;) },"/><br/>
        /// <see langword="     { &quot;email&quot;, typeof(Person).GetProperty(&quot;Email&quot;) },"/><br/>
        /// <see langword="     { &quot;status&quot;, &quot;active&quot; },"/><br/>
        /// <see langword="     { &quot;created_time&quot;, DateTime.Now }"/><br/>
        /// <see langword=" };"/><br/>
        /// </code><br/><br/>
        /// 
        /// 2. 混合使用反射和常量值：<br/>
        /// <code>
        /// <see langword="var columnMap = new Dictionary&lt;string, object&gt;"/><br/>
        /// <see langword="{"/><br/>
        /// <see langword="    { &quot;user_name&quot;, typeof(User).GetProperty(&quot;UserName&quot;) },"/><br/>
        /// <see langword="    { &quot;department&quot;, &quot;技术部&quot; },"/><br/>
        /// <see langword="    { &quot;is_deleted&quot;, false },"/><br/>
        /// <see langword="    { &quot;create_by&quot;, currentUserId },"/><br/>
        /// <see langword="    { &quot;create_time&quot;, DateTime.Now }"/><br/>
        /// <see langword="};"/><br/>
        /// 
        /// </code><br/><br/>
        /// 3. 使用 <see cref="FieldInfo"/>（适用于字段而非属性）：<br/>
        /// <code>
        /// <see langword="var columnMap = new Dictionary&lt;string, object&gt;"/><br/>
        /// <see langword="{"/><br/>
        /// <see langword="    { &quot;name&quot;, typeof(Person).GetField(&quot;name&quot;) },"/><br/>
        /// <see langword="    { &quot;age&quot;, typeof(Person).GetField(&quot;age&quot;) }"/><br/>
        /// <see langword="};"/><br/>
        /// </code>
        /// </param>
        /// <param name="datas">要插入的数据集合,数据类型只能是<see langword="class"/></param>
        /// <returns>受影响的行数</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public int BatchInsert<T>(string tableName, Dictionary<string, object> columnTypeMap, IEnumerable<T> datas) where T : class
        {
            if (datas == null)
            {
                throw new ArgumentNullException(nameof(datas));
            }
            if (columnTypeMap == null)
            {
                throw new ArgumentNullException(nameof(columnTypeMap));
            }
            if (columnTypeMap.Count == 0)
            {
                throw new ArgumentException($"'{nameof(columnTypeMap)}' 列映射的长度不能为0");
            }
            if (!datas.Any())
            {
                return 0;
            }
            // 构建SQL第一部分
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"INSERT INTO `{tableName}` (");
            sqlBuilder.Append(string.Join(", ", columnTypeMap.Keys.Select(col => $"`{col}`")));
            sqlBuilder.Append(") VALUES ");

            var parameters = new Dictionary<string, object>();
            var valueStatements = new List<string>();
            int itemIndex = 0;

            foreach (var item in datas)
            {
                var paramList = new List<string>();

                // 为当前数据项的每一列生成参数
                foreach (var kv in columnTypeMap)
                {
                    string columnName = kv.Key;
                    object colMap = kv.Value;

                    // 生成唯一的参数名
                    string paramName = $"@p_{itemIndex}_{columnName}";
                    paramList.Add(paramName);
                    object value = GetValueFromMap(colMap, item);
                    parameters[paramName] = value ?? DBNull.Value;
                }

                // 添加 VALUES 子句
                valueStatements.Add($"({string.Join(", ", paramList)})");
                itemIndex++;
            }

            // 拼接所有VALUES子句
            sqlBuilder.Append(string.Join(", ", valueStatements));

            string sql = sqlBuilder.ToString();
            return ExecuteNonQuery(sql, parameters);
        }



        /// <summary>
        /// 异步执行类结构集合数据批量插入到数据库
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="columnTypeMap">
        /// 数据库字段与数据类型的映射字典，键值对含义如下：<br/><br/>
        /// 
        /// 【键 (<see cref="string"/>)】：数据库实际字段名<br/>
        /// 示例："name", "age", "email", "created_time"<br/><br/>
        /// 
        /// 【值 (<see cref="object"/>)】：支持三种类型：<br/><br/>
        /// 1. <see cref="PropertyInfo"/>：实体类属性反射对象，从数据对象中获取属性值<br/>
        ///    示例：<see cref="Type.GetProperty(string)"/><br/>
        ///     
        /// 2. <see cref="FieldInfo"/>：实体类字段反射对象，从数据对象中获取字段值<br/>
        ///    示例：<see cref="Type.GetField(string)"/><br/>
        ///     
        /// 3. 常量值 (<see cref="string"/>/<see cref="object"/>)：固定值，所有记录插入相同的值<br/>
        ///    示例："<see  langword="默认值"/> "、<see cref="DateTime"/>、<see langword="123"/>、<see langword="true"/>、<see langword="null"/>等等数据库对应的类型<br/>
        ///    用途：为所有插入记录设置相同的固定值<br/><br/>
        ///     
        /// 【使用示例】：<br/><br/>
        /// 
        /// 1. 映射实体属性到数据库字段：<br/>
        /// <code>
        /// <see langword=" var columnMap = new Dictionary&lt;string, object&gt;"/><br/>
        /// <see langword=" {"/><br/>
        /// <see langword="     { &quot;name&quot;, typeof(Person).GetProperty(&quot;Name&quot;) },"/><br/>
        /// <see langword="     { &quot;age&quot;, typeof(Person).GetProperty(&quot;Age&quot;) },"/><br/>
        /// <see langword="     { &quot;email&quot;, typeof(Person).GetProperty(&quot;Email&quot;) },"/><br/>
        /// <see langword="     { &quot;status&quot;, &quot;active&quot; },"/><br/>
        /// <see langword="     { &quot;created_time&quot;, DateTime.Now }"/><br/>
        /// <see langword=" };"/><br/>
        /// </code><br/><br/>
        /// 
        /// 2. 混合使用反射和常量值：<br/>
        /// <code>
        /// <see langword="var columnMap = new Dictionary&lt;string, object&gt;"/><br/>
        /// <see langword="{"/><br/>
        /// <see langword="    { &quot;user_name&quot;, typeof(User).GetProperty(&quot;UserName&quot;) },"/><br/>
        /// <see langword="    { &quot;department&quot;, &quot;技术部&quot; },"/><br/>
        /// <see langword="    { &quot;is_deleted&quot;, false },"/><br/>
        /// <see langword="    { &quot;create_by&quot;, currentUserId },"/><br/>
        /// <see langword="    { &quot;create_time&quot;, DateTime.Now }"/><br/>
        /// <see langword="};"/><br/>
        /// 
        /// </code><br/><br/>
        /// 3. 使用 <see cref="FieldInfo"/>（适用于字段而非属性）：<br/>
        /// <code>
        /// <see langword="var columnMap = new Dictionary&lt;string, object&gt;"/><br/>
        /// <see langword="{"/><br/>
        /// <see langword="    { &quot;name&quot;, typeof(Person).GetField(&quot;name&quot;) },"/><br/>
        /// <see langword="    { &quot;age&quot;, typeof(Person).GetField(&quot;age&quot;) }"/><br/>
        /// <see langword="};"/><br/>
        /// </code>
        /// </param>
        /// <param name="datas">要插入的数据集合,数据类型只能是<see langword="class"/></param>
        /// <returns>受影响的行数</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<int> BatchInsertAsync<T>(string tableName, Dictionary<string, object> columnTypeMap, IEnumerable<T> datas) where T : class
        {
            if (datas == null)
            {
                throw new ArgumentNullException(nameof(datas));
            }
            if (columnTypeMap == null)
            {
                throw new ArgumentNullException(nameof(columnTypeMap));
            }
            if (columnTypeMap.Count == 0)
            {
                throw new ArgumentException($"'{nameof(columnTypeMap)}' 列映射的长度不能为0");
            }
            if (!datas.Any())
            {
                return 0;
            }
            // 构建SQL第一部分
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"INSERT INTO `{tableName}` (");
            sqlBuilder.Append(string.Join(", ", columnTypeMap.Keys.Select(col => $"`{col}`")));
            sqlBuilder.Append(") VALUES ");

            var parameters = new Dictionary<string, object>();
            var valueStatements = new List<string>();
            int itemIndex = 0;

            foreach (var item in datas)
            {
                var paramList = new List<string>();

                // 为当前数据项的每一列生成参数
                foreach (var kv in columnTypeMap)
                {
                    string columnName = kv.Key;
                    object colMap = kv.Value;

                    // 生成唯一的参数名
                    string paramName = $"@p_{itemIndex}_{columnName}";
                    paramList.Add(paramName);
                    object value = GetValueFromMap(colMap, item);
                    parameters[paramName] = value ?? DBNull.Value;
                }

                // 添加 VALUES 子句
                valueStatements.Add($"({string.Join(", ", paramList)})");
                itemIndex++;
            }

            // 拼接所有VALUES子句
            sqlBuilder.Append(string.Join(", ", valueStatements));

            string sql = sqlBuilder.ToString();
            return await ExecuteNonQueryAsync(sql, parameters);
        }



        /// <summary>
        /// 类结构集合数据批量插入或更新到数据库,数据结构中需要有对应主键才能更新,否则只会将插入
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="columnTypeMap">
        /// 数据库字段与数据类型的映射字典，键值对含义如下：<br/><br/>
        /// 
        /// 【键 (<see cref="string"/>)】：数据库实际字段名<br/>
        /// 示例："name", "age", "email", "created_time"<br/><br/>
        /// 
        /// 【值 (<see cref="object"/>)】：支持三种类型：<br/><br/>
        /// 1. <see cref="PropertyInfo"/>：实体类属性反射对象，从数据对象中获取属性值<br/>
        ///    示例：<see cref="Type.GetProperty(string)"/><br/>
        ///     
        /// 2. <see cref="FieldInfo"/>：实体类字段反射对象，从数据对象中获取字段值<br/>
        ///    示例：<see cref="Type.GetField(string)"/><br/>
        ///     
        /// 3. 常量值 (<see cref="string"/>/<see cref="object"/>)：固定值，所有记录插入相同的值<br/>
        ///    示例："<see  langword="默认值"/> "、<see cref="DateTime"/>、<see langword="123"/>、<see langword="true"/>、<see langword="null"/>等等数据库对应的类型<br/>
        ///    用途：为所有插入记录设置相同的固定值<br/><br/>
        ///     
        /// 【使用示例】：<br/><br/>
        /// 
        /// 1. 映射实体属性到数据库字段：<br/>
        /// <code>
        /// <see langword=" var columnMap = new Dictionary&lt;string, object&gt;"/><br/>
        /// <see langword=" {"/><br/>
        /// <see langword="     { &quot;name&quot;, typeof(Person).GetProperty(&quot;Name&quot;) },"/><br/>
        /// <see langword="     { &quot;age&quot;, typeof(Person).GetProperty(&quot;Age&quot;) },"/><br/>
        /// <see langword="     { &quot;email&quot;, typeof(Person).GetProperty(&quot;Email&quot;) },"/><br/>
        /// <see langword="     { &quot;status&quot;, &quot;active&quot; },"/><br/>
        /// <see langword="     { &quot;created_time&quot;, DateTime.Now }"/><br/>
        /// <see langword=" };"/><br/>
        /// </code><br/><br/>
        /// 
        /// 2. 混合使用反射和常量值：<br/>
        /// <code>
        /// <see langword="var columnMap = new Dictionary&lt;string, object&gt;"/><br/>
        /// <see langword="{"/><br/>
        /// <see langword="    { &quot;user_name&quot;, typeof(User).GetProperty(&quot;UserName&quot;) },"/><br/>
        /// <see langword="    { &quot;department&quot;, &quot;技术部&quot; },"/><br/>
        /// <see langword="    { &quot;is_deleted&quot;, false },"/><br/>
        /// <see langword="    { &quot;create_by&quot;, currentUserId },"/><br/>
        /// <see langword="    { &quot;create_time&quot;, DateTime.Now }"/><br/>
        /// <see langword="};"/><br/>
        /// 
        /// </code><br/><br/>
        /// 3. 使用 <see cref="FieldInfo"/>（适用于字段而非属性）：<br/>
        /// <code>
        /// <see langword="var columnMap = new Dictionary&lt;string, object&gt;"/><br/>
        /// <see langword="{"/><br/>
        /// <see langword="    { &quot;name&quot;, typeof(Person).GetField(&quot;name&quot;) },"/><br/>
        /// <see langword="    { &quot;age&quot;, typeof(Person).GetField(&quot;age&quot;) }"/><br/>
        /// <see langword="};"/><br/>
        /// </code>
        /// </param>
        /// <param name="datas">数据集合</param>
        /// <returns>受影响的行数</returns>
        public int BatchUpsert<T>(string tableName,Dictionary<string, object> columnTypeMap, IEnumerable<T> datas) where T : class
        {
            if (datas == null) throw new ArgumentNullException(nameof(datas));
            if (columnTypeMap == null) throw new ArgumentNullException(nameof(columnTypeMap));
            if (columnTypeMap.Count == 0) throw new ArgumentException($"'{nameof(columnTypeMap)}' 列映射的长度不能为0");
            if (!datas.Any()) return 0;

            // 构建INSERT部分
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"INSERT INTO `{tableName}` (");
            sqlBuilder.Append(string.Join(", ", columnTypeMap.Keys.Select(col => $"`{col}`")));
            sqlBuilder.Append(") VALUES ");

            var parameters = new Dictionary<string, object>();
            var valueStatements = new List<string>();
            int itemIndex = 0;

            foreach (var item in datas)
            {
                var paramList = new List<string>();

                foreach (var kv in columnTypeMap)
                {
                    string columnName = kv.Key;
                    object colMap = kv.Value;
                    string paramName = $"@p_{itemIndex}_{columnName}";
                    paramList.Add(paramName);

                    object value = GetValueFromMap(colMap, item);
                    parameters[paramName] = value ?? DBNull.Value;
                }

                valueStatements.Add($"({string.Join(", ", paramList)})");
                itemIndex++;
            }

            sqlBuilder.Append(string.Join(", ", valueStatements));

            // 构建UPDATE部分
            sqlBuilder.Append(" ON DUPLICATE KEY UPDATE ");

            var updateList = new List<string>();
            var columnsToUpdate = columnTypeMap.Keys.ToList();

            foreach (var columnName in columnsToUpdate)
            {
                updateList.Add($"`{columnName}` = VALUES(`{columnName}`)");
            }

            sqlBuilder.Append(string.Join(", ", updateList));

            string sql = sqlBuilder.ToString();
            return ExecuteNonQuery(sql, parameters);
        }


        
        /// <summary>
        /// 异步执行类结构集合数据批量插入或更新到数据库,数据结构中需要有对应主键才能更新,否则只会将插入
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="columnTypeMap">
        /// 数据库字段与数据类型的映射字典，键值对含义如下：<br/><br/>
        /// 
        /// 【键 (<see cref="string"/>)】：数据库实际字段名<br/>
        /// 示例："name", "age", "email", "created_time"<br/><br/>
        /// 
        /// 【值 (<see cref="object"/>)】：支持三种类型：<br/><br/>
        /// 1. <see cref="PropertyInfo"/>：实体类属性反射对象，从数据对象中获取属性值<br/>
        ///    示例：<see cref="Type.GetProperty(string)"/><br/>
        ///     
        /// 2. <see cref="FieldInfo"/>：实体类字段反射对象，从数据对象中获取字段值<br/>
        ///    示例：<see cref="Type.GetField(string)"/><br/>
        ///     
        /// 3. 常量值 (<see cref="string"/>/<see cref="object"/>)：固定值，所有记录插入相同的值<br/>
        ///    示例："<see  langword="默认值"/> "、<see cref="DateTime"/>、<see langword="123"/>、<see langword="true"/>、<see langword="null"/>等等数据库对应的类型<br/>
        ///    用途：为所有插入记录设置相同的固定值<br/><br/>
        ///     
        /// 【使用示例】：<br/><br/>
        /// 
        /// 1. 映射实体属性到数据库字段：<br/>
        /// <code>
        /// <see langword=" var columnMap = new Dictionary&lt;string, object&gt;"/><br/>
        /// <see langword=" {"/><br/>
        /// <see langword="     { &quot;name&quot;, typeof(Person).GetProperty(&quot;Name&quot;) },"/><br/>
        /// <see langword="     { &quot;age&quot;, typeof(Person).GetProperty(&quot;Age&quot;) },"/><br/>
        /// <see langword="     { &quot;email&quot;, typeof(Person).GetProperty(&quot;Email&quot;) },"/><br/>
        /// <see langword="     { &quot;status&quot;, &quot;active&quot; },"/><br/>
        /// <see langword="     { &quot;created_time&quot;, DateTime.Now }"/><br/>
        /// <see langword=" };"/><br/>
        /// </code><br/><br/>
        /// 
        /// 2. 混合使用反射和常量值：<br/>
        /// <code>
        /// <see langword="var columnMap = new Dictionary&lt;string, object&gt;"/><br/>
        /// <see langword="{"/><br/>
        /// <see langword="    { &quot;user_name&quot;, typeof(User).GetProperty(&quot;UserName&quot;) },"/><br/>
        /// <see langword="    { &quot;department&quot;, &quot;技术部&quot; },"/><br/>
        /// <see langword="    { &quot;is_deleted&quot;, false },"/><br/>
        /// <see langword="    { &quot;create_by&quot;, currentUserId },"/><br/>
        /// <see langword="    { &quot;create_time&quot;, DateTime.Now }"/><br/>
        /// <see langword="};"/><br/>
        /// 
        /// </code><br/><br/>
        /// 3. 使用 <see cref="FieldInfo"/>（适用于字段而非属性）：<br/>
        /// <code>
        /// <see langword="var columnMap = new Dictionary&lt;string, object&gt;"/><br/>
        /// <see langword="{"/><br/>
        /// <see langword="    { &quot;name&quot;, typeof(Person).GetField(&quot;name&quot;) },"/><br/>
        /// <see langword="    { &quot;age&quot;, typeof(Person).GetField(&quot;age&quot;) }"/><br/>
        /// <see langword="};"/><br/>
        /// </code>
        /// </param>
        /// <param name="datas">数据集合</param>
        /// <returns>受影响的行数</returns>
        public async Task<int> BatchUpsertAsync<T>(string tableName,Dictionary<string, object> columnTypeMap, IEnumerable<T> datas) where T : class
        {
            if (datas == null) throw new ArgumentNullException(nameof(datas));
            if (columnTypeMap == null) throw new ArgumentNullException(nameof(columnTypeMap));
            if (columnTypeMap.Count == 0) throw new ArgumentException($"'{nameof(columnTypeMap)}' 列映射的长度不能为0");
            if (!datas.Any()) return 0;

            // 构建INSERT部分
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"INSERT INTO `{tableName}` (");
            sqlBuilder.Append(string.Join(", ", columnTypeMap.Keys.Select(col => $"`{col}`")));
            sqlBuilder.Append(") VALUES ");

            var parameters = new Dictionary<string, object>();
            var valueStatements = new List<string>();
            int itemIndex = 0;

            foreach (var item in datas)
            {
                var paramList = new List<string>();

                foreach (var kv in columnTypeMap)
                {
                    string columnName = kv.Key;
                    object colMap = kv.Value;
                    string paramName = $"@p_{itemIndex}_{columnName}";
                    paramList.Add(paramName);

                    object value = GetValueFromMap(colMap, item);
                    parameters[paramName] = value ?? DBNull.Value;
                }

                valueStatements.Add($"({string.Join(", ", paramList)})");
                itemIndex++;
            }

            sqlBuilder.Append(string.Join(", ", valueStatements));

            // 构建UPDATE部分
            sqlBuilder.Append(" ON DUPLICATE KEY UPDATE ");

            var updateList = new List<string>();
            var columnsToUpdate = columnTypeMap.Keys.ToList();

            foreach (var columnName in columnsToUpdate)
            {
                updateList.Add($"`{columnName}` = VALUES(`{columnName}`)");
            }

            sqlBuilder.Append(string.Join(", ", updateList));

            string sql = sqlBuilder.ToString();
            return await ExecuteNonQueryAsync(sql, parameters);
        }


        

    }
}
