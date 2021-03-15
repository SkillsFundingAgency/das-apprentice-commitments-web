@outerApi
Feature: ConfirmYourTrainingProvider
	As an apprentice who wants to view my Training Provider details 
	So I can confirm them

Scenario: The apprentice is authenticated and should see the Training Provider Name
	Given the apprentice has logged in
	And the apprentice has not verified their training provider
	When accessing the ConfirmYourTrainingProvider page
	Then the response status code should be OK
	And the apprentice should see the training provider name
	And the back link is pointing to the My Apprenticships page


