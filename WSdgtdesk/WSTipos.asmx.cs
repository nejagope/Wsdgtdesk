using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace WSdgtdesk
{
    /// <summary>
    /// Summary description for WSTipos
    /// </summary>
    [WebService(Namespace = "https://wsdgtdesk.azurewebsites.net/WSTipos.asmx")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WSTipos : System.Web.Services.WebService
    {

        public AuthUser User;

        [WebMethod]
        [SoapHeader("User", Required = true)]
        public string SetTipoDenuncia(string nombre_tipo)
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
                        @"insert into complaint_type 
                            (name)
                            values (@name)";

                    SqlCommand cmd = Conexion.CreateCommand();
                    cmd.CommandText = consulta;
                    cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = nombre_tipo;
                    if (cmd.ExecuteNonQuery() > 0)
                        return "OK";
                    else
                    {
                        throw new SoapException("El tipo ya existe o los parámetros proporcionados no son correctos", SoapException.ClientFaultCode);
                    }
                }
                catch (Exception ex)
                {
                    Exception innerEx = ex.InnerException ?? ex;
                    throw new SoapException("No se pudo crear el tipo de denuncia", SoapException.ServerFaultCode, innerEx);
                }
            }

        }

        [WebMethod]
        [SoapHeader("User", Required = true)]
        public TipoDenuncia GetTipoDenuncia(string nombre_tipo)
        {
            if (User == null)
            {
                throw new SoapException("Autenticación requerida", SoapException.ClientFaultCode);
            }
            else if (!User.IsValid())
            {
                throw new SoapException("Unauthorized", SoapException.ClientFaultCode);
            }
            var tipoDenuncia = new TipoDenuncia();
            using (SqlConnection Conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["dgtdeskConnectionString"].ConnectionString))
            {
                try
                {
                    Conexion.Open();
                    string consulta =
                        @"select id, name from complaint_type where name = @name";

                    SqlCommand cmd = Conexion.CreateCommand();
                    cmd.CommandText = consulta;
                    cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = nombre_tipo;

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        tipoDenuncia.Id = dr.GetInt32(0);
                        tipoDenuncia.Name = dr.GetString(1);
                    }
                }
                catch (Exception ex)
                {
                    Exception innerEx = ex.InnerException ?? ex;
                    throw new SoapException("No se pudo encontrar el tipo de denuncia", SoapException.ServerFaultCode, innerEx);
                }
                return tipoDenuncia;
            }
        }

        [WebMethod]
        [SoapHeader("User", Required = true)]
        public List<TipoDenuncia> GetTiposDenuncia()
        {
            if (User == null)
            {
                throw new SoapException("Autenticación requerida", SoapException.ClientFaultCode);
            }
            else if (!User.IsValid())
            {
                throw new SoapException("Unauthorized", SoapException.ServerFaultCode);
            }
            var tiposDenuncia = new List<TipoDenuncia>();
            using (SqlConnection Conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["dgtdeskConnectionString"].ConnectionString))
            {
                try
                {
                    Conexion.Open();
                    string consulta =
                        @"select id, name from complaint_type";

                    SqlCommand cmd = Conexion.CreateCommand();
                    cmd.CommandText = consulta;
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        var tipoDenuncia = new TipoDenuncia();
                        tipoDenuncia.Id = dr.GetInt32(0);
                        tipoDenuncia.Name = dr.GetString(1);
                        tiposDenuncia.Add(tipoDenuncia);
                    }
                }
                catch (Exception ex)
                {
                    Exception innerEx = ex.InnerException ?? ex;
                    throw new SoapException("No se pudo encontrar el tipo de denuncia", SoapException.ServerFaultCode, innerEx);
                }
                return tiposDenuncia;
            }
        }
    }
}
