using System;
using System.Data;
using Interfaces;
using Models;
using MySql.Data.MySqlClient;
using Val;

namespace DataBases
{
    class MySQLDataBase : IDataBase
    {
        private MySqlConnection con;

        public MySQLDataBase()
        {
            INI ini = new INI("settings.ini");
            string host = ini.Read("host", "MySQL");
            string port = ini.Read("port", "MySQL");
            string database = ini.Read("database", "MySQL");
            string user = ini.Read("user", "MySQL");
            string password = ini.Read("password", "MySQL");

            con = new MySqlConnection($"Server={host};Database={database};port={port};User Id={user};password={password};SSL Mode=None");
        }

        public string GetValIdByName(string valName)
        {
            try
            {
                con.Open();

                string sql = "SELECT id_code FROM val_info WHERE iso_char_code = @code";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@code", valName);
                cmd.Prepare();
                string id = cmd.ExecuteScalar().ToString();

                con.Close();

                return id;
            }
            catch (Exception e)
            {
                Console.WriteLine("MySQLDataBase.GetValIdByName: " + e.Message);

                return "";
            }
        }

        public void SaveCurse(ValCurse vc)
        {
            try
            {
                con.Open();

                string sql = "INSERT IGNORE INTO val_curse (id_val, date, value) VALUES (@idVal, @date, @curse)";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@date", vc.Dt);
                cmd.Parameters.AddWithValue("@idVal", vc.IdVal);
                cmd.Parameters.AddWithValue("@curse", vc.Value);
                cmd.Prepare();
                cmd.ExecuteNonQuery();

                con.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("MySQLDataBase.SaveCurse: " + e.Message);
            }
        }

        public void SaveInfo(ValInfo vi)
        {
            try
            {
                con.Open();

                string sql = 
                    "INSERT IGNORE INTO val_info (id_code, name, eng_name, nominal, parent_code, iso_num_code, iso_char_code) " +
                    "VALUES (@idCode, @name, @engName, @nominal, @parentCode, @isoNumCode, @isoCharCode)";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@idCode", vi.IdCode);
                cmd.Parameters.AddWithValue("@name", vi.Name);
                cmd.Parameters.AddWithValue("@engName", vi.EngName);
                cmd.Parameters.AddWithValue("@nominal", vi.Nominal);
                cmd.Parameters.AddWithValue("@parentCode", vi.ParentCode);
                cmd.Parameters.AddWithValue("@isoNumCode", vi.ISONumCode);
                cmd.Parameters.AddWithValue("@isoCharCode", vi.ISOCharCode);
                cmd.Prepare();
                cmd.ExecuteNonQuery();

                con.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("MySQLDataBase.SaveInfo: " + e.Message);
            }
        }

        public decimal GetValCurse(string val, string date)
        {
            try
            {
                con.Open();

                string sql = "SELECT value FROM val_curse WHERE id_val = @idVal AND date = @date";

                MySqlCommand cmd = new MySqlCommand("GET_VAL_CURSE", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new MySqlParameter("val", val));
                cmd.Parameters.Add(new MySqlParameter("dt", date));
                cmd.Parameters.Add("@ireturnvalue", MySqlDbType.Decimal);
                cmd.Parameters["@ireturnvalue"].Direction = ParameterDirection.ReturnValue;
                object res = cmd.ExecuteScalar();

                con.Close();

                if (res == null)
                    return -1;

                return decimal.Parse(res.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("MySQLDataBase.GetValCurse: " + e.Message);

                return -1;
            }
        }
    }
}
