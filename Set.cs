using Program;
using System;
using System.IO;

namespace Set {

class Set {
		public static void Main(string[] arg) {
			ConsoleKeyInfo key;
			if (arg.Length > 0) {
				Scanner scanner = new Scanner(arg[0]);
				Parser parser = new Parser(scanner);
				parser.tab = new SymbolTable(parser);
				parser.gen = new CodeGenerator();
				//вывод текста программы
				string[] readText = File.ReadAllLines(arg[0]);
				foreach (string s in readText) {
					Console.WriteLine(s);
				}
				Console.WriteLine("-----------");
				parser.Parse();
				//консоль ждет нажатия
				key = Console.ReadKey();
				if (parser.errors.count == 0) {
				//	parser.gen.Decode();
				//	parser.gen.Interpret("Set.IN");
				}
			}
			else { Console.WriteLine("-- No source file specified"); }
		 key = Console.ReadKey();
		}
	}
} 
