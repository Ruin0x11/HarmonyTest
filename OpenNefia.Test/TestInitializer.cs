using System;
using Love;
using Love.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using NUnit.Framework;

[SetUpFixture]
public class TestInitializerInNoNamespace
{
    [OneTimeSetUp]
    public void Setup() {
        var bootConfig = new BootConfig()
        {
            WindowTitle = "OpenNefia.NET",
            WindowDisplay = 0,
            WindowMinWidth = 800,
            WindowMinHeight = 600,
            WindowVsync = true,
            WindowResizable = true,
            DefaultRandomSeed = 0
        };

        Boot.Init(bootConfig);
    }

    [OneTimeTearDown]
    public void Teardown()
    {
    }
}