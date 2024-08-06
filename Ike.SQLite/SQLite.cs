using System;
using System.Data.Common;
using System.Data;
using System.Data.SQLite;

namespace Ike
{
	/// <summary>
	/// SQLite操作类
	/// </summary>
	public class SQLite
	{
		/// <summary>
		/// 数据库连接字符串
		/// </summary>
		private readonly string _connectionString;

		/// <summary>
		/// 数据库连接对象
		/// </summary>
		private SQLiteConnection connection;
		/// <summary>
		/// 表示 SQLite 数据库的日志模式
		/// </summary>
		public enum JournalMode
		{
			/// <summary>
			/// 删除模式: 事务提交后立即删除事务日志文件
			/// </summary>
			Delete,

			/// <summary>
			/// 回滚模式: 事务提交后立即将事务日志文件回滚到之前的状态
			/// </summary>
			Truncate,

			/// <summary>
			/// 延迟模式: 事务日志在事务提交后延迟写入
			/// </summary>
			Persist,

			/// <summary>
			/// 内存模式: 事务日志在内存中维护,不写入磁盘
			/// </summary>
			Memory,

			/// <summary>
			/// 禁用模式: 禁用事务日志,事务不进行日志记录
			/// </summary>
			Off
		}

		/// <summary>
		/// 表示 SQLite 数据库的同步模式
		/// </summary>
		public enum Synchronous
		{
			/// <summary>
			/// 同步模式: 每次事务提交后都立即将事务写入磁盘
			/// </summary>
			Full,

			/// <summary>
			/// 关闭模式: 事务提交后不进行同步写入,由操作系统决定何时将事务写入磁盘
			/// </summary>
			Off,

			/// <summary>
			/// 标准模式: 每次事务提交后都同步写入数据库文件的主页
			/// </summary>
			Normal
		}


		/// <summary>
		/// SQLite数据库操作
		/// </summary>
		/// <param name="databasePath">指定数据库文件的路径</param>
		/// <param name="version">指定 SQLite 数据库引擎的版本号</param>
		/// <param name="pooling">启用连接池</param>
		/// <param name="failIfMissing">指示在指定的数据库文件不存在时是否引发错误</param>
		/// <param name="readOnly">指示是否以只读模式打开数据库</param>
		/// <param name="pageSize">指定数据库页的大小</param>
		/// <param name="cacheSize">指定数据库缓存的大小</param>
		/// <param name="journalMode">指定日志模式,用于控制事务日志的写入方式</param>
		/// <param name="synchronous">指定同步模式,用于控制事务的提交方式</param>
		public SQLite(string databasePath, string version = "3", bool pooling = true, bool failIfMissing = false, bool readOnly = false, int pageSize = 4096, int cacheSize = 2000, JournalMode journalMode = JournalMode.Delete, Synchronous synchronous = SQLite.Synchronous.Full)
		{
			_connectionString = $"Data Source={databasePath};Version={version};Pooling={pooling};FailIfMissing={failIfMissing};ReadOnly={readOnly};PageSize={pageSize};Cache Size={cacheSize};Journal Mode={journalMode};Synchronous={synchronous};";
		}

		/// <summary>
		/// 打开数据库
		/// </summary>
		public bool Open()
		{
			if (connection == null)
			{
				connection = new SQLiteConnection(_connectionString);
			}
			if (connection.State != ConnectionState.Open)
			{
				connection.Open();
			}
			return connection.State == ConnectionState.Open;
		}
		/// <summary>
		/// 启用外键约束
		/// </summary>
		public void SetForeignKeys()
		{
			ExecuteNonQuery("PRAGMA foreign_keys = ON;", null);
		}

        /// <summary> 
        /// 对SQLite数据库执行增删改操作,返回受影响的行数
        /// </summary> 
        /// <param name="sqlCommand">要执行的增删改的SQL语句</param> 
        /// <returns></returns> 
        public int ExecuteNonQuery(string sqlCommand)
        {
            return ExecuteNonQuery(sqlCommand, null);
        }

