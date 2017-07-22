using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using FlexFramework.EventSystem;

public class DispatchingTest
{
    private int _index = -1;
    private int _count;
    private int _execute;

    public DispatchingTest()
    {
        this.Resolve();
    }

    [Subscribe("MSG_STRING")]
    private void OnStringMessage()
    {
        _index = 0;
    }

    [Subscribe(MessageType.Single)]
    private void OnSingle(string msg)
    {
        Assert.AreEqual("hello", msg);
        _index = 1;
    }

    [Subscribe(MessageType.Multiple)]
    private void OnMultiple(string a, string b, int c, bool d)
    {
        Assert.AreEqual("hello", a);
        Assert.AreEqual("world", b);
        Assert.AreEqual(1, c);
        Assert.True(d);
        _index = 2;
    }

    [Subscribe(typeof(MessageType))]
    private void OnMessage(EventArgs args)
    {
        _count++;
    }

    [Subscribe(MessageType.Multiple)]
    [Subscribe(MessageType.Single)]
    private void OnMessage()
    {
        _execute = 4;
    }

    [Test]
    public void TestString()
    {
        this.Dispatch("MSG_STRING", "hello");
        Assert.AreEqual(0, _index);
    }

    [Test]
    public void TestSingle()
    {
        this.Dispatch(MessageType.Single, "hello");
        Assert.AreEqual(1, _index);
    }

    [Test]
    public void TestMultiple()
    {
        this.Dispatch(MessageType.Multiple, "hello", "world", 1, true);
        Assert.AreEqual(2, _index);
    }

    [Test]
    public void TestType()
    {
        _count = 0;
        this.Dispatch(MessageType.Single, "hello");
        this.Dispatch(MessageType.Multiple, "hello", "world", 1, true);
        Assert.AreEqual(2, _count);
    }

    [TestCase(MessageType.Single), TestCase(MessageType.Multiple)]
    public void TestCompound(MessageType type)
    {
        this.Dispatch(type, "hello", "world", 1, true);
        Assert.AreEqual(4, _execute);
    }
}

public enum MessageType
{
    Single,
    Multiple
}
