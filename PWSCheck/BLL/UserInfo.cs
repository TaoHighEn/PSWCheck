using PWSCheck.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;


namespace PWSCheck.BLL
{
    internal class UserInfo
    {
        /// <summary>
        /// Log Execute Result
        /// </summary>
        /// <param name="message">Message Need To Log</param>
        internal void Log(string message)
        {
            using (var conn = new SqlConnection("Data Source=10.0.1.15;Initial Catalog='wpap1';Persist Security Info=True;User ID=iemis;Password=ooooo"))
            {
                string sqlcmd = string.Format(
                                @"INSERT INTO CommonLogList (GUID,LogMessage,CreatTime,AlertStatus) values ('{0}','{1}',GETDate(),1)",
                                Guid.NewGuid().ToString(), message
                                );
                new UserInfoRepository().Log(conn,sqlcmd);
            }
        }
        /// <summary>
        /// Execute Start
        /// </summary>
        internal void Start()
        {
            try
            {
                //Select Info According To ERP Valid User  
                using (var conn = new SqlConnection("Data Source=10.0.1.15;Initial Catalog='wpap1';Persist Security Info=True;User ID=iemis;Password=ooooo"))
                {
                    conn.Open();
                    List<User> u_list = new UserInfoRepository().GetUserInfo(conn);
                    var IsInValidList = u_list.Where(x => !(new WebProPswValid48.PswValid().PswValidCheck(x.Password, 8, 20))).ToList();
                    //Update Record Table
                    this.UpdateRecordList(conn, IsInValidList);
                    //Mail To User
                    foreach (User user in IsInValidList)
                    {
                        MailTo(user,conn);
                        this.UpdateUserRecord(conn,user);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// InValid Psw Account Record List Update
        /// </summary>
        /// <param name="conn">DB Connection</param>
        /// <param name="isInValidList">List By InValid Psw User</param>
        private void UpdateRecordList(SqlConnection conn, List<User> isInValidList)
        {
            UserInfoRepository userInfoRepository = new UserInfoRepository();
            userInfoRepository.UpdateRecordList(conn, isInValidList);
        }

        private void MailTo(User user,SqlConnection conn)
        {
            try
            {
                SmtpClient MySMTP = new System.Net.Mail.SmtpClient("192.168.0.19");
                MySMTP.Credentials = new NetworkCredential("itadmin", "A1@345b");
                MySMTP.Send(Mail(user,conn));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Update Mail Time From Record Table
        /// </summary>
        /// <param name="conn">DB Connection</param>
        /// <param name="user">user info</param>
        /// <exception cref="Exception"></exception>
        private void UpdateUserRecord(SqlConnection conn,User user)
        {
            try
            {
                UserInfoRepository userInfoRepository = new UserInfoRepository();
                userInfoRepository.UpdateUserRecord(conn,user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Mail Info Generate
        /// </summary>
        /// <param name="user">User Info</param>
        /// <returns>Mail Info</returns>
        /// <exception cref="Exception"></exception>
        private MailMessage Mail(User user,SqlConnection conn)
        {

            MailMessage message = new MailMessage();
            message.From = new MailAddress("itadmin@web-pro.com.tw");
            message.Subject = "ERP複雜性密碼提醒";
            //查詢user mail Address
            message.To.Add(new MailAddress(user.UserMail));
            //設定Mail Body
            try
            {
                string body = string.Empty;

                if (user.MailTime <= 3)
                {
                    body = string.Format(@"敬愛的衛普同仁 {0}您好：
                            您所使用的ERP密碼不符合密碼複雜度規範，請盡速進行修改！ 
                            密碼複雜度規範： 
                            1、密碼長度8碼~20碼字元 
                            2、需使用大小寫英文及數字
                            3、需使用特殊字元
                            此為第{1}次提醒，您的帳戶密碼將於第3次提醒後由系統逕行修改，並視情況通報貴單位主管。", user.UserName, user.MailTime + 1);
                }
                else
                {
                    string password = new WebProPswValid48.PswValid().GenNewPsw(8, 20);
                    new UserInfoRepository().UpdateUserPsw(conn, user, password);
                    body = string.Format(@"敬愛的衛普同仁 {0}您好：\n\n
                            ERP複雜性密碼提醒已超過3次，系統已將您的ERP密碼修改；\n
                            新密碼：{1}，請盡速登入ERP修改密碼。", user.UserName, password);
                }
                message.Body = body;
                return message;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
public class User
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string UserMail { get; set; }
    public string Password { get; set; }
    public int MailTime { get; set; }
}
