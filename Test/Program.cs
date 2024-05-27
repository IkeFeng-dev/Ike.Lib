using Ike;
using System.Data;

namespace Test
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Ike.MySQL mysql = new MySQL("10.62.202.190", "ike_web_data", "wcdatd", "wcdatd");
			mysql.Instantiation();
			mysql.Open();

			bool b1 = mysql.TableExists("equipment_parameters");
			Dictionary<string,string> k1 = mysql.GetTableColumns("equipment_parameters");
			string s1 = mysql.GetServerVersion();
			double d1 = mysql.GetDatabaseSize();
			double d2 = mysql.GetTableSize("equipment_parameters");
			int i1 = mysql.GetRowCount("equipment_parameters");
			bool b2 = mysql.ColumnExists("equipment_parameters", "host_name");
			List<string> l1 = mysql.GetTableIndexes("equipment_parameters");
			List<string> l2 = mysql.GetAllTableNames();
			string s2 = mysql.GetDatabaseCharset();
			List<string> l3 = mysql.GetCurrentUserPrivileges();
			List<string> l4 = mysql.GetDatabaseList();
			string s3 = mysql.GetTableCreateStatement("equipment_parameters");
			string s4 = mysql.GetTableEngine("equipment_parameters");
			mysql.Dispose();
		}
	}
}
