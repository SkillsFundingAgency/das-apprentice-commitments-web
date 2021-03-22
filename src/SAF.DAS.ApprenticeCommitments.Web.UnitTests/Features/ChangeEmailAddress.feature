Feature: ChangeEmailAddress
	As an apprentice
	I want to be able to change the email address associated with my digital account
	So that I can still access my commitment & receive updates from the service

@mytag
Scenario: Redirect to Identity Service
	When accessing the "ChangeYourEmailAddress" page
	Then the result should redirect to the identity server's change email page

