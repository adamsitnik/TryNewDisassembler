# TryNewDisassembler

This small repo is a sample app configured to use the latest BenchmarkDotNet disassembler.

How to run it:

```cmd
dotnet run -c Release --filter '*'
```

After running the command the exporter exports GitHub Markdown files to `.\BenchmarkDotNet.Artifacts\results\*-asm.md`

Sample results:

## .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), X64 RyuJIT
```assembly
; BenchmarkDotNet.Samples.WithCalls.Benchmark(Int32)
       push      rbp
       push      rbx
       push      rax
       lea       rbp,[rsp+10]
       mov       rbx,rdi
       cmp       esi,7FFFFFFF
       jne       short M00_L00
       call      BenchmarkDotNet.Samples.WithCalls.Static()
       mov       rdi,rbx
       call      BenchmarkDotNet.Samples.WithCalls.Instance()
       mov       rdi,rbx
       call      BenchmarkDotNet.Samples.WithCalls.Recursive()
       mov       rdi,rbx
       mov       esi,1
       mov       rax,offset BenchmarkDotNet.Samples.WithCalls.Benchmark(Boolean)
       lea       rsp,[rbp-8]
       pop       rbx
       pop       rbp
       jmp       rax
M00_L00:
       mov       rdi,offset MT_System.InvalidOperationException
       call      CORINFO_HELP_NEWSFAST
       mov       rbx,rax
       mov       edi,36F
       mov       rsi,7F148E5E4378
       call      CORINFO_HELP_STRCNS
       mov       rsi,rax
       mov       rdi,rbx
       call      System.InvalidOperationException..ctor(System.String)
       mov       rdi,rbx
       call      CORINFO_HELP_THROW
       int       3
; Total bytes of code 125
```
```assembly
; BenchmarkDotNet.Samples.WithCalls.Static()
       ret
; Total bytes of code 1
```
```assembly
; BenchmarkDotNet.Samples.WithCalls.Instance()
       ret
; Total bytes of code 1
```
```assembly
; BenchmarkDotNet.Samples.WithCalls.Recursive()
       push      rbp
       push      r14
       push      rbx
       lea       rbp,[rsp+10]
       mov       rbx,rdi
       mov       rdi,offset MT_System.Random
       call      CORINFO_HELP_NEWSFAST
       mov       r14,rax
       mov       rdi,r14
       mov       esi,7B
       call      System.Random..ctor(Int32)
       mov       rdi,r14
       xor       esi,esi
       mov       edx,0A
       mov       rax,offset System.Random.Next(Int32, Int32)
       call      qword ptr [rax]
       cmp       eax,0B
       jne       short M03_L00
       mov       rdi,rbx
       call      BenchmarkDotNet.Samples.WithCalls.Recursive()
M03_L00:
       nop
       pop       rbx
       pop       r14
       pop       rbp
       ret
; Total bytes of code 84
```
```assembly
; BenchmarkDotNet.Samples.WithCalls.Benchmark(Boolean)
       ret
; Total bytes of code 1
```
