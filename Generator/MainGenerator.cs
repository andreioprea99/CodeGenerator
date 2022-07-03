using CodeGenerator.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CodeGenerator.Generator
{
    public class MainGenerator
    {
        private readonly ILogger<Object> _logger;
        Dictionary<Language, IGenerator> _generators;

        public MainGenerator(ILogger<MainGenerator> logger, IEnumerable<IGenerator> generators)
        {
            _logger = logger;
            _generators = generators.ToDictionary(generator => generator.Type, generator => generator);
        }

        public async Task Generate(MainRequestDTO request, string path)
        {
            await _generators[request.Type].Generate(request, path);
        }

        public static async Task PushInGit(GitRequestModel gitDetails, string generatedCodeDirectory)
        {
            Console.WriteLine(generatedCodeDirectory);
            await InvokePSCommand("git", "init", generatedCodeDirectory);
            await InvokePSCommand("git", "config user.email code-generator@example.com", generatedCodeDirectory);
            await InvokePSCommand("git", $"config user.name {gitDetails.UserName}", generatedCodeDirectory);
            await InvokePSCommand("git", $"remote add origin https://{gitDetails.UserName}:{gitDetails.Password}@{gitDetails.URL}", generatedCodeDirectory);
            await InvokePSCommand("git", "fetch origin", generatedCodeDirectory);
            await InvokePSCommand("git", $"pull origin {gitDetails.Branch}", generatedCodeDirectory);
            await InvokePSCommand("git", "checkout -b generated-code", generatedCodeDirectory);
            await InvokePSCommand("git", "add .", generatedCodeDirectory);
            await InvokePSCommand("git", "commit -m \"Generated Code\"", generatedCodeDirectory);
            await InvokePSCommand("git", "push --set-upstream origin generated-code", generatedCodeDirectory);
        }

        public static async Task InvokePSCommand(string command, string arguments, string workingDirectory = ".")
        {
            Process process = new Process();
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            await process.WaitForExitAsync();
            if (process.ExitCode != 0)
                throw new Exception(process.StandardError.ReadToEnd());
        }
    }
}
