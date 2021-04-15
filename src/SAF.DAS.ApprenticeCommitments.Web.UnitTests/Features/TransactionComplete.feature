@outerApi
Feature: TransactionComplete
	As an apprentice I need to be told I have completed the “Confirm your apprenticeship” journey so that I am:
		Reassured that I have done what I need to do
		My form is submitted
		No further action is required of me
		What I should expect to happen next

Scenario: The apprentice is authenticated and should see Green confirmation box
	Given the apprentice has logged in
	When accessing the TransactionComplete page
	Then the response status code should be Ok
	And the apprentice should see the Green confirmation box with the employers name
