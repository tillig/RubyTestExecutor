using System;

namespace Paraesthesia.Test.Ruby {
	/// <summary>
	/// Describes a file that must be extracted from embedded resources to support a Ruby
	/// unit test.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Marking an NUnit test with a <see cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute"/>
	/// and calling the <see cref="Paraesthesia.Test.Ruby.RubyTestExecutor.ExecuteTest"/> method
	/// from your NUnit test method will result in the specified embedded file resource
	/// being extracted to a filesystem location relative to the location of the Ruby test
	/// script.
	/// </para>
	/// <para>
	/// This is helpful if there are multiple code files that must travel as a set for the
	/// Ruby test to run or if there are external configuration files that need to be handled
	/// during the course of the test.
	/// </para>
	/// <para>
	/// For more information about specifically what the <see cref="Paraesthesia.Test.Ruby.RubyTestExecutor"/>
	/// provides for Ruby-based testing, see the <see cref="Paraesthesia.Test.Ruby.RubyTestExecutor"/>
	/// documentation.
	/// </para>
	/// </remarks>
	/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestExecutor" />
	/// <seealso cref="Paraesthesia.Test.Ruby.RubyTestAttribute" />
	/// <seealso cref="Paraesthesia.Test.Ruby.WatirTestAttribute" />
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=true)]
	public class RubySupportFileAttribute : Attribute {
		/// <summary>
		/// Internal storage for the <see cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute.AssemblyName" /> property.
		/// </summary>
		private string _assemblyName = "";

		/// <summary>
		/// Internal storage for the <see cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute.EmbeddedFilePath" /> property.
		/// </summary>
		private string _embeddedFilePath = "";

		/// <summary>
		/// Internal storage for the <see cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute.TargetFilename" /> property.
		/// </summary>
		private string _targetFilename = "";

		/// <summary>
		/// Gets the name of the assembly that contains the embedded support file.
		/// </summary>
		/// <value>
		/// A <see cref="System.String"/> that indicates the assembly where the embedded
		/// support file is located.  Defaults to <see cref="System.String.Empty"/>.
		/// </value>
		/// <remarks>
		/// <para>
		/// If specified, this value should be the name of an assembly that can be passed
		/// to <see cref="System.Reflection.Assembly.Load"/> so the embedded support file can
		/// be retrieved.  If the value is <see cref="System.String.Empty"/>, it indicates
		/// the assembly is the one where the method with this <see cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute"/>
		/// is declared.
		/// </para>
		/// </remarks>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute" />
		/// <seealso cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute.EmbeddedFilePath" />
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
		/// Gets the path to the embedded resource that is the support file.
		/// </summary>
		/// <value>
		/// A <see cref="System.String"/> with the path to an embedded resource that will
		/// be extracted and used by the Ruby unit test.
		/// </value>
		/// <remarks>
		/// <para>
		/// Unless otherwise specified by the <see cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute.AssemblyName"/>
		/// property, the embedded support file is assumed to be located in the same assembly
		/// as the method with the <see cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute"/>.
		/// </para>
		/// </remarks>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute" />
		/// <seealso cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute.EmbeddedFilePath" />
		public string EmbeddedFilePath {
			get {
				return _embeddedFilePath;
			}
		}

		/// <summary>
		/// Gets the path/filename of the support file relative to the Ruby test script location.
		/// </summary>
		/// <value>
		/// A <see cref="System.String"/> containing the relative path to the extracted
		/// supporting file, including any subfolder, filename, and file extension.
		/// </value>
		/// <remarks>
		/// <para>
		/// This path is relative to the location the Ruby test script is extracted to.
		/// Note that the path must exist in the same folder or in a subfolder of the
		/// folder the Ruby test script is in; it may not be outside of that folder hierarchy.
		/// </para>
		/// </remarks>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute" />
		public string TargetFilename {
			get {
				return _targetFilename;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute" /> class.
		/// </summary>
		/// <param name="embeddedFilePath">Path to the embedded resource that is the Ruby unit test support file.</param>
		/// <param name="targetFilename">Path to the extracted support file, relative to the location of the Ruby unit test.</param>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute" />
		public RubySupportFileAttribute(string embeddedFilePath, string targetFilename) {
			if(embeddedFilePath == null){
				throw new ArgumentNullException("embeddedFilePath", "Path to the embedded support file may not be null.");
			}
			if(targetFilename == null){
				throw new ArgumentNullException("targetFilename", "Target filename of the embedded support file may not be null.");
			}
			if(embeddedFilePath == String.Empty){
				throw new ArgumentException("Path to the embedded support file may not be empty.", "embeddedFilePath");
			}
			if(targetFilename == String.Empty){
				throw new ArgumentException("Target filename of the embedded support file may not be empty.", "targetFilename");
			}
			this._embeddedFilePath = embeddedFilePath;
			this._targetFilename = targetFilename;

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute" /> class.
		/// </summary>
		/// <param name="embeddedFilePath">Path to the embedded resource that is the Ruby unit test support file.</param>
		/// <param name="targetFilename">Path to the extracted support file, relative to the location of the Ruby unit test.</param>
		/// <param name="assemblyName">Name of the assembly containing the embedded support file.</param>
		/// <seealso cref="Paraesthesia.Test.Ruby.RubySupportFileAttribute" />
		public RubySupportFileAttribute(string embeddedFilePath, string targetFilename, string assemblyName) : this(embeddedFilePath, targetFilename){
			if(assemblyName == null){
				throw new ArgumentNullException("assemblyName", "Name of the assembly containing the embedded support file may not be null.");
			}
			if(assemblyName == String.Empty){
				throw new ArgumentException("Name of the assembly containing the embedded support file may not be empty.", "assemblyName");
			}
			this._assemblyName = assemblyName;
		}
	}
}
