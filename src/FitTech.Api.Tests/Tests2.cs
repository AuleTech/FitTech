﻿namespace FitTech.Api.Tests;

[Arguments("Hello")]
[Arguments("World")]
public class MoreTests(string title)
{
    [Test]
    public void ClassLevelDataRow()
    {
        Console.WriteLine(title);
        Console.WriteLine("Did I forget that data injection works on classes too?");
    }

    // You can even inject in ClassDataSources as properties to avoid repetitive constructors if you're using inheritance!
#pragma warning disable TUnit0023
    [ClassDataSource<DataClass>(Shared = SharedType.PerTestSession)]
    public required DataClass DataClass { get; init; }
#pragma warning restore TUnit0023

    [Test]
    [MatrixDataSource]
    public void Matrices(
        [Matrix(1, 2, 3)] int a,
        [Matrix(true, false)] bool b,
        [Matrix("A", "B", "C")] string c)
    {
        Console.WriteLine("A new test will be created for each data row, whether it's on the class or method level!");

        Console.WriteLine("Oh and this is a matrix test. That means all combinations of inputs are attempted.");
    }
}
