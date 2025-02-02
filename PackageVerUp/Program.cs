using System;
using System.IO;
using System.Text.RegularExpressions;

class PackageVerUp
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("使用方法: PackageVerUp <ファイルパス>");
            return;
        }

        string filePath = args[0];

        if (!File.Exists(filePath))
        {
            Console.WriteLine("指定されたファイルが見つかりません。");
            return;
        }

        try
        {
            string content = File.ReadAllText(filePath);
            string pattern = "\"version\": \"(\\d+)\\.(\\d+)\\.(\\d+)\"";

            var match = Regex.Match(content, pattern);
            if (!match.Success)
            {
                Console.WriteLine("バージョン情報が見つかりませんでした。");
                return;
            }

            int major = int.Parse(match.Groups[1].Value);
            int minor = int.Parse(match.Groups[2].Value);
            int patch = int.Parse(match.Groups[3].Value) + 1;

            string newVersion = $"\"version\": \"{major}.{minor}.{patch}\"";
            string updatedContent = Regex.Replace(content, pattern, newVersion);

            File.WriteAllText(filePath, updatedContent);
            Console.WriteLine($"バージョンを {major}.{minor}.{patch} に更新しました。");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"エラーが発生しました: {ex.Message}");
        }
    }
}
