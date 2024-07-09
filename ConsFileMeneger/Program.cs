using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace ConsFileMeneger
{
    internal class Program
    {
        public static int WINDOW_WIDTH = 120;
        public static int WINDOW_HEIGTH = 40;
        public static string ROOT_DIRECTORY = "C:\\Users\\Александра\\Desktop\\";
        public static string currentDirectory = ROOT_DIRECTORY;
        public static string tree = "";
        public static string copyFileOrDirectory = "";

        public static string saveOfCommands = "C:\\Users\\Александра\\Documents\\Visual Studio 2022\\Проекты\\ConsFileMeneger\\ConsFileMeneger\\saveOfCommands.txt";

        public static string exeptions = "C:\\Users\\Александра\\Documents\\Visual Studio 2022\\Проекты\\ConsFileMeneger\\ConsFileMeneger\\exeptions.txt";
        static void Main(string[] args)
        {
            Console.SetWindowSize(WINDOW_WIDTH, WINDOW_HEIGTH);
            Console.SetBufferSize(WINDOW_WIDTH, WINDOW_HEIGTH);

            DrawConsole(0, 0, WINDOW_WIDTH, 18);
            DrawConsole(0, 20, WINDOW_WIDTH, 8);

            File.WriteAllText(saveOfCommands, "");

            string information = "Для навигации наберите menu";
            Console.SetCursorPosition(WINDOW_WIDTH / 2 - information.Length / 2, 25);
            Console.WriteLine(information);

            UpdateConsole();
        }

        public static void DescriptionOfCommandsForUser()
        {
            string menuOfUser ="Команды:\n│ cd - перейти в домашнюю директорию\n│ cd..- переход на уровень выше\n│ ls - вывод списка папок и файлов по заданной странице\n│ ls-l - детальная информация о файлах и папках\n│ cp - копирование файла или папки\n│ mkdir - создание новой папки\n│ touch - создание нового пустого файла\n│ mv - перемещение или переименование файлов или каталогов\n│ rm - удалить файл\n│ rmdir - удалить пустую папку\n│ rm-r - удалить директорию и ее содержимое\n│ cat - вывести содержимое файла\n│ red - открыть файл в текстовом редакторе\n│ echo - передать текст в указанный файл\n│ head - вывести первые 10 строк файла\n│ tree - вывод дерева";

            Console.WriteLine(menuOfUser);

        }

        public static void UpdateConsole()
        {
            DrawInputCommandField(currentDirectory, 1, 31, WINDOW_WIDTH, 1);
            CommandInputProcess();
        }

        public static void CommandInputProcess()
        {
            StringBuilder command = new StringBuilder();

            string saveOfCommandsFile = File.ReadAllText(saveOfCommands);

            char key;
            int left = 0; int top = 0;
            GetCurrentCursorPosition(ref left, ref top);

            do
            {
                int currentLeft = 0; int currentTop = 0;
                GetCurrentCursorPosition(ref currentLeft, ref currentTop);

                key = Console.ReadKey().KeyChar;
                if ((ConsoleKey)key == ConsoleKey.Backspace && Console.CursorLeft < left)
                {
                    Console.Write(">");
                }
                else if ((ConsoleKey)key == ConsoleKey.Backspace)
                {
                    command.Remove(command.Length - 1, 1);
                    Console.Write(" ");
                    Console.SetCursorPosition(currentLeft - 1, currentTop);
                }
                else if (currentLeft == WINDOW_WIDTH - 2)
                {
                    Console.SetCursorPosition(currentLeft, currentTop);
                    Console.Write(" ");
                    Console.SetCursorPosition(currentLeft, currentTop);
                }
                else
                {
                    command.Append(key);
                }
            }
            while ((ConsoleKey)key != ConsoleKey.Enter);
            command.Remove(command.Length - 1, 1);
            File.WriteAllText(saveOfCommands, saveOfCommandsFile + "\n" + command.ToString());
            CommandParser(command);
        }

        private static void CommandParser(StringBuilder command)
        {
            string[] splittedCommand = command.ToString().Split(" ");

            try
            {
                switch (splittedCommand[0])
                {
                    case "menu": /*меню для пользователя*/
                        DrawConsole(0, 0, WINDOW_WIDTH, 18);
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(WINDOW_WIDTH / 2, 2);
                        DescriptionOfCommandsForUser();
                        break;
                    case "cd": /*перейти в папку*/
                        ChangeDirectory(command.ToString());
                        break;
                    case "ls":/*список файлов в папке и детальная информация о файле*/
                        ViewingContent(command.ToString());
                        break;
                    case "cp":  /*копирование файла или директории*/
                        CopyFileOrDirectory(command.ToString());
                        break;
                    case "mkdir":/*создание новой папки*/
                        CreateDirectory(command.ToString());
                        break;
                    case "touch":/*создание нового пустого файла*/
                        CreateFile(command.ToString());
                        break;
                    case "mv":/*перемещение или переименование файлов или каталогов*/
                        RemoveOrMoveFileOrDirectory(command.ToString());
                        break;
                    case "rm":/*удалить файл или папку*/
                        DeleteFileOrDirectory(command.ToString());
                        break;
                    case "rmdir":/*удалить пустую папку*/
                        DeleteDirectory(command.ToString());
                        break;
                    case "cat":/*вывести содержимое файла*/
                        СonclusionСontentOfFile(command.ToString());
                        break;
                    case "red":/*открыть файл в текстовом редакторе*/
                        OpenFileOfTextEditor(command.ToString());
                        break;
                    case "echo":/*передать текст в указанный файл*/
                        SendTextToFile(command.ToString());
                        break;
                    case "head":/*вывести первые 10 строк файла*/
                        OutputFirstTenLinesOfFile(command.ToString());
                        break;
                    case "tree":/*вывод дерева*/
                        DrawTree(int.Parse(splittedCommand[1]), splittedCommand[2]);
                        break;
                       
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(exeptions, ex.ToString());
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.WriteLine("Ошибка, вы сделали что-то неверно: " + ex.Message);
            }

            UpdateConsole();
        }
        /*перейти в папку*/
        public static void ChangeDirectory(string command) 
        {
            string[] splittedCommand = command.ToString().Split(" ");
            if (splittedCommand.Length < 2)
            {
                currentDirectory = ROOT_DIRECTORY;
            }
            else
            {
                if (splittedCommand[1] == "..")
                {
                    string[] splittedDirectory = currentDirectory.Split("\\");
                    currentDirectory = "";
                    for (int i = 0; i < splittedDirectory.Length - 1; i++)
                    {
                        if (i == splittedDirectory.Length - 2)
                        {
                            currentDirectory += splittedDirectory[i];
                            break;
                        }
                        currentDirectory += splittedDirectory[i] + "\\";
                    }
                    InformationOfSuccessfulExecution();
                }
                else if (Directory.Exists(currentDirectory + $"\\{splittedCommand[1]}"))
                {
                    currentDirectory += $"\\{splittedCommand[1]}";
                    InformationOfSuccessfulExecution();
                }
                else if (splittedCommand[1].Contains("\\") && Directory.Exists(splittedCommand[1]))
                {
                    currentDirectory = splittedCommand[1];
                    InformationOfSuccessfulExecution();
                }
                else
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.Write("Такой папки не существует");
                }
            }
        }
        
        //вывод информации об успешном выполнении операции
        public static void InformationOfSuccessfulExecution()
        {
            DrawConsole(0, 20, WINDOW_WIDTH, 8);
            Console.SetCursorPosition(1, 21);
            Console.Write("Операция успешно выполнена");
        }

        //список файлов и папок текущей директории и детальная информация о файлах и директориях
        public static void ViewingContent(string command)
        {
            string[] splittedCommand = command.ToString().Split(" ");
            try
            {
                if (splittedCommand[1] == "-")
                {
                    Directory.GetFileSystemEntries(currentDirectory);
                    DrawTree(int.Parse(splittedCommand[2]), currentDirectory);
                }
                else if (splittedCommand[1] == "-l")
                {
                    string[] secondSplittedCommand = splittedCommand[2].Split(".");

                    if (secondSplittedCommand[secondSplittedCommand.Length - 1] == "txt")
                    {
                        FileInfo fileInfo = new FileInfo(currentDirectory + "\\" + splittedCommand[2]);

                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);

                        Console.Write($"Имя:" + fileInfo.Name + "\n│Расширение:" + fileInfo.Extension + "\n│Размер:" + fileInfo.Length);
                    }
                    else
                    {
                        DirectoryInfo DirectoryInfo = new DirectoryInfo(currentDirectory + "\\" + splittedCommand[2]);

                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);

                        Console.Write($"Имя:" + DirectoryInfo.Name + "\n│Дата и время последней операции записи:" + Directory.GetLastWriteTimeUtc(currentDirectory + "\\" + splittedCommand[2]));
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(exeptions, ex.ToString());
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.WriteLine("Ошибка, вы сделали что-то неверно: " + ex.Message);
            }

        }

        //копирование файла или папки
        public static void CopyFileOrDirectory(string command)
        {
            try
            {
                string[] splittedCommand = command.ToString().Split(" ");
                string[] secondSplittedCommand = splittedCommand[1].Split(".");

                if (secondSplittedCommand[secondSplittedCommand.Length - 1] == "txt")
                {
                    File.Copy(currentDirectory + splittedCommand[1], copyFileOrDirectory);
                }
                else
                {
                    Directory.Move(currentDirectory + splittedCommand[1], copyFileOrDirectory);
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(exeptions, ex.ToString());
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.WriteLine("Ошибка, вы сделали что-то неверно: " + ex.Message);
            }

        }
        //копирование папки
        private static void CopyDirectory(string FromFolder, string ToFolder)
        {
            try
            {
                Directory.CreateDirectory(ToFolder);

                string[] files = Directory.GetFiles(FromFolder);
                for (int i = 0; i < files.Length; i++)
                {
                    File.Copy(files[i], ToFolder + "\\" + Path.GetFileName(files[i]));
                }
                string[] directories = Directory.GetDirectories(FromFolder);
                for (int i = 0; i < directories.Length; i++)
                {
                    CopyDirectory(directories[i], ToFolder + "\\" + Path.GetFileName(directories[i]));
                }
                InformationOfSuccessfulExecution();
            }
            catch (Exception ex)
            {
                File.AppendAllText(exeptions, ex.ToString());
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.WriteLine("Ошибка, вы сделали что-то неверно: " + ex.Message);
            }
        }

        //создание новой папки
        public static void CreateDirectory(string command)
        {
            string[] splittedComman = command.Split(" ");
            try
            {
               Directory.CreateDirectory(currentDirectory + "\\" + splittedComman[1]);
                InformationOfSuccessfulExecution();
            }
            catch (Exception ex)
            {
                File.AppendAllText(exeptions, ex.ToString());
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.WriteLine("Ошибка, вы сделали что-то неверно: " + ex.Message);
            }

        }

        /*создание нового пустого файла*/
        public static void CreateFile(string command)
        {
            try
            {
                string[] splittedComman = command.Split(" ");
                File.Create(currentDirectory + "\\" + splittedComman[1]);
                InformationOfSuccessfulExecution();
            }
            catch (Exception ex)
            {
                File.AppendAllText(exeptions, ex.ToString());
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.WriteLine("Ошибка, вы сделали что-то неверно: " + ex.Message);
            }

        }

        /*перемещение или переименование файлов или каталогов*/
        public static void RemoveOrMoveFileOrDirectory(string command)
        {
            string[] splittedCommand = command.ToString().Split(" ");

            string[] secondSplittedCommand = splittedCommand[1].Split(".");
            try
            {
                if (secondSplittedCommand[secondSplittedCommand.Length - 1] == "txt")
                {
                    if (Directory.Exists(currentDirectory + splittedCommand[2]))
                    {
                        File.Copy(currentDirectory + splittedCommand[1], currentDirectory + splittedCommand[2] + "\\" + splittedCommand[1]);
                        InformationOfSuccessfulExecution();
                    }
                        
                    else
                    {
                        File.Move(currentDirectory + splittedCommand[1], currentDirectory + splittedCommand[2]);
                        InformationOfSuccessfulExecution();
                    }
                }
                else
                {
                    if (Directory.Exists(currentDirectory + splittedCommand[2]))
                    {
                        Directory.Move(currentDirectory + splittedCommand[1], currentDirectory + splittedCommand[2] + "\\" + splittedCommand[1]);
                        InformationOfSuccessfulExecution();
                    }

                    else
                    {
                        Directory.Move(currentDirectory + splittedCommand[1], currentDirectory + splittedCommand[2]);
                        InformationOfSuccessfulExecution();
                    }
                       
                }

            }
            catch (Exception ex)
            {
                File.AppendAllText(exeptions, ex.ToString());
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.WriteLine("Ошибка, вы сделали что-то неверно: " + ex.Message);
            }

        }

        //удаление файла или папки
        public static void DeleteFileOrDirectory(string command)
        {

            try
            {
                string[] splittedCommand = command.ToString().Split(" ");

                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.WriteLine("Вы уверены? Введите да или нет");
                Console.SetCursorPosition(1, 22);
                string userOutput = Console.ReadLine();

                if (splittedCommand.Length >= 3 && splittedCommand[1] == "-r") // Проверка на наличие необходимого количества элементов и флага "-r"
                {
                    if (userOutput == "да")
                    {
                        if (Directory.Exists(currentDirectory + splittedCommand[2]))
                        {
                            bool recursion = true;
                            Directory.Delete(currentDirectory + splittedCommand[2], recursion);
                            InformationOfSuccessfulExecution();
                        }
                        else
                        {
                            DrawConsole(0, 20, WINDOW_WIDTH, 8);
                            Console.SetCursorPosition(1, 21);
                            Console.WriteLine("Ошибка: папка не найдена.");
                        }
                    }
                    else
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.WriteLine("Ошибка: неверный формат команды.");
                    }
                }
                else if (splittedCommand.Length >= 2) // Проверка на наличие необходимого количества элементов для удаления файла
                {
                    if (userOutput == "да")
                    {
                        if (File.Exists(currentDirectory + splittedCommand[1] + ".txt"))
                        {
                            File.Delete(currentDirectory + splittedCommand[1] + ".txt");
                            InformationOfSuccessfulExecution();
                        }
                        else
                        {
                            DrawConsole(0, 20, WINDOW_WIDTH, 8);
                            Console.SetCursorPosition(1, 21);
                            Console.WriteLine("Ошибка: файл не найден.");
                        }
                    }
                    else
                    {
                        DrawConsole(0, 20, WINDOW_WIDTH, 8);
                        Console.SetCursorPosition(1, 21);
                        Console.WriteLine("Ошибка: неверный формат команды.");
                    }
                }
                else
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.WriteLine("Ошибка: неверный формат команды.");
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(exeptions, ex.ToString());
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.WriteLine("Ошибка, вы сделали что-то неверно или нажали нет: " + ex.Message);
            }
        }

        //удалить папку
        public static void DeleteDirectory(string command)
        {
            try
            {
                string[] splittedCommand = command.ToString().Split(" ");
                Console.SetCursorPosition(1, 21);

                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.WriteLine("Вы уверенны? Введите да или нет");
                Console.SetCursorPosition(1, 22);
                string userOutput = Console.ReadLine();

                if (userOutput == "да")
                {
                    Directory.Delete(currentDirectory + splittedCommand[1]);
                    InformationOfSuccessfulExecution();
                }
                
            }
            catch (Exception ex)
            {
                File.AppendAllText(exeptions, ex.ToString());
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.WriteLine("Ошибка, вы сделали что-то неверно или нажали нет: " + ex.Message);
            }

        }

        /*вывести содержимое файла*/
        public static void СonclusionСontentOfFile(string command)
        {
            try
            {
                string[] splittedCommand = command.ToString().Split(" ");

                if (splittedCommand.Length >= 2) // Проверка на наличие необходимого количества элементов
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.WriteLine(File.ReadAllText(currentDirectory + splittedCommand[1] + ".txt"));
                }
                else
                {
                    DrawConsole(0, 20, WINDOW_WIDTH, 8);
                    Console.SetCursorPosition(1, 21);
                    Console.WriteLine("Ошибка: неверный формат команды.");
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(exeptions, ex.ToString());
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.WriteLine("Ошибка, вы сделали что-то неверно при попытке вывода: " + ex.Message);
            }
        }

        /*открыть файл в текстовом редакторе*/
        public static void OpenFileOfTextEditor(string command)
        {
            string[] splittedCommand = command.ToString().Split(" ");
            string pathOfFile = currentDirectory + splittedCommand[1];

            try
            {
                // путь к текстовому редактору
                string pathOfTextEditor = "notepad.exe";

                // открытие файла в текстовом редакторе
                Process.Start(pathOfTextEditor, pathOfFile);
                InformationOfSuccessfulExecution();
            }
            catch (Exception ex)
            {
                File.AppendAllText(exeptions, ex.ToString());
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.WriteLine("Ошибка, вы сделали что-то неверно: " + ex.Message);
            }
        }

        /*передать текст в указанный файл*/
        public static void SendTextToFile(string command)
        {
            try
            {
                string[] splittedCommand = command.ToString().Split(" ");
                {
                    for (int i = 2; i < splittedCommand.Length; i++)
                    {
                        string stringTxt = splittedCommand[i];
                        File.AppendAllText(currentDirectory + splittedCommand[1], stringTxt);
                        InformationOfSuccessfulExecution();
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(exeptions, ex.ToString());
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.WriteLine("Ошибка, вы сделали что-то неверно: " + ex.Message);
            }

        }

        /*вывести первые 10 строк файла*/
        public static void OutputFirstTenLinesOfFile(string command)
        {
            try
            {
                string[] splittedCommand = command.ToString().Split(" ");

                if (splittedCommand[1] == "-f")
                {
                    Console.SetCursorPosition(0, 1);

                    string[] output = File.ReadAllText(currentDirectory + splittedCommand[2] + ".txt").Split("\n");
                    for (int i = 0; i < 10; i++)
                    {
                        Console.WriteLine("│" + output[i]);
                    }
                }
            }
            
            catch (Exception ex)
            {
                File.AppendAllText(exeptions, ex.ToString());
                DrawConsole(0, 20, WINDOW_WIDTH, 8);
                Console.SetCursorPosition(1, 21);
                Console.WriteLine("Ошибка, вы сделали что-то неверно: " + ex.Message);
            }
        }
        public static void GetCurrentCursorPosition(ref int left, ref int top)
        {
            left = Console.CursorLeft; top = Console.CursorTop;
        }
        public static void DrawInputCommandField(string directory, int x, int y, int wigth, int heigth)
        {
            DrawConsole(0, 30, wigth, heigth);
            Console.SetCursorPosition(x, y);
            Console.Write($"{directory}>");
        }
        public static void DrawConsole(int x, int y, int width, int heigth)
        {
            Console.SetCursorPosition(x, y);
            //Вывод шапки
            Console.Write("┌");
            for (int i = 0; i < width - 2; i++)
            {
                Console.Write("─");
            }
            Console.Write("┐");

            //Вывод тела
            for (int i = 0; i < heigth; i++)
            {
                Console.Write("│");
                for (int j = 0; j < width - 2; j++)
                {
                    Console.Write(" ");
                }
                Console.Write("│");
            }

            //Вывод подвала
            Console.Write("└");
            for (int i = 0; i < width - 2; i++)
            {
                Console.Write("─");
            }
            Console.Write("┘");
        }
        public static void GetTree(DirectoryInfo directory, string indent, bool lastDirectory)
        {
            tree += indent;
            if (lastDirectory)
            {
                tree += "└──";
                indent += "   ";
            }
            else
            {
                tree += "├──";
                indent += "│  ";
            }
            tree += directory.Name + "\n";

            try
            {
                DirectoryInfo[] subDirectories = directory.GetDirectories();

                for (int i = 0; i < subDirectories.Length; i++)
                {
                    GetTree(subDirectories[i], indent, i == subDirectories.Length - 1);
                }
            }
            catch (Exception)
            {

            }
        }
        public static void DrawTree(int page, string defaultPath)
        {
            DrawConsole(0, 0, WINDOW_WIDTH, 18);
            GetTree(new DirectoryInfo(defaultPath), "", true);

            string[] line = tree.Split('\n');
            int linesAtPage = 18;
            int pagesQuantity = (int)Math.Ceiling(line.Length / (double)linesAtPage);

            string[,] pages = new string[pagesQuantity, linesAtPage];

            if (line.Length >= linesAtPage)
            {
                for (int i = 0; i < pages.GetLength(0); i++)
                {
                    int counter = 0;
                    for (int j = linesAtPage * i; j < linesAtPage * (i + 1); j++)
                    {
                        if (line[j] == "")
                        {
                            break;
                        }
                        pages[i, counter] = line[j];
                        counter++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < pages.GetLength(0); i++)
                {
                    int counter = 0;
                    for (int j = linesAtPage * i; j < linesAtPage * (i + 1); j++)
                    {
                        pages[i, counter] = line[j];
                        counter++;
                    }
                }
            }

            for (int i = 0; i < pages.GetLength(1); i++)
            {
                Console.SetCursorPosition(1, i + 1);
                Console.WriteLine(pages[page - 1, i]);
            }
            string currentPage = $"Страница: [{page} / {pages.GetLength(0)}]";
            Console.SetCursorPosition(WINDOW_WIDTH / 2 - currentPage.Length / 2, 19);
            Console.WriteLine(currentPage);
            tree = "";
        }
    }
}