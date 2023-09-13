using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using PWSCheck.BLL;
using System.Net;

namespace PSWCheck
{
    internal class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var _exe_date = DateTime.Now;
            PSWCheck pSWCheck = new PSWCheck();
            pSWCheck.Execute(_exe_date);
        }
    }
    public class PSWCheck
    {
        public void Execute(DateTime date)
        {
            UserInfo exec = new UserInfo();
            try
            {
                exec.Start();
            }
            catch (Exception ex)
            {
                exec.Log(ex.Message);
            }
        }
    }
}
