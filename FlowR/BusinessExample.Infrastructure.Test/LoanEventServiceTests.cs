using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BusinessExample.Core.Entities;
using BusinessExample.Core.Interfaces;
using Xunit;

namespace BusinessExample.Infrastructure.Test
{
    public class LoanEventServiceTests
    {
        [Fact]
        public async void Can_raise_event()
        {
            // Arrange

            var outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LoanEventServiceTests", "Can_raise_event");

            var sut = new LoanEventService(outputDirectory);

            // Act

            await sut.RaiseEvent(LoanEventType.Referred, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()); 

            // Assert

            Assert.Single(Directory.GetFiles(outputDirectory));
        }
    }
}
