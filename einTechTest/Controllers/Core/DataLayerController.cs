using einTechTest.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace einTechTest.Controllers.Core
{
    public class DataLayerController
    {
        private const string connectionString = "data source=DELL; initial catalog=eintechTest;Integrated Security=True";

        public int EditPerson(PersonModel person)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter param = new SqlParameter("@PersonID", person.ID);
            param.Direction = ParameterDirection.Input;
            param.SqlDbType = SqlDbType.Int;
            parameters.Add(param);

            param = new SqlParameter("@PersonName", person.Name);
            param.Direction = ParameterDirection.Input;
            param.SqlDbType = SqlDbType.NVarChar;
            param.Size = 255;
            parameters.Add(param);

            param = new SqlParameter("@GroupID", person.Group.ID);
            param.Direction = ParameterDirection.Input;
            param.SqlDbType = SqlDbType.Int;
            parameters.Add(param);

            param = new SqlParameter();
            param.ParameterName = "@count";
            param.Direction = ParameterDirection.Output;
            param.SqlDbType = SqlDbType.Int;
            parameters.Add(param);

            return ExcuteNonQuery("[dbo].[sp_EditPerson]", true, parameters, true, "@count");
        }

        public int DeletePerson(int ID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter param = new SqlParameter("@PersonID", ID);
            param.Direction = ParameterDirection.Input;
            param.SqlDbType = SqlDbType.Int;
            parameters.Add(param);

            param = new SqlParameter();
            param.ParameterName = "@count";
            param.Direction = ParameterDirection.Output;
            param.SqlDbType = SqlDbType.Int;
            parameters.Add(param);

            return ExcuteNonQuery("[dbo].[sp_DeletePerson]", true, parameters, true, "@count");
        }

        public int AddPerson(PersonModel NewPerson)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter param = new SqlParameter("@PersonName", NewPerson.Name);
            param.Direction = ParameterDirection.Input;
            param.SqlDbType = SqlDbType.NVarChar;
            param.Size = 255;
            parameters.Add(param);

            param = new SqlParameter("@GroupID", NewPerson.Group.ID);
            param.Direction = ParameterDirection.Input;
            param.SqlDbType = SqlDbType.Int;
            parameters.Add(param);

            param = new SqlParameter();
            param.ParameterName = "@PersonID";
            param.Direction = ParameterDirection.Output;
            param.SqlDbType = SqlDbType.Int;
            parameters.Add(param);

            return ExcuteNonQuery("[dbo].[sp_AddPerson]", true, parameters, true, "@PersonID");
        }

        public List<PersonModel> GetPersons()
        {
            return SystemController.Instance.DataLayerController.ExcuteObject<PersonModel>("[dbo].[sp_SearchPersonsByName]").ToList();
        }

        public List<GroupModel> GetGroups()
        {
            return SystemController.Instance.DataLayerController.ExcuteObject<GroupModel>("SELECT * FROM [dbo].[Groups]", false).ToList();
        }

        public List<PersonModel> SearchPersonByName(string name) {
            return SystemController.Instance.DataLayerController.ExcuteObject<PersonModel>("[dbo].[sp_SearchPersonsByName]").ToList();
        }

        private int ExcuteNonQuery(string storedProcedureorCommandText, bool isStoredProcedure = true, List<SqlParameter> parameters = null, bool outuput = false, string outputParam = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    if (isStoredProcedure)
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                            foreach (SqlParameter param in parameters)
                                command.Parameters.Add(param);
                    }
                    command.CommandText = storedProcedureorCommandText;
                    connection.Open();
                    command.ExecuteNonQuery();
                    int outuputValue =  outuput && outputParam != null ? Convert.ToInt32(command.Parameters[outputParam].Value) : 0;
                    return outuputValue;
                }
            }
        }

        private DataTable Excute(string storedProcedureorCommandText, bool isStoredProcedure = true, List<SqlParameter> parameters = null)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    if (isStoredProcedure)
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        if(parameters != null)
                            foreach(SqlParameter param in parameters)
                                command.Parameters.Add(param);
                    }
                    command.CommandText = storedProcedureorCommandText;
                    connection.Open();

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dataTable);

                    return dataTable;
                }
            }
        }

        private IEnumerable<T> ExcuteObject<T>(string storedProcedureorCommandText, bool isStoredProcedure = true)
        {
            List<T> items = new List<T>();
            var dataTable = Excute(storedProcedureorCommandText, isStoredProcedure);
            foreach (var row in dataTable.Rows)
            {
                T item = (T)Activator.CreateInstance(typeof(T), row);
                items.Add(item);
            }
            return items;
        }

    }
}