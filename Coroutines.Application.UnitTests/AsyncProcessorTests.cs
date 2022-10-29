// Copyright © 2022 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Collections;

namespace Depra.Coroutines.Application.UnitTests;

[TestFixture(TestOf = typeof(AsyncProcessor))]
public class AsyncProcessorTests
{
    private AsyncProcessor _asyncHandler = null!;

    [SetUp]
    public void Setup()
    {
        _asyncHandler = new AsyncProcessor();
    }

    [Test]
    public void WhenRunCoroutine_AndDurationIsOne_ThenCoroutineCompletedAfterOneSecond()
    {
        // Arrange.
        const int DURATION = 1;
        var asyncProcessor = _asyncHandler;
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
        var asyncProcessor = _asyncHandler;
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
        var asyncProcessor = _asyncHandler;
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
        while (_asyncHandler.IsRunning)
        {
            _asyncHandler.Tick();
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