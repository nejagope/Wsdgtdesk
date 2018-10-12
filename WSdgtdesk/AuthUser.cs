using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;

namespace WSdgtdesk
{
    public class AuthUser: SoapHeader
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public bool IsValid()
        {
            using (SqlConnection Conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["dgtdeskConnectionString"].ConnectionString))
            {
                try
                {
                    Conexion.Open();
                    string consulta =
                        @"select count(*) from wsusers where name = @name and password = @passwordB64";

                    SqlCommand cmd = Conexion.CreateCommand();
                    cmd.CommandText = consulta;
                    cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = UserName;
                    cmd.Parameters.Add("@passwordB64", SqlDbType.VarChar).Value = Utiles.Base64Encode(Password);

                    int conteo = (int)cmd.ExecuteScalar();
                    if (conteo > 0)
                        return true;
                }
                catch (Exception ex)
                {
                    
                }
                return false;
            }
        }
    }
}