require 'watir'
require 'test/unit'
require 'test/unit/assertions'
include Watir

class WatirTest < Test::Unit::TestCase
	def setup
		@ie = IE.new
		@ie.goto("http://www.google.com")
	end
	
	def test_Valid
		assert(@ie.contains_text('Google'), 'This unit test should always pass.')
	end
	
	def test_Invalid
		assert(!@ie.contains_text('Google'), 'This unit test should always fail.')
	end
	
	def teardown
		@ie.close
	end
end
