using System;

namespace Paraesthesia.Test.Ruby
{
	/// <summary>
	/// Specifies the Ruby-specific information for a Ruby/WATIR unit test.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Marking an NUnit test with a <see cref="Paraesthesia.Test.Ruby.WatirTestAttribute"/>
	/// and calling the <see cref="Paraesthesia.Test.Ruby.RubyTestExecutor.ExecuteTest"/> method
	/// from your NUnit test method will result in the execution of a Ruby unit test
	/// based on the WATIR (Web Application Testing In Ruby) framework.
	/// </para>
	/// <para>
	/// Using this attribute instead of the <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute"/>
	/// makes the NUnit test aware that the Ruby test is based in WATIR and will want
	/// to take advantage of WATIR-specific test benefits (like the ability to run the
	/// tests without displaying the browser window).  Non-WATIR unit tests in Ruby should
	/// use the <see cref="Paraesthesia.Test.Ruby.RubyTestAttribute"/> instead.
	/// </para>
	/// <para>
	/// For more information about specifically what the <see cref="Paraesthesia.Test.Ruby.RubyTestExecutor"/>
	/// provides for Ruby-based testing, see the <see cref="Paraesthesia.Test.Ruby.RubyTestExecutor"/>
	/// documentation.
	/// </para>
	/// </remarks>
	/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestExecutor" />
	/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestAttribute" />
	/// <seealso cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute" />
	[AttributeUsage(AttributeTargets.Method)]
	public class WatirTestAttribute : RubyTestAttribute {
		/// <summary>
		/// Initializes a new instance of the <see cref="Paraesthesia.Test.Ruby.WatirTestAttribute" /> class.
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
		/// <seealso cref="Paraesthesia.Test.Ruby.WatirTestAttribute" />
		public WatirTestAttribute(string embeddedScriptPath, string testMethod) : base(embeddedScriptPath, testMethod){}

		/// <summary>
		/// Initializes a new instance of the <see cref="Paraesthesia.Test.Ruby.WatirTestAttribute" /> class.
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
		/// <seealso cref="Paraesthesia.Test.Ruby.WatirTestAttribute" />
		public WatirTestAttribute(string embeddedScriptPath, string testMethod, string assemblyName) : base(embeddedScriptPath, testMethod, assemblyName){}
	}
}
