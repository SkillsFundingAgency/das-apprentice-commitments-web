@outerApi
Feature: ConfirmYourEmployer
	As an apprentice who wants to view my Employer's details 
	So I can confirm them

Scenario: The apprentice is authenticated and should see the Employer's Name
	Given the apprentice has logged in
	And the apprentice has not verified their employer
	When accessing the ConfirmYourEmployer page
	Then the response status code should be Ok
	And the apprentice should see the employer's name
	And the link is pointing to the confirm page

Scenario: The apprentice is authenticated and confirms the employer
	Given the apprentice has logged in
	And the apprentice has not verified their employer
	And the apprentice confirms their employer
	When submitting the ConfirmYourEmployer page
	Then the apprenticeship is updated to show the a 'true' confirmation
	And the user should be redirected back to the overview page

Scenario: The apprentice is authenticated and states that this is not their employer
	Given the apprentice has logged in
	And the apprentice has not verified their employer
	And the apprentice states this is not their employer
	When submitting the ConfirmYourEmployer page
	Then the apprenticeship is updated to show the a 'false' confirmation
	And the user should be redirected to the cannot confirm apprenticeship page

Scenario: The apprentice is authenticated and presses the Confirm actions without select yes or no
	Given the apprentice has logged in
	And the apprentice has not verified their employer
	And the apprentice doesn't select an option
	When submitting the ConfirmYourEmployer page
	Then the response status code should be Ok
	And the model should contain an error message
	And the apprentice should see the employer's name
	And the link is pointing to the confirm page

Scenario: The apprentice is authenticated has previously said this is not their employer
	Given the apprentice has logged in
	And the apprentice has confirmed this is not their employer
	When accessing the ConfirmYourEmployer page
	Then the response status code should be Ok
	And the apprentice should see the employer's name
	And the user should see the confirmation options

Scenario: The apprentice is authenticated has previously said this is their employer
	Given the apprentice has logged in
	And the apprentice has confirmed this is their employer
	When accessing the ConfirmYourEmployer page
	Then the response status code should be Ok
	And the apprentice should see the employer's name
	And the user should not see the confirmation options
	And the user should be able to change their answer

Scenario: The apprentice is authenticated has completed their confirmation
	Given the apprentice has logged in
	And the apprentice has confirmed everything
	When accessing the ConfirmYourEmployer page
	Then the response status code should be Ok
	And the user should not see the confirmation options
	And the user should not be able to change their answer
