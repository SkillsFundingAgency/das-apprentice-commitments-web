@outerApi
Feature: ConfirmIdentity
	As a apprentice who wants to view his commitment statements
	If he hasn't confirmed his identity

Scenario: The apprentice is authenticated and should see the verify identity page
	Given the apprentice has logged in
	And the apprentice has not verified his identity
	When first accessing the commitment statement website
	Then the response status code should be Ok
	And the apprentice should see the verify identity page
