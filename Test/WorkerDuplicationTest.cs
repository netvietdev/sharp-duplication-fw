﻿using Duplication.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Test.Model;

namespace Test
{
    [TestClass]
    public class WorkerDuplicationTest
    {
        [TestMethod]
        public void CanDuplicateAWorker()
        {
            // Arrange
            var w1 = new Worker()
            {
                Id = Guid.NewGuid(),
                FullName = "Worker 1",
            };
            w1.SetAge(10);

            // Act
            var w2 = w1.Duplicate();

            // Assert
            Assert.AreEqual(w1.FullName, w2.FullName);
            Assert.AreNotEqual(w1.Id, w2.Id);
        }
    }
}
