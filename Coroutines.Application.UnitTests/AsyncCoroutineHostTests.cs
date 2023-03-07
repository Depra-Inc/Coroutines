using System.Collections;
using Depra.Coroutines.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace Depra.Coroutines.Application.UnitTests;

[TestFixture(TestOf = typeof(AsyncCoroutineHost))]
internal sealed class AsyncCoroutineHostTests
{
	private AsyncCoroutineHost _asyncCoroutineHost = null!;
	private ICoroutineProcessor _coroutineProcessor = null!;

	[SetUp]
	public void Setup()
	{
		_coroutineProcessor = Substitute.For<ICoroutineProcessor>();
		_asyncCoroutineHost = new AsyncCoroutineHost(_coroutineProcessor);
	}

	[Test]
	public void WhenStartCoroutine_AndGetStatus_ThenNotIsDone()
	{
		// Arrange.
		var process = Substitute.For<IEnumerator>();
		
		// Act.
		var coroutine = _asyncCoroutineHost.StartCoroutine(process);
		
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
		var coroutine = _asyncCoroutineHost.StartCoroutine(process);

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
		_asyncCoroutineHost.StartCoroutine(process);

		// Assert.
		_coroutineProcessor.Received().Process(process);
	}

	[Test]
	public void WhenStopCoroutine_AndGivenARunningCoroutine_ThenStopsTheCoroutine()
	{
		// Arrange.
		var process = Substitute.For<IEnumerator>();
		var coroutineProcessor = new AsyncProcessor();
		var coroutineHost = new AsyncCoroutineHost(coroutineProcessor);
		var coroutine = coroutineHost.StartCoroutine(process);

		// Act.
		coroutineHost.StopCoroutine(coroutine);

		// Assert.
		coroutine.IsDone.Should().BeTrue();
	}

	[Test]
	public void WhenStopCoroutine_ThenCallsStopOnCoroutine()
	{
		// Arrange.
		var coroutine = Substitute.For<ICoroutine>();

		// Act.
		_asyncCoroutineHost.StopCoroutine(coroutine);

		// Assert.
		coroutine.Received().Stop();
	}
}