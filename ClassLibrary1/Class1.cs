using System;
using System.Text.RegularExpressions;
using System.Text;

namespace ClassLibrary1
{
    public class Class1
    {
        /// <summary>
        /// 產生合格複雜性密碼
        /// </summary>
        /// <param name="min">密碼最小長度</param>
        /// <param name="max">密碼最大長度</param>
        /// <returns>string</returns>
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
            Regex regex = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*\W)(?=.*[A-Z])" + ".{" + min + "," + max + "}$");
            if (!regex.IsMatch(sb.ToString()))
            {
                this.GenNewPsw(min, max);
            }
            return sb.ToString();
        }
    }
}

