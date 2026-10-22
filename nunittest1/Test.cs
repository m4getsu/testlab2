using Moq;
using NUnit.Framework;

namespace nunittest1;

public class TestableClass : ClassUnderTest
{
    public TestableClass(IAffectingClass a) : base(a) { }
    public int CallProtected(int arg) => ProtectedMethod(arg);
}

// п.1 Ч параметризованные тесты
public class PublicMethodTests
{
    [TestCase(5, 3, 5)]
    [TestCase(5, 10, 10)]
    [TestCase(5, 5, 5)]
    public void PublicMethod_ReturnsMax(int val, int arg, int expected)
    {
        var aff = new AffectingClass { Val = val };
        var cut = new ClassUnderTest(aff);
        Assert.That(cut.PublicMethod(arg), Is.EqualTo(expected));
    }

    [TestCase(1)]
    [TestCase(42)]
    public void CallAffectingMethod_ReturnsMethodResult(int ret)
    {
        var mock = new Mock<AffectingClass>();
        mock.Setup(m => m.Method()).Returns(ret);
        var cut = new ClassUnderTest2(mock.Object);
        Assert.That(cut.CallAffectingMethod(), Is.EqualTo(ret));
    }

    [Test]
    public void CallStatic_Returns2()
    {
        var cut = new ClassUnderTest(new AffectingClass());
        Assert.That(cut.CallStatic(), Is.EqualTo(2));
    }
}

// п.2 Ч protected-метод и исключение
public class ProtectedMethodTests
{
    [Test]
    public void ProtectedMethod_NonZero_ReturnsVal()
    {
        var cut = new TestableClass(new AffectingClass { Val = 7 });
        Assert.That(cut.CallProtected(1), Is.EqualTo(7));
    }

    [Test]
    public void ProtectedMethod_Zero_Throws()
    {
        var cut = new TestableClass(new AffectingClass());
        Assert.Throws<ArgumentException>(() => cut.CallProtected(0));
    }
}

// п.3 и п.6 Ч Moq с SetUp
public class MockWithSetUpTests
{
    private Mock<IAffectingClass> mock = null!;
    private ClassUnderTest cut = null!;

    [SetUp]
    public void SetUp()
    {
        mock = new Mock<IAffectingClass>();
        mock.Setup(m => m.Val).Returns(10);
        cut = new ClassUnderTest(mock.Object);
    }

    [Test]
    public void PublicMethod_WithMock_ReturnsVal()
    {
        Assert.That(cut.PublicMethod(3), Is.EqualTo(10));
    }

    [Test]
    public void PublicMethod_ReadsVal()
    {
        cut.PublicMethod(3);
        mock.VerifyGet(m => m.Val, Times.AtLeastOnce());
    }
}

// п.3 Ч Moq без SetUp
public class MockWithoutSetUpTests
{
    [Test]
    public void PublicMethod_WithMock_ReturnsArg()
    {
        var mock = new Mock<IAffectingClass>();
        mock.Setup(m => m.Val).Returns(2);
        var cut = new ClassUnderTest(mock.Object);
        Assert.That(cut.PublicMethod(8), Is.EqualTo(8));
    }
}