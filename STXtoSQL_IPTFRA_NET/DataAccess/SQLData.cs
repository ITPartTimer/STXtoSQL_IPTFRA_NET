using System;
using System.Reflection;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using STXtoSQL.Models;

namespace STXtoSQL.DataAccess
{
    class SQLData : Helpers
    {
        // Insert list of IPTFRA from STRATIX into IMPORT
        public int Write_IPTFRA_IMPORT(List<IPTFRA> lstIPTFRA)
        {
            // Returning rows inserted into IMPORT
            int r = 0;

            SqlConnection conn = new SqlConnection(STRATIXDataConnString);

            try
            {
                conn.Open();

                SqlTransaction trans = conn.BeginTransaction();

                SqlCommand cmd = new SqlCommand();

                cmd.Transaction = trans;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;

                // First empty IMPORT table
                try
                {
                    cmd.CommandText = "DELETE from ST_IMPORT_tbl_IPTFRA";

                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }

                try
                {
                    /*
                     * Put data in SQL in two parts.  
                     * 1. For each IPTFRA in the list, INSERT job_no and tot_wdth
                     * 2. Loop through each wdth_X and nbr_X pair
                     * IMPORT_tbl_IPTFRA needs to allow NULLS for the wdth_X and nbr_X fields
                     */
                    cmd.CommandText = "INSERT INTO ST_IMPORT_tbl_IPTFRA (fra_job_no,fra_tot_wdth) VALUES (@arg1,@arg2)";
                  
                    foreach (IPTFRA s in lstIPTFRA)
                    {
                        int j = Convert.ToInt32(s.job_no);

                        cmd.Parameters.Add("@arg1", SqlDbType.Int).Value = j;
                        cmd.Parameters.Add("@arg2", SqlDbType.Decimal).Value = Convert.ToDecimal(s.tot_wdth);

                        cmd.ExecuteNonQuery();

                        for (int a = 1; a < 61; a++)
                        {
                            string w = "wdth_" + a.ToString();
                            string n = "nbr_" + a.ToString();

                            PropertyInfo propertyWdth = s.GetType().GetProperty(w);
                            string wdth = propertyWdth.GetValue(s, null).ToString();

                            PropertyInfo propertyNbr = s.GetType().GetProperty(n);
                            string nbr = propertyWdth.GetValue(s, null).ToString();

                            cmd.CommandText = "UPDATE ST_IMPORT_tbl_IPTFRA SET fra_wdth_" + a.ToString() + "=" + wdth + ",fra_nbr_slit_" + a.ToString() + "=" + nbr + " WHERE fra_Job_No=" + j.ToString();

                            cmd.ExecuteNonQuery();
                        }                       
                    }

                    trans.Commit();
                }
                catch (Exception)
                {
                    // Shouldn't have a Transaction hanging, so rollback
                    trans.Rollback();
                    throw;
                }
                try
                {
                    // Get count of rows inserted into IMPORT
                    cmd.CommandText = "SELECT COUNT(fra_job_no) from ST_IMPORT_tbl_IPTFRA";
                    r = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // No matter what close and dispose of the connetion
                conn.Close();
                conn.Dispose();
            }

            return r;
        }

        // Insert values from IMPORT into WIP IPTFRA
        public int Write_IMPORT_to_IPTFRA()
        {
            // Returning rows inserted into IMPORT
            int r = 0;

            SqlConnection conn = new SqlConnection(STRATIXDataConnString);

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;

                // Call SP to copy IMPORT to IPTFRA table.  Return rows inserted.
                cmd.CommandText = "ST_proc_IMPORT_to_IPTFRA";
              
                AddParamToSQLCmd(cmd, "@rows", SqlDbType.Int, 8, ParameterDirection.Output);

                cmd.ExecuteNonQuery();

                r = Convert.ToInt32(cmd.Parameters["@rows"].Value);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // No matter what close and dispose of the connetion
                conn.Close();
                conn.Dispose();
            }

            return r;
        } 
    }
}
