using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WSdgtdesk
{
    public static class Utiles
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static void AddParameter(SqlCommand comando,string nombre, SqlDbType tipo, object value)
        {
            SqlParameter param = new SqlParameter(nombre, tipo);
            if (value == null)
                param.Value = DBNull.Value;
            else
                param.Value = value;
            comando.Parameters.Add(param);
        }
    }
}