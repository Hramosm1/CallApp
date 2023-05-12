using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CallAPP.BD
{
    class conexionBD
    {

        private static SqlConnection conectar() // Realizar la conexion a la base de datos
        {
            SqlConnection cn = new SqlConnection("SERVER=192.168.8.8;DATABASE=InteligenciaDB_Fase2;USER=sicdesk;PASSWORD=dr8%25H#3%20;Integrated security=true");

            return cn;

        }

        public static DataTable Consulta(String sp, String[] Variables, String[] Valores) //Realizamos la ejecucion de un procedimiento almacenado
        {
            DataTable Consulta = new DataTable();
            try
            {

                conectar().Open();

                SqlCommand comando = conectar().CreateCommand();
                comando.CommandTimeout = 0;
                comando.CommandText = sp;
                comando.CommandType = CommandType.StoredProcedure;

                int cont = -1;
                foreach (string i in Variables)
                {
                    cont += 1;
                    comando.Parameters.AddWithValue(i, Valores[cont]);
                }

                SqlDataAdapter datos = new SqlDataAdapter(comando);
                datos.Fill(Consulta);

                conectar().Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error de BD: " + ex);
            }

            return Consulta;


        }
    }
}
