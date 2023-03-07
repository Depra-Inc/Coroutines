// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Collections;
using FluentAssertions;
using NSubstitute;

namespace Depra.Coroutines.Application.UnitTests;

[TestFixture(TestOf = typeof(AsyncProcessor))]
internal sealed class AsyncProcessorTests
{
    private AsyncProcessor _asyncProcessor = null!;

    [SetUp]
    public void Setup() => 
	    _asyncProcessor = new AsyncProcessor();

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
	    var coroutine = TestCoroutine();
	    var isProcessed = false;

	    IEnumerator TestCoroutine()
	    {
		    yield return null;
		    isProcessed = true;
	    }

	    // Act.
	    _asyncProcessor.Process(coroutine);
	    _asyncProcessor.Tick();

	    // Assert.
	    isProcessed.Should().BeTrue();
    }

    [Test]
    public void Tick_RemovesFinishedCoroutines()
    {
	    // Arrange
	    var processor = new AsyncProcessor();
	    var coroutine = TestCoroutine();
	    IEnumerator TestCoroutine()
	    {
		    yield return null;
	    }

	    // Act
	    processor.Process(coroutine);
	    processor.Tick();
	    var isRunningBefore = processor.IsRunning;
	    processor.Tick();
	    var isRunningAfter = processor.IsRunning;

	    // Assert
	    Assert.IsTrue(isRunningBefore);
	    Assert.IsFalse(isRunningAfter);
    }

    [Test]
    public void WhenRunCoroutine_AndDurationIsOne_ThenCoroutineCompletedAfterOneSecond()
    {
        // Arrange.
        const int DURATION = 1;
        var asyncProcessor = _asyncProcessor;
        var startTime = DateTime.Now.Second;

        // Act.
        asyncProcessor.Process(RunForSecond(DURATION));
        RunProcessesToEnd();

        // Assert.
        Assert.That(DateTime.Now.Second - startTime, Is.GreaterThanOrEqualTo(DURATION));
    }

    [Test]
    public void WhenRunMultipleCoroutines_AndDurationIsOne_ThenCoroutinesCompletedAfterThreeSeconds()
    {
        // Arrange.
        const int DURATION = 1;
        var asyncProcessor = _asyncProcessor;
        var startTime = DateTime.Now.Second;

        // Act.
        // They should run in parallel
        asyncProcessor.Process(RunForSecond(DURATION));
        asyncProcessor.Process(RunForSecond(DURATION));
        asyncProcessor.Process(RunForSecond(DURATION));
        RunProcessesToEnd();

        // Assert.
        Assert.That(DateTime.Now.Second - startTime, Is.GreaterThanOrEqualTo(DURATION));
    }

    [Test]
    public void WhenRunTwoNestedCoroutines_AndDurationIsOne_ThenCoroutinesCompletedAsterTwoSeconds()
    {
        // Arrange.
        const int DURATION = 1;
        var asyncProcessor = _asyncProcessor;
        var startTime = DateTime.Now.Second;

        // Act.
        asyncProcessor.Process(RunNested(DURATION));
        RunProcessesToEnd();

        // Assert.
        Assert.That(DateTime.Now.Second - startTime, Is.GreaterThanOrEqualTo(DURATION * 2));
    }

    private static IEnumerator RunNested(float durationInSeconds)
    {
        yield return RunForSecond(durationInSeconds);
        yield return RunForSecond(durationInSeconds);
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