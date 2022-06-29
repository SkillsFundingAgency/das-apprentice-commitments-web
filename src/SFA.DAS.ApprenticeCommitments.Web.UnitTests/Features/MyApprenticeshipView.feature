@outerApi
Feature: MyApprenticeshipView

Feature: MyApprenticeshipView
	As an apprentice
	I need to have a page showing my last confirmed commitment statement
	So that I know what I previously confirmed

Scenario: The apprentice is authenticated and should see the view page
	Given the apprentice has logged in
	And the apprentice will navigate to the view page
	When accessing the view page
	Then the response status code should be Ok
	And the apprentice should see the apprenticeship view page for the apprenticeship

Scenario: The apprentice is authenticated but cannot see view page as no confirmed apprenticeship exists
	Given the apprentice has logged in
	And the apprentice will navigate to the view page for invalid apprenticeship
	When accessing the view page
	Then the response status code should be internal error
