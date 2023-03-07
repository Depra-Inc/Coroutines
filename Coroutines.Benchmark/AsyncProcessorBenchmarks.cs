using System.Collections;
using BenchmarkDotNet.Attributes;
using Depra.Coroutines.Application;

namespace Coroutines.Benchmark;

public class AsyncProcessorBenchmarks
{
	private AsyncProcessor _asyncProcessor = null!;
	
	[IterationSetup]
	public void Setup()
	{
		_asyncProcessor = new AsyncProcessor();
		_asyncProcessor.Process(Coroutine());
	}

	[Benchmark]
	public void AddCoroutine()
	{
		_asyncProcessor.Process(Coroutine());
	}

	[Benchmark]
	public void OneCoroutine()
	{
		_asyncProcessor.Tick();
	}

	private IEnumerator Coroutine()
	{
		yield return null;
	}
}