using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STXtoSQL.Log;
using STXtoSQL.DataAccess;
using STXtoSQL.Models;

namespace STXtoSQL_IPTFRA_NET
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.LogWrite("MSG", "Start: " + DateTime.Now.ToString());

            // Declare and defaults
            int odbcCnt = 0;
            int insertCnt = 0;
            int importCnt = 0;
            int arborCnt = 0;

            #region FromSTRATIX
            ODBCData objODBC = new ODBCData();

            List<IPTFRA> lstIPTFRA = new List<IPTFRA>();

            try
            {
                lstIPTFRA = objODBC.Get_IPTFRA();
            }
            catch (Exception ex)
            {
                Logger.LogWrite("EXC", ex);
                Logger.LogWrite("MSG", "Return");
                return;
            }
            #endregion

            #region ToSQL
            SQLData objSQL = new SQLData();

            // Only work in SQL database, if records were retreived from Stratix
            if (lstIPTFRA.Count != 0)
            {
                odbcCnt = lstIPTFRA.Count;

                // Put Stratix data in lstIPTFRA into IMPORT IPTFRA table
                try
                {
                    importCnt = objSQL.Write_IPTFRA_IMPORT(lstIPTFRA);
                }
                catch (Exception ex)
                {
                    Logger.LogWrite("EXC", ex);
                    Logger.LogWrite("MSG", "Return");
                    return;
                }

                /*
                 * Build a more useful arbor from the Stratix data for each job
                 * Build Arbor from IMPORT and into Arbor table
                 */
                try
                {
                    arborCnt = objSQL.Build_Arbor();
                }
                catch (Exception ex)
                {
                    Logger.LogWrite("EXC", ex);
                    Logger.LogWrite("MSG", "Return");
                    return;
                }

                // Call SP to put IMPORT IPTFRA table data into WIP IPTFRA table
                try
                {
                    insertCnt = objSQL.Write_IMPORT_to_IPTFRA();
                }
                catch (Exception ex)
                {
                    Logger.LogWrite("EXC", ex);
                    Logger.LogWrite("MSG", "Return");
                    return;
                }

                Logger.LogWrite("MSG", "ODBC/IMPORT/ARBOR/INSERT=" + odbcCnt.ToString() + ":" + importCnt.ToString() + ":" + arborCnt.ToString() + ":" + insertCnt.ToString());
            }
            else
                Logger.LogWrite("MSG", "No data");

            Logger.LogWrite("MSG", "End: " + DateTime.Now.ToString());
            #endregion

            // Testing
            //Console.WriteLine("Press key to exit");
            //Console.ReadKey();
        }
    }
}