		/// <summary> 
		/// 对SQLite数据库执行增删改操作,返回受影响的行数
		/// </summary> 
		/// <param name="sqlCommand">要执行的增删改的SQL语句</param> 
		/// <param name="parameters">执行增删改语句所需要的参数,参数必须以它们在SQL语句中的顺序为准</param> 
		/// <returns></returns> 
		/// <remarks>
		/// 参数<paramref name="parameters"/>的调用示例:
		/// <code>
		/// <see langword="string" />  sqlCommand = @"INSERT INTO Users (username, password) VALUES (@username, @password)";"
		/// <br/>
		/// <see cref="SQLiteParameter"/>[] parameters = <see langword="new" /> <see cref="SQLiteParameter"/>[]
		/// {
		///     <see langword="new" /> <see cref="SQLiteParameter"/>("@username", "user1"),
		///     <see langword="new" /> <see cref="SQLiteParameter"/>("@password", "9Jhd7l#3")
		/// };
		/// </code>
		/// </remarks>
		public int ExecuteNonQuery(string sqlCommand, SQLiteParameter[] parameters)
		{
			int affectedRows = 0;
			using (DbTransaction transaction = connection.BeginTransaction())
			{
				using (SQLiteCommand command = new SQLiteCommand(connection))
				{
					command.CommandText = sqlCommand;
					if (parameters != null)
					{
						command.Parameters.AddRange(parameters);
					}
					affectedRows = command.ExecuteNonQuery();
				}
				transaction.Commit();
			}
			return affectedRows;
		}

		/// <summary>
		/// 执行SQL语句,返回影响的行数
		/// </summary>
		/// <param name="sqlCommand">SQL语句</param>
		/// <returns>影响的行数,如果数据库未打开则返回-1</returns>
		public int ExecuteSql(string sqlCommand)
		{
			return ExecuteSql(sqlCommand,null);
        }
		/// <summary>
		/// 执行SQL语句,返回影响的行数
		/// </summary>
		/// <param name="sqlCommand">SQL语句</param>
		/// <param name="parameters">执行增删改语句所需要的参数,参数必须以它们在SQL语句中的顺序为准</param> 
		/// <returns>影响的行数</returns>
		/// <remarks>
		/// 参数<paramref name="parameters"/>的调用示例:
		/// <code>
		/// <see langword="string" />  sqlCommand = @"INSERT INTO Users (username, password) VALUES (@username, @password)";"
		/// <br/>
		/// <see cref="SQLiteParameter"/>[] parameters = <see langword="new" /> <see cref="SQLiteParameter"/>[]
		/// {
		///     <see langword="new" /> <see cref="SQLiteParameter"/>("@username", "user1"),
		///     <see langword="new" /> <see cref="SQLiteParameter"/>("@password", "9Jhd7l#3")
		/// };
		/// </code>
		/// </remarks>
		public int ExecuteSql(string sqlCommand, SQLiteParameter[] parameters)
		{
			using (SQLiteCommand cmd = new SQLiteCommand(sqlCommand, connection))
			{
				if (parameters != null)
				{
					cmd.Parameters.AddRange(parameters);
				}
				return cmd.ExecuteNonQuery();
			}
		}

        /// <summary>
        /// 执行多条SQL语句,实现数据库事务
        /// </summary>
        /// <param name="sqlCommandList">多条SQL语句</param>    
        public void ExecuteSqlTran(string[] sqlCommandList)
        {
            ExecuteSqlTran(sqlCommandList, null);
        }

        /// <summary>
        /// 执行多条SQL语句,实现数据库事务
        /// </summary>
        /// <param name="sqlCommandList">多条SQL语句</param>    
        /// <param name="parameters">执行增删改语句所需要的参数,参数必须以它们在SQL语句中的顺序为准</param> 
        /// <remarks>
        /// 参数<paramref name="parameters"/>的调用示例:
        /// <code>
        /// <see langword="string" />  sqlCommand = @"INSERT INTO Users (username, password) VALUES (@username, @password)";"
        /// <br/>
        /// <see cref="SQLiteParameter"/>[] parameters = <see langword="new" /> <see cref="SQLiteParameter"/>[]
        /// {
        ///     <see langword="new" /> <see cref="SQLiteParameter"/>("@username", "user1"),
        ///     <see langword="new" /> <see cref="SQLiteParameter"/>("@password", "9Jhd7l#3")
        /// };
        /// </code>
        /// </remarks>
        public void ExecuteSqlTran(string[] sqlCommandList, SQLiteParameter[] parameters)
		{
			using (SQLiteCommand cmd = new SQLiteCommand())
			{
				cmd.Connection = connection;
				if (parameters != null)
				{
					cmd.Parameters.AddRange(parameters);
				}
				using (SQLiteTransaction tx = connection.BeginTransaction())
				{
					cmd.Transaction = tx;
					try
					{
						foreach (string sql in sqlCommandList)
						{
							if (sql.Trim().Length > 1)
							{
								cmd.CommandText = sql;
								cmd.ExecuteNonQuery();
							}
						}
						tx.Commit();
					}
					catch (Exception ex)
					{
						tx.Rollback();
						throw ex;
					}
				}
			}
		}

