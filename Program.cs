using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            NewCode();
            Console.ReadKey();
        }
        static private void OldCode()
        {
            //ログインユーザ情報
            var userInfo = new { IsAdmin = false, Dept = "財部" };

            var sb = new StringBuilder();
            sb.AppendLine("SELECT ID,NAME,DEPT FROM EMPLOYEE");
            sb.AppendLine("WHERE 1=1");
            
            if (!string.IsNullOrEmpty(userInfo.Dept))
            {
                if (!userInfo.IsAdmin && userInfo.Dept != "総務")
                {
                    sb.AppendFormat("AND DEPT = '{0}'{1}", userInfo.Dept, Environment.NewLine);                    
                }
            }
            if (userInfo.IsAdmin) {
                sb.AppendLine("UNION ALL");
                sb.AppendLine("SELECT ID,NAME,DEPT FROM RETIREE");
            }
        

            Console.WriteLine(sb.ToString());
        }


        static private void NewCode() {
            //ログインユーザ情報
            var userInfo = new { IsAdmin = true,Dept="財務" };           

            //条件が長いのでここで。。
            Func<bool> predicate = () => !userInfo.IsAdmin && userInfo.Dept != "総務";
            
            var sb = new StringBuilder();            
            sb.AppendLine("SELECT ID,NAME,DEPT FROM EMPLOYEE")
              .AppendLine("WHERE 1=1") 
              .IfAppendFormatLine(predicate, "AND DEPT = '{0}'" , userInfo.Dept)
              .IfAppendLine(()=> userInfo.IsAdmin, "UNION ALL")
              .IfAppendLine(() => userInfo.IsAdmin, "SELECT ID,NAME,DEPT FROM RETIREE");
            
            Console.WriteLine(sb.ToString());

        }
    }

    static class StringBuilderExtensions {        

        //条件に一致する場合にAppendLineする。
        static public StringBuilder IfAppendLine(this StringBuilder source, Func<bool> fnc, string value)
        {
            if (!fnc()) return source;

            return source.AppendLine(value);
        }

        //書式指定文字列を処理して追加 and 末尾に改行追加
        //書式文字列が空文字列・スペース・Nullならインスタンスをそのまま返す。
        static public StringBuilder AppendFormatLine(this StringBuilder source, string format, params object[] args)
        {
            if (args.Any(obj => string.IsNullOrWhiteSpace(obj.ToString()))) return source;

            return source.AppendLine(string.Format(format, args));
        }

        //書式指定文字列を処理して追加 and 末尾に改行追加する。
        //書式文字列が空文字列・スペース・Nullならインスタンスをそのまま返す。
        //条件に一致しない場合はインスタンスをそのまま返す。
        static public StringBuilder IfAppendFormatLine(this StringBuilder source, Func<bool> fnc, string format,  params object[] args)
        {
            if (args.Any(obj => string.IsNullOrWhiteSpace(obj.ToString()))) return source;

            if (!fnc()) return source; 

            return source.AppendFormatLine(format, args);
        }
        

    }
}
