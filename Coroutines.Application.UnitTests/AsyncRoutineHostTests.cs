// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Collections;
using Depra.Coroutines.Async;
using Depra.Coroutines.Domain.Entities;

namespace Depra.Coroutines.Application.UnitTests;

[TestFixture(TestOf = typeof(AsyncRoutineHost))]
internal sealed class AsyncRoutineHostTests
{
	private ICoroutineHost _asyncRoutineHost = null!;
	private ICoroutineProcessor _coroutineProcessor = null!;

	[SetUp]
	public void Setup()
	{
		_coroutineProcessor = Substitute.For<ICoroutineProcessor>();
		_asyncRoutineHost = new AsyncRoutineHost(_coroutineProcessor);
	}

	[Test]
	public void WhenStartCoroutine_AndGetStatus_ThenNotIsDone()
	{
		// Arrange.
		var process = Substitute.For<IEnumerator>();
		
		// Act.
		var coroutine = _asyncRoutineHost.StartCoroutine(process);
		
		// Arrange.
		coroutine.Should().NotBeNull();
		coroutine.IsDone.Should().BeFalse();
	}

	[Test]
	public void WhenStartACoroutine_AndGivenEnumerator_ThenReturnsACoroutine()
	{ 
		// Arrange.
		var process = Substitute.For<IEnumerator>();
		
		// Act.
		var coroutine = _asyncRoutineHost.StartCoroutine(process);

		// Assert.
		coroutine.Should().NotBeNull();
		coroutine.Should().BeAssignableTo<ICoroutine>();
	}

	[Test]
	public void WhenStartCoroutine_ThenCallsProcessOnCoroutineProcessor()
	{
		// Arrange.
		var process = Substitute.For<IEnumerator>();

		// Act.
		_asyncRoutineHost.StartCoroutine(process);

		// Assert.
		_coroutineProcessor.Received().Process(process);
	}

	[Test]
	public void WhenStopCoroutine_AndGivenARunningCoroutine_ThenStopsTheCoroutine()
	{
		// Arrange.
		var process = Substitute.For<IEnumerator>();
		var coroutineProcessor = new AsyncProcessor();
		var coroutineHost = new AsyncRoutineHost(coroutineProcessor);
		var coroutine = coroutineHost.StartCoroutine(process);

		// Act.
		coroutineHost.StopCoroutine(coroutine);

		// Assert.
		coroutine.IsDone.Should().BeTrue();
	}

	[Test]
	public void WhenStopCoroutine_ThenCallsStopOnProcessor()
	{
		// Arrange.
		var coroutine = Substitute.For<ICoroutine>();

		// Act.
		_asyncRoutineHost.StopCoroutine(coroutine);

		// Assert.
		_coroutineProcessor.Received().Stop(coroutine);
	}
}