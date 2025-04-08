using Microsoft.Data.SqlClient;
using SolaBid.Business.Models;
using System;
using System.Data;

namespace SolaBid.Business.Logics.CommonLogic
{
    public static class GetData
    {
        public static DataTable FromQuery(string query, bool isSiteLineDb = true)
        {
            using (var conn = isSiteLineDb ? TransactionConfig.ExternalDbContext : TransactionConfig.AppDbContextManualConnection)
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    conn.Close();
                    return dt;
                }
                catch (Exception ex)
                {
                    return null;
                }

            }

        }
    }
}
