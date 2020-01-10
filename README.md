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
; TryNewDisassembler.WithDifferentCalls.Benchmark(Int32)
       push      rsi
       sub       rsp,20
       mov       rsi,rcx
       cmp       edx,7FFFFFFF
       jne       M00_L00 
       call      TryNewDisassembler.WithDifferentCalls.Static() 
       mov       rcx,rsi
       call      TryNewDisassembler.WithDifferentCalls.Instance() 
       mov       rcx,rsi
       call      TryNewDisassembler.WithDifferentCalls.Recursive() 
       mov       rcx,rsi
       mov       rax,[rsi]
       mov       rax,[rax+40]
       mov       rax,[rax+20]
       add       rsp,20
       pop       rsi
       jmp       rax
M00_L00:
       mov       rcx MT_System.InvalidOperationException 
       call      CORINFO_HELP_NEWSFAST 
       mov       rsi,rax
       mov       ecx,39
       mov       rdx,7FF8491990E0
       call      CORINFO_HELP_STRCNS 
       mov       rdx,rax
       mov       rcx,rsi
       call      System.InvalidOperationException..ctor(System.String) 
       mov       rcx,rsi
       call      CORINFO_HELP_THROW 
       int       3
; Total bytes of code 117
```
```assembly
; TryNewDisassembler.WithDifferentCalls.Static()
       nop       dword ptr [rax+rax]
       ret
; Total bytes of code 6
```
```assembly
; TryNewDisassembler.WithDifferentCalls.Instance()
       nop       dword ptr [rax+rax]
       ret
; Total bytes of code 6
```
```assembly
; TryNewDisassembler.WithDifferentCalls.Recursive()
       push      rdi
       push      rsi
       sub       rsp,28
       mov       rsi,rcx
       mov       rcx MT_System.Random 
       call      CORINFO_HELP_NEWSFAST 
       mov       rdi,rax
       mov       rcx,rdi
       mov       edx,7B
       call      System.Random..ctor(Int32) 
       mov       rcx,rdi
       xor       edx,edx
       mov       r8d,0A
       call      System.Random.Next(Int32, Int32) 
       cmp       eax,0B
       jne       M03_L00 
       mov       rcx,rsi
       call      TryNewDisassembler.WithDifferentCalls.Recursive() 
M03_L00:
       nop
       add       rsp,28
       pop       rsi
       pop       rdi
       ret
; Total bytes of code 77
```
