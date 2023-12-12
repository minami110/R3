﻿namespace R3.Tests.OperatorTests;

public class ToListTest
{
    [Fact]
    public async Task ToList()
    {
        var publisher = new CompletablePublisher<int, Unit>();

        var listTask = publisher.ToListAsync();

        publisher.PublishOnNext(1);
        publisher.PublishOnNext(2);
        publisher.PublishOnNext(3);
        publisher.PublishOnNext(4);
        publisher.PublishOnNext(5);

        listTask.Status.Should().Be(TaskStatus.WaitingForActivation);

        publisher.PublishOnCompleted(Unit.Default);

        (await listTask).Should().Equal(1, 2, 3, 4, 5);
    }

    [Fact]
    public async Task ResultCompletableFault()
    {
        var publisher = new CompletablePublisher<int, Result<Unit>>();

        var listTask = publisher.ToListAsync();

        publisher.PublishOnNext(1);
        publisher.PublishOnNext(2);
        publisher.PublishOnNext(3);
        publisher.PublishOnNext(4);
        publisher.PublishOnNext(5);

        listTask.Status.Should().Be(TaskStatus.WaitingForActivation);

        publisher.PublishOnCompleted(Result.Failure<Unit>(new Exception("foo")));

        await Assert.ThrowsAsync<Exception>(async () => await listTask);
    }

    [Fact]
    public async Task ResultCompletableCancel()
    {
        var cts = new CancellationTokenSource();
        var isDisposed = false;

        var publisher = new CompletablePublisher<int, Result<Unit>>();

        var listTask = publisher.DoOnDisposed(() => isDisposed = true).ToListAsync(cts.Token);

        publisher.PublishOnNext(1);
        publisher.PublishOnNext(2);
        publisher.PublishOnNext(3);
        publisher.PublishOnNext(4);
        publisher.PublishOnNext(5);

        listTask.Status.Should().Be(TaskStatus.WaitingForActivation);

        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(async () => await listTask);

        isDisposed.Should().BeTrue();
    }


    [Fact]
    public async Task ToArray()
    {
        var publisher = new CompletablePublisher<int, Unit>();

        var listTask = publisher.ToArrayAsync();

        publisher.PublishOnNext(1);
        publisher.PublishOnNext(2);
        publisher.PublishOnNext(3);
        publisher.PublishOnNext(4);
        publisher.PublishOnNext(5);

        listTask.Status.Should().Be(TaskStatus.WaitingForActivation);

        publisher.PublishOnCompleted(Unit.Default);

        (await listTask).Should().Equal(1, 2, 3, 4, 5);
    }

    [Fact]
    public async Task ToArray2()
    {
        var publisher = new CompletablePublisher<int, Result<Unit>>();

        var listTask = publisher.ToArrayAsync();

        publisher.PublishOnNext(1);
        publisher.PublishOnNext(2);
        publisher.PublishOnNext(3);
        publisher.PublishOnNext(4);
        publisher.PublishOnNext(5);

        listTask.Status.Should().Be(TaskStatus.WaitingForActivation);

        publisher.PublishOnCompleted(Result.Failure<Unit>(new Exception("foo")));

        await Assert.ThrowsAsync<Exception>(async () => await listTask);
    }
}