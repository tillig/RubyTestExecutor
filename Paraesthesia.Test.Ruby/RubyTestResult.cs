using System;

namespace Paraesthesia.Test.Ruby {
	/// <summary>
	/// Contains the results for an individual Ruby test.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The results of a Ruby test script executing get stored in one of these structures
	/// and used to assert success of the test and output any messages from the test.
	/// </para>
	/// </remarks>
	/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestExecutor.ExecuteTest" />
	public class RubyTestResult {

		#region RubyTestResult Variables

		/// <summary>
		/// Internal storage for the <see cref="Paraesthesia.Test.Ruby.RubyTestResult.Assertions" /> property.
		/// </summary>
		private int _assertions = 0;

		/// <summary>
		/// Internal storage for the <see cref="Paraesthesia.Test.Ruby.RubyTestResult.Errors" /> property.
		/// </summary>
		private int _errors = 0;

		/// <summary>
		/// Internal storage for the <see cref="Paraesthesia.Test.Ruby.RubyTestResult.Failures" /> property.
		/// </summary>
		private int _failures = 0;

		/// <summary>
		/// Internal storage for the <see cref="Paraesthesia.Test.Ruby.RubyTestResult.Message" /> property.
		/// </summary>
		private string _message = "";

		/// <summary>
		/// Internal storage for the <see cref="Paraesthesia.Test.Ruby.RubyTestResult.Tests" /> property.
		/// </summary>
		private int _tests = 0;

		#endregion



		#region RubyTestResult Properties

		/// <summary>
		/// Gets the number of assertions that were executed in the test.
		/// </summary>
		/// <value>
		/// An <see cref="System.Int32"/> with the number of test assertions
		/// executed as reported by Ruby.
		/// </value>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestResult" />
		public int Assertions {
			get {
				return _assertions;
			}
		}

		/// <summary>
		/// Gets the number of errors that were encountered in the test.
		/// </summary>
		/// <value>
		/// An <see cref="System.Int32"/> with the number of errors that occcurred during
		/// test execution as reported by Ruby.
		/// </value>
		/// <remarks>
		/// <para>
		/// An error indicates that something went wrong programmatically during
		/// the test.  If an assertion failed, that information will be in the
		/// <see cref="Paraesthesia.Test.Ruby.RubyTestResult.Failures"/> value.
		/// </para>
		/// </remarks>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestResult" />
		public int Errors {
			get {
				return _errors;
			}
		}

		/// <summary>
		/// Gets the number of assertion failures that were encountered in the test.
		/// </summary>
		/// <value>
		/// An <see cref="System.Int32"/> with the number of assertion failures that occcurred during
		/// test execution as reported by Ruby.
		/// </value>
		/// <remarks>
		/// <para>
		/// A failure indicates that an assertion was incorrect and the test failed.
		/// If a something went programmatically wrong during the test, that information will be in the
		/// <see cref="Paraesthesia.Test.Ruby.RubyTestResult.Errors"/> value.
		/// </para>
		/// </remarks>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestResult" />
		public int Failures {
			get {
				return _failures;
			}
		}

		/// <summary>
		/// Gets any output from the test.
		/// </summary>
		/// <value>
		/// A <see cref="System.String"/> containing any output resulting from the test.
		/// Defaults to <see cref="System.String.Empty"/>.
		/// </value>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestResult" />
		public string Message {
			get {
				return _message;
			}
		}

		/// <summary>
		/// Gets the test success indicator.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the test succeeded; <see langword="false" /> otherwise.
		/// </value>
		/// <remarks>
		/// <para>
		/// The test is considered successful if <see cref="Paraesthesia.Test.Ruby.RubyTestResult.Failures"/>
		/// is <c>0</c> and <see cref="Paraesthesia.Test.Ruby.RubyTestResult.Errors"/>
		/// is <c>0</c>.
		/// </para>
		/// </remarks>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestResult" />
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestResult.Errors" />
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestResult.Failures" />
		public bool Success {
			get {
				return this.Failures == 0 && this.Errors == 0;
			}
		}

		/// <summary>
		/// Gets the number of tests that are included in this result.
		/// </summary>
		/// <value>
		/// A <see cref="System.Int32"/> with the number of tests that make up the
		/// set of results contained in this <see cref="Paraesthesia.Test.Ruby.RubyTestResult"/>.
		/// </value>
		/// <remarks>
		/// <para>
		/// Typically this value should be <c>1</c>.  If multiple tests are combined into
		/// one <see cref="Paraesthesia.Test.Ruby.RubyTestResult"/>, there is no way to
		/// determine if any individual test was successful; any error or failure will
		/// indicate failure of all contained tests.
		/// </para>
		/// </remarks>
		public int Tests {
			get {
				return _tests;
			}
		}

		#endregion



		#region RubyTestResult Implementation

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Paraesthesia.Test.Ruby.RubyTestResult" /> class.
		/// </summary>
		/// <param name="tests">Number of tests that make up this result.</param>
		/// <param name="assertions">Number of assertions that were executed.</param>
		/// <param name="failures">Number of assertions that failed.</param>
		/// <param name="errors">Number of errors encountered.</param>
		/// <param name="message">Output from the test.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if:
		/// <list type="bullet">
		/// <item>
		/// <term><paramref name="tests" /> &lt;= 0</term>
		/// </item>
		/// <item>
		/// <term><paramref name="assertions" /> &lt; 0</term>
		/// </item>
		/// <item>
		/// <term><paramref name="failures" /> &lt; 0</term>
		/// </item>
		/// <item>
		/// <term><paramref name="errors" /> &lt; 0</term>
		/// </item>
		/// </list>
		/// </exception>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestResult" />
		public RubyTestResult(int tests, int assertions, int failures, int errors, string message) {
			if(tests <= 0){
				throw new ArgumentOutOfRangeException("tests", tests, "Number of tests contained in a RubyTestResult must be at least 1.");
			}
			this._tests = tests;

			if(assertions < 0){
				throw new ArgumentOutOfRangeException("assertions", assertions, "Number of assertions contained in a RubyTestResult must be at least 0.");
			}
			this._assertions = assertions;

			if(failures < 0){
				throw new ArgumentOutOfRangeException("failures", failures, "Number of failures contained in a RubyTestResult must be at least 0.");
			}
			this._failures = failures;

			if(errors < 0){
				throw new ArgumentOutOfRangeException("errors", errors, "Number of errors contained in a RubyTestResult must be at least 0.");
			}
			this._errors = errors;

			if(message != null){
				this._message = message;
			}
		}

		#endregion

		#endregion

	}
}
