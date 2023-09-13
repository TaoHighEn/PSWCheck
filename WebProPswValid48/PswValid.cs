using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebProPswValid48
{
    public class PswValid
    {
        /// <summary>
        /// Create Valid Psw
        /// </summary>
        /// <param name="min">Length Min</param>
        /// <param name="max">Length Max</param>
        /// <returns>New PSW</returns>
        public string GenNewPsw(int min, int max)
        {
            const string char1 = "0123456789";
            const string char2 = "abcdefghijklmnopqrstuvwxyz";
            const string char3 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            int psw_length = rnd.Next(min, max + 1);
            while (sb.Length != psw_length)
            {
                int ch = rnd.Next(1, 5);
                int index = 0;
                switch (ch)
                {
                    case 1:
                        index = rnd.Next(char1.Length);
                        sb.Append(char1[index]);
                        break;
                    case 2:
                        index = rnd.Next(char1.Length);
                        sb.Append(char2[index]);
                        break;
                    case 3:
                        index = rnd.Next(char1.Length);
                        sb.Append(char3[index]);
                        break;
                    case 4:
                        int A = rnd.Next(1, 5);
                        switch (A)
                        {
                            case 1:
                                index = rnd.Next(33, 48);
                                sb.Append(((char)index).ToString());
                                break;
                            case 2:
                                index = rnd.Next(58, 65);
                                sb.Append(((char)index).ToString());
                                break;
                            case 3:
                                index = rnd.Next(91, 97);
                                sb.Append(((char)index).ToString());
                                break;
                            case 4:
                                index = rnd.Next(123, 127);
                                sb.Append(((char)index).ToString());
                                break;
                        }
                        break;
                }
            }
            if (!PswValidCheck(sb.ToString(),min,max))
            {
                this.GenNewPsw(min, max);
            }
            return sb.ToString();
        }
        /// <summary>
        /// Check Psw
        /// </summary>
        /// <param name="c_psw">Psw</param>
        /// <param name="min">Length Min</param>
        /// <param name="max">Length Max</param>
        /// <returns>Boolean</returns>
        public bool PswValidCheck(string c_psw,int min,int max)
        {
            Regex regex = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*\W)(?=.*[A-Z])" + ".{" + min + "," + max + "}$");
            return regex.IsMatch(c_psw);
        }
    }
}
