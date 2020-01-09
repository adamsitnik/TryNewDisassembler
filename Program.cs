using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System;
using System.Runtime.CompilerServices;

namespace TryNewDisassembler
{
    class Program
    {
        static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, CreateConfig());

        static IConfig CreateConfig()
            => DefaultConfig.Instance
                .AddJob(
                    Job.Dry // Dry job runs the benchmark just once
                       .WithEnvironmentVariable("COMPlus_TieredCompilation", "0") // When using Tiered JIT the ClrMD reports addresses of Tier 0 method, not Tier 1 https://github.com/dotnet/BenchmarkDotNet/issues/899
                       .AsDefault())
                .AddDiagnoser(
                    new DisassemblyDiagnoser(
                        new DisassemblyDiagnoserConfig(
                            maxDepth: 2, // you can change it to a bigger value if you want to get more framework methods disassembled
                            exportGithubMarkdown: true)));
    }

    public class WithDifferentCalls
    {
        [Benchmark]
        [Arguments(int.MaxValue)]
        public void Benchmark(int someArgument)
        {
            if (someArgument != int.MaxValue)
                throw new InvalidOperationException("Wrong value of the argument!!");

            Static();
            Instance();
            Recursive();
            Virtual();
        }

        [MethodImpl(MethodImplOptions.NoInlining)] public static void Static() { }

        [MethodImpl(MethodImplOptions.NoInlining)] public void Instance() { }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Recursive()
        {
            if (new Random(123).Next(0, 10) == 11) // never true
                Recursive();
        }

        public virtual void Virtual() { }
    }
}
