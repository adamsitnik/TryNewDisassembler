module TailCall =
  open System
  open System.Linq
  open System.Diagnostics

  //    let inline push      r v      = match r v with () -> ()
  //    let inline push      r v      = r v

  // Minimalistic PushStream
  //  A PushStream accepts a receiver function that will be called
  //  with each value in the PushStream
  type 'T PushStream = ('T -> unit) -> unit

  module PushStream =
    let inline zero      ()       = LanguagePrimitives.GenericZero
    let inline push      r v      = r v

    // Creates a PushStream with all integers from b to e (inclusive)
    let inline fromRange b e    r = for i = b to e do push r i
    // Maps all values in ps using mapping function f
    let inline map       f   ps r = ps (fun v -> push r (f v))
    // Filters all values in ps using filter function f
    let inline filter    f   ps r = ps (fun v -> if f v then push r v)
    // Sums all values in ps
    let inline sum           ps   = let mutable s = zero () in ps (fun v -> s <- s + v); s

  module Tests =
    open BenchmarkDotNet.Attributes
    open BenchmarkDotNet.Configs
    open BenchmarkDotNet.Jobs
    open BenchmarkDotNet.Horology;
    open BenchmarkDotNet.Running
    open BenchmarkDotNet.Diagnostics.Windows.Configs

    [<TailCallDiagnoser(logFailuresOnly = false, filterByNamespace = false)>]
    [<DisassemblyDiagnoser>]
    type Benchmarks () =
      [<Params (10000, 1000, 100)>] 
      member val public Count = 100 with get, set

      [<Benchmark>]
      member x.SimpleImperativeTest () =
        let mutable i   = x.Count
        let mutable sum = 0L
        while i >= 0 do
          let v =  int64 i
          i     <- i - 1
          if (v &&& 1L) = 0L then
            sum <- sum + (v + 1L)
        sum

      [<Benchmark>]
      member x.SimpleLinqTest () =
        Enumerable.Range(0, x.Count)
          .Select(int64)
          .Where(fun v -> (v &&& 1L) = 0L)
          .Select((+) 1L)
          .Sum()

      [<Benchmark>]
      member x.SimplePushStreamTest () =
        PushStream.fromRange  0 x.Count
        |> PushStream.map     int64
        |> PushStream.filter  (fun v -> (v &&& 1L) = 0L)
        |> PushStream.map     ((+) 1L)
        |> PushStream.sum

      [<Benchmark>]
      member x.StructLinqStreamTest () =
        Enumerable.Range(0, x.Count)
          .Select(int64)
          .Select(fun v -> struct ((v &&& 1L) = 0L, v))
          .Select(fun struct (b, v) -> struct (b, v + 1L))
          .Select(fun struct (b, v) -> if b then v else 0L)
          .Sum()

      [<Benchmark>]
      member x.StructPushStreamTest () =
        PushStream.fromRange 0 x.Count
        |> PushStream.map     int64
        |> PushStream.map     (fun v -> struct ((v &&& 1L) = 0L, v))
        |> PushStream.map     (fun struct (b, v) -> struct (b, v + 1L))
        |> PushStream.map     (fun struct (b, v) -> if b then v else 0L)
        |> PushStream.sum


    let run argv = 
      let job = Job.Default
                    .WithWarmupCount(1) // 1 warmup is enough for our purpose
                    .WithIterationTime(TimeInterval.FromMilliseconds(250.0)) // the default is 0.5s per iteration, which is slighlty too much for us
                    .WithMinIterationCount(15)
                    .WithMaxIterationCount(20)
                    .AsDefault()
      let config = DefaultConfig.Instance.AddJob(job)
      let b = BenchmarkSwitcher [|typeof<Benchmarks>|]
      let summary = b.Run(argv, config)
      printfn "%A" summary

[<EntryPoint>]
let main argv =
  TailCall.Tests.run argv
  0