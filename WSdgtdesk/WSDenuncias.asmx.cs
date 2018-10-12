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
    /// <summary>
    /// Summary description for WSDenuncias
    /// </summary>
    [WebService(Namespace = "https://wsdgtdesk.azurewebsites.net/WSDenuncias.asmx")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WSDenuncias : System.Web.Services.WebService
    {

        public AuthUser User;

        [WebMethod]
        [SoapHeader("User", Required = true)]
        public string SetDenuncia(
            string EmailUsuario, 
            int TipoDenunciaId, 
            string Detalles, 
            string Direccion,            
            DateTime FechaDenuncia,
            //opcionales
            bool Cerrada = false,
            string TelefonoUsuario = null,
            string Acciones = null,
            DateTime? FechaAccion = null,
            DateTime? FechaCierre = null
        )
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
                        @"insert into complaint 
                                   (details, address, user_email, user_phone, complaint_date, actions, action_date, closed, close_date, complaint_type_id)
                            values (@details, @address, @user_email, @user_phone, @complaint_date, @actions, @action_date, @closed, @close_date, @complaint_type_id)";

                    SqlCommand cmd = Conexion.CreateCommand();
                    cmd.CommandText = consulta;
                    Utiles.AddParameter(cmd, "@details", SqlDbType.VarChar, Detalles);
                    Utiles.AddParameter(cmd, "@address", SqlDbType.VarChar, Direccion);
                    Utiles.AddParameter(cmd, "@user_email", SqlDbType.VarChar, EmailUsuario);
                    Utiles.AddParameter(cmd, "@user_phone", SqlDbType.VarChar, TelefonoUsuario);
                    Utiles.AddParameter(cmd, "@complaint_date", SqlDbType.DateTime, FechaDenuncia);
                    Utiles.AddParameter(cmd, "@actions", SqlDbType.VarChar, Acciones);
                    Utiles.AddParameter(cmd, "@action_date", SqlDbType.DateTime, FechaAccion);
                    Utiles.AddParameter(cmd, "@closed", SqlDbType.Bit, Cerrada);
                    Utiles.AddParameter(cmd, "@close_date", SqlDbType.DateTime, FechaCierre);
                    Utiles.AddParameter(cmd, "@complaint_type_id", SqlDbType.Int, TipoDenunciaId);
                    
                    if (cmd.ExecuteNonQuery() > 0)
                        return "OK";
                    else
                    {
                        throw new SoapException("Los parámetros proporcionados no son correctos", SoapException.ClientFaultCode);
                    }
                }
                catch (Exception ex)
                {
                    Exception innerEx = ex.InnerException ?? ex;
                    throw new SoapException("No se pudo crear la denuncia", SoapException.ServerFaultCode, innerEx);
                }
            }
        }


        [WebMethod]
        [SoapHeader("User", Required = true)]
        public string UpdateDenuncia(int DenunciaId, string Acciones)
        {

            
            if (User == null)
            {
                throw new SoapException("Autenticación requerida", SoapException.ClientFaultCode);
            }
            else if (!User.IsValid())
            {
                throw new SoapException("Unauthorized", SoapException.ClientFaultCode);
            }
            
            
            DateTime? FechaAccion = DateTime.Now;

            using (SqlConnection Conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["dgtdeskConnectionString"].ConnectionString))
            {
                try
                {
                    Conexion.Open();
                    string consulta =
                        @"update complaint set
                                   actions = @actions, action_date = @action_date                            
                            where id = @id";

                    SqlCommand cmd = Conexion.CreateCommand();
                    cmd.CommandText = consulta;
                    cmd.Parameters.Add("@actions", SqlDbType.VarChar).Value = Acciones;
                    cmd.Parameters.Add("@action_date", SqlDbType.DateTime).Value = FechaAccion;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = DenunciaId;                    

                    if (cmd.ExecuteNonQuery() > 0)
                        return "OK";
                    else
                    {
                        throw new SoapException("Los parámetros proporcionados no son correctos", SoapException.ClientFaultCode);
                    }
                }
                catch (Exception ex)
                {
                    Exception innerEx = ex.InnerException ?? ex;
                    throw new SoapException("No se pudo crear la denuncia", SoapException.ServerFaultCode, innerEx);
                }
            }
        }

        [WebMethod]
        [SoapHeader("User", Required = true)]
        public string CerrarDenuncia(int DenunciaId)
        {
            
            if (User == null)
            {
                throw new SoapException("Autenticación requerida", SoapException.ClientFaultCode);
            }
            else if (!User.IsValid())
            {
                throw new SoapException("Unauthorized", SoapException.ClientFaultCode);
            }
            

            DateTime? FechaCierre = DateTime.Now;

            using (SqlConnection Conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["dgtdeskConnectionString"].ConnectionString))
            {
                try
                {
                    Conexion.Open();
                    string consulta =
                        @"update complaint set
                                   closed = 1, close_date = @close_date                            
                            where id = @id";

                    SqlCommand cmd = Conexion.CreateCommand();
                    cmd.CommandText = consulta;                    
                    cmd.Parameters.Add("@close_date", SqlDbType.DateTime).Value = FechaCierre;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = DenunciaId;

                    if (cmd.ExecuteNonQuery() > 0)
                        return "OK";
                    else
                    {
                        throw new SoapException("Los parámetros proporcionados no son correctos", SoapException.ClientFaultCode);
                    }
                }
                catch (Exception ex)
                {
                    Exception innerEx = ex.InnerException ?? ex;
                    throw new SoapException("No se pudo crear la denuncia", SoapException.ServerFaultCode, innerEx);
                }
            }
        }
        
        /*
        [WebMethod]
        //[SoapHeader("User", Required = true)]
        public string ImportDenunciasUsuarios(DateTime FechaDesde, DateTime FechaHasta)
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
                        @"insert into complaint 
                                   (details, address, user_email, user_phone, complaint_date, actions, action_date, closed, close_date, complaint_type_id)
                            values (@details, @address, @user_email, @user_phone, @complaint_date, @actions, @action_date, @closed, @close_date, @complaint_type_id)";

                    SqlCommand cmd = Conexion.CreateCommand();
                    cmd.CommandText = consulta;
                    Utiles.AddParameter(cmd, "@details", SqlDbType.VarChar, Detalles);
                    Utiles.AddParameter(cmd, "@address", SqlDbType.VarChar, Direccion);
                    Utiles.AddParameter(cmd, "@user_email", SqlDbType.VarChar, EmailUsuario);
                    Utiles.AddParameter(cmd, "@user_phone", SqlDbType.VarChar, TelefonoUsuario);
                    Utiles.AddParameter(cmd, "@complaint_date", SqlDbType.DateTime, FechaDenuncia);
                    Utiles.AddParameter(cmd, "@actions", SqlDbType.VarChar, Acciones);
                    Utiles.AddParameter(cmd, "@action_date", SqlDbType.DateTime, FechaAccion);
                    Utiles.AddParameter(cmd, "@closed", SqlDbType.Bit, Cerrada);
                    Utiles.AddParameter(cmd, "@close_date", SqlDbType.DateTime, FechaCierre);
                    Utiles.AddParameter(cmd, "@complaint_type_id", SqlDbType.Int, TipoDenunciaId);

                    if (cmd.ExecuteNonQuery() > 0)
                        return "OK";
                    else
                    {
                        throw new SoapException("Los parámetros proporcionados no son correctos", SoapException.ClientFaultCode);
                    }
                }
                catch (Exception ex)
                {
                    Exception innerEx = ex.InnerException ?? ex;
                    throw new SoapException("No se pudo crear la denuncia", SoapException.ServerFaultCode, innerEx);
                }
            }
        }
        */

    }
}
