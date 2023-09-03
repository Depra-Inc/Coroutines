// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections;
using BenchmarkDotNet.Attributes;
using Depra.Coroutines.Async;
using Depra.Coroutines.Entities;

namespace Coroutines.Benchmark;

public class AsyncProcessorBenchmarks
{
	private const int COROUTINES_COUNT = 100;

	private List<ICoroutine> _coroutines = null!;
	private AsyncProcessor _asyncProcessor = null!;

	[GlobalSetup]
	public void Setup()
	{
		_coroutines = new List<ICoroutine>();
		_asyncProcessor = new AsyncProcessor();

		for (var index = 0; index < COROUTINES_COUNT; index++)
		{
			var coroutine = _asyncProcessor.Process(TestProcess());
			_coroutines.Add(coroutine);
		}
	}

	[GlobalCleanup]
	public void TearDown()
	{
		foreach (var coroutine in _coroutines)
		{
			_asyncProcessor.Stop(coroutine);
		}

		_coroutines.Clear();
	}

	[Benchmark]
	public void Tick() => _asyncProcessor.Tick();

	private IEnumerator TestProcess()
	{
		yield return null;
	}
}