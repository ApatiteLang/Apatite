using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Utils;

namespace ApatiteCompiler
{
    internal class Program
    {

        const string CompilerVersion = "0.1.0";

        const string DefaultProgram = "import std \n\nfunction main() { \n    print(\"Hello, world\") \n}";

        static List<string> added_libraries = new List<string>();

        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine(@"Apatite Compiler "+ CompilerVersion);
                Console.WriteLine("Available options:");
                Console.WriteLine("     build <full : optional> -> Compile your program into cpp code and into executable if 'full' option was used");
                Console.WriteLine("     new [name : optional] -> Create new project");


                return;
            }

            if(args[0] == "new")
            {
                string app_name = "app";
                if(args.Length > 1)
                    app_name = args[1];


                Directory.CreateDirectory(app_name);
                Directory.CreateDirectory(app_name+"\\src");
                Directory.CreateDirectory(app_name + "\\src\\cpp");
                Directory.CreateDirectory(app_name+"\\bin");



                File.Create(app_name + "\\" + "src\\main.aptt").Close();
                File.WriteAllText(app_name + "\\" + "src\\main.aptt", DefaultProgram);


            }

            else if(args.Length > 1)
            {
                if(args[0] == "build")
                {
                    string[] result = compile(Directory.GetCurrentDirectory()+"\\src\\main.aptt",true,"aptt");

                    if(result[0] == "0")
                    {
                        if (!File.Exists(Directory.GetCurrentDirectory() + "\\src\\cpp\\source.cpp"))
                            File.Create(Directory.GetCurrentDirectory() + "\\src\\cpp\\source.cpp").Close();

      
                        File.WriteAllText(Directory.GetCurrentDirectory() + "\\src\\cpp\\source.cpp", result[1]);

                        Console.WriteLine("Successfuly compiled main");

                        if (args.Length > 1)
                        {
                            if(args [1] == "full")
                            {
                                Console.WriteLine("\nCompiling C++ Source");
                                Process.Start("cmd", "/C"+ AppDomain.CurrentDomain.BaseDirectory+"mingw64\\bin\\g++ " + Directory.GetCurrentDirectory() + "\\src\\cpp\\source.cpp -o " + Directory.GetCurrentDirectory() + "\\bin\\app");
                            }
                        }

                        Console.WriteLine("\nTask finished.");



                        
                    }
                    else if(result[0] == "1")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Cannot find file: "+ Directory.GetCurrentDirectory() + "\\src\\main.aptt");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        return;
                    }



                }
            }
        }

        public static string[] compile(string path, bool include_main, string language)
        {
            string c_source = "";

            if (!File.Exists(path))
                return new string[2] {"1",c_source};

            string aptt_source = File.ReadAllText(path);

            if(language == "cpp")
            {
                return new string[2] {"0", aptt_source};
            }
                

            if(include_main)
            {
                c_source += "int exit_code = 0;\n";
            }

            foreach (string s in aptt_source.Split('\n'))
            {
                string line = s.Trim();

                if (line.line().is_import()) // import std     -> "lib\std.aptt"
                {
                    if (!added_libraries.Contains(line.line().get_import_path()))
                    {
                        string[] res = compile(line.line().get_import_path(), false, line.line().get_import_ext());
                        if(res[0] == "0")
                        {
                            Console.WriteLine("Successfuly compiled library: " + line.line().get_import_name());    
                            c_source += res[1] + "\n";
                            added_libraries.Add(line.line().get_import_path());
                        }
                        else
                        {
                            Console.WriteLine(line.line().get_import_path());
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Cannot compile: " + line.line().get_import_path());
                            Console.ForegroundColor = ConsoleColor.Gray;

                        }
                    }
                    else
                    {
                        Console.WriteLine("Skipping library: " + line.line().get_import_name() + " (already included)");
                    }

                }
                else if(line.line().is_function_defenition())
                {
                    //                      void                                        __FUNCTION__APATITE__function                                                               (int arg1, int arg2)
                    c_source += line.line().get_function_defenition_return_type() +" __FUNCTION__APATITE__"+line.line().get_function_defenition_name()+"("+line.line().get_function_defenition_args().line().remmap_f_d_type_system()+")";

                    if (line.EndsWith("{"))
                        c_source += "{";

                    c_source += "\n";
                }
                else if(line.line().is_function_call())
                {
                    c_source += line.line().remapp_apatite_functions() + ";\n";
                }   
                else if(line.line().should_keep())
                {
                    c_source += line + "\n";
                }
                else if(line.line().is_var_def())
                {
                    c_source += line.line().aptt_to_cpp_var() + ";\n";
                }
                else if(line.line().is_var_set())
                {
                    c_source += line.Split('=')[0]+" = "+ line.Split('=')[1].line().remapp_apatite_functions() + ";\n";
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Cannot compile "+path);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Unexpected line: " + line);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    return new string[2] {"-1",c_source};
                }


            }

            if (include_main)
            {
                c_source += "int main(void) { __FUNCTION__APATITE__main(); return exit_code;}";
            }


            return new string[2] {"0", c_source};
        }
    }
}
