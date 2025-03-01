using System;
using System.IO;
using System.Timers;
using System.Text;

class Program
{
    static System.Timers.Timer timer;
    static string logFilePath = "disk_log.txt";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        File.WriteAllText(logFilePath, string.Empty);

        timer = new System.Timers.Timer(5000);
        timer.Elapsed += CheckDrives;
        timer.AutoReset = true;
        timer.Enabled = true;
        timer.Start();

        Console.WriteLine("Мониторинг дисков запущен. Нажмите Enter для выхода...");
        Console.ReadLine();
    }

    static void CheckDrives(object? sender, ElapsedEventArgs e)
    {
        DriveInfo[] drives = DriveInfo.GetDrives();

        using (StreamWriter log = new StreamWriter(logFilePath, true))
        {
            log.WriteLine($"[{DateTime.Now}] Проверка дисков:");

            foreach (DriveInfo drive in drives)
            {
                try
                {
                    string type = drive.DriveType.ToString();
                    string name = drive.Name;
                    long totalSize = drive.IsReady ? drive.TotalSize : 0;
                    long freeSpace = drive.IsReady ? drive.AvailableFreeSpace : 0;
                    long usedSpace = totalSize - freeSpace;

                    string info = $"Диск {name} ({type}) | Всего: {FormatSize(totalSize)}, Свободно: {FormatSize(freeSpace)}, Использовано: {FormatSize(usedSpace)}";

                    Console.WriteLine(info);
                    log.WriteLine(info);
                }
                catch (Exception ex)
                {
                    log.WriteLine($"Ошибка при чтении {drive.Name}: {ex.Message}");
                }
            }

            log.WriteLine("--------------------------------------------------");
        }
    }

    static string FormatSize(long size)
    {
        if (size == 0) return "Недоступно";
        string[] sizes = { "Б", "КБ", "МБ", "ГБ", "ТБ" };
        int order = 0;
        double formattedSize = size;

        while (formattedSize >= 1024 && order < sizes.Length - 1)
        {
            order++;
            formattedSize /= 1024;
        }

        return $"{formattedSize:0.##} {sizes[order]}";
    }
}
