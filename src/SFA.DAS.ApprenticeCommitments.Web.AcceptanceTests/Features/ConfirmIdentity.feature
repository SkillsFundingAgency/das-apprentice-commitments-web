@outerApi
Feature: ConfirmIdentity
	As a apprentice who wants to view his commitment statements
	If he hasn't confirmed his identity

Scenario: The apprentice is authenticated and should see the verify identity page
	Given the apprentice has logged in
	And the apprentice has not verified their identity
	When accessing the "ConfirmYourIdentity" page
	Then the response status code should be Ok
	And the apprentice should see the verify identity page

Scenario: The apprentice is authenticated and should be redirected to the overview page
	Given the apprentice has logged in
	And the apprentice has verified their identity
	When accessing the "ConfirmYourIdentity" page
	And the apprentice should be shown the "Overview" page
