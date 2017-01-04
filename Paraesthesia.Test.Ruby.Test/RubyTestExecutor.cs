using System;
using NUnit.Framework;
using RTE = Paraesthesia.Test.Ruby.RubyTestExecutor;

namespace Paraesthesia.Test.Ruby.Test {
	/// <summary>
	/// Unit tests for the <see cref="Paraesthesia.Test.Ruby.RubyTestExecutor"/> class.
	/// </summary>
	[TestFixture]
	public class RubyTestExecutor {
		[Test(Description="Verifies you may run standard NUnit-only tests.")]
		public void NUnitOnly_NoRubyAttrib(){
			Assert.IsTrue(true, "This NUnit-only test should always pass.");
		}

		[RubyTest("Paraesthesia.Test.Ruby.Test.Scripts.RubyTest.rb", "test_Invalid")]
		[Test(Description="Verifies that NUnit-only tests are not affected by the Ruby attributes unless the ExecuteTest method is called.")]
		public void NUnitOnly_RubyAttrib(){
			Assert.IsTrue(true, "This NUnit-only test should always pass.");
		}

		[RubyTest("Paraesthesia.Test.Ruby.Test.Scripts.RubyTest.rb", "test_Valid")]
		[WatirTest("Paraesthesia.Test.Ruby.Test.Scripts.WatirTest.rb", "test_Valid")]
		[Test(Description="Verifies you may only have one RubyTestAttribute (or derived) on your unit test class at a time.")]
		[ExpectedException(typeof(System.NotSupportedException))]
		public void RubyWatirTestAttributesTogether(){
			RTE.ExecuteTest();
		}

		[RubyTest("Paraesthesia.Test.Ruby.Test.Scripts.RubyTest.rb", "test_Valid")]
		[Test(Description="Verifies a valid Ruby test will execute and allow success.")]
		public void RubyTest_Valid(){
			RTE.ExecuteTest();
		}

		[WatirTest("Paraesthesia.Test.Ruby.Test.Scripts.WatirTest.rb", "test_Valid")]
		[Test(Description="Verifies a valid WATIR test will execute and allow success.")]
		public void WatirTest_Valid(){
			RTE.ExecuteTest();
		}

		[RubyTest("Paraesthesia.Test.Ruby.Test.Scripts.RubyTest.rb", "test_Invalid")]
		[Test(Description="Verifies a valid Ruby test will execute and allow failure.")]
		[ExpectedException(typeof(NUnit.Framework.AssertionException))]
		public void RubyTest_Invalid(){
			RTE.ExecuteTest();
		}

		[WatirTest("Paraesthesia.Test.Ruby.Test.Scripts.WatirTest.rb", "test_Invalid")]
		[Test(Description="Verifies a valid WATIR test will execute and allow failure.")]
		[ExpectedException(typeof(NUnit.Framework.AssertionException))]
		public void WatirTest_Invalid(){
			RTE.ExecuteTest();
		}

		[RubySupportFile("Paraesthesia.Test.Ruby.Test.Scripts.supportfile.txt", "supportfile.txt")]
		[RubySupportFile("Paraesthesia.Test.Ruby.Test.Scripts.SubFolder1.supportfile1.txt", @"SubFolder1\supportfile1.txt")]
		[RubySupportFile("Paraesthesia.Test.Ruby.Test.Scripts.SubFolder1.SubFolder2.supportfile2.txt", @"SubFolder1\SubFolder2\supportfile2.txt")]
		[RubyTest("Paraesthesia.Test.Ruby.Test.Scripts.RubyTest.rb", "test_RubySupportFile")]
		[Test(Description="Verifies Ruby support files can correctly be extracted.")]
		public void RubySupportFile_Valid(){
			RTE.ExecuteTest();
		}
	}
}
