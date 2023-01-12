using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Libraries
{
    public class Base64Crypt
    {
        private string S;
        private string K;
        private List<char> T;
        public Base64Crypt()
        {
            T = new List<char>();
            // K = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやよらりるれろわをぐげござじずぞだぢづでばびぶべぱぴぷぺぽ";
            K = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";//标准码表
        }
        public string Token
        {
            get
            {
                return S == null ? K : S;
            }
            set
            {
                T.Clear();
                S = value;
                if (S == null)
                {
                    foreach (var item in K)
                    {
                        T.Add(item);
                    }
                }else if (S.Length < 64)
                {
                    foreach (var item in S)
                    {
                        T.Add(item);
                    }
                    for (int i = 0; i < 64-S.Length; i++)
                    {
                        T.Add(K[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < 64; i++)
                    {
                        T.Add(S[i]);
                    }
                }
            }
        }
 
        public string Encode(string x)
        {
            return string.IsNullOrEmpty(x) ? x : InternalEncode(Encoding.UTF8.GetBytes(x));
        }
        public string Decode(string x)
        {
            return string.IsNullOrEmpty(x) ? x : Encoding.UTF8.GetString(InternalDecode(x));
        }
 
        public byte[] Encode(byte[] x)
        {
            return x == null ? null : Encoding.UTF8.GetBytes(InternalEncode(x));
        }
        public byte[] Decode(byte[] x)
        {
            return x == null ? null : InternalDecode(Encoding.UTF8.GetString(x));
        }
        private void CheckToken()
        {
            if (T.Count != 64)
            {
                Token = K;
            }
        }
        private byte[] InternalDecode(string x)
        {
            CheckToken();
            byte[] r;
            string t;
            int p = 0;
            int m = x.Length / 4;
            int n = x.Length % 4;
            if (n == 0)
            {
                r = new byte[3 * m];
            }
            else
            {
                r = new byte[3 * m + n-1];
                t = string.Empty;
 
                for (int i = n; i > 0; i--)
                {
                    t += ByteToBin((byte)T.IndexOf(x[x.Length - i])).Substring(2);
                }
 
                for (int i = 0; i < n-1 ; i++)
                {
                    r[3 * m + i] = BinToByte(t.Substring(8 * i, 8));
                }
            }
            for (int i = 0; i < m; i++)
            {
                t = string.Empty;
                for (int j = 0; j < 4; j++)
                {
                    t += ByteToBin((byte)T.IndexOf(x[4*i+j])).Substring(2);
                }
                for (int j = 0; j < t.Length/8; j++)
                {
                    r[p++] = BinToByte(t.Substring(8*j,8));
                }
            }
            return r;
        }
        private string InternalEncode(byte[] x)
        {
            CheckToken();
            string r = string.Empty;
            string t;
            int m = x.Length / 3;
            int n = x.Length % 3;
            for (int i = 0; i < m; i++)
            {
                t = string.Empty;
                for (int j = 0; j < 3; j++)
                {
                    t += ByteToBin(x[3 * i + j]);
                }
                r += base64Encode(t);
            }
 
            if (n == 1)
            {
                t = ByteToBin(x[x.Length-1]).PadRight(12,'0');
                r += base64Encode(t);
            }
            else if (n == 2)
            {
                t = string.Empty;
                for (int i = n; i > 0; i--)
                {
                    t += ByteToBin(x[x.Length - i]);
                }
                t = t.PadRight(18,'0');
                r += base64Encode(t);
            }
            return r;
        }
        private string base64Encode(string x)
        {
            string r = string.Empty;
            for (int i = 0; i < x.Length / 6; i++)
            {
                r += T[BinToByte(x.Substring(6 * i, 6))];
            }
            return r;
        }
        
        private string ByteToBin(byte x)
        {
            return Convert.ToString(x,2).PadLeft(8,'0');
        }
        private byte BinToByte(string x)
        {
            return Convert.ToByte(x,2);
        }
 
    }
}