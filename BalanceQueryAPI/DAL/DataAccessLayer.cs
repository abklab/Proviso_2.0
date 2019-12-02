using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BalanceQueryAPI.Models;

namespace BalanceQueryAPI.DAL
{
    public class DataAccessLayer
    {
        private string _errorMessage;
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
        }
        #region Data Access Operations

        string connectionString = ConfigurationManager.ConnectionStrings["PROVISO_ConnectionString"].ToString();

        SqlConnection connection;

        DataTable GetData(SqlCommand cmd)
        {
            DataTable data = new DataTable();
            string msg = "";
            try
            {
                connection = new SqlConnection(connectionString);
                using (connection)
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(data);

                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return data;
        }

        string Save(SqlCommand cmd)
        {
            connection = new SqlConnection(connectionString);
            string msg = "";
            try
            {
                using (connection)
                {

                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    int i = cmd.ExecuteNonQuery();
                    msg = i > 0 ? "Success" : "Fail";

                }
                return msg;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            finally
            {
                connection.Close();

                cmd.Dispose();
            }
            return msg;
        }
        #endregion

        //Get All accounts
        private IEnumerable<LoanAccount> GetLoanAccounts()
        {
            try
            {
                using (var command = new SqlCommand())
                {
                    var accounts = new List<LoanAccount>();

                    command.CommandText = "usp_GetAll_LoanOutStandingBalance";

                    var dt = GetData(command);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow r in dt.Rows)
                        {
                            var account = new LoanAccount
                            {
                                Account_Number = r["Account_Number"].ToString(),
                                Account_Name = r["Account_Name"].ToString(),
                                Balance = Convert.ToDecimal(r["Balance"])
                            };
                            accounts.Add(account);
                        }
                    }
                    return accounts;
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
            return null;
        }

        //Get Loan Account Outstanding by Account number
        public LoanAccount GetLoanOutstandingBalance(string account_number)
        {
            try
            {
                using (var command = new SqlCommand())
                {
                    command.CommandText = "usp_Get_LoanOutStandingBalance";
                    command.Parameters.AddWithValue("@accountnumber", account_number);

                    var dt = GetData(command);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var r = dt.Rows[0];

                        var account = new LoanAccount
                        {
                            Account_Number = r["Account_Number"].ToString(),
                            Account_Name = r["Account_Name"].ToString(),
                            Balance = Convert.ToDecimal(r["Balance"])
                        };
                        return account;
                    }
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;

            }
            return null;
        }

        //Verify apikey
        public bool AuthenticateKey(string apikey, string clientname)
        {
            var today = DateTime.Today;
            bool isVerified = false;
            DateTime experationDate;

            try
            {
                using (var command = new SqlCommand())
                {
                    command.CommandText = "usp_AuthenticateKey";
                    command.Parameters.AddWithValue("@apikey", apikey);
                    command.Parameters.AddWithValue("@clientname", clientname);

                    var dt = GetData(command);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var r = dt.Rows[0];
                        var isActive = (bool)r["IsActive"];
                        var isValidDate = DateTime.TryParse((r["ExpirationDate"]).ToString(), out experationDate);

                        if (isValidDate && experationDate > today && isActive)
                            isVerified = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
            return isVerified;
        }
    }
}