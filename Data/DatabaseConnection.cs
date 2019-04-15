using System;
using System.Collections.Generic;
using System.Data.Odbc;

namespace JNCC.Microsite.SAC.Data
{
    public class DatabaseConnection : IDisposable
    {
        private OdbcConnection _conn;

        public DatabaseConnection(string filePath)
        {
            _conn = new OdbcConnection(String.Format("Driver={{Microsoft Access Driver (*.mdb, *.accdb)}};DBQ={0}", filePath));
            _conn.Open();
        }

        public OdbcCommand CreateCommand(string command) {
            return new OdbcCommand(command, _conn);
        }

        public OdbcDataReader RunCommand(OdbcCommand command) {
            return command.ExecuteReader();
        }

        public void Dispose()
        {
            _conn.Close();
        }
    }
}