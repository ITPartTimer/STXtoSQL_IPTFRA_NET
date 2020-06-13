using System;
using System.Reflection;
using System.Data.Odbc;
using System.Collections.Generic;
using System.Text;
using STXtoSQL.Models;

namespace STXtoSQL.DataAccess
{
    public class ODBCData : Helpers
    {
        public List<IPTFRA> Get_IPTFRA()
        {

            List<IPTFRA> lstIPTFRA = new List<IPTFRA>();

            OdbcConnection conn = new OdbcConnection(ODBCDataConnString);

            try
            {
                conn.Open();

                // Try to split with verbatim literal
                OdbcCommand cmd = conn.CreateCommand();
              
                cmd.CommandText = @"select
                                    fra_job_no,fra_wdth_1,fra_nbr_slit_1,fra_wdth_2,fra_nbr_slit_2,fra_wdth_3,fra_nbr_slit_3,fra_wdth_4,fra_nbr_slit_4,fra_wdth_5,fra_nbr_slit_5,
                                    fra_wdth_6,fra_nbr_slit_6,fra_wdth_7,fra_nbr_slit_7,fra_wdth_8,fra_nbr_slit_8,fra_wdth_9,fra_nbr_slit_9,fra_wdth_10,fra_nbr_slit_10,
                                    fra_wdth_11,fra_nbr_slit_11,fra_wdth_12,fra_nbr_slit_12,fra_wdth_13,fra_nbr_slit_13,fra_wdth_14,fra_nbr_slit_14,fra_wdth_15,fra_nbr_slit_15,
                                    fra_wdth_16,fra_nbr_slit_16,fra_wdth_17,fra_nbr_slit_17,fra_wdth_18,fra_nbr_slit_18,fra_wdth_19,fra_nbr_slit_19,fra_wdth_20,fra_nbr_slit_20,
                                    fra_wdth_21,fra_nbr_slit_21,fra_wdth_22,fra_nbr_slit_22,fra_wdth_23,fra_nbr_slit_23,fra_wdth_24,fra_nbr_slit_24,fra_wdth_25,fra_nbr_slit_25,
                                    fra_wdth_26,fra_nbr_slit_26,fra_wdth_27,fra_nbr_slit_27,fra_wdth_28,fra_nbr_slit_28,fra_wdth_29,fra_nbr_slit_29,fra_wdth_30,fra_nbr_slit_30,
                                    fra_wdth_31,fra_nbr_slit_31,fra_wdth_32,fra_nbr_slit_32,fra_wdth_33,fra_nbr_slit_33,fra_wdth_34,fra_nbr_slit_34,fra_wdth_35,fra_nbr_slit_35,
                                    fra_wdth_36,fra_nbr_slit_36,fra_wdth_37,fra_nbr_slit_37,fra_wdth_38,fra_nbr_slit_38,fra_wdth_39,fra_nbr_slit_39,fra_wdth_40,fra_nbr_slit_40,
                                    fra_wdth_41,fra_nbr_slit_41,fra_wdth_42,fra_nbr_slit_42,fra_wdth_43,fra_nbr_slit_43,fra_wdth_44,fra_nbr_slit_44,fra_wdth_45,fra_nbr_slit_45,
                                    fra_wdth_46,fra_nbr_slit_46,fra_wdth_47,fra_nbr_slit_47,fra_wdth_48,fra_nbr_slit_48,fra_wdth_49,fra_nbr_slit_49,fra_wdth_50,fra_nbr_slit_50,
                                    fra_wdth_51,fra_nbr_slit_51,fra_wdth_52,fra_nbr_slit_52,fra_wdth_53,fra_nbr_slit_53,fra_wdth_54,fra_nbr_slit_54,fra_wdth_55,fra_nbr_slit_55,
                                    fra_wdth_56,fra_nbr_slit_56,fra_wdth_57,fra_nbr_slit_57,fra_wdth_58,fra_nbr_slit_58,fra_wdth_59,fra_nbr_slit_59,fra_wdth_60,fra_nbr_slit_60,
                                    fra_tot_wdth
                                    from iptfra_rec
                                    where fra_job_no in
                                    (select psh_job_no
                                    from iptpsh_rec s
                                    inner join iptjob_rec j
                                    on j.job_job_no = s.psh_job_no
                                    where s.psh_whs = 'SW'
                                    and psh_sch_seq_no <> 99999999
                                    and(job_job_sts = 0 or job_job_sts = 1)
                                    and (job_prs_cl = 'SL' or job_prs_cl = 'CL' or job_prs_cl = 'MB'))";

                OdbcDataReader rdr = cmd.ExecuteReader();

                using (rdr)
                {
                    while (rdr.Read())
                    {
                        IPTFRA b = new IPTFRA();

                        b.job_no = Convert.ToInt32(rdr["fra_job_no"]);
                        b.tot_wdth = Convert.ToDecimal(rdr["fra_tot_wdth"]);

                        /*
                         * Step through each pair of wdth_X and nbr_X properties in IPTFRA
                         * Use reflection to get the property name, then set the value using
                         * a variable for wdth_X and nbr_X
                         */
                        PropertyInfo[] props = b.GetType().GetProperties();

                        for (int a = 1; a < 61; a++)
                        {
                            string w = "wdth_" + a.ToString();
                            string n = "nbr_" + a.ToString();

                            PropertyInfo propertyWdth = b.GetType().GetProperty(w);
                            propertyWdth.SetValue(b, Convert.ToDecimal(rdr["fra_wdth_" + a.ToString()]));

                            PropertyInfo propertyNbr = b.GetType().GetProperty(n);
                            propertyNbr.SetValue(b, Convert.ToInt32(rdr["fra_nbr_slit_" + a.ToString()]));
                        }

                        lstIPTFRA.Add(b);
                    }
                }
            }
            catch (OdbcException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            return lstIPTFRA;
        }
    }
}
