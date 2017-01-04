using System;

namespace Paraesthesia.Test.Ruby
{
	/// <summary>
	/// Specifies the Ruby-specific information for a Ruby unit test.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Marking an NUnit test with a <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute"/>
	/// and calling the <see cref="Paraesthesia.Test.Ruby.RubyTestExecutor.ExecuteTest"/> method
	/// from your NUnit test method will result in the execution of a Ruby unit test.
	/// </para>
	/// <para>
	/// Note that if the test is based in WATIR (Web Application Testing In Ruby), the
	/// <see cref="Paraesthesia.Test.Ruby.WatirTestAttribute"/> should be used to take
	/// advantage of WATIR-specific test benefits (like the ability to run the tests without
	/// displaying the browser window).  While WATIR tests will run correctly using
	/// the <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute"/>, they will not have
	/// such WATIR-specific benefits.
	/// </para>
	/// <para>
	/// For more information about specifically what the <see cref="Paraesthesia.Test.Ruby.RubyTestExecutor"/>
	/// provides for Ruby-based testing, see the <see cref="Paraesthesia.Test.Ruby.RubyTestExecutor"/>
	/// documentation.
	/// </para>
	/// </remarks>
	/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestExecutor" />
	/// <seealso cref="Paraesthesia.Test.Ruby.WatirTestAttribute" />
	/// <seealso cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute" />
	[AttributeUsage(AttributeTargets.Method)]
	public class RubyTestAttribute : System.Attribute {
		/// <summary>
		/// Internal storage for the <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute.AssemblyName" /> property.
		/// </summary>
		private string _assemblyName = "";

		/// <summary>
		/// Internal storage for the <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute.EmbeddedScriptPath" /> property.
		/// </summary>
		private string _embeddedScriptPath = "";

		/// <summary>
		/// Internal storage for the <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute.TestMethod" /> property.
		/// </summary>
		private string _testMethod = "";

		/// <summary>
		/// Gets the name of the assembly that contains the embedded Ruby script.
		/// </summary>
		/// <value>
		/// A <see cref="System.String"/> that indicates the assembly where the embedded
		/// Ruby script is located.  Defaults to <see cref="System.String.Empty"/>.
		/// </value>
		/// <remarks>
		/// <para>
		/// If specified, this value should be the name of an assembly that can be passed
		/// to <see cref="System.Reflection.Assembly.Load"/> so the embedded script can
		/// be retrieved.  If the value is <see cref="System.String.Empty"/>, it indicates
		/// the assembly is the one where the method with this <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute"/>
		/// is declared.
		/// </para>
		/// </remarks>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestAttribute" />
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestAttribute.EmbeddedScriptPath" />
		public string AssemblyName {
			get {
				return _assemblyName;
			}
			set {
				if(value == null){
					_assemblyName = "";
				}
				else{
					_assemblyName = value;
				}
			}
		}

		/// <summary>
		/// Gets the path to the embedded resource that is the Ruby test script.
		/// </summary>
		/// <value>
		/// A <see cref="System.String"/> with the path to an embedded resource that will
		/// be extracted and executed by the Ruby interpreter.
		/// </value>
		/// <remarks>
		/// <para>
		/// Unless otherwise specified by the <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute.AssemblyName"/>
		/// property, the embedded script is assumed to be located in the same assembly
		/// as the method with the <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute"/>.
		/// </para>
		/// </remarks>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestAttribute" />
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestAttribute.AssemblyName" />
		public string EmbeddedScriptPath {
			get {
				return _embeddedScriptPath;
			}
		}

		/// <summary>
		/// Gets the name of the method that executes the unit test.
		/// </summary>
		/// <value>
		/// A <see cref="System.String"/> with the name of the Ruby unit test method found
		/// in the script indicated by <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute.EmbeddedScriptPath"/>.
		/// </value>
		/// <remarks>
		/// <para>
		/// This method is the name of the Ruby unit test that needs to be executed.
		/// Generally speaking, this value will start with the prefix <c>test_</c>.
		/// </para>
		/// </remarks>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestAttribute" />
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestAttribute.AssemblyName" />
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestAttribute.EmbeddedScriptPath" />
		public string TestMethod {
			get {
				return _testMethod;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute" /> class.
		/// </summary>
		/// <param name="embeddedScriptPath">Path to the embedded resource that is the Ruby script to execute.</param>
		/// <param name="testMethod">Name of the Ruby unit test method.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="embeddedScriptPath" /> or <paramref name="testMethod" />
		/// is <see langword="null" />.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// Thrown if <paramref name="embeddedScriptPath" /> or <paramref name="testMethod" />
		/// is <see cref="System.String.Empty"/>.
		/// </exception>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestAttribute" />
		public RubyTestAttribute(string embeddedScriptPath, string testMethod) {
			if(embeddedScriptPath == null){
				throw new ArgumentNullException("embeddedScriptPath", "Path to the embedded Ruby script may not be null.");
			}
			if(testMethod == null){
				throw new ArgumentNullException("testMethod", "Name of the Ruby unit test method may not be null.");
			}
			if(embeddedScriptPath == String.Empty){
				throw new ArgumentException("Path to the embedded Ruby script may not be empty.", "embeddedScriptPath");
			}
			if(testMethod == String.Empty){
				throw new ArgumentException("Name of the Ruby unit test method may not be empty.", "testMethod");
			}
			this._embeddedScriptPath = embeddedScriptPath;
			this._testMethod = testMethod;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute" /> class.
		/// </summary>
		/// <param name="embeddedScriptPath">Path to the embedded resource that is the Ruby script to execute.</param>
		/// <param name="testMethod">Name of the Ruby unit test method.</param>
		/// <param name="assemblyName">Name of the assembly containing the embedded Ruby script.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="embeddedScriptPath" />, <paramref name="testMethod" />
		/// or <paramref name="assemblyName" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// Thrown if <paramref name="embeddedScriptPath" />, <paramref name="testMethod" />
		/// or <paramref name="assemblyName" /> is <see cref="System.String.Empty"/>.
		/// </exception>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestAttribute" />
		public RubyTestAttribute(string embeddedScriptPath, string testMethod, string assemblyName) : this(embeddedScriptPath, testMethod){
			if(assemblyName == null){
				throw new ArgumentNullException("assemblyName", "Name of the assembly containing the embedded Ruby script may not be null.");
			}
			if(assemblyName == String.Empty){
				throw new ArgumentException("Name of the assembly containing the embedded Ruby script may not be empty.", "assemblyName");
			}
			this._assemblyName = assemblyName;
		}
	}
}
