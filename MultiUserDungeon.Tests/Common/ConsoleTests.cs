using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MultiUserDungeon.Common.Tests
{
    [TestClass]
    public class ConsoleTests
    {
        [TestMethod]
        public void ConsoleTestsMethodologyTest()
        {
            var stream = new MemoryStream();
            Console.SetOut(new StreamWriter(stream));
            Console.WriteLine("Testing Testing");
            Console.Out.Flush();
            Assert.IsTrue(stream.Length > 0);
        }

        [TestMethod]
        public void ConsoleOutputTest()
        {
            var mucInStream = new MemoryStream();
            var mucOutStream = new MemoryStream();
            Console.SetIn(new StreamReader(mucInStream));
            Console.SetOut(new StreamWriter(mucOutStream));

            var mucOut = new StreamReader(mucOutStream);
            var mucIn = new StreamWriter(mucInStream);
            
            var muc = new MuConsole(1);

            const string TEST_STR_1 = "Test 1";
            muc.ServerSays(TEST_STR_1).Wait();

            mucOutStream.Seek(0, SeekOrigin.Begin);
            var says = mucOut.ReadLine();
            Assert.AreEqual(TEST_STR_1, says);
        }

        [TestMethod]
        public void ConsoleInputTest()
        {
            var mucInStream = new MemoryStream();
            var mucOutStream = new MemoryStream();
            Console.SetIn(new StreamReader(mucInStream));
            Console.SetOut(new StreamWriter(mucOutStream));

            var mucOut = new StreamReader(mucOutStream);
            var mucIn = new StreamWriter(mucInStream);

            var muc = new MuConsole(1);

            const string TEST_STR_2 = "Input Test";
            mucIn.WriteAndRewind(TEST_STR_2);
            
            Assert.AreEqual(TEST_STR_2, muc.ReadLine());
        }
    }
}
