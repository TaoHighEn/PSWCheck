using Dapper;
using PWSCheck.BLL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PWSCheck.DAL
{
    internal class UserInfoRepository
    {
        /// <summary>
        /// Get ERP User List
        /// </summary>
        /// <param name="conn">DB Connection</param>
        /// <returns></returns>
        public List<User> GetUserInfo(SqlConnection conn)
        {
            string sqlcmd = string.Format(@"Select distinct 
     CASE  WHEN a.pa_no IS NULL
	 THEN b.cu_sale 
	 ELSE a.pa_no END  AS UserId,
	 b.pr_fname UserName,
     b.pr_psword Password,
     d.[EMAIL] UserMail,
     e.MAILTIME MailTime
     FROM iess01h b
	 Left join iepa00h a on a.pa_no = b.pr_name
	 Left join iepb03h c on a.pa_no = c.cu_sale
	 Left Join [10.10.16.13\WFSQLSERVER].[UOF].[dbo].[TB_EB_USER] d on d.ACCOUNT COLLATE Chinese_Taiwan_Stroke_CI_AS = a.pa_no OR d.[NAME] COLLATE Chinese_Taiwan_Stroke_CI_AS = b.pr_fname
     Left Join MailTime_Record e on e.[USER_ID] = a.pa_no or e.[USER_ID] = b.cu_sale
	 where (c.pa_id2 =1  OR c.pa_id2 is NULL) AND
	 (a.pa_oudat =''or a.pa_oudat is null) and
	 d.[EMAIL] is not NULL "); 
#if DEBUG
    sqlcmd += " and (a.dp_no = 'I0100' or pr_fname = '馬恩奇')";
#endif
            return conn.Query<User>(sqlcmd).ToList();
        }

        internal void Log(SqlConnection conn, string sqlcmd)
        {
            conn.Execute(sqlcmd);
        }

        /// <summary>
        /// InValid Psw Account Record List Update
        /// </summary>
        /// <param name="conn">DB Connection</param>
        /// <param name="isInValidList">List By InValid Psw User</param>
        internal void UpdateRecordList(SqlConnection conn, List<User> isInValidList)
        {
            try
            {
                string sqlcmd = string.Empty;
                if (isInValidList.Count == 0)
                {
                    return;
                }
                var ulist = isInValidList.Select(x => x.UserId).ToArray(); 
                sqlcmd = string.Format(@"delete from MailTime_Record where USER_ID NOT IN ('{0}')",string.Join("','",ulist));
                conn.Query(sqlcmd);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        internal void UpdateUserPsw(SqlConnection conn, User user, string password)
        {
            try 
            {
                string UserId = user.UserId;
                string sqlcmd = "Update iess01h set pr_psword = @password Where pr_name = @UserId";
                SqlCommand command = new SqlCommand(sqlcmd,conn);
                command.Parameters.AddWithValue("@password",password);
                command.Parameters.AddWithValue("@UserId",UserId);
                command.ExecuteNonQuery();
            }
            catch(Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Update Mail Time From Record Table
        /// </summary>
        /// /// <param name="conn">DB Connection</param>
        /// <param name="user">User Info</param>
        internal void UpdateUserRecord(SqlConnection conn, User user)
        {
            try
            {
                string sqlcmd = string.Empty;
                if (user.MailTime == 0)
                {
                    sqlcmd = string.Format("insert into MailTime_Record values ('{0}',1,GETDATE())",user.UserId);
                    conn.Query(sqlcmd);
                }
                else
                {
                    sqlcmd = string.Format("update MailTime_Record set MAILTIME = {0} Where USER_ID = '{0}'", user.MailTime + 1,user.UserId);
                    conn.Query(sqlcmd);
                }
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
