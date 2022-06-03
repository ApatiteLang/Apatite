using System;
using System.IO;

namespace Utils
{

    public enum importType
    {
        normal_aptt = 0,
        normal_c = 1,
        normal_folder = 2,
        user_aptt = 3,
        user_c = 4,
        user_folder = 5,
        unknown = 6
           
    }

    public class line
    {
        string val;

        public line(string v)
        {
            this.val = v;
        }


        public bool is_import()
        {
            return val.StartsWith("import ");
        }
        public importType get_import_type()
        {
            string line = val.remove_start("import ");

            if (line.EndsWith(".aptt")) // import lib.aptt
                return importType.normal_aptt;
            if (line.EndsWith(".cpp"))// import lib.cpp
                return importType.normal_c;
            if (line.EndsWith("*")) // import folder*
                return importType.normal_folder;

            if (line.StartsWith("\"") && line.EndsWith("\""))
            {
                line = line.brackets("\"");

                if (line.EndsWith(".aptt")) // import "lib.aptt"
                    return importType.user_aptt;
                if (line.EndsWith(".cpp")) // import "lib.cpp"
                    return importType.user_c;
                if (line.EndsWith("*")) // import "folder*"
                    return importType.user_folder;

            }

            return importType.normal_aptt;

        }
        public string get_import_ext()
        {
            string nval = val.remove_start("import ");
            if (nval.Split(".").Length > 1) //  lib.aptt or  "lib.aptt"
            {
                return nval.Split(".")[nval.Split(".").Length - 1].remove_end("\"");
            }

            if (nval.brackets("\"") == nval) // std
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                if (File.Exists(dir + "lib\\" + nval + ".aptt"))
                    return "aptt";
                if (File.Exists(dir + "lib\\" + nval + ".cpp"))
                    return "cpp";

            }
            else
            {
                if (File.Exists(nval + ".aptt"))
                    return "aptt";
                if (File.Exists(nval + ".cpp"))
                    return "cpp";
            }

            return "aptt";

        }
        public string get_import_name() // std std.aptt "std.aptt" "not\simple.at.all.aptt"
        {
            string nval = val.remove_start("import ");
            if (val.Split("\\").Length > 0)
                nval = nval.Split("\\")[nval.Split("\\").Length - 1];

            if (val.Split(".").Length > 0)
                nval = nval.brackets("\"").remove_end("." + nval.Split(".")[nval.Split(".").Length - 1]);

            return nval;
        }
        public string get_import_path() // import std -> "lib\std.aptt"        import "std" -> "std"
        {
            if (get_import_type() == importType.normal_c || get_import_type() == importType.normal_aptt || get_import_type() == importType.normal_folder) // -> not user defined
            {
                return AppDomain.CurrentDomain.BaseDirectory+"lib\\" + get_import_name() + "." + get_import_ext();
            }
            else
            {
                return val.remove_start("import ").brackets("\"");
            }

        }

        public bool is_function_defenition() // function name() -> int {
        {
            return val.StartsWith("function ");
        }

        public string get_function_defenition_name()
        {
            return val.remove_start("function ").Split("(")[0];
        }
        public string get_function_defenition_return_type() // function name() -> TYPE {
        {
            if(val.splitstr("->").Length > 1)
                return val.splitstr("->")[1].TrimEnd('{').Trim();

            return "void";
        }
        public string get_function_defenition_args() // function name(a,b) -> type           a,b
        {
            return val.Trim().TrimEnd('{').remove_start("function ").remove_start(get_function_defenition_name()).Trim().remove_end(get_function_defenition_return_type()).Trim().remove_end("->").Trim().brackets("(", ")");
        }
        public string remmap_f_d_type_system() // a: int, b: float      int a, int b
        {
            string res = "";

            if(val == "")
                return res;

            foreach(string c in val.Split(","))
            {
                res += c.Trim().Split(":")[1].Trim()+" "+ c.Trim().Split(":")[0].Trim() + ", ";
            }
            res = res.Trim().TrimEnd(',');

            return res;
        }

        public bool is_function_call() // call()
        {

            if(!val.Contains("(")) // call)
                return false;
            if(!val.Contains(")")) // call(
                return false;
            if (val.Split("(")[0].Contains(" ")) //c all()
                return false;
            if (val.Split("(")[0].Trim().Length == 0) // ()
                return false;

            return true;
        }

        public string get_function_c_name()
        {
            return val.Trim().Split('(')[0];
        }
        public string get_function_c_args()
        {
            return val.Trim().remove_start(get_function_c_name()).brackets("(", ")");
        }

        public string space_nonstr()
        {
            string spaced = "";


            bool in_str = false;
            char pc = ' ';
            foreach(char c in val)
            {
                if (c == '"' && pc != '\\')
                    in_str = !in_str;

                if (c == '+' && !in_str)
                {
                    spaced += " + ";
                }

                if (c == '-' && !in_str)
                {
                    spaced += " - ";
                }

                if (c == '*' && !in_str)
                {
                    spaced += " * ";
                }

                if (c == '/' && !in_str)
                {
                    spaced += " / ";
                }

                if (c == '%' && !in_str)
                {
                    spaced += " % ";
                }

                if (c == '!' && !in_str)
                {
                    spaced += " ! ";
                }
                else
                {
                    spaced += c;
                }
                    

                pc = c;
            }

            return spaced;
        }

        public bool is_var_def() // var a: int = 5
        {
            if(!val.StartsWith("var"))
                return false;
            if (!(val.Split(" ").Length >= 5))
                return false;
            if(!val.Split(" ")[1].EndsWith(":"))
                return false;

            return true;
        }

        public bool is_var_set()
        {
            return val.Split("=").Length == 2;
        }

        public string aptt_to_cpp_var()
        {
            return val.Split(" ")[2] + " " + val.Split(" ")[1].TrimEnd(':') + " = "+ val.Split("=")[1].line().remapp_apatite_functions() ;
        }

        public string remapp_apatite_functions()
        {
            string result = "";
            string nval = space_nonstr().Replace("(","( ").Replace(")"," )");


            foreach (string i in nval.Split(' '))
            {

                if((i.line().is_function_call() || (i+")").line().is_function_call()))
                {
                    result += "__FUNCTION__APATITE__" + i + " ";
                }
                else
                {
                    result += i + " ";
                }
            }

            return result;
                    
        }

        public bool should_keep()
        {

            if(val.Trim() == "" || val.Trim() == string.Empty || val == "{" || val == "}")
            {
                return true;
            }


            return false;
        }
    }

    public static class StringExtension
    {
        public static string[] splitstr(this string str, string by)
        {
             return str.Split(new string[] { by }, StringSplitOptions.None);
        }
        public static string[] splitstr_trim(this string str, string by)
        {
            return str.Split(new string[] { by }, StringSplitOptions.TrimEntries);
        }
        public static line line(this string str)
        {
            return new line(str);
        }
        public static string remove_start(this string str, string expected)
        {
            if (str.StartsWith(expected))
            {
                return str.Remove(0, expected.Length);
            }
            return str;
        }
        public static string remove_end(this string str, string expected)
        {
            if (str.EndsWith(expected))
            {
                return str.Remove(str.Length - expected.Length, expected.Length);
            }
            return str;
        }
        public static string brackets(this string str, string expected_start, string expected_end)
        {
            return str.remove_start(expected_start).remove_end(expected_end);
        }
        public static string brackets(this string str, string expected)
        {
            return str.remove_start(expected).remove_end(expected);
        }

    }
}
