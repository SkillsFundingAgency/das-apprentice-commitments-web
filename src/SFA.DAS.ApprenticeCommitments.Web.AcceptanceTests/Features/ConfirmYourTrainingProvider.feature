@outerApi
Feature: ConfirmYourTrainingProvider
	As an apprentice who wants to view my Training Provider details 
	So I can confirm them

Scenario: The apprentice is authenticated and should see the Training Provider Name
	Given the apprentice has logged in
	And the apprentice has not verified their training provider
	When accessing the ConfirmYourTrainingProvider page
	Then the response status code should be OK
	And the apprentice should see the training provider's name
	And the back link is pointing to the My Apprenticships page

Scenario: The apprentice is authenticated and confirms the training provider
	Given the apprentice has logged in
	And the apprentice has not verified their training provider
	And the apprentice confirms their training provider
	When submitting the ConfirmYourTrainingProvider page
	Then the user should be redirected back to the My Apprenticeships page

Scenario: The apprentice is authenticated and states that this is not their training provider
	Given the apprentice has logged in
	And the apprentice has not verified their training provider
	And the apprentice states this is not their training provider
	When submitting the ConfirmYourTrainingProvider page
	Then the user should be redirected to the cannot confirm apprenticeship page

Scenario: The apprentice is authenticated and presses the Confirm actions without select yes or no
	Given the apprentice has logged in
	And the apprentice has not verified their training provider
	And the apprentice doesn't select an option
	When submitting the ConfirmYourTrainingProvider page
	Then the response status code should be Ok
	And the model should contain an error message
	And the apprentice should see the training provider's name
	And the back link is pointing to the My Apprenticships page
