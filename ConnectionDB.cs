using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.Data.OleDb;

namespace PrimerParcial
{
    public static class ConnectionDB
    {
        //private static OleDbConnection conn = new OleDbConnection();

        public static string StrConnection()
        {
           // OleDbConnection conn = new OleDbConnection();
            return new OleDbConnection().ConnectionString = ConfigurationManager.ConnectionStrings["Connection"].ToString();            
        }

        // abrir conexión a Base de Datos de Access
        public static void ConnectToDataBase(OleDbConnection conn)
        {
            DisconnectDataBase(conn);
            conn.Open();
        }

        // cerrar conexión a Base de Datos de Access
        public static void DisconnectDataBase(OleDbConnection conn)
        {
            if (conn !=null && conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        // Query para insertar datos
        public static OleDbCommand InsertCalificacionAlumno(OleDbConnection conn)
        {
            ConnectionDB.ConnectToDataBase(conn);
            return new OleDbCommand("INSERT INTO Calificaciones ([Carrera],[Nombre],[Id],[Calificacion],[Fecha]) values (@Carrera,@Nombre,@Id,@Calificacion,@Fecha)", conn);
        }
    }
}
