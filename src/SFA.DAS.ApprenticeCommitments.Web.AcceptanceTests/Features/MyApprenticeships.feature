@outerApi
Feature: MyApprenticeships
	As an apprentice
	I need to have a start screen showing all the steps of my commitment statement
	So that I know what steps are involved in signing my commitment statement
	And I can be directed to sign every aspect of my statement

Scenario: The apprentice is authenticated and should see the verify identity page
	Given the apprentice has logged in
	And there is one apprenticeship
	When accessing the "Apprenticeships" page
	Then the response should Redirect the apprenticeship page
