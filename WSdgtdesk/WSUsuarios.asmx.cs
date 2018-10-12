using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace WSdgtdesk
{
    
    [WebService(Namespace = "https://wsdgtdesk.azurewebsites.net/WSUsuarios.asmx")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WSUsuarios : System.Web.Services.WebService
    {

        public AuthUser User;

        [WebMethod]        
        [SoapHeader("User", Required =true)]
        public string SetUsuario(string user_name, string password, string name, string email)
        {
            if (User == null)
            {
                throw new SoapException("Autenticación requerida", SoapException.ClientFaultCode);
            }
            else if (!User.IsValid())
            {
                throw new SoapException("Unauthorized", SoapException.ClientFaultCode);
            }
            using (SqlConnection Conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["dgtdeskConnectionString"].ConnectionString))
            {
                try
                {
                    Conexion.Open();
                    string consulta =
                        @"insert into users 
                            (user_name, password, email, name)
                            values (@user_name, @password, @email, @name)";

                    SqlCommand cmd = Conexion.CreateCommand();
                    cmd.CommandText = consulta;
                    cmd.Parameters.Add("@user_name", SqlDbType.VarChar).Value = user_name;
                    cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = Utiles.Base64Encode(password);
                    cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
                    cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = name;
                    if (cmd.ExecuteNonQuery() > 0)
                        return "OK";
                    else
                    {                        
                        throw new SoapException("El usuario ya existe o los parámetros proporcionados no son correctos", SoapException.ClientFaultCode);
                    }
                }
                catch (Exception ex)
                {
                    Exception innerEx = ex.InnerException ?? ex;
                    throw new SoapException("No se pudo crear el usuario", SoapException.ServerFaultCode, innerEx);
                }
            }

        }

        [WebMethod]
        [SoapHeader("User", Required = true)]
        public Usuario GetUsuario(string user_name, string password)
        {
            if (User == null)
            {                
                throw new SoapException("Autenticación requerida", SoapException.ClientFaultCode);
            }
            else if (!User.IsValid())
            {
                throw new SoapException("Unauthorized", SoapException.ClientFaultCode);
            }
            Usuario usuario = new Usuario();
            using (SqlConnection Conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["dgtdeskConnectionString"].ConnectionString))
            {                
                try
                {
                    Conexion.Open();
                    string consulta =
                        @"select id, name, email from users where user_name = @user_name and password = @passwordB64";

                    SqlCommand cmd = Conexion.CreateCommand();
                    cmd.CommandText = consulta;
                    cmd.Parameters.Add("@user_name", SqlDbType.VarChar).Value = user_name;
                    cmd.Parameters.Add("@passwordB64", SqlDbType.VarChar).Value = Utiles.Base64Encode(password);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        usuario.Id = dr.GetInt32(0);
                        usuario.Name = dr.GetString(1);
                        usuario.Email = dr.GetString(2);
                        usuario.UserName = user_name;
                    }
                }
                catch(Exception ex)
                {
                    Exception innerEx = ex.InnerException ?? ex;
                    throw new SoapException("No se pudo encontrar al usuario", SoapException.ServerFaultCode, innerEx);
                }
                return usuario;
            }
        }      
    }
}