		/// <summary>
		/// 执行一条计算查询结果语句
		/// </summary>
		/// <param name="sqlCommand">计算查询结果语句</param>
		/// <returns>查询结果</returns>
		public object ExecutionQuery(string sqlCommand)
		{
			return ExecutionQuery(sqlCommand, null);
        }

        /// <summary>
        /// 执行一条计算查询结果语句
        /// </summary>
        /// <param name="sqlCommand">计算查询结果语句</param>
        /// <param name="parameters">执行增删改语句所需要的参数,参数必须以它们在SQL语句中的顺序为准</param> 
        /// <returns>查询结果</returns>
        /// <remarks>
        /// 参数<paramref name="parameters"/>的调用示例:
        /// <code>
        /// <see langword="string" />  sqlCommand = @"INSERT INTO Users (username, password) VALUES (@username, @password)";"
        /// <br/>
        /// <see cref="SQLiteParameter"/>[] parameters = <see langword="new" /> <see cref="SQLiteParameter"/>[]
        /// {
        ///     <see langword="new" /> <see cref="SQLiteParameter"/>("@username", "user1"),
        ///     <see langword="new" /> <see cref="SQLiteParameter"/>("@password", "9Jhd7l#3")
        /// };
        /// </code>
        /// </remarks>
        public object ExecutionQuery(string sqlCommand, SQLiteParameter[] parameters)
		{
			using (SQLiteCommand cmd = new SQLiteCommand(sqlCommand, connection))
			{
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                return cmd.ExecuteScalar();
			}
		}

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="sqlCommand">SQL语句</param>
        /// <param name="dataTable">包含查询结果的<see cref="DataTable"/>对象</param>
        /// <returns>数据表</returns>
        public void ExecutionRead(string sqlCommand, out DataTable dataTable)
		{
			dataTable = new DataTable();
			using (SQLiteCommand cmd = new SQLiteCommand(sqlCommand, connection))
            {
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                {
                    adapter.Fill(dataTable);
                }
            }
		}

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="sqlCommand">SQL语句</param>
        /// <param name="dataTable">包含查询结果的<see cref="DataTable"/>对象</param>
        /// <param name="parameters">执行增删改语句所需要的参数,参数必须以它们在SQL语句中的顺序为准</param> 
        /// <returns>数据表</returns>
        /// <remarks>
        /// 参数<paramref name="parameters"/>的调用示例:
        /// <code>
        /// <see langword="string" />  sqlCommand = @"INSERT INTO Users (username, password) VALUES (@username, @password)";"
        /// <br/>
        /// <see cref="SQLiteParameter"/>[] parameters = <see langword="new" /> <see cref="SQLiteParameter"/>[]
        /// {
        ///     <see langword="new" /> <see cref="SQLiteParameter"/>("@username", "user1"),
        ///     <see langword="new" /> <see cref="SQLiteParameter"/>("@password", "9Jhd7l#3")
        /// };
        /// </code>
        /// </remarks>
        public void ExecutionRead(string sqlCommand, out DataTable dataTable,SQLiteParameter[] parameters)
		{
			dataTable = new DataTable();
			using (SQLiteCommand cmd = new SQLiteCommand(sqlCommand, connection))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                {
                    adapter.Fill(dataTable);
                }
            }
		}


        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="sqlCommand">SQL语句</param>
        /// <param name="dataSet">包含查询结果的<see cref="DataSet"/>对象</param>
        /// <returns>数据表</returns>
        public void ExecutionRead(string sqlCommand, out DataSet dataSet)
		{
			dataSet = new DataSet();
			using (SQLiteCommand cmd = new SQLiteCommand(sqlCommand, connection))
            {
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                {
                    adapter.Fill(dataSet);
                }
            }
		}

		/// <summary>
		/// 数据表是否存在
		/// </summary>
		/// <param name="tableName">表名称</param>
		/// <returns></returns>
		public bool IsTableExists(string tableName)
		{
			object obj = ExecutionQuery($"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}';");
			return (obj ?? "").ToString() == tableName;
		}

		/// <summary>
		/// 获取数据库信息
		/// </summary>
		/// <returns>将相关表信息储存到<see cref="DataTable"/>中返回</returns>
		public DataTable GetDatabaseInfo()
		{
			ExecutionRead("SELECT * FROM sqlite_master;", out DataTable dataTable);
			return dataTable;
		}

	}
}
