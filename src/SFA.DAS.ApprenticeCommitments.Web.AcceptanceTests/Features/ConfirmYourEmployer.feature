@outerApi
Feature: ConfirmYourEmployer
	As an apprentice who wants to view my Employers details 
	So I can confirm them

Scenario: The apprentice is authenticated and should see the Employers Name
	Given the apprentice has logged in
	And the apprentice has not verified their employer
	When accessing the ConfirmYourEmployer page
	Then the response status code should be Ok
	And the apprentice should see the employer's name
	And the link is pointing to the confirm page
