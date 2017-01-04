require 'test/unit'
require 'test/unit/assertions'

class RubyTest < Test::Unit::TestCase
	def test_Valid
		assert(true, 'This unit test should always pass.')
	end
	
	def test_Invalid
		assert(false, 'This unit test should always fail.')
	end
	
	def test_RubySupportFile
		assert(File.exist?("supportfile.txt"), "supportfile.txt does not exist.")
		assert(File.exist?("SubFolder1\\supportfile1.txt"), "SubFolder1\\supportfile1.txt does not exist.")
		assert(File.exist?("SubFolder1\\SubFolder2\\supportfile2.txt"), "SubFolder1\\SubFolder2\\supportfile2.txt does not exist.")
	end
end
