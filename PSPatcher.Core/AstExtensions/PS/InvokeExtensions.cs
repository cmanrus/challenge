using System;
using System.Diagnostics;
using System.Text;

namespace PSPatcher.Core.AstExtensions.PS
{
    public static class InvokeExtensions
    {
        public static string Invoke(this PSScriptAst ast)
        {
            string command = ast.GetText();
            var bytes = Encoding.Unicode.GetBytes(command);
            var encodedCommand = Convert.ToBase64String(bytes);

            string args = String.Format("-EncodedCommand {0}", encodedCommand, ast.GetText().Replace("\"", "\\\""));
            ProcessStartInfo processStartInfo = new ProcessStartInfo("powershell", args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            
            var process = Process.Start(processStartInfo);

            var result = process.StandardOutput.ReadToEnd();
            
            process.WaitForExit();
            
            return result;
        }
    }
}