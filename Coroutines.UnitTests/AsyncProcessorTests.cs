// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections;
using Depra.Coroutines.Async;
using Depra.Coroutines.Entities;
using Depra.Coroutines.Exceptions;

namespace Depra.Coroutines.UnitTests;

[TestFixture(TestOf = typeof(AsyncProcessor))]
internal sealed class AsyncProcessorTests
{
	private ICoroutineProcessor _asyncProcessor = null!;

	[SetUp]
	public void Setup() => _asyncProcessor = new AsyncProcessor();

	[Test]
	public void WhenCreated_ThenNotRunning()
	{
		// Act.
		var isRunning = _asyncProcessor.IsRunning;

		// Assert.
		isRunning.Should().BeFalse();
	}

	[Test]
	public void WhenProcessCoroutine_ThenIsRunning()
	{
		// Arrange.
		var process = Substitute.For<IEnumerator>();

		// Act.
		_asyncProcessor.Process(process);
		var isRunning = _asyncProcessor.IsRunning;

		// Assert.
		isRunning.Should().BeTrue();
	}

	[Test]
	public void WhenTick_AndCoroutinesRunning_ThenAdvancesAllFrames()
	{
		// Arrange.
		var process = TestCoroutine();
		var isProcessed = false;

		IEnumerator TestCoroutine()
		{
			yield return null;
			isProcessed = true;
		}

		// Act.
		_asyncProcessor.Process(process);
		_asyncProcessor.Tick();
		_asyncProcessor.Tick();

		// Assert.
		isProcessed.Should().BeTrue();
	}

	[Test]
	public void WhenTick_ThenRemovesFinishedCoroutines()
	{
		// Arrange.
		var process = TestCoroutine();

		IEnumerator TestCoroutine()
		{
			yield return null;
		}

		// Act.
		_asyncProcessor.Process(process);
		_asyncProcessor.Tick();
		var isRunningBefore = _asyncProcessor.IsRunning;
		_asyncProcessor.Tick();
		var isRunningAfter = _asyncProcessor.IsRunning;

		// Assert.
		isRunningBefore.Should().BeTrue();
		isRunningAfter.Should().BeFalse();
	}

	[Test]
	public void WhenTickMultipleRoutines_AndContainException_ThenContainsObjectStackTrace()
	{
		// Arrange
		var process = TargetCoroutine();

		IEnumerator NestedCoroutine()
		{
			yield break;
		}

		IEnumerator CoroutineWithException()
		{
			yield return null;
			throw new Exception("Test exception");
		}

		IEnumerator TargetCoroutine()
		{
			yield return NestedCoroutine();
			yield return CoroutineWithException();
		}

		_asyncProcessor.Process(process);

		// Act.
		var act = () =>
		{
			while (_asyncProcessor.IsRunning)
			{
				_asyncProcessor.Tick();
			}
		};

		// Assert.
		act.Should().Throw<CoroutineException>();
	}

	[Test]
	public void WhenTick_AndCoroutineContainException_ThenExceptionIsThrown()
	{
		// Arrange.
		var process = CoroutineWithException();

		IEnumerator CoroutineWithException()
		{
			yield return null;
			throw new Exception();
		}

		// Act.
		_asyncProcessor.Process(process);
		var act = () =>
		{
			while (_asyncProcessor.IsRunning)
			{
				_asyncProcessor.Tick();
			}
		};

		// Assert.
		act.Should().Throw<CoroutineException>();
	}

	[Test]
	public void WhenStartCoroutine_AndDurationIsOne_ThenCoroutineCompletesAfterOneSecond()
	{
		// Arrange.
		const int DURATION = 1;
		var asyncProcessor = _asyncProcessor;
		var startTime = DateTime.Now.Second;
		var process = RunForSecond(DURATION);

		// Act.
		asyncProcessor.Process(process);
		RunProcessesToEnd();

		// Assert.
		var now = DateTime.Now.Second;
		var elapsed = now - startTime;
		elapsed.Should().BeGreaterThanOrEqualTo(DURATION);
	}

	[Test]
	public void WhenStartMultipleCoroutines_AndDurationIsOne_ThenCoroutinesCompletedAfterThreeSeconds()
	{
		// Arrange.
		const int DURATION = 1;
		var asyncProcessor = _asyncProcessor;
		var startTime = DateTime.Now.Second;
		var process = RunForSecond(DURATION);

		// Act.
		// They should run in parallel.
		asyncProcessor.Process(process);
		asyncProcessor.Process(process);
		asyncProcessor.Process(process);
		RunProcessesToEnd();

		// Assert.
		var now = DateTime.Now.Second;
		var elapsed = now - startTime;
		elapsed.Should().BeGreaterOrEqualTo(DURATION);
	}

	[Test]
	public void WhenStartTwoNestedCoroutines_AndDurationIsOne_ThenCoroutinesCompletedAsterTwoSeconds()
	{
		// Arrange.
		const int DURATION = 1;
		const int EXPECTED_DURATION = DURATION * 2;
		var asyncProcessor = _asyncProcessor;
		var startTime = DateTime.Now.Second;

		IEnumerator RunNested(float durationInSeconds)
		{
			yield return RunForSecond(durationInSeconds);
			yield return RunForSecond(durationInSeconds);
		}

		// Act.
		asyncProcessor.Process(RunNested(DURATION));
		RunProcessesToEnd();

		// Assert.
		var now = DateTime.Now.Second;
		var elapsed = now - startTime;
		elapsed.Should().BeGreaterThanOrEqualTo(EXPECTED_DURATION);
	}

	private void RunProcessesToEnd()
	{
		while (_asyncProcessor.IsRunning)
		{
			_asyncProcessor.Tick();
			Thread.Sleep(10);
		}
	}

	private static IEnumerator RunForSecond(float durationInSeconds)
	{
		var start = DateTime.Now;
		while (DateTime.Now.Second - start.Second < durationInSeconds)
		{
			yield return null;
		}
	}
}