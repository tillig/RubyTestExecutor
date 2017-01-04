using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Paraesthesia.Test.Ruby
{
	/// <summary>
	/// Executes Ruby unit tests.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class is for use in integrating Ruby/WATIR scripts into an NUnit unit testing
	/// framework.  Using the <see cref="Paraesthesia.Test.Ruby.RubyTestExecutor.ExecuteTest"/>
	/// method in combination with custom attributes on the NUnit unit test method, a
	/// Ruby script embedded into an NUnit assembly as a resource along with any helper
	/// files that may be required can be extracted, executed, and cleaned up automatically
	/// with the results of the unit test being correctly piped to NUnit.
	/// </para>
	/// <para>
	/// The custom attributes that can be used on your NUnit test methods are as follows:
	/// </para>
	/// <list type="bullet">
	/// <item>
	/// <term><see cref="Paraesthesia.Test.Ruby.RubyTestAttribute"/></term>
	/// <description>
	/// This attribute indicates the unit test method is going to execute a Ruby test script.
	/// You may only have one of these per NUnit test method, and you may not use it in
	/// combination with the <see cref="Paraesthesia.Test.Ruby.WatirTestAttribute"/>.  The
	/// two are mutually exclusive.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see cref="Paraesthesia.Test.Ruby.WatirTestAttribute"/></term>
	/// <description>
	/// This attribute indicates the unit test method is going to execute a WATIR-based Ruby test script.
	/// You may only have one of these per NUnit test method, and you may not use it in
	/// combination with the <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute"/>.  The
	/// two are mutually exclusive.  Note that while using the <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute"/>
	/// will work for WATIR scripts, certain WATIR-specific configuration options are only
	/// available to WATIR script execution if a <see cref="Paraesthesia.Test.Ruby.WatirTestAttribute"/>
	/// is used.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute"/></term>
	/// <description>
	/// This attribute denotes a support file that will be extracted alongside the Ruby
	/// test script.  The Ruby script may use these as external data or they may contain
	/// additional Ruby code that gets included into the primary Ruby script.
	/// </description>
	/// </item>
	/// </list>
	/// <para>
	/// Using the <see cref="Paraesthesia.Test.Ruby.RubyTestExecutor"/> is a very simple process
	/// in the scheme of unit test writing.  The first step is to write your Ruby or WATIR
	/// script in the usual fashion, deriving your classes from the Ruby <c>Test::Unit::TestCase</c>
	/// class or a derived class thereof, and ensure that the scripts themselves work.
	/// If your script requires extra support files, make sure those files are located in
	/// the same folder as the Ruby script or in a subfolder under that.  This makes it
	/// simple to know which files may be affected by a Ruby script simply by looking at
	/// the filesystem, and it ensures portability of an individual script.
	/// </para>
	/// <para>
	/// Create your NUnit test class as usual.  Add a reference to the <c>Paraesthesia.Test.Ruby</c>
	/// assembly so you have access to the <see cref="Paraesthesia.Test.Ruby.RubyTestExecutor"/>
	/// and associated attributes.
	/// </para>
	/// <para>
	/// If your Ruby/WATIR scripts are not already in the same filesystem tree as the NUnit
	/// assembly, move them in there.  Include them in the Visual Studio project for the
	/// NUnit test assembly as Embedded Resources.  This way they will be embedded into the
	/// NUnit assembly for easy portability and distribution.
	/// </para>
	/// <para>
	/// To create a Ruby test inside NUnit, mark your NUnit test method with a standard
	/// NUnit <see cref="NUnit.Framework.TestAttribute"/>, then add a <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute" />
	/// (if it's a Ruby-only test) or a <see cref="Paraesthesia.Test.Ruby.WatirTestAttribute"/>
	/// (if it's a Ruby/WATIR test).
	/// </para>
	/// <para>
	/// If you have any support files, add one <see cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute"/>
	/// for each of them to the test method.
	/// </para>
	/// <para>
	/// Finally, add a call to <see cref="Paraesthesia.Test.Ruby.RubyTestExecutor.ExecuteTest"/>
	/// to your method.  That call will automatically execute the Ruby script test method,
	/// parse the results, and perform the assertions in NUnit to ensure the NUnit test
	/// succeeds or fails according to the result of the Ruby test.  All embedded scripts
	/// and supporting files will automatically be extracted to a temporary location and
	/// cleaned up for you.
	/// </para>
	/// <para>
	/// You may modify the test assembly's <c>app.config</c> file with the following <c>appSettings</c>
	/// to change the behavior of the Ruby test execution:
	/// </para>
	/// <list type="bullet">
	/// <item>
	/// <term><c>ShowBrowserWindow</c> ("true" or "false")</term>
	/// <description>
	/// Indicates if the browser window should show when WATIR tests are run.  This only
	/// affects tests marked with a <see cref="Paraesthesia.Test.Ruby.WatirTestAttribute"/>.
	/// Defaults to <see langword="false" />.
	/// </description>
	/// </item>
	/// <item>
	/// <term><c>DeleteTempFilesWhenFinished</c> ("true" or "false")</term>
	/// <description>
	/// Indicates whether the temporary files that get extracted during a test should
	/// be automatically cleaned up after the test runs.  Helpful in debugging. Defaults
	/// to <see langword="true" />.
	/// </description>
	/// </item>
	/// <item>
	/// <term><c>FailureStream</c> (<c>None</c>, <c>Out</c>, or <c>Error</c>)</term>
	/// <description>
	/// Indicates the Console stream that failure messages get pushed to (nowhere/swallowed, <see cref="System.Console.Out" /> or <see cref="System.Console.Error" />).
	/// Defaults to <c>None</c>.
	/// </description>
	/// </item>
	/// <item>
	/// <term><c>SuccessStream</c> (<c>None</c>, <c>Out</c>, or <c>Error</c>)</term>
	/// <description>
	/// Indicates the Console stream that success messages get pushed to (nowhere/swallowed, <see cref="System.Console.Out" /> or <see cref="System.Console.Error" />).
	/// Defaults to <c>Out</c>.
	/// </description>
	/// </item>
	/// <item>
	/// <term><c>FailMessageFromTestOutput</c> ("true" or "false")</term>
	/// <description>
	/// Indicates whether the assertion failure message from a test should be the output
	/// from the test execution.  Defaults to <see langword="true" />.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	/// <example>
	/// <para>
	/// The following code shows a unit test script that contains both NUnit native and
	/// Ruby/WATIR tests.
	/// </para>
	/// <code lang="C#">
	/// using System;
	/// using NUnit.Framework;
	/// using Paraesthesia.Test.Ruby;
	///
	/// namespace MyNamespace {
	///   [TestFixture]
	///   public class MyTestClass {
	///     [Test]
	///     public void NUnitOnly_NoRubyAttrib(){
	///       Assert.IsTrue(true, "This NUnit-only test should always pass.");
	///     }
	///
	///     [RubyTest("Paraesthesia.Test.Ruby.Test.Scripts.RubyTest.rb", "test_Invalid")]
	///     [Test]
	///     public void NUnitOnly_RubyAttrib(){
	///       // This will pass and never execute Ruby or extract files.
	///       Assert.IsTrue(true, "This NUnit-only test should always pass.");
	///     }
	///
	///     [RubyTest("Paraesthesia.Test.Ruby.Test.Scripts.RubyTest.rb", "test_Valid")]
	///     [Test]
	///     public void RubyTest(){
	///       RubyTestExecutor.ExecuteTest();
	///     }
	///
	///     [WatirTest("Paraesthesia.Test.Ruby.Test.Scripts.WatirTest.rb", "test_Valid")]
	///     [Test]
	///     public void WatirTest(){
	///       RubyTestExecutor.ExecuteTest();
	///     }
	///
	///     [RubySupportFile("MyAssembly.MyNamespace.file.txt", "supportfile.txt")]
	///     [RubySupportFile("MyAssembly.MyNamespace.Sub1.file1.txt", @"Sub1\file1.txt")]
	///     [RubySupportFile("MyAssembly.MyNamespace.Sub1.Sub2.file2.txt", @"Sub1\Sub2\file2.txt")]
	///     [RubyTest("Paraesthesia.Test.Ruby.Test.Scripts.RubyTest.rb", "test_SupportFile")]
	///     [Test]
	///     public void SupportFiles(){
	///       RubyTestExecutor.ExecuteTest();
	///     }
	///   }
	/// }
	/// </code>
	/// <para>
	/// The corresponding Ruby/WATIR files are below.
	/// </para>
	/// <para>
	/// <c>RubyTest.rb</c>:
	/// </para>
	/// <code>
	/// require 'test/unit'
	/// require 'test/unit/assertions'
	///
	/// class RubyTest &lt; Test::Unit::TestCase
	///   def test_Valid
	///     assert(true, 'This unit test should always pass.')
	///   end
	///
	///   def test_Invalid
	///     assert(false, 'This unit test should always fail.')
	///   end
	///
	///   def test_RubySupportFile
	///     assert(File.exist?("file.txt"))
	///     assert(File.exist?("Sub1\\file1.txt"))
	///     assert(File.exist?("Sub1\\Sub2\\file2.txt"))
	///   end
	/// end
	/// </code>
	/// <para>
	/// <c>WatirTest.rb</c>:
	/// </para>
	/// <code>
	/// require 'watir'
	/// require 'test/unit'
	/// require 'test/unit/assertions'
	/// include Watir
	///
	/// class WatirTest &lt; Test::Unit::TestCase
	///   def setup
	///     @ie = IE.new
	///     @ie.goto("http://www.google.com")
	///   end
	///
	///   def test_Valid
	///     assert(@ie.contains_text('Google'))
	///   end
	///
	///   def teardown
	///     @ie.close
	///   end
	/// end
	/// </code>
	/// <para>
	/// Below is a sample <c>app.config</c> file with <c>appSettings</c> that set
	/// the browser window to show during the execution of WATIR tests and instructs
	/// the <see cref="Paraesthesia.Test.Ruby.RubyTestExecutor"/> not to delete the
	/// temporary files when the test is finished.  Failures will be swallowed, but the
	/// assertion failure message will be the output from the failure. Successes will go
	/// to <see cref="System.Console.Out"/>.
	/// </para>
	/// <code>
	/// &lt;?xml version="1.0" encoding="utf-8" ?&gt;
	/// &lt;configuration&gt;
	///   &lt;appSettings&gt;
	///     &lt;add key="ShowBrowserWindow" value="true" /&gt;
	///     &lt;add key="DeleteTempFilesWhenFinished" value="false" /&gt;
	///     &lt;add key="FailureStream" value="None" /&gt;
	///     &lt;add key="SuccessStream" value="Out" /&gt;
	///     &lt;add key="FailMessageFromTestOutput" value="true" /&gt;
	///   &lt;/appSettings&gt;
	/// &lt;/configuration&gt;
	/// </code>
	/// </example>
	/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestAttribute" />
	/// <seealso cref="Paraesthesia.Test.Ruby.WatirTestAttribute" />
	/// <seealso cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute" />
	public sealed class RubyTestExecutor {

		#region RubyTestExecutor Variables

		#region Constants

		/// <summary>
		/// String that serves as a divider between test results.
		/// </summary>
		const string STR_RESULTS_DIVIDER = "\n----------\n";

		/// <summary>
		/// App settings key for whether to delete temp files when the test is finished.
		/// </summary>
		const string SETTINGS_DELETEFILESWHENFINISHED = "DeleteTempFilesWhenFinished";

		/// <summary>
		/// App settings key for display/hide the browser window in a WATIR test.
		/// </summary>
		const string SETTINGS_SHOWBROWSERWINDOW = "ShowBrowserWindow";

		/// <summary>
		/// App settings key for failure output stream.
		/// </summary>
		const string SETTINGS_FAILURESTREAM = "FailureStream";

		/// <summary>
		/// App settings key for success output stream.
		/// </summary>
		const string SETTINGS_SUCCESSSTREAM = "SuccessStream";

		/// <summary>
		/// App settings key for success output stream.
		/// </summary>
		const string SETTINGS_FAILMESSAGEFROMTESTOUTPUT = "FailMessageFromTestOutput";

		#endregion

		#endregion



		#region RubyTestExecutor Properties

		/// <summary>
		/// Gets the value from configuration indiciating if the test failure assertion message
		/// should be the output from the test execution failure.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to make the test failure assertion message be the results
		/// of executing the test; <see langword="false" /> for a more succinct failure message
		/// that implies the results of the test will be investigated from the output
		/// of the test failure results.  Defaults to <see langword="true" />.
		/// </value>
		/// <remarks>
		/// <para>
		/// Since automated tools generally only use the assertion failure message to
		/// illustrate why a test failed instead of providing all of the test output, this
		/// setting defaults to <see langword="true" />.  If you have a more intelligent
		/// tool, use the <see cref="Paraesthesia.Test.Ruby.RubyTestExecutor.SuccessStream"/>
		/// and <see cref="Paraesthesia.Test.Ruby.RubyTestExecutor.FailureStream"/> properties
		/// to direct your output accordingly and set this property to <see langword="false" />.
		/// </para>
		/// <para>
		/// Set this value by setting the configuration appSettings key "FailMessageFromTestOutput"
		/// to a Boolean value ("true" or "false").
		/// </para>
		/// </remarks>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestExecutor" />
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestExecutor.FailureStream" />
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestExecutor.SuccessStream" />
		public static bool FailMessageFromTestOutput{
			get {
				return ParseAppSettingsBoolean(SETTINGS_FAILMESSAGEFROMTESTOUTPUT, true);
			}
		}

		/// <summary>
		/// Gets the value from configuration indicating if the browser window should
		/// display in WATIR tests.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to show the window during WATIR tests; <see langword="false" />
		/// otherwise.  Defaults to <see langword="false" />.
		/// </value>
		/// <remarks>
		/// <para>
		/// Set this value by setting the configuration appSettings key "ShowBrowserWindow"
		/// to a Boolean value ("true" or "false").
		/// </para>
		/// </remarks>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestExecutor" />
		public static bool ShowBrowserWindow{
			get {
				return ParseAppSettingsBoolean(SETTINGS_SHOWBROWSERWINDOW, false);
			}
		}

		/// <summary>
		/// Gets the value from configuration indicating if temporary files extracted
		/// during a test should be deleted.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to delete temporary files; <see langword="false" />
		/// otherwise.  Defaults to <see langword="true" />.
		/// </value>
		/// <remarks>
		/// <para>
		/// Set this value by setting the configuration appSettings key "DeleteTempFilesWhenFinished"
		/// to a Boolean value ("true" or "false").
		/// </para>
		/// </remarks>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestExecutor" />
		public static bool DeleteTempFilesWhenFinished{
			get {
				return ParseAppSettingsBoolean(SETTINGS_DELETEFILESWHENFINISHED, true);
			}
		}

		/// <summary>
		/// Gets the stream that failure output should be piped to based on configuration.
		/// </summary>
		/// <value>
		/// <see cref="System.Console.Out"/>, <see cref="System.Console.Error"/>, or <see langword="null" />, based on configuration.
		/// Defaults to <see langword="null" />.
		/// </value>
		/// <remarks>
		/// <para>
		/// Set this value by setting the configuration appSettings key "FailureStream"
		/// to an accepted value (<c>None</c>, <c>Out</c>, <c>Error</c>).
		/// </para>
		/// </remarks>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestExecutor" />
		public static TextWriter FailureStream{
			get {
				string streamName = System.Configuration.ConfigurationSettings.AppSettings[SETTINGS_FAILURESTREAM];
				TextWriter writer = null;
				switch(streamName){
					case "Error":
						writer = Console.Error;
						break;
					case "Out":
						writer = Console.Out;
						break;
					case "None":
					default:
						writer = null;
						break;
				}
				return writer;
			}
		}

		/// <summary>
		/// Gets the stream that success output should be piped to based on configuration.
		/// </summary>
		/// <value>
		/// <see cref="System.Console.Out"/>, <see cref="System.Console.Error"/>, or <see langword="null" />, based on configuration.
		/// Defaults to <see cref="System.Console.Out" />.
		/// </value>
		/// <remarks>
		/// <para>
		/// Set this value by setting the configuration appSettings key "SuccessStream"
		/// to an accepted value (<c>None</c>, <c>Out</c>, <c>Error</c>).
		/// </para>
		/// </remarks>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestExecutor" />
		public static TextWriter SuccessStream{
			get {
				string streamName = System.Configuration.ConfigurationSettings.AppSettings[SETTINGS_SUCCESSSTREAM];
				TextWriter writer = null;
				switch(streamName){
					case "Error":
						writer = Console.Error;
						break;
					case "None":
						writer = null;
						break;
					case "Out":
					default:
						writer = Console.Out;
						break;
				}
				return writer;
			}
		}

		#endregion



		#region RubyTestExecutor Implementation

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Paraesthesia.Test.Ruby.RubyTestExecutor" /> class.
		/// </summary>
		private RubyTestExecutor(){}

		#endregion

		#region Methods

		/// <summary>
		/// Executes the Ruby unit test specified by the calling method.
		/// </summary>
		/// <param name="makeAssertions">
		/// <see langword="true" /> to automatically make the NUnit assertion of Ruby
		/// test success, <see langword="false" /> to simply return results.
		/// </param>
		/// <param name="displayOutput">
		/// <see langword="true" /> to output successful test data to
		/// <see cref="System.Console.Out"/> and error test data to
		/// <see cref="System.Console.Error"/>; <see langword="false" /> to simply return
		/// results.
		/// </param>
		/// <returns>
		/// A <see cref="Paraesthesia.Test.Ruby.RubyTestResult"/> with the results of executing
		/// the Ruby test.
		/// </returns>
		/// <exception cref="System.NotSupportedException">
		/// Thrown if more than one <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute"/>
		/// is found on the executing method (this includes attributes derived from
		/// <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute"/>).
		/// </exception>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestExecutor" />
		public static RubyTestResult ExecuteTest(bool makeAssertions, bool displayOutput){
			// Get the RubyTest attribute on the calling method
			StackTrace trace = new StackTrace();
			MethodBase method = null;
			for(int i = 0; i < trace.FrameCount; i++){
				MethodBase frameMethod = trace.GetFrame(i).GetMethod();
				if(frameMethod.ReflectedType != typeof(RubyTestExecutor)){
					method = frameMethod;
					break;
				}
			}
			RubyTestAttribute rubyTest = GetRubyTestAttribute(method);
			if(rubyTest == null){
				Assert.Fail("GetResultsFromRuby called from a method that does not have the [RubyTest] attribute in place.");
				return null;
			}

			// Get the configuration setting indicating if temp files should be deleted
			bool deleteFilesWhenFinished = RubyTestExecutor.DeleteTempFilesWhenFinished;

			// Extract files and run the test
			TempFileCollection tempFiles = null;
			try{
				// Prepare for temporary file extraction
				tempFiles = new TempFileCollection();
				tempFiles.KeepFiles = !deleteFilesWhenFinished;
				if(!Directory.Exists(tempFiles.BasePath)){
					Directory.CreateDirectory(tempFiles.BasePath);
				}

				// Extract the Ruby test
				string rubyScriptPath = Path.GetFullPath(Path.Combine(tempFiles.BasePath, "__RubyTestScript.rb"));
				Assembly scriptAssembly = null;
				if(rubyTest.AssemblyName != ""){
					try{
						scriptAssembly = Assembly.Load(rubyTest.AssemblyName);
					}
					catch{
						Console.Error.WriteLine("Error loading assembly [{0}].", rubyTest.AssemblyName);
						throw;
					}
				}
				else{
					scriptAssembly = method.ReflectedType.Assembly;
				}
				ExtractResourceToFile(scriptAssembly, rubyTest.EmbeddedScriptPath, rubyScriptPath);
				if(!File.Exists(rubyScriptPath)){
					throw new FileNotFoundException("Error extracting Ruby test script to file.", rubyScriptPath);
				}
				tempFiles.AddFile(rubyScriptPath, !deleteFilesWhenFinished);

				// Extract support files
				RubySupportFileAttribute[] supportFiles = (RubySupportFileAttribute[])method.GetCustomAttributes(typeof(RubySupportFileAttribute), true);
				if(supportFiles != null){
					foreach(RubySupportFileAttribute supportFile in supportFiles){
						// Calculate the location for the support file
						string supportFilePath = Path.GetFullPath(Path.Combine(tempFiles.BasePath, supportFile.TargetFilename));
						string supportFileDirectory = Path.GetDirectoryName(supportFilePath);

						// Ensure the location is valid
						if(supportFileDirectory.IndexOf(tempFiles.BasePath) != 0){
							throw new ArgumentOutOfRangeException("TargetFilename", supportFile.TargetFilename, "Support file target location must be at or below the location of the extracted Ruby script (no '..' parent folders).");
						}

						// Create any missing folders
						if(!Directory.Exists(supportFileDirectory)){
							Directory.CreateDirectory(supportFileDirectory);
						}

						// Get the assembly the support file is in
						Assembly fileAssembly = null;
						if(supportFile.AssemblyName != ""){
							try{
								fileAssembly = Assembly.Load(supportFile.AssemblyName);
							}
							catch{
								Console.Error.WriteLine("Error loading assembly [{0}].", supportFile.AssemblyName);
								throw;
							}
						}
						else{
							fileAssembly = method.ReflectedType.Assembly;
						}

						// Extract the support file
						ExtractResourceToFile(fileAssembly, supportFile.EmbeddedFilePath, supportFilePath);
						tempFiles.AddFile(supportFilePath, !deleteFilesWhenFinished);
					}
				}

				// Run test
				RubyTestResult result = RunTest(rubyScriptPath, rubyTest, tempFiles.BasePath);

				// Create the test description string
				string scriptDesc = String.Format("Script [{0}]; Test [{1}]", rubyTest.EmbeddedScriptPath, rubyTest.TestMethod);

				// Write output
				if(displayOutput && result != null && result.Message != ""){
					System.IO.TextWriter output = null;
					string successMsg = "";

					if(!result.Success){
						output = RubyTestExecutor.FailureStream;
						successMsg = "FAILED";
					}
					else{
						output = RubyTestExecutor.SuccessStream;
						successMsg = "SUCCESS";
					}
					if(output != null){
						output.WriteLine("{0}: {1}", scriptDesc, successMsg);
						output.WriteLine(result.Message);
						output.WriteLine(STR_RESULTS_DIVIDER);
					}
				}

				// Make assertions
				if(makeAssertions){
					if(result == null){
						Assert.Fail("Ruby test result not correctly returned from test execution.");
					}
					else{
						string failMsg = String.Format("{0}: FAILED", scriptDesc);
						if(RubyTestExecutor.FailMessageFromTestOutput){
							failMsg += result.Message + STR_RESULTS_DIVIDER;
						}
						Assert.IsTrue(result.Success, failMsg);
					}
				}

				// Return results
				return result;
			}
			finally{
				// Delete any temporary files
				if(tempFiles != null){
					tempFiles.Delete();
					if(Directory.Exists(tempFiles.BasePath) && deleteFilesWhenFinished){
						Directory.Delete(tempFiles.BasePath, true);
					}
					tempFiles = null;
				}
			}
		}

		/// <summary>
		/// Executes the Ruby unit test specified by the calling method.
		/// </summary>
		/// <exception cref="System.NotSupportedException">
		/// Thrown if more than one <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute"/>
		/// is found on the executing method (this includes attributes derived from
		/// <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute"/>).
		/// </exception>
		/// <returns>
		/// A <see cref="Paraesthesia.Test.Ruby.RubyTestResult"/> with the results of executing
		/// the Ruby test.
		/// </returns>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestExecutor" />
		public static RubyTestResult ExecuteTest(){
			return ExecuteTest(true, true);
		}

		/// <summary>
		/// Parses a <see cref="System.Boolean"/> from configuration settings
		/// </summary>
		/// <param name="appSettingsKey">The key to look at in app settings.</param>
		/// <param name="defaultValue">If the value isn't found or a parsing error is encountered, the default value to return.</param>
		/// <returns>
		/// A <see cref="System.Boolean"/> with the app setting.
		/// </returns>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestExecutor" />
		private static bool ParseAppSettingsBoolean(string appSettingsKey, bool defaultValue){
			bool retVal = defaultValue;
			string retValStr = System.Configuration.ConfigurationSettings.AppSettings[appSettingsKey];
			if(retValStr != null && retValStr != ""){
				try{
					retVal = bool.Parse(retValStr);
				}
				catch{
					retVal = defaultValue;
				}
			}
			return retVal;
		}

		/// <summary>
		/// Executes the Ruby test and returns the results.
		/// </summary>
		/// <param name="scriptFilename">The full path to the Ruby test script to execute.</param>
		/// <param name="attrib">The <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute"/> with information about the test being executed.</param>
		/// <param name="workingDirectory">The working directory for the test.  This is where the test script was extracted to.</param>
		/// <remarks>
		/// <para>
		/// This method executes the Ruby script, parses the results, and places those results
		/// into a <see cref="Paraesthesia.Test.Ruby.RubyTestResult"/>.
		/// </para>
		/// </remarks>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestExecutor" />
		private static RubyTestResult RunTest(string scriptFilename, RubyTestAttribute attrib, string workingDirectory){
			// Build the basic command line
			string command = "ruby.exe";
			string arguments = String.Format("\"{0}\" --name={1}", scriptFilename, attrib.TestMethod);

			// WATIR-specific options
			if(attrib is WatirTestAttribute){
				// Determine if we show/hide the browser window
				bool showBrowserWindow = RubyTestExecutor.ShowBrowserWindow;
				if(!showBrowserWindow){
					arguments += " -b";
				}
			}

			// Test execution
			RubyTestResult results = null;
			using (Process proc = new Process()) {
				// Create the process
				proc.StartInfo.UseShellExecute = false;
				proc.StartInfo.RedirectStandardOutput = true;
				proc.StartInfo.CreateNoWindow = true;
				proc.StartInfo.FileName = command;
				proc.StartInfo.Arguments = arguments;
				proc.StartInfo.WorkingDirectory = workingDirectory;

				// Execute the process
				proc.Start();
				proc.WaitForExit();

				// Get the output
				string output = proc.StandardOutput.ReadToEnd();

				// Clean up the process
				proc.Close();

				// Parse the results
				// Results in format: X tests, Y assertions, Z failures, Q errors
				// where X, Y, Z and Q are integers
				RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline;
				Regex regex = new Regex(@"(\d+)\s*tests[^\d]*(\d+)\s*assertions[^\d]*(\d+)\s*failures[^\d]*(\d+)\s*errors", options);
				Match match = regex.Match(output);
				if(match != null && match.Success){
					int tests = Convert.ToInt32(match.Groups[1].Value, System.Globalization.CultureInfo.CurrentCulture);
					int assertions = Convert.ToInt32(match.Groups[2].Value, System.Globalization.CultureInfo.CurrentCulture);
					int failures = Convert.ToInt32(match.Groups[3].Value, System.Globalization.CultureInfo.CurrentCulture);
					int errors = Convert.ToInt32(match.Groups[4].Value, System.Globalization.CultureInfo.CurrentCulture);
					results = new RubyTestResult(tests, assertions, failures, errors, output);
				}
				else{
					results = new RubyTestResult(1, 0, 0, 1, "*** Unable to parse output from Ruby test ***\n" + output);
				}
			}

			return results;
		}

		/// <summary>
		/// Retrieves the <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute"/> from the
		/// given method.
		/// </summary>
		/// <param name="method">The method to investigate for the <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute"/>.</param>
		/// <returns>
		/// The method's <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute"/> if found, <see langword="null" />
		/// if not.
		/// </returns>
		/// <exception cref="System.NotSupportedException">
		/// Thrown if more than one <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute"/>
		/// is found on the method (this includes derived types).
		/// </exception>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestExecutor" />
		private static RubyTestAttribute GetRubyTestAttribute(MethodBase method){
			RubyTestAttribute[] rubyTests = (RubyTestAttribute[])method.GetCustomAttributes(typeof(RubyTestAttribute), true);
			if(rubyTests == null || rubyTests.Length == 0){
				return null;
			}
			if(rubyTests.Length > 1){
				throw new NotSupportedException("You may not have more than one RubyTestAttribute (or derived) associated with a single method.");
			}
			RubyTestAttribute rubyTest = rubyTests[0];
			return rubyTest;
		}

		/// <summary>
		/// Extracts an embedded resource to a file in the filesystem.
		/// </summary>
		/// <param name="assembly">The <see cref="System.Reflection.Assembly"/> containing the resource.</param>
		/// <param name="resourcePath">The path to the embedded resource.</param>
		/// <param name="destinationPath">The absolute path of the destination where the resource should be extracted.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="assembly" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// Thrown if <paramref name="resourcePath" /> or <paramref name="destinationPath" />
		/// is <see langword="null" /> or <see cref="System.String.Empty"/>.
		/// </exception>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestExecutor" />
		private static void ExtractResourceToFile(Assembly assembly, string resourcePath, string destinationPath){
			// Validate parameters
			if(assembly == null){
				throw new ArgumentNullException("assembly", "Assembly containing embedded resource may not be null.");
			}
			if(resourcePath == null || resourcePath == String.Empty){
				throw new ArgumentException("resourcePath", "Path to embedded resource may not be null or empty.");
			}
			if(destinationPath == null || destinationPath == String.Empty){
				throw new ArgumentException("destinationPath", "Destination path of embedded resource may not be null or empty.");
			}

			System.IO.Stream resStream = null;
			FileStream fstm = null;
			try{
				// Get the stream from the assembly resource.
				resStream = assembly.GetManifestResourceStream(resourcePath);

				// Get a filestream to write the data to.
				fstm = new FileStream(destinationPath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);

				// Initialize properties for reading stream data
				long numBytesToRead = resStream.Length;
				int numBytesRead = 0;
				int bufferSize = 1024;
				byte[] bytes = new byte[bufferSize];

				// Read the file from the resource stream and write to the file system
				while(numBytesToRead > 0){
					int numReadBytes = resStream.Read(bytes, 0, bufferSize);
					if(numReadBytes == 0){
						break;
					}
					if(numReadBytes < bufferSize){
						fstm.Write(bytes, 0, numReadBytes);
					}
					else{
						fstm.Write(bytes, 0, bufferSize);
					}
					numBytesRead += numReadBytes;
					numBytesToRead -= numReadBytes;
				}
				fstm.Flush();
			}
			catch{
				Console.Error.WriteLine("Unable to write resource [{0}] from assembly [{1}] to destination [{2}].", resourcePath, assembly.FullName, destinationPath);
				throw;
			}
			finally{
				// Close the resource stream
				if(resStream != null){
					resStream.Close();
				}

				// Close the file
				if(fstm != null){
					fstm.Close();
				}
			}
		}

		#endregion

		#endregion


	}
}
