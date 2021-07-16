using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FileInsert
{
    class Program
    {
        public class Country
        {
            public int CountryId;
            public string CountryName;
        }
        public DataTable GetDataTable(string[] lines, string insert)
        {
            string[] line = { };

            List<Country> CountryList = new List<Country>();
            List<string> StateList = new List<string>();
            List<string> FileStateList = new List<string>();
            DataTable dt = new DataTable();
            string country;
            string state;
            int countryId;
            bool dbStateExist;
            bool fileStateExist;

            CountryList = getCountryList();
            StateList = getStateList();
            if (insert == "insertCountry")
            {
                dt.Columns.Add("CountryName");
            }
            else
            {
                dt.Columns.Add("CountryId");
                dt.Columns.Add("StateName");
            }

            foreach (var i in lines)
            {
                line = i.Split("|");

                if (line.Length > 1)
                {
                    country = line[0].Trim();
                    state = line[1].Trim();

                    if (country == "Country" && state == "State")
                        continue;

                    countryId = getCountryId(country, CountryList);
                    if (insert == "insertCountry")
                    {
                        if (countryId == 0)
                        {
                            if (country != "")
                            {
                                dt.Rows.Add(country);
                            }
                        }
                    }
                    else
                    {
                        dbStateExist = StateList.Exists(x => x == state);

                        if (country != "" && dbStateExist == false)
                        {
                            fileStateExist = FileStateList.Contains(state);
                            if (fileStateExist == false)
                            {
                                dt.Rows.Add(countryId, state);
                                FileStateList.Add(state);
                            }
                        }
                    }
                }
            }

            return dt;
        }

        public void insert()
        {
            string[] lines = { };
            string[] line = { };
            string curFile = "E:\\states.txt";

            lines = System.IO.File.ReadAllLines(curFile);
            string cs;
            cs = @"server=(localdb)\MSSQLLocalDB;database=POC;integrated security=SSPI";

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spInsertCountry", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramTVP = new SqlParameter()
                {
                    ParameterName = "@CountryTableType",
                    Value = GetDataTable(lines, "insertCountry")
                };
                cmd.Parameters.Add(paramTVP);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spInsertStates", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramTVP = new SqlParameter()
                {
                    ParameterName = "@StateTableType",
                    Value = GetDataTable(lines, "insertState")
                };
                cmd.Parameters.Add(paramTVP);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public int getCountryId(string CountryName, List<Country> CountryList)
        {
            if (CountryList.Count != 0)
            {
                foreach (var i in CountryList)
                {
                    if (CountryName == i.CountryName)
                        return i.CountryId;
                }
            }

            return 0;
        }

        public List<Country> getCountryList()
        {
            string cs;
            List<Country> CountryList = new List<Country>();
            Country country = null;

            cs = @"server=(localdb)\MSSQLLocalDB;database=POC;integrated security=SSPI";
            using (SqlConnection connection = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("Select CountryId, CountryName from Country", connection);
                connection.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        country = new Country();
                        country.CountryId = Convert.ToInt32(rdr["CountryId"]);
                        country.CountryName = rdr["CountryName"].ToString();
                        CountryList.Add(country);
                    }
                }
            }
            return CountryList;
        }

        public List<string> getStateList()
        {
            string cs;
            List<string> StateList = new List<string>();

            cs = @"server=(localdb)\MSSQLLocalDB;database=POC;integrated security=SSPI";
            using (SqlConnection connection = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("Select StateName from State", connection);
                connection.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        StateList.Add(rdr["StateName"].ToString());
                    }
                }
            }
            return StateList;
        }

        static void Main(string[] args)
        {
            Program fi = new Program();
            fi.insert();
            Console.WriteLine("Success");

        }
    }
}
