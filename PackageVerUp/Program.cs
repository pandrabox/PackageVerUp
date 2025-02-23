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

            // バージョン番号の更新
            content = UpdateVersion(content);

            // Vase のバージョン更新
            content = UpdateVaseVersion(content);

            File.WriteAllText(filePath, content);
            Console.WriteLine("パッケージ情報を更新しました。");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"エラーが発生しました: {ex.Message}");
        }
    }

    static string UpdateVersion(string content)
    {
        string pattern = "\"version\": \"(\\d+)\\.(\\d+)\\.(\\d+)\"";

        var match = Regex.Match(content, pattern);
        if (!match.Success)
        {
            Console.WriteLine("バージョン情報が見つかりませんでした。");
            return content;
        }

        int major = int.Parse(match.Groups[1].Value);
        int minor = int.Parse(match.Groups[2].Value);
        int patch = int.Parse(match.Groups[3].Value) + 1;

        string newVersion = $"\"version\": \"{major}.{minor}.{patch}\"";
        return Regex.Replace(content, pattern, newVersion);
    }

    static string UpdateVaseVersion(string content)
    {
        string vasePattern = "\"com\\.github\\.pandrabox\\.pandravase\":\\s*\">?=?\\d+\\.\\d+\\.\\d+\"";

        if (!Regex.IsMatch(content, vasePattern))
        {
            Console.WriteLine("pandravase への依存はありません。");
            return content; // 依存関係なし
        }

        string vasePackagePath = @"C:\UnityP\vpm\Project\FlatsPlus\Packages\com.github.pandrabox.pandravase\package.json";
        if (!File.Exists(vasePackagePath))
        {
            Console.WriteLine("pandravase の package.json が見つかりません。");
            return content;
        }

        try
        {
            string vaseContent = File.ReadAllText(vasePackagePath);
            string versionPattern = "\"version\":\\s*\"(\\d+\\.\\d+\\.\\d+)\"";
            var match = Regex.Match(vaseContent, versionPattern);

            if (!match.Success)
            {
                Console.WriteLine("pandravase のバージョン情報が見つかりませんでした。");
                return content;
            }

            string vaseVersion = match.Groups[1].Value;
            string newVaseDependency = $"\"com.github.pandrabox.pandravase\": \">={vaseVersion}\"";

            return Regex.Replace(content, vasePattern, newVaseDependency);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"pandravase のバージョン取得中にエラーが発生しました: {ex.Message}");
            return content;
        }
    }
}
